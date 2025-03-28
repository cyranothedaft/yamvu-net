using System;
using System.Collections.Immutable;


namespace ConsoleSample.View;

internal record ProgramView(
      ImmutableList<string> TextLines
);
