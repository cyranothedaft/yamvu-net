using System;
using System.Windows.Forms;



namespace WinFormsCounterSample.gui.ViewPlatform;

internal interface IMvuControlContainer {
   Control MvuComponentContainer { get; }
}
