using System;
using WelterKit.Std.StaticUtilities;


namespace CounterSample.AppCore.Mvu.Messages;


public static partial class MvuMessages {
   public static Message Request_Quit() => new Request_QuitMessage();
}


public record Request_QuitMessage() : Message {
   public override string ToString() => $"Request:Quit".SurroundWith('$');
}
