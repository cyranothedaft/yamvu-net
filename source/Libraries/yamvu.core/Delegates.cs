using System;
using yamvu.core.Primitives;



namespace yamvu.core;


public delegate void MvuCommandDispatchDelegate(IMvuCommand command);
public delegate void MvuMessageDispatchDelegate(IMvuMessage message);
public delegate void TypedMvuMessageDispatchDelegate<in TMsg>(TMsg message) where TMsg:  IMvuMessage;
public delegate void TypedMvuCommandDispatchDelegate<in T>(T command) where T : IMvuCommand;


public delegate TView                                     ViewDelegate  <in TModel, out TView>(MvuMessageDispatchDelegate dispatch, TModel model);
public delegate (TModel newModel, IMvuCommand[] commands) UpdateDelegate<TModel>              (MvuCommandDispatchDelegate dispatch, TModel model, IMvuMessage message);

// TODO: remove of this is the same as ViewDelegate
public delegate TView ViewFuncDelegate<in TModel, out TView>(MvuMessageDispatchDelegate dispatch, TModel model
                                                  // , ProgramInputSources programInputSources
                                                  );


//public delegate void ExecuteEffectDelegate<TEff>(IMvuEffectCommand<TEff> effectCommand) where TEff : IMvuEffect;
//public delegate void ExecuteEffectDelegate<TEff>((TEff effect, IHasEffectResultHandler hasResultHandler) tuple) where TEff : IMvuEffect;

// TODO: ? remove TEff parameter and replace with IMvuEffect
public delegate void ExecuteEffectDelegate<in TEff>(TEff effect, IHasEffectResultHandler hasResultHandler) where TEff : IMvuEffect;

public delegate void EffectResultCallbackDelegate<in TResult>(TResult result);



public static class DelegateExtensions {

   public static Action<TMsg> AsMessageAction<TMsg>(this Action<IMvuCommand> commandAction)
         where TMsg : IMvuMessage
      => throw new NotImplementedException("TODO: remove?");
      // => message => commandAction(new MvuMessageCommand(message));


   public static MvuMessageDispatchDelegate AsMessageDispatcher(this MvuCommandDispatchDelegate commandDelegate)
      => throw new NotImplementedException("TODO: remove?");
      // => message => commandDelegate(new MvuMessageCommand(message));
}
