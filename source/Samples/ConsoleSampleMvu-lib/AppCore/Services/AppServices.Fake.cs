using System;
using Microsoft.Extensions.Logging;


namespace ConsoleSampleMvu.AppCore.Services;

internal class AppServices_Fake : IAppServices {


   public const int NotSoRandomNumber = 42;


   private readonly ILogger? _serviceLogger;


   public AppServices_Fake(ILogger? serviceLogger) {
      _serviceLogger = serviceLogger;
   }


   public int GenerateRandomNumber()
      => AppServices_Fake.NotSoRandomNumber;
}
