using System;
using System.Threading.Tasks;
using ConsoleSampleMvu.AppCore.Services;
using ConsoleSampleMvu.Mvu.Effects;



namespace ConsoleSampleMvu.AppCore;

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
