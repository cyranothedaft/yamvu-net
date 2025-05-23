using System.Windows.Forms;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.Extensions.WinForms;



namespace WinFormsCounterSample.View;

internal class ViewBuilder {

   public static ProgramView BuildInitialView()
      => new([ buildInitialView() ],
             new ViewInputBindings(),
             new ExternalInputBindings());


   public static ProgramView BuildViewFromModel(MvuMessageDispatchDelegate dispatch, Model model, ILogger? uilogger) {
      // (TODO: this isn't ideal) All events that generate messages (both external and internal to the view) must be funnelled through the view,
      // because that's where the dispatch delegate is known to be.
      ExternalInputBindings externalInputBindings = new ExternalInputBindings(FormClosed: () => dispatch(MvuMessages.Request_Quit()));
      ViewInputBindings viewInputBindings = new ViewInputBindings(Increment1ButtonPressed: () => dispatch(MvuMessages.Request_Increment1()),
                                                                  IncrementRandomButtonPressed: () => dispatch(MvuMessages.Request_IncrementRandom()));

      Control mainPanel = buildMainPanel(model, viewInputBindings);

      ProgramView platformView = new ProgramView([ mainPanel ],
                                                 viewInputBindings,
                                                 externalInputBindings);

      return platformView;
   }


   private static Control buildInitialView() {
      FlowLayoutPanel panel = new FlowLayoutPanel()
                                 {
                                    FlowDirection = FlowDirection.TopDown,
                                    Dock          = DockStyle.Fill
                                 };
      panel.Controls.AddRange([
            new Label() { Text = "(not initialized)", AutoSize = true },
         ]);

      return panel;
   }


   private static Control buildMainPanel(Model model, ViewInputBindings inputBindings) {
      FlowLayoutPanel panel = new FlowLayoutPanel()
                                 {
                                    FlowDirection = FlowDirection.TopDown,
                                    Dock          = DockStyle.Fill
                                 };
      Button increment1Button = new Button() { Text = "Increment 1", AutoSize = true };
      Button incrementRandomButton = new Button() { Text = "Increment Random", AutoSize = true };
      increment1Button.Click      += (_, _) => inputBindings.Increment1ButtonPressed?.Invoke();
      incrementRandomButton.Click += (_, _) => inputBindings.IncrementRandomButtonPressed?.Invoke();
      panel.Controls.AddRange([
            new Label() { Text = "Counter:", AutoSize = true },
            new Label() { Text = model.Counter.ToString(), AutoSize = true },
            increment1Button,
            incrementRandomButton,
         ]);

      return panel;
   }
}
