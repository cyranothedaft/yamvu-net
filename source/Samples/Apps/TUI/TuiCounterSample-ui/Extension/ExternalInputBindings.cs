using System;

namespace yamvu.Extensions.Tui;

public record ExternalInputBindings(
      Action? MainWindowClosed
) {
   public ExternalInputBindings() : this(MainWindowClosed: null) { }
}
