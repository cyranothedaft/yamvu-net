using System;

namespace WinFormsCounterSample.View;

public record ExternalInputBindings(
      Action? FormClosed
) {
   public ExternalInputBindings()
         : this(FormClosed: null) { }
}
