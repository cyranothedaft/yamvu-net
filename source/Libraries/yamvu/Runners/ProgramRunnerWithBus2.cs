using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WelterKit.Buses;
using WelterKit.Channels;
using WelterKit.StaticUtilities;
using WelterKit.Std.Functional;
using WelterKit.Std.StaticUtilities;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.core.util.Channels;



namespace yamvu.Runners;


public delegate void EmitViewDelegate<in TView>(TView view, bool isInitialView);


public interface IMvuProgramRunnerEvents<out TView> {
   event EmitViewDelegate<TView>? ViewEmitted;
}



public interface IMvuProgramRunner<TView> : IMvuProgramRunnerEvents<TView> {
   //TODO?      Task<TModel> RunProgramAsync<TModel>();

   // #region view stuff
   // public event Action<TView>? ViewEmitted;
   // #endregion view stuff


   Task<TModel> RunProgramAsync2<TModel>(IPublisher<IMvuCommand> commandBusPublisher,
                                         IBusWriter<IMvuCommand> commonBusWriter,
                                         IMvuProgram2<TModel, TView> program,
                                         ProgramInfo programInfo,
                                         Func<IMvuMessage, IMvuCommand> messageAsCommand,
                                         ExecuteEffectDelegate<IMvuEffect> executeEffect,
                                         Func<IMvuMessage, bool> isQuitMessage,
                                         Func<TView, bool>? queryTerminationAndSimulateInput = null,
                                         IMvuCommand[]? initialCommands = null,
                                         ExternalMessageDispatcher? messageFromOutsideDispatcher = null,
                                         CancellationToken cancellationToken = default) 
         // where TEffect : IMvuEffect
         ;
}



public class ProgramRunner2<TCmd, TView> : IMvuProgramRunner<TView> where TCmd : IMvuCommand {
   private readonly ILogger? _programRunnerLogger;
   private readonly ProgramCommandChannel<TCmd> _programCommandChannel;


   #region view stuff
   public event EmitViewDelegate<TView>? ViewEmitted;
   #endregion view stuff


   public ProgramRunner2(ILogger? programRunnerLogger) {
      _programRunnerLogger   = programRunnerLogger;
      _programCommandChannel = new ProgramCommandChannel<TCmd>(programRunnerLogger?.WithPrefix("[pcc] "));
   }


   #region Public Static

   // TODO: compare with Test_SamplePrograms.TestWithBusAsync2.TestWithBusAsync2 (which this was copied from) and consolidate

   public static async Task<TModel> RunWithBusAsync<TModel>(IMvuProgram2<TModel, TView> program,
                                                            ProgramInfo programInfo,
                                                            IMvuProgramRunner<TView> programRunner,
                                                            Func<IMvuMessage, IMvuCommand> messageAsCommand,
                                                            ExecuteEffectDelegate<IMvuEffect> executeEffectAction,
                                                            Func<IMvuMessage, bool> isQuitMessage,
                                                            CancellationToken busCancellationToken,
                                                            CancellationToken programCancellationToken,
                                                            Action<IMvuCommand>? recordCommand = null,
                                                            EmitViewDelegate<TView>? recordView = null,
                                                            Func<TView, bool>? queryTerminationAndSimulateInput = null,
                                                            IMvuCommand[]? initialCommands = null,
                                                            ExternalMessageDispatcher? messageFromOutsideDispatcher = null,
                                                            TimeSpan? taskTimeout = default,
                                                            ILogger? busLogger = null,
                                                            ILogger? runWrapperLogger = null) 
         // where TEff : IMvuEffect
         {

      (TModel finalModel_actual, MessageBusStats stats) = await BusRunner.RunBusAndDoAsync(whatToDoWithTheBus,
                                                                                           throwOnUnroutableMessage: true,
                                                                                           // onReadAction:,
                                                                                           // onWriteAction:,
                                                                                           busLogger,
                                                                                           busCancellationToken);
      // logger?.LogDebug("final bus stats: {stats}", stats);
      return finalModel_actual;


      async Task<TModel> whatToDoWithTheBus(IMessageBus<IMvuCommand> bus) {
         return await ((Func<Task<TModel>>)(() => runProgramAsync(bus)))
                      .wrapWithViewRecording2<TView, TModel>(programRunner, recordView)
                      .wrapWithFeedRecording2<IMvuCommand, TModel>((IChannelEvents<IMvuCommand>)bus, recordCommand)
                      .wrapWithLogs(runWrapperLogger)
                      ();
      }

      async Task<TModel> runProgramAsync(IMessageBus<IMvuCommand> bus)
         => await programRunner.RunProgramAsync2<TModel>(bus.Publisher,
                                                        bus.Writer,
                                                        program,
                                                        programInfo,
                                                        messageAsCommand,
                                                        executeEffectAction,
                                                        isQuitMessage,
                                                        queryTerminationAndSimulateInput,
                                                        initialCommands,
                                                        messageFromOutsideDispatcher,
                                                        programCancellationToken);

   }

