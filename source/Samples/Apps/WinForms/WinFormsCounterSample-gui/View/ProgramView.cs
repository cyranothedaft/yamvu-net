using System;
using System.Collections.Immutable;
using System.Windows.Forms;
using yamvu.Extensions.WinForms;


namespace WinFormsCounterSample.View;

public record ProgramView(
      IImmutableList<Control> Contents,
      ViewInputBindings InputBindings,
      ExternalInputBindings ExternalInputBindings
) : IWinFormsView;
