using System.Threading.Tasks;
using System.Windows.Forms;


namespace WinFormsCounterSample.gui.UI;

public partial class MainForm : Form {
   public MainForm() {
      InitializeComponent();
   }


   public async Task InitializeAsync() {
      // TODO: do stuff
      await Task.Delay(1000);
   }
}
