using System;


namespace ConsoleSample.PlatAgnAppCore;

public interface IAppServices {
   DateTimeOffset GetCurrentTime_utc();
}
