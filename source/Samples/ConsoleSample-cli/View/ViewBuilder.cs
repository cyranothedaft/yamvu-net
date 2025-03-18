using System;
using System.Collections.Immutable;
using ConsoleSampleMvu.AppCore;
using CounterMvu_lib;
using CounterMvu_lib.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;



namespace ConsoleSample.View;

internal class ViewBuilder {

   public static View BuildInitialView()
      => new View(renderViewLines("(There's no model yet)"  // TODO: is this even needed/wanted? Seems better if the view definition doesn't have to account for this case
                                  ), () => { });


   // because of how many UI platforms work, the visual part of the view is coupled with the input-event handling part
   // to support that: here, they can be defined together or separately.
   // for this app, we're defining them separately (since the input and output of the console are also defined separately)
   //
   // all input/platform/app events that should become MVU messages must ultimately get handled here, since this is where the dispatch function is available
   // 

   public static View BuildFromModel(MvuMessageDispatchDelegate dispatch, Model model,
                                     ProgramEventSources eventSources, // <-- provided by the "view platform" and pre-translated into "program" events;
                                                                       //     on some ui platforms these events will originate from the view instance itself,
                                                                       //     That's not the case for this console app, so we can handle them in a separate layer
                                                                       //     and keep the view definition as clean as possible
                                     ILogger? uilogger) {

      
      // there must be a better way do to this...  (currently, we're just letting the garbage collector handle unsubscribing the events (assuming that even works??))
      void dispatch_Request_Quit()            { dispatch(MvuMessages.Request_Quit()           ); }
      void dispatch_Request_Increment1()      { dispatch(MvuMessages.Request_Increment1()     ); }
      void dispatch_Request_IncrementRandom() { dispatch(MvuMessages.Request_IncrementRandom()); }
      eventSources.QuitButtonPressed            += dispatch_Request_Quit;
      eventSources.Increment1ButtonPressed      += dispatch_Request_Increment1;
      eventSources.IncrementRandomButtonPressed += dispatch_Request_IncrementRandom;
      void onDispose() {
         eventSources.QuitButtonPressed            -= dispatch_Request_Quit;
         eventSources.Increment1ButtonPressed      -= dispatch_Request_Increment1;
         eventSources.IncrementRandomButtonPressed -= dispatch_Request_IncrementRandom;
      }


      return new View(renderViewLines(counterText: model.Counter.ToString() + (model.Counter == 0 ? " (the model's initial value)" : "")
                                      ), onDispose);
   }


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
