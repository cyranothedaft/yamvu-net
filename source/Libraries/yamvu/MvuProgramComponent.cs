using System;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Runners;


namespace yamvu;

public record MvuProgramComponent<TModel, TView>(
      IMvuProgram2<TModel, TView> Program,
      IMvuProgramRunner<TView> ProgramRunner,
      ProgramInfo ProgramInfo,
      Func<IMvuMessage, IMvuCommand> MessageAsCommandFunc,
      Func<IMvuMessage, bool> IsQuitMessageFunc,
      ExecuteEffectDelegate<IMvuEffect> ExecuteEffectDelegate
);
