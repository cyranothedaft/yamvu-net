using System;
using WelterKit.Std.StaticUtilities;
using yamvu.core.Primitives;


namespace CounterMvu_lib.Messages;


public static partial class MvuMessages {
   public static Message IncrementCounter(int Increment) => new IncrementCounterMessage(Increment);
}


public record IncrementCounterMessage(int Increment) : Message {
   public override string ToString() => $"IncrementCounter({Increment:O})".SurroundWith('$');


   public static (Model, IMvuCommand[]) Handle(Model currentModel, int increment)
      => (currentModel with
             {
                Counter = currentModel.Counter + increment,
             },
          []
         );
}
