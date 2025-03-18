using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WelterKit.Buses;
using yamvu.core.Primitives;



namespace yamvu.core.util.Channels;

public static class BusHelper {
   public static async Task<(TResult result, MessageBusStats stats)> DoWithMainBus<TResult>(Func<IMessageBus<IMvuCommand>, Task<TResult>> asyncFunc,
                                                                                            CancellationToken cancellationToken, ILogger? logger) {
      using var mainBus = new FifoMessageBus<IMvuCommand>(logger);
      Task<MessageBusStats> busTask = mainBus.StartAndAwaitCompletionAsync(throwOnUnroutableMessage: true, cancellationToken);
      TResult result = await asyncFunc(mainBus);
      mainBus.Writer.Stop();
      MessageBusStats stats = await busTask;
      return (result, stats);
   }
}
