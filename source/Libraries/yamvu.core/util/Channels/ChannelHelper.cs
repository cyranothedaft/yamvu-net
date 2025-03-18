using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WelterKit.Channels;
using WelterKit.StaticUtilities;



namespace yamvu.core.util.Channels;


//internal interface IChannelWriterWithConvert<TInt, TExt> : IChannelWriter<TInt> {
//   void Write(TExt submission_externalType) => Write(ConvertFromExt(submission_externalType));
//   protected TInt ConvertFromExt(TExt value_externalType);
//}


//internal interface IChannelReaderWithConvert<TInt, TExt> : IChannelReader<TExt> {
//   new TExt Read() => ConvertToExt(.Read());
//   protected TExt ConvertToExt(TInt value);
//}



public interface IDisposableChannel<T> : IChannel<T>, IDisposable { }



public static class ChannelHelper {
   public static async Task<TResult> DoWithChannelAsync<T, TResult>(IChannel<T> channel,
                                                                    Func<IChannelWriter<T>, IChannelReader<T>, Task<TResult>> asyncFunc,
                                                                    // Action<T, bool>? onWrite = null, Action<T>? onRead = null,
                                                                    [CallerFilePath] string? callerFilePath = null,
                                                                    [CallerMemberName] string? callerMemberName = null,
                                                                    ILogger? logger = null) {
      // Channel<T> channel = BuildChannel<T>(TODO, TODO);
      try {
         logger?.LogTrace("running with channel");

         //logger?.LogTrace(className: callerFilePath ?? "?" /*===TODO===*/, "running with channel", callerMemberName);
         return await asyncFunc(channel.Writer, channel.Reader);
      }
      finally {
         // ReSharper disable once MergeIntoPattern

         //logger.LogWarning(className: callerFilePath ?? "?" /*===TODO===*/, "Channel closing with {leftoverCount} items left inside",
         //                  callerMemberName: callerMemberName, channel.Reader.Count);
      }
   }


   /// <summary>
   /// Returns final state
   /// </summary>
   /// <param name="initialIterationFunc">Return true to terminate</param>
   public static async Task<TState> Loop_n_ReadAsync<T, TState>(IChannelReader<T> reader,
                                                                Func<TState, bool> initialIterationFunc,
                                                                Func<T, TState, int, (TState state, bool terminateLoop)> perIterationFunc,
                                                                Func<TState> getInitialState,
                                                                Action? beforeIterationAction = null,
                                                                CancellationToken cancellationToken = default) {
      // TODO: break this method down some more

      TState state = getInitialState();

      beforeIterationAction?.Invoke();

      // foreach (T item in initialItems ?? Enumerable.Empty<T>())
      //    writer.Write(item);

      bool terminate = initialIterationFunc(state);
      if (terminate)
         return state;

      int i = 0;
      await foreach (T item in reader.ReadAllAsync(cancellationToken)) {
         (TState newState, terminate) = perIterationFunc(item, state, i);
         state = newState;
         if (terminate) break;
         ++i;
      }
      return state;
   }


   /// <summary>
   /// Returns final state
   /// </summary>
   public static Task<TState> Loop_n_ReadAsync<T, TState>(IChannelReader<T> reader,
                                                          Action<TState> initialIterationAction,
                                                          Func<T, TState, int, (TState state, bool terminateLoop)> perIterationFunc,
                                                          Func<TState> getInitialState,
                                                          Action? beforeIterationAction = null,
                                                          CancellationToken cancellationToken = default)
      => Loop_n_ReadAsync(reader,
                          initialIterationFunc: state => {
                                                   initialIterationAction(state);
                                                   return false;
                                                },
                          perIterationFunc, getInitialState, beforeIterationAction, cancellationToken);


   //public static async Task<TResult> WithChannelAsync<T, T, TResult>(Func<IChannelWriter<T>, IChannelReader<T>, Task<TResult>> asyncFunc,
   //                                                               Func<T,T> convertToExt,Func<T,T> convertFromExt,
   //                                                               Action<T, bool>? onWrite = null, Action<T>? onRead = null,
   //                                                               [CallerFilePath] string? callerFilePath = null,
   //                                                               [CallerMemberName] string? callerMemberName = null,
   //                                                               MoreLogger? logger = null) {
   //   Channel<T> channel = buildChannel<T>();
   //   try {
   //      logger?.LogTrace(className: callerFilePath ?? "?" /*===TODO===*/, "running with channel", callerMemberName);
   //      return await asyncFunc(channel.Writer.WrappedWithConvert(onWrite, convertFromExt),
   //                             channel.Reader.WrappedWithConvert(onRead , convertToExt));
   //   }
   //   finally {
   //      // ReSharper disable once MergeIntoPattern
   //      if (logger is not null && channel.Reader.CanCount && channel.Reader.Count > 0)
   //         logger.LogWarning(className: callerFilePath ?? "?" /*===TODO===*/, "Channel closing with {leftoverCount} items left inside",
   //                           callerMemberName: callerMemberName, channel.Reader.Count);
   //   }
   //}


   public static IDisposableChannel<T> BuildChannel<T>(ILogger? logger = null, bool isForProgramWithoutCommandBus = false)
      => new ChannelWrapper<T>(logger, isForProgramWithoutCommandBus);


   // private static IChannelWriter<T> Wrapped<T>(this ChannelWriter<T> channelWriter, Action<T, bool>? onWrite)
   //    => new WriterWrapper<T>(channelWriter, onWrite);


