using System;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;



namespace WinFormsCounterSample.ViewPlatform;

internal class WinFormsViewPlatform : IDisposable {

   public WinFormsViewPlatform(MainForm mainForm, ILogger? inpLogger, ILogger? uiLogger) {
      // _inputWatcher = new InputWatcher(inpLogger);
      //
      // // define MVU program and wire up message loop
      // (_,
      //  AppEventSinks appEventSinks // <-- handlers for "Application" events; has been wired up to raise "Program" events (via the program event sources).
      //                               //     Important! - This still needs to be wired up (later) to the emitter of the actual "Platform" events (inputWatcher, in this case)
      //  // (turns out we don't need to know about this here?):   ProgramEventSources programInputSources // <-- sources which raise "Program" events, which will be passed to the MVU program to be handled there
      // ) = wireUpEvents(handleKeyPressAndRaiseProgramEvent, uiLogger);
      // _inputWatcher.SetHandler(appEventSinks);
   }


   public void Dispose() {
      // _inputWatcher.Dispose();
   }



   // private static (AppEventSources, AppEventSinks
   //       // , ProgramEventSources
   //       ) wireUpEvents(Func<IKeyPressInfo, bool> handleKeyPressAndRaiseProgramEvent, ILogger? uiLogger) {
   //    // wire up event dispatchers
   //    AppEventSources appEventSources = new(uiLogger);
   //    AppEventSinks appEventSinks = new(handleAppKeyPressed: appEventSources.RaiseAppKeyPressed, uiLogger);
   //
   //    // ProgramEventSources programInputSources = new(uiLogger);
   //
   //    // translate from app-level input event to program-level input event
   //    appEventSources.AppKeyPressed += args => handleKeyPressAndRaiseProgramEvent(args);
   //
   //    return (appEventSources, appEventSinks
   //            // , programInputSources
   //            );
   // }
}

