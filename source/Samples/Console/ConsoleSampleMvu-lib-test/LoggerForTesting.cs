using System;
using Microsoft.Extensions.Logging;



namespace ConsoleSampleMvu_lib_test;

internal class LoggerForTesting : ILogger {
   private readonly LogLevel _minimumLogLevel;


   public LoggerForTesting(LogLevel minimumLogLevel = LogLevel.Trace) {
      _minimumLogLevel = minimumLogLevel;
   }


   public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
      if (IsEnabled(logLevel))
         Console.WriteLine("## " + formatter(state, exception));
   }


   public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLogLevel;


   public IDisposable? BeginScope<TState>(TState state) where TState : notnull
      => new EmptyDisposable(); // TODO?



   private class EmptyDisposable : IDisposable {
      public void Dispose() { }
   }
}
