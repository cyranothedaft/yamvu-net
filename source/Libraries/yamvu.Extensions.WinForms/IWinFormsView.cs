using System;
using System.Collections.Immutable;
using System.Windows.Forms;


namespace yamvu.Extensions.WinForms;

public interface IWinFormsView {
   IImmutableList<Control> Contents { get; }
   ExternalInputBindings ExternalInputBindings { get; }
}
