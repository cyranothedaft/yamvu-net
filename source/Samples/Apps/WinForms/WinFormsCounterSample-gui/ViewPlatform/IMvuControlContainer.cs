using System;
using System.Windows.Forms;



namespace WinFormsCounterSample.gui.ViewPlatform;

public interface IMvuControlContainer {
   Control MvuComponentContainer { get; }
}
