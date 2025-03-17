using System;
using WelterKit.Std.StaticUtilities;
using yamvu.core.Primitives;


namespace ConsoleSampleMvu.Mvu.Messages;

public record SetKeyCharMessage(char KeyChar) : Message {
   public override string ToString() => $"SetKeyChar({KeyChar})".SurroundWith('$');


   public static (Model, IMvuCommand[]) Handle(Model currentModel, char keyChar)
      => (currentModel with
             {
                KeyChar = keyChar,
             },
          []
         );
}


public static partial class MvuMessages {
   public static Message SetKeyChar(char keyChar) => new SetKeyCharMessage(keyChar);
}