   #endregion Public Static


   private delegate Task<TModel> RunLoopAsyncDelegate<TModel>(Action<IMvuCommand> dispatchCommandAction);



   public Task<TModel> RunProgramAsync2<TModel>(IPublisher<IMvuCommand> commonBusPublisher,
                                                IBusWriter<IMvuCommand> commonBusWriter,
                                                IMvuProgram2<TModel, TView> program,
                                                ProgramInfo programInfo,
                                                Func<IMvuMessage, IMvuCommand> messageAsCommand,
                                                ExecuteEffectDelegate<IMvuEffect> executeEffect,
                                                Func<IMvuMessage, bool> isQuitMessage,
                                                Func<TView, bool>? queryTerminationAndSimulateInput = null,
                                                IMvuCommand[]? initialCommands = null,
                                                ExternalMessageDispatcher? messageFromOutsideDispatcher = null,
                                                CancellationToken cancellationToken = default)
         // where TEff : IMvuEffect
         // where TEff : IActionableEffectBase
   {
      ILogger? programLogger = _programRunnerLogger?.WithPrefix($"[run program {programInfo.Name}] ");

      if (messageFromOutsideDispatcher is not null) {
         programLogger?.LogTrace("Subscribing to " + nameof( messageFromOutsideDispatcher.MessageFromOutside ) + " event");
         messageFromOutsideDispatcher.MessageFromOutside += message => dispatchCommand(messageAsCommand(message));
      }

      Func<Task<TModel>> wrapped = ((Func<Task<TModel>>)(() => runProgramLoopAsync(dispatchCommand)))
                                  .wrapWithCommonBusLink2<TModel, TCmd>(commonBusPublisher, handleCommand)
                                  .wrapWithLogs2(programLogger);
      return wrapped();

      void dispatchCommand(IMvuCommand command) => dispatchCommandToCommonBus(commonBusWriter, _programRunnerLogger, command);
      void handleCommand(TCmd command) => handleCommandFromCommonBus(_programCommandChannel.Writer, _programRunnerLogger, command);
      void emitView(TView view, bool isInitialView) => ViewEmitted?.Invoke(view, isInitialView);

      Task<TModel> runProgramLoopAsync(Action<IMvuCommand> dispatchCommandAction)
         => runProgramLoopAsync2<TModel>(_programCommandChannel.Reader,
                                         program,
                                         _programRunnerLogger,
                                         dispatchCommandAction,
                                         messageAsCommand,
                                         isQuitMessage,
                                         queryTerminationAndSimulateInput.ToMaybeFromNullable(),
                                         executeEffect,
                                         emitView,
                                         initialCommands,
                                         cancellationToken);
   }


