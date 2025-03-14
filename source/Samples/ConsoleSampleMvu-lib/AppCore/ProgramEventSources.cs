using System;
using Microsoft.Extensions.Logging;


namespace ConsoleSampleMvu.AppCore;

// ??? we need a more direct mechanism for the view to define handlers for user input events.
// ??? we don't want to subscribe/unsubscribe each time a new (immutable!) view is instantiated/destroyed

public class ProgramEventSources : IProgramEventSource_OtherButtonPressed,
                                   IProgramEventSource_QuitButtonPressed {
   public event Action? OtherButtonPressed;
   public event Action? QuitButtonPressed;

   private readonly ILogger? _inputLogger;


   public ProgramEventSources(ILogger? inputLogger) {
      _inputLogger = inputLogger;
   }


   void IProgramEventSource_OtherButtonPressed.RaiseOtherButtonPressed() {
      _inputLogger?.LogTrace("[app] --> [program]  :OtherButtonPressed:");
      OtherButtonPressed?.Invoke();
   }


   void IProgramEventSource_QuitButtonPressed.RaiseQuitButtonPressed() {
      _inputLogger?.LogTrace("[app] --> [program]  :QuitButtonPressed:");
      QuitButtonPressed?.Invoke();
   }
}
