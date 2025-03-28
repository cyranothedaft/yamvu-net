using System;
using System.Threading.Tasks;
using CounterMvu_lib.Effects;
using CounterSample.AppCore.Services;



namespace CounterSample.AppCore;

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
