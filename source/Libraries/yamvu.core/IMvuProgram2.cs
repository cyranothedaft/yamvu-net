using yamvu.core.Primitives;



namespace yamvu.core;

public interface IMvuProgram2<TModel, out TView>
      : IMvuProgramUpdatable<TModel>,
        IMvuProgramViewable<TModel, TView>;


public interface IMvuProgramUpdatable<TModel> {
   (TModel initialModel, IMvuCommand[] initialCommands) Init();
   (TModel newModel, IMvuCommand[] commands) Update(MvuCommandDispatchDelegate dispatch, TModel model, IMvuMessage message);

   // TODO: this way instead?
   // TView View<TView>(MvuMessageDispatchDelegate dispatch, TModel newModel);
}


public interface IMvuProgramViewable<in TModel, out TView> {
   TView View(MvuMessageDispatchDelegate dispatch, TModel newModel);
}
