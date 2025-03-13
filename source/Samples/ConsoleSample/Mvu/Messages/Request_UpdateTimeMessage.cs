using System;
using ConsoleSample.Mvu.Effects;
using WelterKit.Std.StaticUtilities;
using yamvu.core;
using yamvu.core.Primitives;



namespace ConsoleSample.Mvu.Messages;

public record Request_UpdateTimeMessage() : Message {
   public override string ToString() => $"Request:UpdateTime".SurroundWith('$');


   public static (Model, IMvuCommand[]) Handle(MvuCommandDispatchDelegate dispatch, Model currentModel)
      => (currentModel,
          [
             MvuEffects.RetrieveCurrentTime(time
                   => dispatch(MvuMessages.SetTime(time).AsCommand()))
          ]
         );
}



public static partial class MvuMessages {
   public static Message Request_UpdateTime() => new Request_UpdateTimeMessage();
}
