using System;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.Runners;



namespace CounterSample.AppCore.Mvu.Program;

public static class Program {
   public const string Name = "CounterSample";
   public static readonly ProgramInfo Info = new ProgramInfo(Name);

   // public static Func<IMvuMessage, MvuMessageCommand> MessageAsCommandFunc => MessageExtensions.AsCommand;
   // public static Func<IMvuMessage, bool> IsQuitMessageFunc => msg => msg is Request_QuitMessage;


   public static IMvuProgram2<Model, TView> Build<TView>(ViewDelegate<Model, TView> viewFunc, ILogger? programLogger)
      => new ProgramWithView<TView>(new ProgramUpdate(programLogger), viewFunc);


   public static IMvuProgramRunner<TView> BuildRunner<TView>(Func<string, ILogger?> createLoggerFunc)
      => new ProgramRunner2<ICounterMvuCommand, TView>(createLoggerFunc("run"));
}
