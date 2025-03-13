using yamvu.core;
using yamvu.core.Primitives;



namespace ConsoleSample.Mvu.Effects;


public static class EffectExtensions {
   public static MvuEffectCommand AsCommandWithResultHandler<TResult>(this IMvuEffect effect, EffectResultCallbackDelegate<TResult> handleResult)
      => new WindowRecallEffectCommand<TResult>(effect, handleResult);
}



public sealed record WindowRecallEffectCommand<TResult>(IMvuEffect Effect, EffectResultCallbackDelegate<TResult> HandleResult)
      : MvuEffectCommandWithTypedResultHandler<TResult>(Effect, HandleResult), IConsoleSampleCommand;
