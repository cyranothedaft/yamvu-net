using ConsoleSample.AppEvents;
using ConsoleSample.UIBasics;
using Microsoft.Extensions.Logging;



namespace ViewPlatform;

// TODO: allow events to pass-through instead of catching and re-throwing them?

internal class InputWatcher : IDisposable {
   private readonly ILogger? _logger;
   private readonly KeyPressMonitor _keyPressMonitor;

   public InputWatcher(ILogger? logger = null) {
      _logger = logger;
      hideCursor();
      _keyPressMonitor = new KeyPressMonitor(debounce: true, isDebugging: true, logger);
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
                                        _logger?.LogTrace("[platform] --> [app]  :KeyPress: {appKeyInfo}", args.KeyInfo.GetDisplayText());
                                        bool isHandled = appKeyPressedEventSink.HandleAppKeyPress(args.KeyInfo.ToKeyPressInfo());
                                        _logger?.LogTrace("[platform] --> [app]  :KeyPress: {appKeyInfo} - {handledOrNot}", args.KeyInfo.GetDisplayText(), isHandled ? "handled" : "not handled");
                                     };
   }


   private static void showCursor() { Console.CursorVisible = true; }
   private static void hideCursor() { Console.CursorVisible = false; }
}
