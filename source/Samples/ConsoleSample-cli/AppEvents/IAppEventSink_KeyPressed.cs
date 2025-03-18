using ConsoleSample.UIBasics;


namespace ConsoleSample.AppEvents;

/// <summary>
/// This is (intended to be) a generic, platform-agnostic, representation of user input events that
/// may be raised
/// that this application is designed to handle.
/// </summary>
internal interface IAppEventSink_KeyPressed {
   /// <summary>
   /// Returns true if key was handled
   /// </summary>
   bool HandleAppKeyPress(IKeyPressInfo keyPressInfo);
}
