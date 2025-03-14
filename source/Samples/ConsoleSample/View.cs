using System;
using System.Collections.Immutable;



namespace ConsoleSample;

internal record View(
      ImmutableList<string> TextLines
);
