using ConsoleSample.UIBasics;


namespace ConsoleSample.AppEvents;

internal interface IAppEventSink_KeyPressed {
   /// <summary>
   /// Returns true if key was handled
   /// </summary>
   bool HandleAppKeyPress(IKeyPressInfo keyPressInfo);
}
