using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WelterKit.Buses;
using WelterKit.Channels;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.gui.ViewPlatform;
using WinFormsCounterSample.View;
using yamvu;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Runners;



namespace WinFormsCounterSample.gui;

internal static class EntryPoint {
   private static ILogger? _globalAppLogger = null;


   /// <summary>
   ///  The main entry point for the application.
   /// </summary>
   [STAThread]
   static void Main() {
      ApplicationConfiguration.Initialize();

      const LogLevel minimumLogLevel = LogLevel.Trace;
      using ( ILoggerFactory? loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                      .SetMinimumLevel(minimumLogLevel))) {
         Func<string, ILogger?> createProgramLogger(string prefix) => category => loggerFactory?.CreateLogger(prefix + category);

         _globalAppLogger = loggerFactory?.CreateLogger("main");

         MainForm mainForm = new MainForm();

         startBus

         ILogger? busLogger = loggerFactory?.CreateLogger("bus");
         CancellationToken busCancellationToken;
         (Model finalModel_actual, MessageBusStats stats) = await BusRunner.RunBusAndDoAsync(bus => {

                                                                                             },
                                                                                             throwOnUnroutableMessage: true,
                                                                                             // onReadAction:,
                                                                                             // onWriteAction:,
                                                                                             busLogger,
                                                                                             busCancellationToken);

         async Task run1Async(ExternalMessageDispatcher? externalMessageDispatcher, Action<PlatformView<ProgramView>> replaceViewAction, Func<string, ILogger?> createLoggerFunc) {
            ILogger? uiLogger = createLoggerFunc("ui");
            ILogger? servicesLogger = createLoggerFunc("svcs");
            IAppServices appServices = new AppServices_Real(servicesLogger);

            PlatformView<ProgramView> view(MvuMessageDispatchDelegate dispatch, Model model) => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger);

            MvuProgramComponent<Model, PlatformView<ProgramView>> mvuComponent = Component.GetAsComponent(appServices, view, createLoggerFunc("prog"), createLoggerFunc);


            ProgramInfo programInfo = mvuComponent.ProgramInfo with
                                         {
                                            Name = mvuComponent.ProgramInfo + "1"
                                         };
            ILogger? hostLogger       = createLoggerFunc("host");
            ILogger? busLogger1        = createLoggerFunc("bus");
            ILogger? runWrapperLogger = createLoggerFunc("wrap");

            IMvuProgramRunner<PlatformView<ProgramView>> programRunner = mvuComponent.BuildProgramRunner();
            programRunner.ViewEmitted += (view1, isInitialView) => {
                                            replaceViewAction(view1);
                                         };
            hostLogger?.LogTrace("Attached ViewEmitted event");

            CancellationToken busCancellationToken     = new();
            CancellationToken programCancellationToken = new();

            IMvuProgram2<Model, PlatformView<ProgramView>> program = mvuComponent.BuildProgram();




            async Task<Model> whatToDoWithTheBus(IMessageBus<IMvuCommand> bus) {
               return await ((Func<Task<Model>>)(() => runProgramAsync(bus)))
                            .wrapWithViewRecording2<PlatformView<ProgramView>, Model>(programRunner, recordView)
                            .wrapWithFeedRecording2<IMvuCommand, Model>((IChannelEvents<IMvuCommand>)bus, recordCommand)
                            .wrapWithLogs(runWrapperLogger)
                            ();
            }

            async Task<Model> runProgramAsync(IMessageBus<IMvuCommand> bus)
               => await programRunner.RunProgramAsync2<Model>(bus.Publisher,
                                                              bus.Writer,
                                                              program,
                                                              programInfo,
                                                              messageAsCommand,
                                                              executeEffectAction,
                                                              isQuitMessage,
                                                              queryTerminationAndSimulateInput,
                                                              initialCommands,
                                                              externalMessageDispatcher,
                                                              programCancellationToken);



         }

