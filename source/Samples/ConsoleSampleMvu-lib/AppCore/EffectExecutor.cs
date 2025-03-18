using System;
using System.Threading.Tasks;
using ConsoleSampleMvu.AppCore.Services;
using CounterMvu_lib.Effects;



namespace ConsoleSampleMvu.AppCore;

internal class EffectExecutor : IEffects {
   private readonly IAppServices _services;

   internal EffectExecutor(IAppServices services) {
      _services = services;
   }


   public async ValueTask<int> GenerateRandomNumberAsync() {
      // not really async
      await Task.CompletedTask;
      return _services.GenerateRandomNumber();
   }
}
