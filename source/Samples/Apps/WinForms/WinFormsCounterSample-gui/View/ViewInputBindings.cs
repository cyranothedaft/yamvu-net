using System;

namespace WinFormsCounterSample.View;

public record ViewInputBindings(
      Action? Increment1ButtonPressed,
      Action? IncrementRandomButtonPressed
) {
   public ViewInputBindings()
         : this(null, null) { }
}
