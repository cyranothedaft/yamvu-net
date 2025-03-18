using System;

namespace ConsoleSample.View;

public record ViewInputBindings(
      Action? QuitButtonPressed,
      Action? Increment1ButtonPressed,
      Action? IncrementRandomButtonPressed
) {
   public ViewInputBindings()
         : this(null, null, null) { }
}
