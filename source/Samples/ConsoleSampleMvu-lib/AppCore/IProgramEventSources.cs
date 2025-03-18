using System;

namespace ConsoleSampleMvu.AppCore;

public interface IProgramEventSource_QuitButtonPressed             { void RaiseQuitButtonPressed();            }
public interface IProgramEventSource_Increment1ButtonPressed       { void RaiseIncrement1ButtonPressed();      }
public interface IProgramEventSource_IncrementRandomButtonPressed  { void RaiseIncrementRandomButtonPressed(); }
