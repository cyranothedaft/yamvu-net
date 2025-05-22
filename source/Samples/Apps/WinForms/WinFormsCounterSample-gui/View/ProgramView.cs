using System;
using System.Collections.Immutable;
using System.Windows.Forms;



namespace WinFormsCounterSample.View;

public record ProgramView(
      ImmutableList<Control> Controls
);
