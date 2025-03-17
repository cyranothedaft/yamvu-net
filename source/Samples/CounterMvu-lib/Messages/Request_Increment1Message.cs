using System;
using WelterKit.Std.StaticUtilities;
using yamvu.core;
using yamvu.core.Primitives;


namespace CounterMvu_lib.Messages;


public static partial class MvuMessages {
   public static Message Request_Increment1() => new Request_Increment1Message();
}


public record Request_Increment1Message() : Message {
   public override string ToString() => $"Request:Increment1".SurroundWith('$');


   public static (Model, IMvuCommand[]) Handle(MvuCommandDispatchDelegate dispatch, Model currentModel)
      => (currentModel,
          [ MvuMessages.Request_Increment1().AsCommand() ]
         );
}
