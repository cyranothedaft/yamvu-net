using System;
using Microsoft.Extensions.Logging;


namespace CounterSample.AppCore.Services;

public class AppServices_Real : IAppServices {
   private static readonly Random _rng = new Random(); // seeds with datetime ticks by default

   private readonly ILogger? _serviceLogger;


   public AppServices_Real(ILogger? serviceLogger) {
      _serviceLogger = serviceLogger;
   }


   public int GenerateRandomNumber() {
      int number = _rng.Next(minValue: 2, maxValue: 999);
      _serviceLogger?.LogDebug("Generated random number: {number}", number);
      return number;
   }
}
