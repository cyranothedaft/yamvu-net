using System;

namespace ConsoleSample.View;

public class MutableViewInputBindings {

   private readonly System.Threading.Lock _quitLock = new();
   private readonly System.Threading.Lock _increment1Lock = new();
   private readonly System.Threading.Lock _incrementRandomLock = new();


   private Action? _quitButtonPressed;
   private Action? _increment1ButtonPressed;
   private Action? _incrementRandomButtonPressed;


   public Action? QuitButtonPressed {
      get { lock ( _quitLock ) { return _quitButtonPressed; } }
      set { lock ( _quitLock ) {        _quitButtonPressed = value; } }
   }

   public Action? Increment1ButtonPressed {
      get { lock ( _increment1Lock ) { return _increment1ButtonPressed; } }
      set { lock ( _increment1Lock ) {        _increment1ButtonPressed = value; } }
   }

   public Action? IncrementRandomButtonPressed {
      get { lock ( _incrementRandomLock ) { return _incrementRandomButtonPressed; } }
      set { lock ( _incrementRandomLock ) {        _incrementRandomButtonPressed = value; } }
   }


   public MutableViewInputBindings() {
      _quitButtonPressed            = null;
      _increment1ButtonPressed      = null;
      _incrementRandomButtonPressed = null;
   }
}
