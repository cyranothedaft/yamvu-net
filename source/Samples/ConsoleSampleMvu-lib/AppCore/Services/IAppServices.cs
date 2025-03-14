using System;


namespace ConsoleSampleMvu.AppCore.Services;

public interface IAppServices {
   DateTimeOffset GetCurrentTime_utc();
}
