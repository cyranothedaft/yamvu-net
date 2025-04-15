using System.Windows.Forms;
using WinFormsCounterSample.gui.ViewPlatform;



namespace WinFormsCounterSample.gui.UI;

public partial class MainForm : Form {

   private readonly MvuContainerWrapper _program1Container;
   internal IMvuControlContainer Program1Container => _program1Container;

   // protected override void OnLoad(EventArgs e) => ContainerLoaded?.Invoke(this, e);
   // protected override void OnFormClosing(FormClosingEventArgs e) => ContainerClosing?.Invoke(this, e);


   // N.B.: the form's DoubleBuffered property has also been set to true, to try reducing flickering when view components are replaced
   public MainForm() {
      InitializeComponent();
      _program1Container =  MvuContainerWrapper.AttachToForm(this, this.Program1GroupBox);
   }



}
