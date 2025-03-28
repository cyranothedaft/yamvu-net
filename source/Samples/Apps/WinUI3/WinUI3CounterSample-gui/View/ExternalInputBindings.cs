using System;

namespace WinUI3CounterSample.View;

public record ExternalInputBindings(
      Action? FormClosed
) {
   public ExternalInputBindings() : this(FormClosed: null) { }
}
