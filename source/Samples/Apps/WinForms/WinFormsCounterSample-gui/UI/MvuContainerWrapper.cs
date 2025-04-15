using System;
using System.Windows.Forms;
using WinFormsCounterSample.gui.ViewPlatform;


namespace WinFormsCounterSample.gui.UI;

internal class MvuContainerWrapper : IMvuControlContainer {
   public event EventHandler<EventArgs>? ContainerLoaded;
   public event EventHandler<FormClosingEventArgs>? ContainerClosing;

   private readonly Action _closeAction;

   public Control ContainerControl { get; }

   private MvuContainerWrapper(Control containerControl, Action closeAction) {
      ContainerControl  = containerControl;
      _closeAction = closeAction;
   }

   public void Close() => _closeAction();

   private void SignalLoaded(object? sender, EventArgs args) => ContainerLoaded?.Invoke(sender,args);
   private void SignalClosing(object? sender, FormClosingEventArgs args) => ContainerClosing?.Invoke(sender, args);


   public static MvuContainerWrapper AttachToForm(Form parentForm, Control containerControl) {
      var wrapper = new MvuContainerWrapper(containerControl, closeAction: parentForm.Close);
      parentForm.Load        += wrapper.SignalLoaded;
      parentForm.FormClosing += wrapper.SignalClosing;
      return wrapper;
   }
}
