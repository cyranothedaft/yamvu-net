using System;
using System.Collections.Immutable;
using System.Windows.Forms;
using yamvu.Extensions.WinForms;


namespace WinFormsCounterSample.View;

// because of how many UI platforms work, the visual part of the view is coupled with the input-event handling part
// to support that: the 'view' that's created has two components: (1) the view itself (the raw data for rendering the view: ProgramView),
// and (2) a table of input bindings (definitions of what happens when user input occurs: ViewInputBindings).
// 
public record ProgramView(
      IImmutableList<Control> Contents,
      ViewInputBindings InputBindings,
      ExternalInputBindings ExternalInputBindings
) : IWinFormsView;
