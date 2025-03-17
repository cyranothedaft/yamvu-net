using System;
using CounterMvu;
using Microsoft.Extensions.Logging;
using WelterKit.Std.StaticUtilities;
using yamvu.core;
using yamvu.core.Primitives;


namespace CounterMvu_lib.Effects;

using ResultType = int;


public partial interface IEffects {
   ValueTask<ResultType> GenerateRandomNumberAsync();
}


public record GenerateRandomNumberEffect() : IMvuEffect {
   public override string ToString() => $"GenerateRandomNumber()".SurroundWith('!');



   public static async Task Dispatch(IEffects executor, GenerateRandomNumberEffect effect, IHasEffectResultHandler? resultHandler, ILogger? logger) {
      if (resultHandler is IHasEffectResultHandler<ResultType> withResultHandler)
         await ProgramEffectHelper.ExecuteAsync(() => executor.GenerateRandomNumberAsync(),
                                                effect, withResultHandler.HandleResult, logger);
      else
         throw new Exception($"Effect {effect} has wrong handler type: [{resultHandler}]");
   }
}


public static partial class MvuEffects {
   public static IMvuEffectCommand GenerateRandomNumber(EffectResultCallbackDelegate<ResultType> handleResult)
      => new GenerateRandomNumberEffect().AsCommandWithResultHandler(handleResult);
}
