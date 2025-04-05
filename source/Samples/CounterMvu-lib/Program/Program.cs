using CounterMvu_lib.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Runners;



namespace CounterMvu_lib.Program;

public static class Program {
   public const string Name = "CounterSample";
   public static readonly ProgramInfo Info = new ProgramInfo(Name);


   public static Func<IMvuMessage, MvuMessageCommand> MessageAsCommandFunc => MessageExtensions.AsCommand;
   public static Func<IMvuMessage, bool> IsQuitMessageFunc => msg => msg is Request_QuitMessage;


   public static IMvuProgram2<Model, TView> Build<TView>(ViewDelegate<Model, TView> viewFunc, ILogger? programLogger)
      => new ProgramWithView<TView>(new ProgramUpdate(programLogger), viewFunc);


   public static IMvuProgramRunner<TView> BuildRunner<TView>(ILoggerFactory? loggerFactory)
      => new ProgramRunner2<ICounterMvuCommand, TView>(loggerFactory?.CreateLogger("run"));
}