   // private static IChannelWriter<T> WrappedWithConvert<T, T>(this ChannelWriter<T> innerChannelWriter, Action<T, bool>? onWrite, Func<T,T> convertFromExt) 
   //    => new WriterWrapperWithConvert<T, T>(innerChannelWriter, onWrite, convertFromExt);


   // private static IChannelReader<T> Wrapped<T>(this ChannelReader<T> channelReader, Action<T>? onRead)
   //    => new ReaderWrapper<T>(channelReader, onRead);


   // private static IChannelReader<T> WrappedWithConvert<T, T>(this ChannelReader<T> innerChannelReader, Action<T>? onRead, Func<T, T> convertToExt)
   //    => new ReaderWrapperWithConvert<T, T>(innerChannelReader, onRead, convertToExt);



   private class ChannelWrapper<T> : IDisposableChannel<T> {
      private readonly ILogger? _logger;
      private readonly Channel<T> _channel;

      public IChannelWriter<T> Writer { get; }
      public IChannelReader<T> Reader { get; }

      public event Action<T, bool>? OnWrite;
      public event Action<T>? OnRead;


      public ChannelWrapper(ILogger? logger = null, bool isForProgramWithoutCommandBus = false) {
         _logger = logger;
         _channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions()
                                                  {
                                                     SingleWriter = false,
                                                     SingleReader = true,
                                                  });
         _logger?.LogTrace("created");
         Writer = new WriterWrapper<T>(_channel.Writer, onWrite, logger?.WithPrefix("[writer] "));
         Reader = new ReaderWrapper<T>(_channel.Reader, onRead,  logger?.WithPrefix("[reader] "), isForProgramWithoutCommandBus);
      }


      private void onWrite(T item, bool success) {
         OnWrite?.Invoke(item, success);
      }


      private void onRead(T item) {
         OnRead?.Invoke(item);
      }


      public void Dispose() {
         if (_logger is not null && _channel.Reader.CanCount && _channel.Reader.Count > 0)
            _logger.LogWarning("Channel closing with {leftoverCount} items left inside", _channel.Reader.Count);
      }
   }


   private class WriterWrapper<T> : IChannelWriter<T> {
      private readonly ChannelWriter<T> _channelWriter;
      private readonly Action<T, bool>? _onWrite;
      private readonly ILogger? _logger;


      public WriterWrapper(ChannelWriter<T> channelWriter, Action<T, bool>? onWrite, ILogger? logger) {
         _channelWriter = channelWriter;
         _onWrite       = onWrite;
         _logger        = logger;
      }


      public bool Write(T submission) {
         bool success = _channelWriter.TryWrite(submission);
         _logger?.LogTrace("Write [{submission}] (success:{success})", submission, success);
         _onWrite?.Invoke(submission, success);
         return success;
      }


      public bool Complete() {
         bool success = _channelWriter.TryComplete();
         _logger?.LogTrace("Complete (success:{success})", success);
         return success;
      }
   }



   // private class WriterWrapperWithConvert<TInt, TExt> : IChannelWriter<TExt> {
   //    private readonly IChannelWriter<TInt> _innerWriter;
   //    private readonly Func<TExt, TInt> _convertFromExt;
   //
   //    public WriterWrapperWithConvert(ChannelWriter<TInt> innerWriter, Action<TInt, bool>? onWrite, Func<TExt, TInt> convertFromExt) {
   //       _convertFromExt     = convertFromExt;
   //       _innerWriter = innerWriter.Wrapped(onWrite);
   //    }
   //
   //    public void Write(TExt value_externalType) => _innerWriter.Write(_convertFromExt(value_externalType));
   // }



   private class ReaderWrapper<T> : IChannelReader<T> {
      private readonly ChannelReader<T> _channelReader;
      private readonly Action<T>? _onRead;
      private readonly ILogger? _logger;
      private readonly bool _isForProgramWithoutCommandBus;


      public ReaderWrapper(ChannelReader<T> channelReader, Action<T>? onRead, ILogger? logger, bool isForProgramWithoutCommandBus = false) {
         _channelReader                 = channelReader;
         _onRead                        = onRead;
         _logger                        = logger;
         _isForProgramWithoutCommandBus = isForProgramWithoutCommandBus;
      }


      public async IAsyncEnumerable<T> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken) {
         await foreach (T emission in _channelReader.ReadAllAsync(cancellationToken)) {
            if (_isForProgramWithoutCommandBus) _logger?.LogDebug("(feed) Read [{emission}]", emission);
            else                                _logger?.LogDebug("Read [{emission}]",        emission);

            _onRead?.Invoke(emission);
            yield return emission;
         }
      }


      public int Count => _channelReader.Count;
   }



   // private class ReaderWrapperWithConvert<TInt, TExt> : IChannelReader<TExt> {
   //    private readonly IChannelReader<TInt> _innerReader;
   //    private readonly Func<TInt, TExt> _convertToExt;
   //
   //    public ReaderWrapperWithConvert(ChannelReader<TInt> innerReader, Action<TInt>? onRead, Func<TInt, TExt> convertToExt) {
   //       _convertToExt     = convertToExt;
   //       _innerReader = innerReader.Wrapped(onRead);
   //    }
   //
   //    public IAsyncEnumerable<TExt> ReadAllAsync(CancellationToken cancellationToken = default)
   //       => _innerReader.ReadAllAsync(cancellationToken)
   //                      .Select(_convertToExt);
   // }
}
