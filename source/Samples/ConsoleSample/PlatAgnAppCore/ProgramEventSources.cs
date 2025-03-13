using Microsoft.Extensions.Logging;


namespace ConsoleSample.PlatAgnAppCore;

public class ProgramEventSources : IProgramEventSource_OtherButtonPressed,
                                   IProgramEventSource_QuitButtonPressed {
   public event Action? OtherButtonPressed;
   public event Action? QuitButtonPressed;

   private readonly ILogger? _uiLogger;


   public ProgramEventSources(ILogger? uiLogger) {
      _uiLogger = uiLogger;
   }


   void IProgramEventSource_OtherButtonPressed.RaiseOtherButtonPressed() {
      _uiLogger?.LogTrace("Program Event - OtherButtonPressed - raising");
      OtherButtonPressed?.Invoke();
   }


   void IProgramEventSource_QuitButtonPressed.RaiseQuitButtonPressed() {
      _uiLogger?.LogTrace("Program Event - QuitButtonPressed - raising");
      QuitButtonPressed?.Invoke();
   }
}
