using System;
using System.Collections.Immutable;
using yamvu.Extensions.Tui;


namespace TuiCounterSample.ui.View;

public record ProgramView(
      ImmutableList<Terminal.Gui.ViewBase.View> Contents,
      ViewInputBindings InputBindings,
      ExternalInputBindings ExternalInputBindings
) : ITuiView;
