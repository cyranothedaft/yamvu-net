using System;
using yamvu.core.Primitives;

namespace yamvu;

public class ExternalMessageDispatcher {
   public event Action<IMvuMessage>? MessageFromOutside; // TODO: Command instead of Message?


   public void Dispatch(IMvuMessage message) {
      MessageFromOutside?.Invoke(message);
   }
}
