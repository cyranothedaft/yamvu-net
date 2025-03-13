using System;
using ConsoleSample.Mvu.Effects;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.core.Primitives;



namespace ConsoleSample.Mvu;

public static partial class EffectDispatcher {
   public static Task DispatchToExecutorAsync(IEffects executor, IMvuEffectCommand<IMvuEffect> effectCommand, ILogger? logger) {
      return DispatchToExecutorAsync(executor, effectCommand.Effect,
                                     effectCommand as IMvuEffectCommandWithResultHandler<IMvuEffect>,
                                     logger);
   }


   public static Task DispatchToExecutorAsync(IEffects executor, IMvuEffect effect, IHasEffectResultHandler? resultHandler, ILogger? logger)
      => effect switch
         {
            RetrieveCurrentTimeEffect eff => RetrieveCurrentTimeEffect.Dispatch(executor, eff, resultHandler, logger),
            _                             => ignoreEffectAsync(effect, logger)
         };


   private static async Task ignoreEffectAsync(IMvuEffect effect, ILogger? logger) {
      logger?.LogWarning($"Effect was not handled and will be ignored: {effect}");
      await ProgramEffectHelper.IgnoreEffectAsync();
   }
}
