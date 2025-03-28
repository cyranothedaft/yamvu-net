using System;
using System.Collections.Immutable;
using CounterMvu_lib;
using CounterMvu_lib.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;



namespace ConsoleSample.View;

internal class ViewBuilder {

   public static PlatformView<ProgramView> BuildInitialView()
      => new(buildInitialMvuView(),
             new ViewInputBindings());


   private static ProgramView buildInitialMvuView()
      => new ProgramView(renderViewLines("(There's no model yet)"  // TODO: is this even needed/wanted? Seems better if the view definition doesn't have to account for this case
                                  ));


   // Because of how many UI platforms work, the visual part of the view is coupled with the input-event handling part
   // to support that: the 'view' that's created has two components: (1) the view itself (the raw data for rendering the view: MvuView),
   // and (2) a table of input bindings (definitions of what happens when user input occurs: ViewInputBindings).
   // In this case, all input events originate outside the View object, so all are represented in the ViewInputBindings.
   // 

   public static PlatformView<ProgramView> BuildViewFromModel(MvuMessageDispatchDelegate dispatch, Model model, ILogger? uilogger) {
      ProgramView programView = buildMvuViewFromModel(model, uilogger);

      // (TODO: this isn't ideal) All events that generate messages (both external and internal to the view) must be funnelled through the view,
      // because that's where the dispatch delegate is known to be.
      ViewInputBindings inputBindings = new ( QuitButtonPressed:            () => dispatch(MvuMessages.Request_Quit()           ),
                                              Increment1ButtonPressed:      () => dispatch(MvuMessages.Request_Increment1()     ),
                                              IncrementRandomButtonPressed: () => dispatch(MvuMessages.Request_IncrementRandom()) );
      PlatformView<ProgramView> platformView = new PlatformView<ProgramView>(programView, inputBindings);
      return platformView;
   }


   private static ProgramView buildMvuViewFromModel(Model model, ILogger? uilogger)
      => new ProgramView(renderViewLines(counterText: model.Counter.ToString() + (model.Counter == 0 ? " (the model's initial value)" : "")));


   private static ImmutableList<string> renderViewLines(string counterText)
      => [
            // "---------------------------------------------------------------------",
            $"Current Counter:  >> {counterText} <<",
            $"Press:  To:",
            $"    1:  Increment by 1",
            $"    ?:  Increment by a random amount",
            $"    Q:  Quit",
            $">",
            // ".....................................................................",
         ];
}
