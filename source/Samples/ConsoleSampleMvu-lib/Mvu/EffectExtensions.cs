using CounterSample.AppCore.Mvu;
using yamvu.core;
using yamvu.core.Primitives;


namespace CounterMvu;


public static class EffectExtensions {
   public static MvuEffectCommand AsCommandWithResultHandler<TResult>(this IMvuEffect effect, EffectResultCallbackDelegate<TResult> handleResult)
      => new CounterMvuEffectCommand<TResult>(effect, handleResult);
}


public sealed record CounterMvuEffectCommand<TResult>(IMvuEffect Effect, EffectResultCallbackDelegate<TResult> HandleResult)
      : MvuEffectCommandWithTypedResultHandler<TResult>(Effect, HandleResult), ICounterMvuCommand;