   private static async Task<TModel> runProgramLoopAsync2<TModel>(IChannelReader<TCmd> programCommandChannelReader,
                                                                  IMvuProgram2<TModel, TView> program,
                                                                  ILogger? logger,
                                                                  Action<IMvuCommand> dispatchCommand,
                                                                  Func<IMvuMessage, IMvuCommand> messageAsCommand,
                                                                  Func<IMvuMessage, bool> isQuitMessage,
                                                                  Maybe<Func<TView, bool>> queryTerminationAndSimulateInput,
                                                                  ExecuteEffectDelegate<IMvuEffect> effectAction,
                                                                  EmitViewDelegate<TView>? viewAction,
                                                                  IMvuCommand[]? initialCommands = null,
                                                                  CancellationToken cancellationToken = default) {
      void dispatchMessage(IMvuMessage msg) => dispatchCommand(messageAsCommand(msg));
      TModel finalModel = await ChannelHelper.Loop_n_ReadAsync(
                                                               programCommandChannelReader,
                                                               initialIterationFunc: model => queryTerminationAndGetInitialView2(program, logger, queryTerminationAndSimulateInput, viewAction, model, dispatchMessage),
                                                               perIterationFunc: (msg, model, iterationNumber) => handleNextCommand(program, logger, queryTerminationAndSimulateInput, model, msg,
                                                                                                                                    command => dispatchCommand(command), dispatchMessage, isQuitMessage,
                                                                                                                                    effectAction, viewAction, iterationNumber),
                                                               getInitialState: () => init2<TModel>(program, logger, dispatchCommand),
                                                               beforeIterationAction: () => {
                                                                                         foreach (IMvuCommand initialCommand in initialCommands ?? Enumerable.Empty<IMvuCommand>())
                                                                                            dispatchCommand(initialCommand);
                                                                                      },
                                                               cancellationToken
                                                              );
      logger?.LogDebug("final model: {model}", finalModel);
      return finalModel;
   }


   private static TModel init2<TModel>(IMvuProgram2<TModel, TView> program, ILogger? logger, Action<IMvuCommand> dispatchCommand) {
      (TModel initialModel, IMvuCommand[] initialCommands) = program.Init();

      // dispatch initial commands
      foreach (IMvuCommand initialCommand in initialCommands)
         dispatchCommand(initialCommand);

      logger?.LogDebug("* Init:  () -> {initialModel}", initialModel);
      return initialModel;
   }


   private static bool queryTerminationAndGetInitialView2<TModel>(IMvuProgram2<TModel, TView> program, ILogger? logger,
                                                                  Maybe<Func<TView, bool>> queryTerminationAndSimulateInput_,
                                                                  EmitViewDelegate<TView>? emitView,
                                                                  TModel initialModel, MvuMessageDispatchDelegate dispatchMessage) {
      TView initialView = program.View(dispatchMessage, initialModel);
      logger?.LogDebug("* View (initial):  {model} -> {view}", initialModel, initialView);
      emitView?.Invoke(initialView, isInitialView: true);
      bool quit = queryTerminationAndSimulateInput_.Map(queryTerminationAndSimulateInput
                                                              => queryAndLog(queryTerminationAndSimulateInput, initialView, logger))
                                                   .Reduce(false);
      return quit;
   }


   private static (TModel newModel, bool quit) handleNextCommand<TModel>(IMvuProgram2<TModel, TView> program, ILogger? logger, Maybe<Func<TView, bool>> queryTerminationAndSimulateInput_,
                                                                         TModel model, TCmd command,
                                                                         MvuCommandDispatchDelegate dispatchCommand, MvuMessageDispatchDelegate dispatchMessage,
                                                                         Func<IMvuMessage, bool> isQuitMessage,
                                                                         ExecuteEffectDelegate<IMvuEffect> effectAction, EmitViewDelegate<TView>? viewAction, int iterationNumber
         ) 
   //      where TEff : IMvuEffect 
   {
      logger?.LogTrace(">> iteration ({iterationNumber}) beginning: {command}", iterationNumber, command);

      (TModel newModel, bool quit) result = command switch
         {
            IMvuMessageCommand messageCommand => handleNextMessage2(program, logger, queryTerminationAndSimulateInput_, model, messageCommand, dispatchCommand, dispatchMessage, isQuitMessage, viewAction, iterationNumber),
            IMvuEffectCommand   effectCommand => handleNextEffect2 (logger, model, effectCommand, effectAction, iterationNumber),
            _                                 => throw new ArgumentOutOfRangeException(nameof( command ), command, null)
         };

      logger?.LogTrace("<< iteration ({iterationNumber}) returning: {modelAndQuit}", iterationNumber, result);
      return result;
   }


