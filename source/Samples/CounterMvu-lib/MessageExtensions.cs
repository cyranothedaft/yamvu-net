using System;
using yamvu.core.Primitives;


namespace CounterMvu_lib;

public static class MessageExtensions {
   public static MvuMessageCommand AsCommand(this IMvuMessage message)
      => new CounterMvuMessageCommand(message);
}


public sealed record CounterMvuMessageCommand(IMvuMessage Message)
      : MvuMessageCommand(Message), ICounterMvuCommand {
}
