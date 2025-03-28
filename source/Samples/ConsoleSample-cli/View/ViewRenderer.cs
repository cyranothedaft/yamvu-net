using System;


namespace ConsoleSample.View;

internal static class ViewRenderer {
   public static void DisplayView(PlatformView<ProgramView> view, Action<ViewInputBindings> updateBindingsAction) {
      // update the (admitedly mutable) key bindings table according to invoke functions in the latest view
      updateBindingsAction(view.InputBindings);

      Console.WriteLine("~~~ The View -- displayed with every MVU message processed ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      foreach (string line in view.MvuView.TextLines) {
         Console.WriteLine(line);
      }

      //Console.WriteLine("/~~ The current View ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      //foreach (string line in view.TextLines) {
      //   // indent each line
      //   Console.WriteLine("|  " + line);
      //}
   }
}
