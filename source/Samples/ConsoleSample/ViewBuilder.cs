using System;
using System.Collections.Immutable;
using ConsoleSample.Mvu;
using ConsoleSample.PlatAgnAppCore;
using Microsoft.Extensions.Logging;
using yamvu.core;



namespace ConsoleSample;

internal class ViewBuilder {

   private const string BasePrompt = "Press Any Key  or  [Esc] to quit)";


   public static View BuildInitialView()
      => new View(renderViewLines(status: "(no status yet)",
                                  prompt: BasePrompt));


   public static View BuildFromModel(MvuMessageDispatchDelegate dispatch, Model model, ProgramEventSources eventSources, ILogger? uilogger)
      => new View(renderViewLines(status: model.KeyChar.HasValue
                                                ? $"User pressed key '{model.KeyChar.Value}'"
                                                : $"User has not pressed a key.",
                                  prompt: BasePrompt));


   private static ImmutableList<string> renderViewLines(string status, string prompt)
      => [
            "---------------------------------------------------------------------",
            $">> {status}",
            $">> {prompt} >> ",
            ".....................................................................",
         ];
}
