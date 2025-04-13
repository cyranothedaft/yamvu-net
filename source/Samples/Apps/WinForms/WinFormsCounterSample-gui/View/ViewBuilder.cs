using System.Windows.Forms;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;


namespace WinFormsCounterSample.View;

internal class ViewBuilder {

   public static PlatformView<ProgramView> BuildInitialView()
      => new(new ProgramView([ buildInitialView() ]),
             new ViewInputBindings(),
             new ExternalInputBindings());


   // because of how many UI platforms work, the visual part of the view is coupled with the input-event handling part
   // to support that: the 'view' that's created has two components: (1) the view itself (the raw data for rendering the view: ProgramView),
   // and (2) a table of input bindings (definitions of what happens when user input occurs: ViewInputBindings).
   // 

   public static PlatformView<ProgramView> BuildViewFromModel(MvuMessageDispatchDelegate dispatch, Model model, ILogger? uilogger) {

      // (TODO: this isn't ideal) All events that generate messages (both external and internal to the view) must be funnelled through the view,
      // because that's where the dispatch delegate is known to be.
      ExternalInputBindings externalInputBindings = new ExternalInputBindings(FormClosed: () => dispatch(MvuMessages.Request_Quit()));
      ViewInputBindings viewInputBindings = new ViewInputBindings(Increment1ButtonPressed: () => dispatch(MvuMessages.Request_Increment1()),
                                                                  IncrementRandomButtonPressed: () => dispatch(MvuMessages.Request_IncrementRandom()));

      Control mainPanel = buildMainPanel(model, viewInputBindings);

      ProgramView programView = new ProgramView([ mainPanel ]);

      PlatformView<ProgramView> platformView = new PlatformView<ProgramView>(programView, viewInputBindings, externalInputBindings);

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
