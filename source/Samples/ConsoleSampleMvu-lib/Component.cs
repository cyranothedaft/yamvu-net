using System;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Effects;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Mvu.Program;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using yamvu;
using yamvu.core;


namespace CounterSample.AppCore;

public static class Component {
   public static MvuProgramComponent<Model, TView> GetAsComponent<TView>(IAppServices appServices, ViewDelegate<Model, TView> viewFunc,
                                                                         ILogger? programLogger, Func<string, ILogger?> createLoggerFunc) {
      IEffects effectExecutor = new EffectExecutor(appServices);
      ILogger? effectLogger = createLoggerFunc("fx");
      return new MvuProgramComponent<Model, TView>(() => Program.Build(viewFunc, programLogger),
                                                   () => Program.BuildRunner<TView>(createLoggerFunc),
                                                   Program.Info,
                                                   MessageExtensions.AsCommand,
                                                   message => message is Request_QuitMessage,
                                                   // this fires-and-forgets without proper exception trapping - TODO: await !!!
                                                   (effect, handler) => EffectDispatcher.DispatchToExecutorAsync(effectExecutor, effect, handler, effectLogger)
                                                  );
   }
}
