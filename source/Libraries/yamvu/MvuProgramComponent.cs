using System;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Runners;


namespace yamvu;

public record MvuProgramComponent<TModel, TView>(
      Func<IMvuProgram2<TModel, TView>> BuildProgram,
      Func<IMvuProgramRunner<TView>> BuildProgramRunner,
      ProgramInfo ProgramInfo,
      Func<IMvuMessage, IMvuCommand> MessageAsCommandFunc,
      Func<IMvuMessage, bool> IsQuitMessageFunc,
      ExecuteEffectDelegate<IMvuEffect> ExecuteEffectDelegate
);