         embedMvuProgramInContainer(mainForm.Program1Container, run1Async, createProgramLogger("[1]"));
         embedMvuProgramInContainer(mainForm.Program2Container, runMvuProgram2Async,                                                                                  createProgramLogger("[2]"));
         Application.Run(mainForm);
      }
   }


   private static void embedMvuProgramInContainer(IMvuControlContainer container,
                                                  Func<ExternalMessageDispatcher, Action<PlatformView<ProgramView>>, Func<string, ILogger?>, Task> runMvuProgramAsync,
                                                  Func<string, ILogger?> createLoggerFunc) {
      ExternalMessageDispatcher externalMessageDispatcher = new();

      async void onLoadRunMvuProgram(object? sender, EventArgs e) {
         try {
            // form has loaded, so start (asynchronously run) the MVU program


            await runMvuProgramAsync(externalMessageDispatcher,
                                     view => replaceMvuComponents(container.ContainerControl, view),
                                     createLoggerFunc);


            // the MVU program has terminated normally, so signal the form to close
            container.Close();
         }
         catch (Exception exception) {
            _globalAppLogger?.LogError(exception, "General exception while running MVU program");

            // TODO: form.Close() ?
         }
      }

      void onClosingStopMvuProgram(object? sender, FormClosingEventArgs e) {
         // form is closing, so signal the MVU program to terminate
         externalMessageDispatcher.Dispatch(MvuMessages.Request_Quit());
      }

      // TODO: have an "un-embed" method that unsubscribes these
      container.ContainerLoaded  += onLoadRunMvuProgram;
      container.ContainerClosing += onClosingStopMvuProgram;
   }


   private static async Task runMvuProgram1Async(ExternalMessageDispatcher? externalMessageDispatcher, Action<PlatformView<ProgramView>> replaceViewAction,
                                                 Func<string, ILogger?> createLoggerFunc) {
      ILogger? uiLogger       = createLoggerFunc("ui");
      ILogger? servicesLogger = createLoggerFunc("svcs");
      IAppServices appServices = new AppServices_Real(servicesLogger);

      PlatformView<ProgramView> view(MvuMessageDispatchDelegate dispatch, Model model)
         => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger);

      MvuProgramComponent<Model, PlatformView<ProgramView>> mvuComponent = Component.GetAsComponent(appServices, view, createLoggerFunc("prog"), createLoggerFunc);
      // var programRunnerWithServices = ProgramRunnerWithServices<PlatformView<ProgramView>>.Build(externalMessageDispatcher, _loggerFactory);
      var finalModel = await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuComponent.BuildProgramRunner,
                                                                               mvuComponent.BuildProgram,
                                                                               replaceViewAction,
                                                                               createLoggerFunc,
                                                                               externalMessageDispatcher,
                                                                               mvuComponent.ProgramInfo with { Name = mvuComponent.ProgramInfo + "1" },
                                                                               mvuComponent.MessageAsCommandFunc,
                                                                               mvuComponent.ExecuteEffectDelegate,
                                                                               mvuComponent.IsQuitMessageFunc);
   }


   private static async Task runMvuProgram2Async(ExternalMessageDispatcher? externalMessageDispatcher, Action<PlatformView<ProgramView>> replaceViewAction,
                                                 Func<string, ILogger?> createLoggerFunc) {
      ILogger? uiLogger       = createLoggerFunc("ui");
      ILogger? servicesLogger = createLoggerFunc("svcs");
      IAppServices appServices = new AppServices_Real(servicesLogger);

      PlatformView<ProgramView> view(MvuMessageDispatchDelegate dispatch, Model model)
         => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger);

      MvuProgramComponent<Model, PlatformView<ProgramView>> mvuComponent = Component.GetAsComponent(appServices, view, createLoggerFunc("prog"), createLoggerFunc);
      // var programRunnerWithServices = ProgramRunnerWithServices<PlatformView<ProgramView>>.Build(externalMessageDispatcher, _loggerFactory);
      var finalModel = await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuComponent.BuildProgramRunner,
                                                                               mvuComponent.BuildProgram,
                                                                               replaceViewAction,
                                                                               createLoggerFunc,
                                                                               externalMessageDispatcher,
                                                                               mvuComponent.ProgramInfo with { Name = mvuComponent.ProgramInfo + "2" },
                                                                               mvuComponent.MessageAsCommandFunc,
                                                                               mvuComponent.ExecuteEffectDelegate,
                                                                               mvuComponent.IsQuitMessageFunc);
   }


   private static void replaceMvuComponents(Control componentContainer, PlatformView<ProgramView> view) {
      componentContainer.SuspendLayout();
      componentContainer.Controls.Clear();
      componentContainer.Controls.AddRange(view.MvuView.Controls.ToArray());
      componentContainer.ResumeLayout();
      componentContainer.Invalidate();
   }
}
