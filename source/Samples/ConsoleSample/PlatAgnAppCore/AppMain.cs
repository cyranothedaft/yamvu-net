using System;
using ConsoleSample.Mvu;
using ConsoleSample.Mvu.Effects;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Runners;


namespace ConsoleSample.PlatAgnAppCore;


public delegate TView ViewFuncDelegate<out TView>(MvuMessageDispatchDelegate dispatch, Model model, ProgramEventSources programEventSources,
                                                  ILogger? uiLogger);

public class AppMain<TView> {
   public static AppMain<TView> Build(ProgramEventSources programEventSources,
                                      (ILogger? app, ILogger? service, ILogger? programRunner, ILogger? program, ILogger? effect, ILogger? bus) loggers)
      => new (buildServices(loggers.service),
              buildProgramRunner(loggers.programRunner),
              programEventSources,
              (loggers.app,
               loggers.service,
               loggers.program,
               loggers.effect,
               loggers.bus,
               loggers.programRunner)
             );


   private static IAppServices buildServices(ILogger? servicesLogger)
         // => new FakeAppServices(servicesLogger);
         => new AppServices(servicesLogger);


   private static IMvuProgramRunner<TView> buildProgramRunner(ILogger? programRunnerLogger)
         => new ProgramRunner2<IConsoleSampleCommand, TView>(programRunnerLogger);


   private readonly IAppServices _services;
   private readonly IMvuProgramRunner<TView> _programRunner;
   private readonly ProgramEventSources _programEventSources;
   private readonly (ILogger? app, ILogger? ui, ILogger? program, ILogger? effect, ILogger? bus, ILogger? programRunner) _loggers;


   public AppMain(IAppServices services,
                  IMvuProgramRunner<TView> programRunner,
                  ProgramEventSources programEventSources,
                  (ILogger? app, ILogger? services, ILogger? program, ILogger? effect, ILogger? bus, ILogger? programRunner) loggers) {
      _services            = services;
      _programRunner       = programRunner;
      _programEventSources = programEventSources;
      _loggers             = loggers;
   }


   public async Task RunProgramWithCommonBusAsync(Action<TView> replaceViewAction, ViewFuncDelegate<TView> viewFunc) {
      _programRunner.ViewEmitted += (view, isInitialView) => {
                                       replaceViewAction(view);
                                    };
      _loggers.app?.LogDebug("Attached to runner");

      await runProgramWithCommonBusAsync(_services, _programRunner, _programEventSources, viewFunc,
                                         (_loggers.ui,
                                          _loggers.program,
                                          _loggers.effect,
                                          _loggers.bus,
                                          _loggers.programRunner));
   }


   private static async Task runProgramWithCommonBusAsync(IAppServices services,
                                                          IMvuProgramRunner<TView> programRunner,
                                                          ProgramEventSources programEventSources,
                                                          ViewFuncDelegate<TView> viewFunc,
                                                          (ILogger? ui, ILogger? program, ILogger? effect, ILogger? bus, ILogger? programRunner) loggers) {
      var programInfo = Mvu.Program.Info;
      var program     = Mvu.Program.Build(viewFunc: (dispatch, model) => viewFunc(dispatch, model, programEventSources, loggers.ui),
                                          loggers.program);
      var busCancellationToken     = new CancellationToken();
      var programCancellationToken = new CancellationToken();

      var effectExecutor = new EffectExecutor(services);

      void executeTheEffect(IMvuEffect effect, IHasEffectResultHandler resultHandler) => executeEffect(effectExecutor, effect, resultHandler, loggers.effect);

      var finalModel = await ProgramRunner2<IMvuCommand, TView>.RunWithBusAsync<Model>(program, programInfo, programRunner, MessageExtensions.AsCommand, executeTheEffect,
                                                                                       isQuitMessage: msg => msg is Mvu.Messages.Request_QuitMessage,
                                                                                       busCancellationToken, programCancellationToken,
                                                                                       busLogger: loggers.bus,
                                                                                       programRunnerLogger: loggers.programRunner);
   }


   private static void executeEffect(IEffects effectExecutor, IMvuEffect effect, IHasEffectResultHandler resultHandler, ILogger? effectLogger) {
      // TODO: await !!!
      // this fires-and-forgets without proper exception trapping
      EffectDispatcher.DispatchToExecutorAsync(effectExecutor, effect, resultHandler, effectLogger);
   }

}
