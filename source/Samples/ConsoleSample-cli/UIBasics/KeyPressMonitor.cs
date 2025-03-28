using System;
using Microsoft.Extensions.Logging;
using ViewPlatform;



namespace ConsoleSample.UIBasics;

public class KeyPressMonitor : IViewPlatformKeyboardEventSource, IDisposable {
   // private readonly bool _debounce;
   private readonly bool _isDebugging;
   private readonly ILogger? _logger;

   private CancellationTokenSource? _interiorCancellationTokenSource = null;
   private CancellationTokenSource _jointCancellationTokenSource; // module-scoped
   private CancellationToken _jointCancellationToken ;
   private Task _runTask;


   public KeyPressMonitor(
         // bool debounce = true, 
         bool isDebugging = false, ILogger? logger = null) {
      // _debounce    = debounce;
      _isDebugging = isDebugging;
      _logger      = logger;
   }


   public void Start(CancellationToken? cancellationToken = null)
         // TODO: first flush input buffer?
      => start(keyPressLoop, cancellationToken);


   private void start(Action listenMethod, CancellationToken? cancellationToken = null) {
      _interiorCancellationTokenSource = new CancellationTokenSource();
      _jointCancellationTokenSource = cancellationToken.HasValue
                                            ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value, _interiorCancellationTokenSource.Token)
                                            : _interiorCancellationTokenSource;
      _jointCancellationToken = _jointCancellationTokenSource.Token;

      _runTask = Task.Run(listenMethod, _jointCancellationTokenSource.Token);
   }


   public void StopSafe() {
      _interiorCancellationTokenSource?.Cancel();
   }


   private void keyPressLoop() {
      bool userCancelled = false;
      while (!userCancelled && !_jointCancellationToken.IsCancellationRequested) {
         readAndRaiseKey();
      }
   }


   private bool readAndRaiseKey() {
      // if (_debounce) clearInputBuffer();
      if (_isDebugging) _logger?.LogDebug("-> awaiting key press");
      //Debug.WriteLineIf(_isDebugging, $"-> reading key"); // TODO: use ILogger instead?
      ConsoleKeyInfo key = Console.ReadKey(intercept: true);
      if (_isDebugging) _logger?.LogDebug("<- read     key: {key} - raising app event", key.GetDisplayText());
      //Debug.WriteLineIf(_isDebugging, $"<- read    key: {key.DisplayText()}");
      return raiseKeyPressed(key);
   }


   // private void clearInputBuffer() {
   //    if (_isDebugging) _logger?.LogDebug("-> clearing input buffer");
   //    while (Console.KeyAvailable) {
   //       ConsoleKeyInfo key = Console.ReadKey();
   //       if (_isDebugging) _logger?.LogDebug("---> cleared: {key}", key.GetDisplayText());
   //    }
   // }


   #region Events

   public event EventHandler<KeyPressedEventArgs> KeyPressed;

   /// <summary>
   /// Returns true if cancellation is requested; false otherwise.
   /// </summary>
   private bool raiseKeyPressed(ConsoleKeyInfo key) {
      var eventArgs = new KeyPressedEventArgs(key);
      KeyPressed?.Invoke(this, eventArgs);
      return eventArgs.Cancel;
   }

   #endregion Events


   public void Dispose() {
      StopSafe();
   }
}



public static class Extensions {
   public static string GetDisplayText(this ConsoleKeyInfo key)
      => ( key.Modifiers == 0 ? string.Empty : ( key.Modifiers.ToString() + " " ) )
       + key.Key.ToString()
       + ( key.KeyChar >= ' ' ? $" '{key.KeyChar}'" : string.Empty );


   public static bool IsEscape(this ConsoleKeyInfo key)
      => key.Key       == ConsoleKey.Escape
      && key.Modifiers == 0;


   /// <summary>
   /// Case-insensitive test
   /// </summary>
   public static bool IsLetter(this ConsoleKeyInfo keyInfo, char letter)
      => char.ToUpperInvariant(keyInfo.KeyChar) == char.ToUpperInvariant(letter);
}
