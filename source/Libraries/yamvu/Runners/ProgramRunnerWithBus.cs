using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.core.Primitives;


namespace yamvu.Runners;


public delegate TView ViewFuncDelegate<in TModel, out TView>(MvuMessageDispatchDelegate dispatch, TModel model,
                                                             //(input bindings are handled differently now) ProgramEventSources programEventSources,
                                                             ILogger? uiLogger);

public class ProgramRunnerWithBus {

   public static async Task<TModel> RunProgramWithCommonBusAsync<TModel, TView>(IMvuProgramRunner<TView> programRunner, IMvuProgram2<TModel, TView> program,
                                                                                Action<TView> replaceViewAction, ILoggerFactory? loggerFactory,
                                                                                ExternalMessageDispatcher? externalMessageDispatcher,
                                                                                ProgramInfo programInfo, Func<IMvuMessage, IMvuCommand> messageAsCommand,
                                                                                ExecuteEffectDelegate<IMvuEffect> executeEffectAction, Func<IMvuMessage, bool> isQuitMessage) {
      ILogger? hostLogger = loggerFactory?.CreateLogger("host");
      ILogger? busLogger = loggerFactory?.CreateLogger("bus");
      ILogger? runWrapperLogger = loggerFactory?.CreateLogger("wrap");

      programRunner.ViewEmitted += (view, isInitialView) => {
                                       replaceViewAction(view);
                                    };
      hostLogger?.LogTrace("Attached ViewEmitted event");

      CancellationToken busCancellationToken     = new();
      CancellationToken programCancellationToken = new();

      TModel finalModel = await ProgramRunner2<IMvuCommand, TView>.RunWithBusAsync(program, programInfo, programRunner,
                                                                                   messageAsCommand, executeEffectAction, isQuitMessage,
                                                                                   busCancellationToken, programCancellationToken,
                                                                                   externalMessageDispatcher: externalMessageDispatcher,
                                                                                   busLogger: busLogger,
                                                                                   runWrapperLogger: runWrapperLogger);
      return finalModel;
   }


   // private static async Task<TModel> runProgramWithCommonBusAsync<TModel>(IMvuProgramRunner<TView> programRunner,
   //                                                                        ExternalMessageDispatcher? messageFromOutsideDispatcher,
   //                                                                        // ProgramEventSources programEventSources,
   //                                                                        IMvuProgram2<TModel, TView> program,
   //                                                                        ProgramInfo programInfo, Func<IMvuMessage, IMvuCommand> messageAsCommand,
   //                                                                        ExecuteEffectDelegate<IMvuEffect> executeEffectAction, Func<IMvuMessage, bool> isQuitMessage,
   //                                                                        // (ILogger? ui, ILogger? program, ILogger? effect, ILogger? bus, ILogger? programRunner) loggers
   //                                                                        ILoggerFactory? loggerFactory
   // ) {
   //    ILogger? busLogger = loggerFactory?.CreateLogger("bus");
   //    ILogger? runWrapperLogger = loggerFactory?.CreateLogger("wrap");
   //
   //    CancellationToken busCancellationToken     = new();
   //    CancellationToken programCancellationToken = new();
   //
   //    TModel finalModel = await ProgramRunner2<IMvuCommand, TView>.RunWithBusAsync(program, programInfo, programRunner,
   //                                                                                 messageAsCommand, executeEffectAction, isQuitMessage,
   //                                                                                 busCancellationToken, programCancellationToken,
   //                                                                                 messageFromOutsideDispatcher: messageFromOutsideDispatcher,
   //                                                                                 busLogger: busLogger,
   //                                                                                 runWrapperLogger: runWrapperLogger);
   //    return finalModel;
   // }
}
