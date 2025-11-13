using System;
using System.Collections.Immutable;
using Terminal.Gui.ViewBase;


namespace yamvu.Extensions.Tui;

public interface ITuiView {
   ImmutableList<View> Contents { get; }
}
