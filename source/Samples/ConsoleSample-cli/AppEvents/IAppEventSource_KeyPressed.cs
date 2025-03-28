using System;
using ConsoleSample.UIBasics;



namespace ConsoleSample.AppEvents;

internal interface IAppEventSource_KeyPressed {
   event Func<IKeyPressInfo, bool>? AppKeyPressed;
   /// <summary>
   /// Returns true if key was handled
   /// </summary>
   bool RaiseAppKeyPressed(IKeyPressInfo keyPressInfo);
}
