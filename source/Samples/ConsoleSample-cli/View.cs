using System;
using System.Collections.Immutable;
using ConsoleSample.UIBasics;
using ViewPlatform;


namespace ConsoleSample;

internal record View(
      ImmutableList<string> TextLines
) : IViewPlatformKeyboardEventSource {
   // view is defined such that it is the source from which these events are raised
   public event EventHandler<KeyPressedEventArgs>? KeyPressed;
}
