using System;
using System.Threading.Tasks;
using CounterMvu_lib;
using CounterMvu_lib.Effects;
using CounterSample.AppCore;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using Photino.NET;
using yamvu;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Runners;
using CounterProgram = CounterMvu_lib.Program.Program;



namespace PhotinoHtmlCounterSample.gui;

internal static class MvuProgramRunner {
   internal static IMvuProgram2<Model, PhotinoView> BuildMvuProgram(ILoggerFactory? loggerFactory) {
      IMvuProgram2<Model, PhotinoView> mvuProgram = CounterProgram.Build(ViewBuilder.BuildView, loggerFactory?.CreateLogger("prog"));
      return mvuProgram;
   }


   internal static IMvuProgramRunner<PhotinoView> BuildMvuProgramRunner(PhotinoWindow window, IAppServices appServices,
                                                                                                            ExternalMessageDispatcher? externalMessageDispatcher, ILoggerFactory? loggerFactory

         // Action<Exception> handleException
   ) {

      IMvuProgramRunner<PhotinoView> mvuProgramRunner = CounterProgram.BuildRunner<PhotinoView>(loggerFactory);


      return mvuProgramRunner;

   }


   public static async Task RunMvuProgramAsync(PhotinoWindow window, IAppServices appServices, IMvuProgramRunner<PhotinoView> mvuProgramRunner, IMvuProgram2<Model, PhotinoView> mvuProgram,
                                               ExternalMessageDispatcher? externalMessageDispatcher, ILoggerFactory? loggerFactory) {
      IEffects effectExecutor = new EffectExecutor (appServices);
      Model finalModel = await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuProgramRunner, mvuProgram,
                                                                                 updateView, buildView,
                                                                                 loggerFactory, externalMessageDispatcher, CounterProgram.Info,
                                                                                 CounterProgram.MessageAsCommandFunc, effecuteEffect, CounterProgram.IsQuitMessageFunc);


      static void updateView(PhotinoView view) {
         /*===*/
      }

      static PhotinoView buildView(MvuMessageDispatchDelegate dispatch, Model model, ILogger? uilogger)
         => ViewBuilder.BuildView(dispatch, model);

      void effecuteEffect(IMvuEffect effect, IHasEffectResultHandler hasresulthandler)
         => executeEffect2(effectExecutor, effect, hasresulthandler, loggerFactory?.CreateLogger("fx"));

      static void executeEffect2(IEffects effectExecutor, IMvuEffect effect, IHasEffectResultHandler resultHandler, ILogger? effectLogger) {
         // TODO: await !!!
         // this fires-and-forgets without proper exception trapping
         EffectDispatcher.DispatchToExecutorAsync(effectExecutor, effect, resultHandler, effectLogger);
      }
   }
}
