using System;
using System.Collections.Immutable;
using System.Windows.Forms;



namespace WinFormsCounterSample.View;

internal record ProgramView(
      ImmutableList<Control> Controls
);
