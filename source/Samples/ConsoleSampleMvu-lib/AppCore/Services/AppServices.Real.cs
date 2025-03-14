using System;
using Microsoft.Extensions.Logging;


namespace ConsoleSampleMvu.AppCore.Services;

internal class AppServices : IAppServices {
   private readonly ILogger? _serviceLogger;


   public AppServices(ILogger? serviceLogger) {
      _serviceLogger = serviceLogger;
   }


   public DateTimeOffset GetCurrentTime_utc()
      => DateTimeOffset.UtcNow;
}
