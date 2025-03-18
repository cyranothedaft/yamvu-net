using System;

namespace ConsoleSample.View;

public struct ViewInputBindings {
   // !!! === TODO === !!!   make this tread-safe !!!
   public Action? QuitButtonPressed;
   public Action? Increment1ButtonPressed;
   public Action? IncrementRandomButtonPressed;


   public ViewInputBindings(Action? requestQuitAction, Action? requestIncrement1Action, Action? requestIncrementRandomAction) {
      QuitButtonPressed            = requestQuitAction;
      Increment1ButtonPressed      = requestIncrement1Action;
      IncrementRandomButtonPressed = requestIncrementRandomAction;
   }
}
