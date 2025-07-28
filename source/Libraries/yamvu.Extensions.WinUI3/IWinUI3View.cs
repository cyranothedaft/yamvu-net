using System;
using System.Collections.Immutable;
using Microsoft.UI.Xaml;



namespace yamvu.Extensions.WinUI3;

public interface IWinUI3View {
   IImmutableList<UIElement> Contents { get; }
}
