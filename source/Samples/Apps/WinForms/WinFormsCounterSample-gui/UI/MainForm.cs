using System;
using System.Windows.Forms;
using WinFormsCounterSample.gui.ViewPlatform;



namespace WinFormsCounterSample.gui.UI;

public partial class MainForm : Form, IMvuControlContainer {

   public Control MvuComponentContainer => Program1GroupBox;

   public event EventHandler<EventArgs>? ContainerLoaded;
   public event EventHandler<FormClosingEventArgs>? ContainerClosing;

   protected override void OnLoad(EventArgs e) => ContainerLoaded?.Invoke(this, e);
   protected override void OnFormClosing(FormClosingEventArgs e) => ContainerClosing?.Invoke(this, e);


   // N.B.: the form's DoubleBuffered property has also been set to true, to try reducing flickering when view components are replaced
   public MainForm() {
      InitializeComponent();
   }

}
