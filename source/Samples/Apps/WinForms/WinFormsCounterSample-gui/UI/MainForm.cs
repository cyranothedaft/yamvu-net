using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsCounterSample.gui.ViewPlatform;



namespace WinFormsCounterSample.gui.UI;

public partial class MainForm : Form, IMvuControlContainer {
   public MainForm() {
      InitializeComponent();
   }


   public Control MvuComponentContainer => this;


   public async Task InitializeAsync() {
      // TODO: do stuff
      await Task.Delay(1000);
   }


}
