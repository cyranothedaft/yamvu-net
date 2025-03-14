using System;
using System.Collections.Immutable;
using ConsoleSampleMvu.AppCore;
using ConsoleSampleMvu.Mvu;
using ConsoleSampleMvu.Mvu.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;



namespace ConsoleSample;

internal class ViewBuilder {

   private const string BasePrompt = "Press Any Key  /  [Esc] to quit)";


   public static View BuildInitialView()
      => new View(renderViewLines(status: "(no status yet)",
                                  prompt: BasePrompt));


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

      
      // there's got to be a better way do to this...  (currently, we're just letting the garbage collector handle unsubscribing the events (assuming that even works??))
      eventSources.QuitButtonPressed += () => dispatch(MvuMessages.Request_Quit());

      
      return new View(renderViewLines(status: model.KeyChar.HasValue
                                                    ? $"User pressed key '{model.KeyChar.Value}'"
                                                    : $"User has not pressed a key.",
                                      prompt: BasePrompt));
   }


   private static ImmutableList<string> renderViewLines(string status, string prompt)
      => [
            "---------------------------------------------------------------------",
            $">> {status}",
            $">> {prompt} >> ",
            ".....................................................................",
         ];
}
