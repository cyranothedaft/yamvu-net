using System.Windows.Forms;



namespace WinFormsCounterSample.gui.UI;

public partial class MainForm : Form {

   internal MvuContainerWrapper Program1Container { get; }
   internal MvuContainerWrapper Program2Container { get; }


   // N.B.: the form's DoubleBuffered property has also been set to true, to try reducing flickering when view components are replaced
   public MainForm() {
      InitializeComponent();
      Program1Container =  MvuContainerWrapper.AttachToForm(this, this.Program1GroupBox);
      Program2Container =  MvuContainerWrapper.AttachToForm(this, this.Program2GroupBox);
   }


}
