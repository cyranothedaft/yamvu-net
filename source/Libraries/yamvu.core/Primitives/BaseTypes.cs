namespace yamvu.core.Primitives;

//public interface IMvuBase; TODO: do we readlly need a base type to unify Message and Effect (the data-only types)?



// public interface IHasId {
//    public AutoGenId Id => AutoGenId.None;
// }


public interface IMvuCommand 
      // : IHasId
;



public interface IMvuMessageCommand : IMvuCommand, IHasMessage;
public interface IMvuMessageCommand<out TMsg> : IMvuMessageCommand, IHasMessage<TMsg> where TMsg : IMvuMessage;

public interface IMvuEffectCommand  : IMvuCommand, IHasEffect;
public interface IMvuEffectCommand<out TEff>  : IMvuEffectCommand, IHasEffect<TEff> where TEff : IMvuEffect;

public interface IMvuEffectCommandWithResultHandler  : IMvuEffectCommand, IHasEffectResultHandler;
public interface IMvuEffectCommandWithResultHandler<out TEff>  : IMvuEffectCommandWithResultHandler, IMvuEffectCommand<TEff> where TEff : IMvuEffect ;
public interface IMvuEffectCommandWithTypedResultHandler<in TResult>  : IMvuEffectCommandWithResultHandler, IHasEffectResultHandler<TResult>;
public interface IMvuEffectCommandWithTypedResultHandler<out TEff, in TResult> : 
      IMvuEffectCommandWithResultHandler<TEff>,
      IMvuEffectCommandWithTypedResultHandler<TResult> where TEff : IMvuEffect;



public interface IHasMessage { IMvuMessage Message { get; } }
public interface IHasMessage<out TMsg> : IHasMessage where TMsg:IMvuMessage { new TMsg Message { get; } }

public interface IHasEffect { IMvuEffect Effect { get; } }
public interface IHasEffect<out TEff> : IHasEffect where TEff:IMvuEffect { new TEff Effect { get; } }

public interface IHasEffectResultHandler; // marker interface
public interface IHasEffectResultHandler<in TResult> : IHasEffectResultHandler {
   EffectResultCallbackDelegate<TResult> HandleResult { get; }
}



public interface IMvuMessage; // marker interface
public interface IMvuEffect;  // marker interface



public abstract record MvuMessageCommand(IMvuMessage Message) : IMvuMessageCommand;
public abstract record MvuMessageCommand<TMsg>(TMsg TypedMessage) : MvuMessageCommand(TypedMessage), IMvuMessageCommand<TMsg> where TMsg : IMvuMessage {
   TMsg IHasMessage<TMsg>.Message => this.TypedMessage;

   // public AutoGenId Id { get; } = new AutoGenId();
   public override string ToString() => $"{nameof( MvuMessageCommand<TMsg> )}: {TypedMessage}";
   //public override string ToString() => $"(id:{Id}){nameof( MvuMessageCommand<TMsg> )}: {TypedMessage}";
}

public abstract record MvuEffectCommand(IMvuEffect Effect) : IMvuEffectCommand;
public abstract record MvuEffectCommand<TEff>(TEff TypedEffect) : MvuEffectCommand(TypedEffect), IMvuEffectCommand<TEff> where TEff : IMvuEffect {
   TEff IHasEffect<TEff>.Effect => this.TypedEffect;

   // public AutoGenId Id { get; } = new AutoGenId();
   public override string ToString() => $"{nameof( MvuEffectCommand<TEff> )}: {TypedEffect}";
   //public override string ToString() => $"(id:{Id}){nameof( MvuEffectCommand<TEff> )}: {TypedEffect}";
}



public abstract record MvuEffectCommandWithTypedResultHandler<TResult>(IMvuEffect Effect, EffectResultCallbackDelegate<TResult> HandleResult) : MvuEffectCommand(Effect), IHasEffectResultHandler<TResult> {
   public override string ToString() => $"{nameof( MvuEffectCommandWithTypedResultHandler<TResult> )}: {Effect}";
   //public override string ToString() => $"(id:{((IMvuCommand)this).Id}){nameof( MvuEffectCommandWithTypedResultHandler<TResult> )}: {Effect}";
}



public abstract record MvuEffectCommandWithTypedResultHandler<TEff, TResult>(TEff TypedEffect, EffectResultCallbackDelegate<TResult> HandleResult) : MvuEffectCommand<TEff>(TypedEffect),
                                                                                                                                            IMvuEffectCommandWithTypedResultHandler<TEff, TResult>
      where TEff : IMvuEffect {
   public override string ToString() => $"{nameof( MvuEffectCommandWithTypedResultHandler<TEff, TResult> )}: {TypedEffect}";
   //public override string ToString() => $"(id:{((IMvuCommand)this).Id}){nameof( MvuEffectCommandWithTypedResultHandler<TEff, TResult> )}: {TypedEffect}";
}



// public static class MvuExtensions {
//    public static IMvuMessageCommand<TMsg> AsCommand<TMsg>(this TMsg message)
//          where TMsg : IMvuMessage
//       => throw new NotImplementedException();
//       // => new MvuMessageCommand<TMsg>(message);
// }
