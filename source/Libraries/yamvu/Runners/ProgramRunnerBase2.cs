using System;
using Microsoft.Extensions.Logging;
using WelterKit.Channels;
using yamvu.core.Primitives;
using yamvu.core.util.Channels;



namespace yamvu.Runners;

// public interface IDispatchable<T> : IDisposable {
//    public void DispatchMessage(T message);
// }


internal class ProgramCommandChannel<TCmd> : IDisposable
      where TCmd : IMvuCommand {
   // private readonly ILogger? _logger;
   private readonly IDisposableChannel<TCmd> _channel;

   public IChannelReader<TCmd> Reader => _channel.Reader;
   public IChannelWriter<TCmd> Writer => _channel.Writer;


   internal ProgramCommandChannel(ILogger? pccLogger) {
      _channel = ChannelHelper.BuildChannel<TCmd>(pccLogger);
   }


   public void Dispose() {
      _channel.Dispose();
   }


   // public void DispatchMessage(TCmd message) {
   //    _channel.Writer.Write(message);
   // }


}
