using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using Microsoft.Extensions.Logging;
using Terminal.Gui.Views;
using yamvu.core;
using yamvu.Extensions.Tui;


namespace TuiCounterSample.ui.View;

internal class ViewBuilder {

   // TODO: why is this not used?
   public static ProgramView BuildInitialView()
      => new([ buildInitialView() ],
             new ViewInputBindings(),
             new ExternalInputBindings());


   public static ProgramView BuildViewFromModel(MvuMessageDispatchDelegate dispatch, Model model, ILogger? uilogger) {
      // (TODO: this isn't ideal) All events that generate messages (both external and internal to the view) must be funnelled through the view,
      // because that's where the dispatch delegate is known to be.
      ExternalInputBindings externalInputBindings = new ExternalInputBindings(MainWindowClosed: () => dispatch(MvuMessages.Request_Quit()));
      ViewInputBindings viewInputBindings = new ViewInputBindings(Increment1ButtonPressed: () => dispatch(MvuMessages.Request_Increment1()),
                                                                  IncrementRandomButtonPressed: () => dispatch(MvuMessages.Request_IncrementRandom()));

      return new ProgramView([], viewInputBindings, externalInputBindings);

//      Control mainPanel = buildMainPanel(model, viewInputBindings);
//
//      ProgramView platformView = new ProgramView([ mainPanel ],
//                                                 viewInputBindings,
//                                                 externalInputBindings);
//
//      return platformView;

   }


   private static Terminal.Gui.ViewBase.View buildInitialView() {
      return new Label()
                {
                   Text = "labeltext",
                   X    = 6,
                   Y    = 7,
                };
   }


//   private static Control buildMainPanel(Model model, ViewInputBindings inputBindings) {
//      FlowLayoutPanel panel = new FlowLayoutPanel()
//                                 {
//                                    FlowDirection = FlowDirection.TopDown,
//                                    Dock          = DockStyle.Fill
//                                 };
//      Button increment1Button = new Button() { Text = "Increment 1", AutoSize = true };
//      Button incrementRandomButton = new Button() { Text = "Increment Random", AutoSize = true };
//      increment1Button.Click      += (_, _) => inputBindings.Increment1ButtonPressed?.Invoke();
//      incrementRandomButton.Click += (_, _) => inputBindings.IncrementRandomButtonPressed?.Invoke();
//      panel.Controls.AddRange([
//            new Label() { Text = "Counter:", AutoSize = true },
//            new Label() { Text = model.Counter.ToString(), AutoSize = true },
//            increment1Button,
//            incrementRandomButton,
//         ]);
//
//      return panel;
//   }
}
