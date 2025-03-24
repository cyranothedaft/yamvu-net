using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;



namespace WinFormsCounterSample.gui;


internal class WinFormProgram {
   private readonly MainForm _mainForm;
   private Control _componentContainer;


   public event Action? MainFormCloseRequested;


   public WinFormProgram() {
      _mainForm = new MainForm();
   }


   // public Control ComponentContainer { get; }


   public void ReplaceView(ProgramView view) {
      _componentContainer.SuspendLayout();
      _componentContainer.Controls.Clear();
      _componentContainer.Controls.AddRange(view.Controls.ToArray());
      _componentContainer.ResumeLayout();
      _componentContainer.Invalidate();
   }


   public void AttachFormBehavior(Action<MainForm> formAction) {
      formAction(_mainForm);
   }


   public async Task StartAsync() {
      _mainForm.FormClosed += MainForm_FormClosed;
      await _mainForm.InitializeAsync();
      _mainForm.Show();
   }


   private void MainForm_FormClosed(object? sender, FormClosedEventArgs e) {
      OnExitRequested(EventArgs.Empty);
   }


   #region Events

   public event EventHandler<EventArgs>? ExitRequested;


   protected virtual void OnExitRequested(EventArgs e) {
      ExitRequested?.Invoke(this, e);
   }

   #endregion Events


}
