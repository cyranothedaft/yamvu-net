using System;
using CounterMvu_lib.Effects;
using WelterKit.Std.StaticUtilities;
using yamvu.core;
using yamvu.core.Primitives;


namespace CounterMvu_lib.Messages;


public static partial class MvuMessages {
   public static Message Request_IncrementRandom() => new Request_IncrementRandomMessage();
}


public record Request_IncrementRandomMessage() : Message {
   public override string ToString() => $"Request:IncrementRandom".SurroundWith('$');


   public static (Model, IMvuCommand[]) Handle(MvuCommandDispatchDelegate dispatch, Model currentModel)
      => (currentModel,
          [
             MvuEffects.GenerateRandomNumber(randomNumber
                   => dispatch(MvuMessages.IncrementCounter(randomNumber).AsCommand()))
          ]
         );
}
