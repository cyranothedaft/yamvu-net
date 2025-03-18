using System;
using ConsoleSample.UIBasics;
using ConsoleSampleMvu.AppCore;


namespace ConsoleSample;

internal static class ProgramKeyDispatcher {
   /// <summary>
   /// Returns true if this call handled the keypress
   /// </summary>
   public static bool Handle(ProgramEventSources programInputSources, IKeyPressInfo keyPressed) {
         if      (isQuitKey           (keyPressed))  { ((IProgramEventSource_QuitButtonPressed           )programInputSources).RaiseQuitButtonPressed           (); return true; }
         else if (isIncrement1Key     (keyPressed))  { ((IProgramEventSource_Increment1ButtonPressed     )programInputSources).RaiseIncrement1ButtonPressed     (); return true; }
         else if (isIncrementRandomKey(keyPressed))  { ((IProgramEventSource_IncrementRandomButtonPressed)programInputSources).RaiseIncrementRandomButtonPressed(); return true; }
         else
            return false; // not handled

         bool isQuitKey           (IKeyPressInfo keyPressInfo) => keyPressInfo.KeyData.IsLetter('Q');
         bool isIncrement1Key     (IKeyPressInfo keyPressInfo) => keyPressInfo.KeyData.KeyChar == '1';
         bool isIncrementRandomKey(IKeyPressInfo keyPressInfo) => keyPressInfo.KeyData.KeyChar == '?';
   }
}
