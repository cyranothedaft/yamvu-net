using System;
using System.Windows.Forms;



namespace WinFormsCounterSample.gui.ViewPlatform;

internal interface IMvuControlContainer {
   event EventHandler<EventArgs>? ContainerLoaded;
   event EventHandler<FormClosingEventArgs>? ContainerClosing;

   Control ContainerControl { get; }

   void Close();
}
