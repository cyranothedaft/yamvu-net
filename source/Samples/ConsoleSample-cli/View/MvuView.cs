﻿using System;
using System.Collections.Immutable;


namespace ConsoleSample.View;

internal record MvuView(
      ImmutableList<string> TextLines
);
