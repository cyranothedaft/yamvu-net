using System;
using Microsoft.Extensions.Logging;
using WelterKit.Std.StaticUtilities;
using yamvu.core;
using yamvu.core.Primitives;



namespace ConsoleSample.Mvu.Effects;

using ResultType = DateTimeOffset;


public record RetrieveCurrentTimeEffect() : IMvuEffect {
   public override string ToString() => $"RetrieveCurrentTime()".SurroundWith('!');



   public static async Task Dispatch(IEffects executor, RetrieveCurrentTimeEffect effect, IHasEffectResultHandler? resultHandler, ILogger? logger) {
      if (resultHandler is IHasEffectResultHandler<ResultType> withResultHandler)
         await ProgramEffectHelper.ExecuteAsync(() => executor.RetrieveCurrentTimeAsync(),
                                                effect, withResultHandler.HandleResult, logger);
      else
         throw new Exception($"Effect {effect} has wrong handler type: [{resultHandler}]");
   }
}


public static partial class MvuEffects {
   public static IMvuEffectCommand RetrieveCurrentTime(EffectResultCallbackDelegate<ResultType> handleResult)
      => new RetrieveCurrentTimeEffect().AsCommandWithResultHandler(handleResult);
}


public partial interface IEffects {
   ValueTask<ResultType> RetrieveCurrentTimeAsync();
}
