using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;



namespace WinFormsCounterSample.gui;


internal class WinFormProgram {
   private readonly MainForm _mainForm;

   private bool _wasAlreadyStarted = false;
   private bool _isFormClosing = false;
   // private bool _wasExitAlreadyRequested = false;


   public WinFormProgram() {
      _mainForm = new MainForm();
   }


   // public Control ComponentContainer { get; }


   public void ReplaceView(ProgramView view) {
      replaceView(view, _mainForm.MvuComponentContainer);
   }


   private static void replaceView(ProgramView view, Control componentContainer) {
      componentContainer.SuspendLayout();
      componentContainer.Controls.Clear();
      componentContainer.Controls.AddRange(view.Controls.ToArray());
      componentContainer.ResumeLayout();
      componentContainer.Invalidate();
   }


   public void InternalExitRequested() {
      // if ( _wasExitAlreadyRequested ) return;
      // _wasExitAlreadyRequested = true;

      if ( !_isFormClosing )
         _mainForm.Close();
   }


   public async Task StartAsync() {
      if ( _wasAlreadyStarted ) return;
      _wasAlreadyStarted = true;

      _mainForm.FormClosed += MainForm_FormClosed;
      await _mainForm.InitializeAsync();
      _mainForm.Show();
   }


   private void MainForm_FormClosed(object? sender, FormClosedEventArgs e) {
      _isFormClosing = true;
      OnExitRequested(EventArgs.Empty);
   }


   #region Events

   public event EventHandler<EventArgs>? ExitRequested;


   protected virtual void OnExitRequested(EventArgs e) {
      ExitRequested?.Invoke(this, e);
   }

   #endregion Events


}
