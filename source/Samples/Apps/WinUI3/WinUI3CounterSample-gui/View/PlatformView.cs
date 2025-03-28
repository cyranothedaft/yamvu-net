using System;

namespace WinUI3CounterSample.View;

public record PlatformView<TView>(
      TView MvuView,
      ViewInputBindings InputBindings,
      ExternalInputBindings externalInputBindings
);
