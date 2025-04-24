using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WelterKit.Buses;
using yamvu.core.Primitives;



namespace yamvu.Runners;

public static class BusRunner {

   public static async Task<(TResult actionResult, MessageBusStats stats)> RunBusAndDoAsync<TResult>(Func<IMessageBus<IMvuCommand>, Task<TResult>> whatToDoWithTheBusAsync,
                                                                                                     bool throwOnUnroutableMessage = false,
                                                                                                     // Action<IMvuCommand>? onReadAction = null,
                                                                                                     // Action<IMvuCommand, bool>? onWriteAction = null,
                                                                                                     ILogger? busLogger = null,
                                                                                                     CancellationToken busCancellationToken = default) {
      using var bus = new FifoMessageBus<IMvuCommand>(busLogger);

      // ((IChannelEvents<IMvuCommand>)bus).OnRead  += onReadAction;
      // ((IChannelEvents<IMvuCommand>)bus).OnWrite += onWriteAction;

      Task<MessageBusStats> busTask = bus.StartAndAwaitCompletionAsync(throwOnUnroutableMessage, busCancellationToken);
      TResult actionResult = await whatToDoWithTheBusAsync(bus);

      // action is finished; now stop the bus
      bus.Writer.Stop();
      MessageBusStats busResult = await busTask;

      // ((IChannelEvents<IMvuCommand>)bus).OnRead  -= onReadAction;
      // ((IChannelEvents<IMvuCommand>)bus).OnWrite -= onWriteAction;

      return (actionResult, busResult);
   }
}
