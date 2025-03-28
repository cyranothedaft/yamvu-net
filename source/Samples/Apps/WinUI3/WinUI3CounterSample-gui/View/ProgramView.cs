using System;
using System.Collections.Immutable;
using Microsoft.UI.Xaml;



namespace WinUI3CounterSample.View;

internal record ProgramView(
      ImmutableList<UIElement> Controls
);
