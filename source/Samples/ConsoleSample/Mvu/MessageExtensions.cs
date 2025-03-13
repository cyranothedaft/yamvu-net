using System;
using yamvu.core.Primitives;



namespace ConsoleSample.Mvu;

public static class MessageExtensions {
   public static MvuMessageCommand AsCommand(this IMvuMessage message)
      => new WindowRecallMessageCommand(message);
}


public sealed record WindowRecallMessageCommand(IMvuMessage Message)
      : MvuMessageCommand(Message), IConsoleSampleCommand {
}
