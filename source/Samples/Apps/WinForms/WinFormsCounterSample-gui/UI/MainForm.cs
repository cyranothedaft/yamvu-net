using System;
using System.Windows.Forms;
using WinFormsCounterSample.gui.ViewPlatform;



namespace WinFormsCounterSample.gui.UI;

public partial class MainForm : Form, IMvuControlContainer {
   public MainForm() {
      InitializeComponent();
   }


   public Control MvuComponentContainer => this;
}
