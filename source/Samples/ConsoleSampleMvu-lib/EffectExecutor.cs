using System;
using System.Threading.Tasks;
using CounterSample.AppCore.Mvu.Effects;
using CounterSample.AppCore.Services;



namespace CounterSample.AppCore;

public class EffectExecutor : IEffects {
   private readonly IAppServices _services;

   public EffectExecutor(IAppServices services) {
      _services = services;
   }


   public async ValueTask<int> GenerateRandomNumberAsync() {
      // not really async
      await Task.CompletedTask;
      return _services.GenerateRandomNumber();
   }
}
