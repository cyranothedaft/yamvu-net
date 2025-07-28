using System;

namespace WinUI3CounterSample.View;

public record ExternalInputBindings(
      Action? WindowClosed
) {
   public ExternalInputBindings() : this(WindowClosed: null) { }
}
