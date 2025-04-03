// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
//
// namespace PhotinoHtmlCounterSample.gui;
//
//
// public delegate TView ViewFuncDelegate<out TView>(MvuMessageDispatchDelegate dispatch, Model model,
//                                                   //(input bindings are handled differently now) ProgramEventSources programEventSources,
//                                                   ILogger? uiLogger);
//
// public class ProgramRunnerWithServices<TView> {
//    public static ProgramRunnerWithServices<TView> Build(ExternalMessageDispatcher? messageFromOutsideDispatcher,
//                                                         // ProgramEventSources programEventSources,
//                                                         // (ILogger? app, ILogger? service, ILogger? programRunner, ILogger? program, ILogger? effect, ILogger? bus) loggers
//                                                         ILoggerFactory? loggerFactory
//          ) {
//       ILogger? servicesLogger = loggerFactory?.CreateLogger("svcs");
//
//       return new(buildServices(servicesLogger),
//                  buildProgramRunner(loggerFactory),
//                  messageFromOutsideDispatcher,
//                  loggerFactory
//                  // programEventSources,
//                  // (loggers.app,
//                  //  loggers.service,
//                  //  loggers.program,
//                  //  loggers.effect,
//                  //  loggers.bus,
//                  //  loggers.programRunner)
//                 );
//    }
//
//
//    private static IAppServices buildServices(ILogger? servicesLogger)
//          // => new FakeAppServices(servicesLogger);
//          => new AppServices_Real(servicesLogger);
//
//
//    private static IMvuProgramRunner<TView> buildProgramRunner(ILoggerFactory? loggerFactory)
//          => new ProgramRunner2<ICounterMvuCommand, TView>(loggerFactory?.CreateLogger("run"));
//
//
//    private readonly IAppServices _services;
//    private readonly IMvuProgramRunner<TView> _programRunner;
//    private readonly ExternalMessageDispatcher? _messageFromOutsideDispatcher;
//    // private readonly ProgramEventSources _programEventSources;
//    // private readonly (ILogger? app, ILogger? ui, ILogger? program, ILogger? effect, ILogger? bus, ILogger? programRunner) _loggers;
//    private readonly ILogger? _hostLogger;
//
//
//    public ProgramRunnerWithServices(IAppServices services,
//                                     IMvuProgramRunner<TView> programRunner,
//                                     ExternalMessageDispatcher? messageFromOutsideDispatcher,
//                                     // ProgramEventSources programEventSources,
//                                     // (ILogger? app, ILogger? services, ILogger? program, ILogger? effect, ILogger? bus, ILogger? programRunner) loggers
//                                     ILoggerFactory? loggerFactory
//          ) {
//       _services                     = services;
//       _programRunner                = programRunner;
//       _messageFromOutsideDispatcher = messageFromOutsideDispatcher;
//       // _programEventSources = programEventSources;
//       // _loggers                     = loggers;
//       _hostLogger = loggerFactory?.CreateLogger("host");
//    }
//
//
//    public async Task RunProgramWithCommonBusAsync(Action<TView> replaceViewAction, ViewFuncDelegate<TView> viewFunc, ILoggerFactory? loggerFactory) {
//       _programRunner.ViewEmitted += (view, isInitialView) => {
//                                        replaceViewAction(view);
//                                     };
//       _hostLogger?.LogTrace("Attached ViewEmitted event");
//
//       await runProgramWithCommonBusAsync(_services, _programRunner, 
//                                          _messageFromOutsideDispatcher,
//                                          // _programEventSources, 
//                                          viewFunc,
//                                          loggerFactory
//                                          // (_loggers.ui,
//                                          //  _loggers.program,
//                                          //  _loggers.effect,
//                                          //  _loggers.bus,
//                                          //  _loggers.programRunner)
//                                          );
//    }
//
//
//    private static async Task runProgramWithCommonBusAsync(IAppServices services,
//                                                           IMvuProgramRunner<TView> programRunner,
//                                                           ExternalMessageDispatcher? messageFromOutsideDispatcher,
//                                                           // ProgramEventSources programEventSources,
//                                                           ViewFuncDelegate<TView> viewFunc,
//                                                           // (ILogger? ui, ILogger? program, ILogger? effect, ILogger? bus, ILogger? programRunner) loggers
//                                                           ILoggerFactory? loggerFactory
//          ) {
//       ILogger? uiLogger = loggerFactory?.CreateLogger("ui");
//       ILogger? programLogger = loggerFactory?.CreateLogger("mvu");
//       ILogger? effectLogger = loggerFactory?.CreateLogger("fx");
//       ILogger? busLogger = loggerFactory?.CreateLogger("bus");
//       ILogger? runWrapperLogger = loggerFactory?.CreateLogger("wrap");
//
//       ProgramInfo programInfo = CounterProgram.Info;
//       IMvuProgram2<Model, TView> program = CounterProgram.Build(viewFunc: (dispatch, model) => viewFunc(dispatch, model, 
//                                                                                                         // programEventSources, 
//                                                                                                         uiLogger),
//                                                          programLogger);
//
//       CancellationToken busCancellationToken     = new();
//       CancellationToken programCancellationToken = new();
//
//       EffectExecutor effectExecutor = new(services);
//
//       void executeTheEffect(IMvuEffect effect, IHasEffectResultHandler resultHandler) => executeEffect(effectExecutor, effect, resultHandler, effectLogger);
//
//       Model finalModel = await ProgramRunner2<IMvuCommand, TView>.RunWithBusAsync(program, programInfo, programRunner, MessageExtensions.AsCommand, executeTheEffect,
//                                                                                   isQuitMessage: msg => msg is Request_QuitMessage,
//                                                                                   busCancellationToken, programCancellationToken,
//                                                                                   messageFromOutsideDispatcher: messageFromOutsideDispatcher,
//                                                                                   busLogger: busLogger,
//                                                                                   runWrapperLogger: runWrapperLogger);
//       // TODO: return finalModel
//    }
//
//
//    private static void executeEffect(IEffects effectExecutor, IMvuEffect effect, IHasEffectResultHandler resultHandler, ILogger? effectLogger) {
//       // TODO: await !!!
//       // this fires-and-forgets without proper exception trapping
//       EffectDispatcher.DispatchToExecutorAsync(effectExecutor, effect, resultHandler, effectLogger);
//    }
//
// }
