using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using yamvu.core.Primitives;



namespace yamvu.core;

public static class ProgramEffectHelper {
   public static Task ExecuteAsync<TResult>(Func<ValueTask<TResult>> effectAsync,
                                                  IMvuEffect effect,
                                                  EffectResultCallbackDelegate<TResult>? resultHandler = null,
                                                  ILogger? logger = null)
      => ExecuteAsync(effectAsync: (Func<Task<TResult>>)(async () => await effectAsync()),
                      effect,
                      resultHandler,
                      logger);


   public static async Task ExecuteAsync<TResult>(Func<Task<TResult>> effectAsync,
                                                  IMvuEffect effect,
                                                  EffectResultCallbackDelegate<TResult>? resultHandler = null,
                                                  ILogger? logger = null) {

      TResult result = await performEffect();
      if (resultHandler is not null)
         handleResult(result, resultHandler);


      async Task<TResult> performEffect() {
         // perform effect
         TResult effectResult;
         try {
            logger?.LogTrace("--> exec effect [{effect}]", effect); // effectDesc);
            effectResult = await effectAsync();
            logger?.LogTrace("<-- exec effect: [{effect}] - result: {result}", effect, effectResult);
         }
         catch (Exception exception) {
            logger?.LogError(exception, "Error while executing effect [{effect}]", effect);
            throw;
         }
         // logger?.LogEffectResult(effect, result);
         return effectResult;
      }


      void handleResult(TResult effectResult, EffectResultCallbackDelegate<TResult> handlerDelegate) {
         // perform callback with effect result
         try {
            handlerDelegate(effectResult);
         }
         catch (Exception exception) {
            logger?.LogError(exception, "Error while handling effect [{effect}] result", effect);
            throw;
         }
      }
   }


   // TODO?
   // public static Task ExecuteAsync<TResult>(Func<ValueTask<TResult>> effectAsync, MoreLogger? logger)
   //    => ExecuteAsync(async () => {
   //                       await effectAsync();
   //                    }, logger);


   // TODO?
   // public static async Task ExecuteAsync(Func<Task> effectAsync, MoreLogger? logger) {
   //    // perform effect
   //    await effectAsync();
   //    // logger?.LogEffectResult(effect, result);
   // }


   public static Task IgnoreEffectAsync() => Task.CompletedTask;


   // public static IHandlesEffectResult NullEffectResultHandler { get; } = new NullResultHandler();
   // private record struct NullResultHandler : IHandlesEffectResult;


   public static IMvuEffectCommandWithTypedResultHandler<TEffect, TResult> MakeEffectCommand2<TEffect, TResult>(TEffect effect, EffectResultCallbackDelegate<TResult> handleResult)
         where TEffect : IMvuEffect // TODO? , IHasResultType<TResult>
      => throw new NotImplementedException();
      // => new MvuEffectCommandWithTypedResultHandler<TEffect, TResult>(effect, handleResult);


   public static IMvuEffectCommandWithTypedResultHandler<IMvuEffect, TResult> MakeEffectCommand22<TResult>(IMvuEffect effect, EffectResultCallbackDelegate<TResult> handleResult)
      => throw new NotImplementedException("TODO: remove?");
      // => new MvuEffectCommandWithTypedResultHandler<IMvuEffect, TResult>(effect, handleResult);


   // public static IMvuEffectCommandWithResultHandler<TEffect> MakeEffectCommand2<TEffect>(TEffect effect, EffectResultCallbackDelegate<TResult> handleResult)
   //       where TEffect : IMvuEffect // TODO? , IHasResultType<TResult>
   //    => new MvuEffectCommandWithResultHandler<TEffect, TResult>(effect, handleResult);


   public static IMvuEffectCommand<TEffect> MakeEffectCommandNoHandler2<TEffect>(TEffect effect)
         where TEffect : IMvuEffect // TODO? , IHasResultType<TResult>
      => throw new NotImplementedException();
      // => new MvuEffectCommand<TEffect>(effect);


   // /// <summary>
   // /// Makes an Effect command with no result handler.
   // /// </summary>
   // public static IEffectAndResultHandler MakeEffectCommand<TEffect>(TEffect effect) 
   //       where TEffect : IMvuEffect 
   //    => new EffectAndNoResultHandler<TEffect>(effect);
}
