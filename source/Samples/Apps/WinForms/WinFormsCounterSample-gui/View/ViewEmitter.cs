using System;
using System.Linq;
using System.Windows.Forms;



namespace WinFormsCounterSample.View;

internal static class ViewEmitter {
   public static void DisplayView(Control componentContainer, PlatformView<ProgramView> view) {
      // update the (admitedly mutable) key bindings table according to invoke functions in the latest view
      // updateBindingsAction(view.InputBindings);


      //Console.WriteLine("/~~ The current View ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      //foreach (string line in view.TextLines) {
      //   // indent each line
      //   Console.WriteLine("|  " + line);
      //}
   }
}
