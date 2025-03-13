using Microsoft.Extensions.Logging;
using yamvu.core;



namespace ConsoleSample.Mvu;

public static class Program {
   public const string Name = nameof( ConsoleSample );
   public static readonly ProgramInfo Info = new ProgramInfo(Name);


   public static IMvuProgram2<Model, TView> Build<TView>(ViewDelegate<Model, TView> viewFunc, ILogger? logger)
      => new ProgramWithView<TView>(new ProgramUpdate(logger), viewFunc);
}
