using yamvu.core;
using yamvu.core.Primitives;


namespace CounterSample.AppCore.Mvu.Program;

public class ProgramWithView<TView> : IMvuProgram2<Model, TView> {
   private readonly ProgramUpdate _programUpdate;
   private readonly ViewDelegate<Model, TView> _viewFunc;


   public ProgramWithView(ProgramUpdate programUpdate, ViewDelegate<Model, TView> viewFunc) {
      _programUpdate = programUpdate;
      _viewFunc      = viewFunc;
   }


   (Model initialModel, IMvuCommand[] initialCommands) IMvuProgramUpdatable<Model>
      .Init()
      => _programUpdate.Init();


   (Model newModel, IMvuCommand[] commands) IMvuProgramUpdatable<Model>
      .Update(MvuCommandDispatchDelegate dispatch, Model model, IMvuMessage message)
       => _programUpdate.Update(dispatch, model, message);


   TView IMvuProgramViewable<Model, TView>
      .View(MvuMessageDispatchDelegate dispatch, Model newModel)
      => _viewFunc(dispatch, newModel);
}
