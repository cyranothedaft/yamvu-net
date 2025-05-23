using System;
using System.Collections.Immutable;
using Microsoft.UI.Xaml;
using yamvu.Extensions.WinUI3;


namespace WinUI3CounterSample.View;

internal record ProgramView(
      IImmutableList<UIElement> Contents,
      ViewInputBindings InputBindings,
      ExternalInputBindings externalInputBindings
) : IWinUI3View;
