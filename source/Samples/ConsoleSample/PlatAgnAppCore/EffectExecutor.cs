using System;
using ConsoleSample.Mvu.Effects;



namespace ConsoleSample.PlatAgnAppCore;

internal class EffectExecutor : IEffects {
   private readonly IAppServices _services;

   internal EffectExecutor(IAppServices services) {
      _services = services;
   }


   public async ValueTask<DateTimeOffset> RetrieveCurrentTimeAsync() {
      // not really async
      await Task.CompletedTask;
      return _services.GetCurrentTime_utc();
   }
}
