using System;
using WelterKit.Std.StaticUtilities;



namespace ConsoleSample.Mvu.Messages;

public record Request_QuitMessage() : Message {
   public override string ToString() => $"Request:Quit".SurroundWith('$');
}


public static partial class MvuMessages {
   public static Message Request_Quit() => new Request_QuitMessage();
}
