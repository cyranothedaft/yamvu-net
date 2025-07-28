using System;

namespace yamvu.Extensions.WinForms;

public record ExternalInputBindings(
      Action? FormClosed
) {
   public ExternalInputBindings() : this(FormClosed: null) { }
}
