using System;
using System.Collections.Generic;


namespace TuiCounterSample.ui;

public static class TuiHelper {
   public static void ReplaceViewContents(Terminal.Gui.ViewBase.View view, IEnumerable<Terminal.Gui.ViewBase.View> subviews) {
      view.RemoveAll();
      foreach (Terminal.Gui.ViewBase.View subview in view.SubViews )
         view.Add(subview);
      view.Draw();
   }
}
