// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;
// using WelterKit.Buses;
// using yamvu.core.Primitives;
//
//
// namespace yamvu;
//
// public class CommonBus : IDisposable {
//    private readonly bool _throwOnUnroutableMessage;
//    private readonly CancellationToken _busCancellationToken;
//    private readonly FifoMessageBus<IMvuCommand> _commandBus;
//
//    private bool _hasStopped = false;
//
//
//    public CommonBus(bool throwOnUnroutableMessage = false,
//                     // Action<IMvuCommand>? onReadAction = null,
//                     // Action<IMvuCommand, bool>? onWriteAction = null,
//                     ILogger? busLogger = null,
//                     CancellationToken busCancellationToken = default) {
//       _throwOnUnroutableMessage = throwOnUnroutableMessage;
//       _busCancellationToken     = busCancellationToken;
//       _commandBus               = new FifoMessageBus<IMvuCommand>(busLogger);
//
//       // ((IChannelEvents<IMvuCommand>)bus).OnRead  += onReadAction;
//       // ((IChannelEvents<IMvuCommand>)bus).OnWrite += onWriteAction;
//
//       // ((IChannelEvents<IMvuCommand>)bus).OnRead  -= onReadAction;
//       // ((IChannelEvents<IMvuCommand>)bus).OnWrite -= onWriteAction;
//    }
//
//
//    public async Task<MessageBusStats> StartAndAwaitCompletionAsync()
//       => await _commandBus.StartAndAwaitCompletionAsync(_throwOnUnroutableMessage, _busCancellationToken);
//
//
//    public void Stop() {
//       // TODO: ensure thread safety?
//       if (_hasStopped) return;
//       _commandBus.Writer.Stop();
//       _hasStopped = true;
//    }
//
//
//    public void Dispose() {
//       Stop();
//       _commandBus.Dispose();
//    }
// }
