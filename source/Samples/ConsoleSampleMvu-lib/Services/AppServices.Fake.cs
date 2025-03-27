using System;
using Microsoft.Extensions.Logging;


namespace CounterSample.AppCore.Services;

internal class AppServices_Fake : IAppServices {


   public const int NotSoRandomNumber = 42;


   private readonly ILogger? _serviceLogger;


   public AppServices_Fake(ILogger? serviceLogger) {
      _serviceLogger = serviceLogger;
   }


   public int GenerateRandomNumber()
      => NotSoRandomNumber;
}
