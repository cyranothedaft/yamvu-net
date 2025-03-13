using System;
using WelterKit.Std.StaticUtilities;
using yamvu.core.Primitives;


namespace ConsoleSample.Mvu.Messages;

public record SetCharMessage(char Char) : Message {
   public override string ToString() => $"SetChar({Char})".SurroundWith('$');


   public static (Model, IMvuCommand[]) Handle(Model currentModel, char ch)
      => (currentModel with
             {
                Char = ch,
             },
             []
         );
}


public static partial class MvuMessages {
   public static Message SetChar(char ch) => new SetCharMessage(ch);
}