   private static (TModel newModel, bool quit) handleNextMessage2<TModel>(IMvuProgram2<TModel, TView> program, ILogger? logger, Maybe<Func<TView, bool>> queryTerminationAndSimulateInput_,
                                                                          TModel model, IMvuMessageCommand messageCommand,
                                                                          MvuCommandDispatchDelegate dispatchCommand,
                                                                          MvuMessageDispatchDelegate dispatchMessage,
                                                                          Func<IMvuMessage, bool> isQuitMessage,
                                                                          EmitViewDelegate<TView>? emitView, int iterationNumber) {
      IMvuMessage message = messageCommand.Message;
      logger?.LogTrace("-- iteration ({iterationNumber}) - message: {message}", iterationNumber, message);

      (TModel newModel, bool quit) result;

      if (isQuitMessage(message)) {
         logger?.LogDebug("detected QUIT message");
         result = (newModel: model, quit: true);
      }
      else {
         (TModel newModel, IMvuCommand[] commands) = program.Update(dispatchCommand, model, message);
         logger?.LogDebug("* Update:  ({model}, {message}) -> {newModel}", model, message, newModel);

         // dispatch commands
         foreach (IMvuCommand command in commands)
            dispatchCommand(command);

         void dispatchMessageGen(IMvuMessage genMessage) => dispatchMessage(genMessage);
         TView view = program.View(dispatchMessageGen, newModel);
         logger?.LogDebug("* View:  {model} -> {view}", newModel, view);
         emitView?.Invoke(view, isInitialView: false);

         bool quit = queryTerminationAndSimulateInput_.Map(queryTerminationAndSimulateInput
                                                                 => queryAndLog(queryTerminationAndSimulateInput!, view, logger))
                                                      .Reduce(false);
         result = (newModel, quit);
      }

      return result;
   }


   private static (TModel newModel, bool quit) handleNextEffect2<TModel>(ILogger? logger,
                                                                         TModel model, IMvuEffectCommand effectCommand, 
                                                                         ExecuteEffectDelegate<IMvuEffect> effectAction, 
                                                                         int iterationNumber) 
//         where TEff : IMvuEffect
{

      IMvuEffect effect = effectCommand.Effect;
      logger?.LogTrace("-- iteration ({iterationNumber}) - effect: {effect}", iterationNumber, effect);

      // TODO: remove TEff - it shouldn't be needed anymore
         if (effectCommand is IHasEffectResultHandler hasResultHandler)
            effectAction(effect, hasResultHandler);
         else
            throw new NotImplementedException("type " + effectCommand.GetType().Name);

      return (model, quit: false);
   }


   private static bool queryAndLog(Func<TView, bool> queryTerminationAndSimulateInput, TView view, ILogger? logger) {
      bool quit = queryTerminationAndSimulateInput(view);
      logger?.LogTrace("queryTerminationAndSimulateInput: terminate={flag} <- {view}", quit, view);
      return quit;
   }


   // private delegate void DispatchCommandDelegate(IMvuCommand command, Action<IMvuCommand>? handleCommandAction);




   // <-- to bus - dispatch commands
   private static void dispatchCommandToCommonBus(
         IBusWriter<IMvuCommand> commandBusWriter,
         ILogger? logger,
         IMvuCommand command
      ) {
      dispatchCommandWithLog(command);

      void dispatchCommandWithLog(IMvuCommand cmd) { logDispatchCommand(cmd);
                                                     dispatchCommand(cmd); }
      void dispatchCommand       (IMvuCommand cmd) => commandBusWriter.Write(cmd);
      void logDispatchCommand    (IMvuCommand cmd) => logger?.LogDebug("(dispatching {command})", cmd);
   }


   // --> from bus - re-dispatch to program's specific command channel
   private static void handleCommandFromCommonBus(IChannelWriter<TCmd> programCommandChannelWriter,
                                                  ILogger? logger,
                                                  TCmd command) {
      logger?.LogTrace("handling command by forwarding it to PCC: {command}", command);
      programCommandChannelWriter.Write(command);
   }


}


// TODO? make -^ static and move this into it ?
public static class TempExtensions2 {

