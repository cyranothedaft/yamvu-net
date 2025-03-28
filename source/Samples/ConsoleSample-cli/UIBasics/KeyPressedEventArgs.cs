using System.ComponentModel;



namespace ConsoleSample.UIBasics;

public class KeyPressedEventArgs : CancelEventArgs {
   public ConsoleKeyInfo KeyInfo { get; }


   public KeyPressedEventArgs(ConsoleKeyInfo keyInfo) {
      KeyInfo = keyInfo;
   }


   public KeyPressedEventArgs(ConsoleKeyInfo keyInfo, bool cancel)
         : base(cancel) {
      KeyInfo = keyInfo;
   }
}
