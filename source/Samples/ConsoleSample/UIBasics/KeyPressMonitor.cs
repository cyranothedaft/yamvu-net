using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSample.UIBasics;


public class KeyPressMonitor : IDisposable {
   private readonly bool _isDebugging;

   private CancellationTokenSource? _interiorCancellationTokenSource = null;
   private CancellationTokenSource _jointCancellationTokenSource; // module-scoped
   private CancellationToken _jointCancellationToken ;
   private Task _runTask;


   public KeyPressMonitor(bool isDebugging) {
      _isDebugging = isDebugging;
   }


   public KeyPressMonitor() : this(isDebugging: false) { }


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
      Debug.WriteLineIf(_isDebugging, $"-> reading key"); // TODO: use ILogger instead
      ConsoleKeyInfo key = Console.ReadKey(intercept: true);
      Debug.WriteLineIf(_isDebugging, $"<- read    key: {key.DisplayText()}");
      return raiseKeyPressed(key);
   }


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


   public class KeyPressedEventArgs : CancelEventArgs {
      public ConsoleKeyInfo KeyInfo { get; }


      public KeyPressedEventArgs(ConsoleKeyInfo keyInfo) {
         KeyInfo = keyInfo;
      }


      public KeyPressedEventArgs(ConsoleKeyInfo keyInfo, bool cancel)
            : base(cancel) {
         KeyInfo = keyInfo;
      }
   }
   #endregion Events


   public void Dispose() {
      StopSafe();
   }
}



public static class Extensions {
   public static string DisplayText(this ConsoleKeyInfo key)
      => ( key.Modifiers == 0 ? string.Empty : ( key.Modifiers.ToString() + " " ) )
       + key.Key.ToString()
       + ( key.KeyChar >= ' ' ? $" '{key.KeyChar}'" : string.Empty );


   public static bool IsEscape(this ConsoleKeyInfo key)
      => key.Key       == ConsoleKey.Escape
      && key.Modifiers == 0;


   public static bool IsLetter(this ConsoleKeyInfo keyInfo, char letter)
      => char.ToUpperInvariant(keyInfo.KeyChar) == char.ToUpperInvariant(letter);
}
