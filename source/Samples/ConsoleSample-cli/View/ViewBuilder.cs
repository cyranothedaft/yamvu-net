using System;
using System.Collections.Immutable;
using ConsoleSampleMvu.AppCore;
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
   // to support that: here, they can be defined together or separately.
   // for this app, we're defining them separately (since the input and output of the console are also defined separately)
   //
   // all input/platform/app events that should become MVU messages must ultimately get handled here, since this is where the dispatch function is available
   // 

   public static PlatformView<MvuView> BuildViewFromModel(MvuMessageDispatchDelegate dispatch, Model model,
                                     ProgramEventSources eventSources, // <-- provided by the "view platform" and pre-translated into "program" events;
                                                                       //     on some ui platforms these events will originate from the view instance itself,
                                                                       //     That's not the case for this console app, so we can handle them in a separate layer
                                                                       //     and keep the view definition as clean as possible
                                     ILogger? uilogger) {

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
