using System;

namespace WinFormsCounterSample.View;

public record PlatformView<TView>(
      TView MvuView,
      ViewInputBindings InputBindings,
      ExternalInputBindings externalInputBindings
);
