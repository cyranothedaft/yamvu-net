using System;

namespace WinUI3CounterSample.View;

public record ViewInputBindings(
      Action? Increment1ButtonPressed,
      Action? IncrementRandomButtonPressed
) {
   public ViewInputBindings()
         : this(null, null) { }
}
