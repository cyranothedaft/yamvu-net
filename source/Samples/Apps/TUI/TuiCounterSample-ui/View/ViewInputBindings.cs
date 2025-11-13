using System;

namespace TuiCounterSample.ui.View;

public record ViewInputBindings(
      Action? Increment1ButtonPressed,
      Action? IncrementRandomButtonPressed
) {
   public ViewInputBindings()
         : this(null, null) { }
}
