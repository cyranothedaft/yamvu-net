using System;
using System.Windows.Forms;
using yamvu.Extensions.WinForms;



namespace WinFormsCounterSample.gui.UI;

public partial class MainForm : Form, IMvuControlContainer {
   public MainForm() {
      InitializeComponent();
   }

   // N.B.: the form's DoubleBuffered property has also been set to true, to try reducing flickering when view components are replaced
   public Control MvuComponentContainer => this;
}