   public static Func<Task<TModel>> wrapWithCommonBusLink2<TModel, TCmd>(this Func<Task<TModel>> runAsyncFunc,
                                                                   IPublisher<IMvuCommand> commandBusPublisher,
                                                                   Action<TCmd> handleCommandAction)
         where TCmd : IMvuCommand
      => () => runWrappedWithBusLinkAsync2<TModel, TCmd>(runAsyncFunc, commandBusPublisher, handleCommandAction);


   private static Task<TModel> runWrappedWithBusLinkAsync2<TModel, TCmd>(Func<Task<TModel>> runProgramAsync,
                                                                         IPublisher<IMvuCommand> commandBusPublisher,
                                                                         Action<TCmd> handleCommandAction)
         where TCmd : IMvuCommand
      => runProgramAsync.WrapAsync(beforeAction: () => commandBusPublisher.SubscribeTypedMessageHandler<TCmd>(msg => handleCommandAction(msg)), // TODO: also unsubscribe when program ends
                                    afterAction: // <-- TODO: remove the requirement for this
                                                 () => { }
                                  );


   public static Func<Task<TModel>> wrapWithLogs<TModel>(this Func<Task<TModel>> runAsyncFunc, ILogger? logger)
      => () => runWrappedWithLogsAsync(runAsyncFunc, logger);

   public static Func<Task<TModel>> wrapWithLogs2<TModel>(this Func<Task<TModel>> runAsyncFunc, ILogger? logger)
      => () => runWrappedWithLogsAsync2(runAsyncFunc, logger);


   private static async Task<TResult> runWrappedWithLogsAsync<TResult>(Func<Task<TResult>> runAsync, ILogger? logger)
      => await runAsync.WrapAsync(beforeAction:    () => logger?.LogTrace("start"),
                                  afterAction: result => logger?.LogTrace("end, result: {result}", result));

   private static async Task<TResult> runWrappedWithLogsAsync2<TResult>(Func<Task<TResult>> runAsync, ILogger? logger)
      => await runAsync.WrapAsync(beforeAction:    () => logger?.LogTrace("start"),
                                  afterAction: result => logger?.LogTrace("end, result: {result}", result));


   public static Func<Task<TResult>> wrapWithFeedRecording2<TElem, TResult>(this Func<Task<TResult>> runAsyncFunc,
                                                                       IChannelEvents<TElem> channelEvents,
                                                                       Action<TElem>? feedRecorderAction)  // TODO: Option<> instead of nullable
      => () => runWrappedWithFeedRecordingAsync2(runAsyncFunc, feedRecorderAction, channelEvents);


   private static async Task<TResult> runWrappedWithFeedRecordingAsync2<T, TResult>(Func<Task<TResult>> runAsync, Action<T>? feedRecorderAction,
                                                                                    IChannelEvents<T> channelEvents) {
      if (feedRecorderAction is null) {
         return await runAsync();
      }
      else {
         void recordChannelMessage(T message) => feedRecorderAction?.Invoke(message);

         channelEvents.OnRead += recordChannelMessage;
         TResult result = await runAsync();
         channelEvents.OnRead -= recordChannelMessage;
         return result;
      }
   }


   public static Func<Task<TResult>> wrapWithViewRecording2<TView, TResult>(this Func<Task<TResult>> runAsyncFunc,
                                                                            IMvuProgramRunnerEvents<TView> runnerEvents,
                                                                            EmitViewDelegate<TView>? viewRecorderAction) // TODO: Option<> instead of nullable
      => () => runWrappedWithViewRecordingAsync2(runAsyncFunc, viewRecorderAction, runnerEvents);


   private static async Task<TResult> runWrappedWithViewRecordingAsync2<T, TResult>(Func<Task<TResult>> runAsync, EmitViewDelegate<T>? viewRecorderAction,
                                                                                    IMvuProgramRunnerEvents<T> runnerEvents) {
      if (viewRecorderAction is null) {
         return await runAsync();
      }
      else {
         void recordEmittedView(T view, bool isInitial) => viewRecorderAction.Invoke(view, isInitial);

         runnerEvents.ViewEmitted += recordEmittedView;
         TResult result = await runAsync();
         runnerEvents.ViewEmitted -= recordEmittedView;
         return result;
      }
   }

}
