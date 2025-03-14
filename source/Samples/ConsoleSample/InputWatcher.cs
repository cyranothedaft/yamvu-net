using System.Diagnostics;
using ConsoleSample.AppEvents;
using ConsoleSample.UIBasics;


namespace ConsoleSample;


internal class InputWatcher : IDisposable {
   private readonly KeyPressMonitor _keyPressMonitor;

   public InputWatcher() {
      hideCursor();
      _keyPressMonitor = new KeyPressMonitor();
   }


   public void Dispose() {
      StopMonitoringKeyboard();
      _keyPressMonitor.Dispose();
      showCursor();
   }


   public void StartMonitoringKeyboard(CancellationToken cancellationToken = default) {
      _keyPressMonitor.Start(cancellationToken);
   }


   public void StopMonitoringKeyboard() {
      _keyPressMonitor.StopSafe();
   }


   public void SetHandler(IAppEventSink_KeyPressed appKeyPressedEventSink) {
      _keyPressMonitor.KeyPressed += (_, args) => {
                                        Debug.WriteLine($"** [platform:KeyPressed] -->  [app:KeyPressed] - {args.KeyInfo.DisplayText()}"); // TODO: log to ILogger instead
                                        bool isHandled = appKeyPressedEventSink.HandleAppKeyPress(args.KeyInfo.ToKeyPressInfo());
                                        Debug.WriteLine($"**                                               : {(isHandled ? "" : "not ")}handled"); // TODO: log to ILogger instead
                                     };
   }


   private static void showCursor() { Console.CursorVisible = true; }
   private static void hideCursor() { Console.CursorVisible = false; }
}
