using System;
using CounterMvu_lib;
using CounterMvu_lib.Effects;
using CounterMvu_lib.Messages;
using CounterMvu_lib.Program;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using yamvu;
using yamvu.core;


namespace CounterSample.AppCore;

public static class Component {
   public static MvuProgramComponent<Model, TView> GetAsComponent<TView>(IAppServices appServices, ViewDelegate<Model, TView> viewFunc,
                                                                         ILogger? programLogger, ILoggerFactory? loggerFactory) {
      IEffects effectExecutor = new EffectExecutor(appServices);
      ILogger? effectLogger = loggerFactory?.CreateLogger("fx");
      return new MvuProgramComponent<Model, TView>(Program.Build(viewFunc, programLogger),
                                                   Program.BuildRunner<TView>(loggerFactory),
                                                   Program.Info,
                                                   MessageExtensions.AsCommand,
                                                   message => message is Request_QuitMessage,
                                                   // this fires-and-forgets without proper exception trapping - TODO: await !!!
                                                   (effect, handler) => EffectDispatcher.DispatchToExecutorAsync(effectExecutor, effect, handler, effectLogger)
                                                  );
   }
}
