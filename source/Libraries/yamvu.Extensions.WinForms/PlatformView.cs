using System;

namespace yamvu.Extensions.WinForms;

public record PlatformView<TView>(
      TView MvuView,
      ExternalInputBindings ExternalInputBindings
);
