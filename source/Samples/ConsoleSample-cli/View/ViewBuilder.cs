using System;
using System.Collections.Immutable;
using CounterMvu_lib;
using CounterMvu_lib.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;



namespace ConsoleSample.View;

internal class ViewBuilder {

   public static PlatformView<MvuView> BuildInitialView()
      => new(buildInitialMvuView(),
             new ViewInputBindings());


   private static MvuView buildInitialMvuView()
      => new MvuView(renderViewLines("(There's no model yet)"  // TODO: is this even needed/wanted? Seems better if the view definition doesn't have to account for this case
                                  ));


   // because of how many UI platforms work, the visual part of the view is coupled with the input-event handling part
   // to support that: the 'view' that's created has two components: (1) the view itself (the raw data for rendering the view),
   // and (2) a table of input bindings (definitions of what happens when user input occurs).
   // 

   public static PlatformView<MvuView> BuildViewFromModel(MvuMessageDispatchDelegate dispatch, Model model, ILogger? uilogger) {
      MvuView mvuView = buildMvuViewFromModel(model, uilogger);
      ViewInputBindings inputBindings = new (() => dispatch(MvuMessages.Request_Quit()           ),
                                             () => dispatch(MvuMessages.Request_Increment1()     ),
                                             () => dispatch(MvuMessages.Request_IncrementRandom()));
      PlatformView<MvuView> platformView = new PlatformView<MvuView>(mvuView, inputBindings);
      return platformView;
   }


   private static MvuView buildMvuViewFromModel(Model model, ILogger? uilogger)
      => new MvuView(renderViewLines(counterText: model.Counter.ToString() + (model.Counter == 0 ? " (the model's initial value)" : "")));


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
