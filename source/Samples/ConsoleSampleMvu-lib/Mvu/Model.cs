using System;

namespace ConsoleSampleMvu.Mvu;

public record Model(
      char? KeyChar
      // DateTimeOffset? Time
) {
   public Model() : this(KeyChar: null) { }
}
