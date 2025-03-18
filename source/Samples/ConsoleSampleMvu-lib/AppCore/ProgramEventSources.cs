using System;
using Microsoft.Extensions.Logging;


namespace ConsoleSampleMvu.AppCore;

// ??? we need a more direct mechanism for the view to define handlers for user input events.
// ??? we don't want to subscribe/unsubscribe each time a new (immutable!) view is instantiated/destroyed

public class ProgramEventSources : IProgramEventSource_QuitButtonPressed,
                                   IProgramEventSource_Increment1ButtonPressed,
                                   IProgramEventSource_IncrementRandomButtonPressed {
   public event Action? QuitButtonPressed;
   public event Action? Increment1ButtonPressed;
   public event Action? IncrementRandomButtonPressed;

   private readonly ILogger? _inputLogger;


   public ProgramEventSources(ILogger? inputLogger) {
      _inputLogger = inputLogger;
   }


   void IProgramEventSource_QuitButtonPressed.RaiseQuitButtonPressed() {
      _inputLogger?.LogTrace("[app] --> [program]  :QuitButtonPressed:");
      QuitButtonPressed?.Invoke();
   }


   void IProgramEventSource_Increment1ButtonPressed.RaiseIncrement1ButtonPressed() {
      _inputLogger?.LogTrace("[app] --> [program]  :Increment1ButtonPressed:");
      Increment1ButtonPressed?.Invoke();
   }

   void IProgramEventSource_IncrementRandomButtonPressed.RaiseIncrementRandomButtonPressed() {
      _inputLogger?.LogTrace("[app] --> [program]  :IncrementRandomButtonPressed:");
      IncrementRandomButtonPressed?.Invoke();
   }
}
