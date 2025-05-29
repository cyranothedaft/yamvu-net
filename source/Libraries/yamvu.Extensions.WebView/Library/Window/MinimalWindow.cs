using System;
using Windows.Win32.Foundation;



namespace yamvu.Extensions.WebView.Library.Window;

public partial class MinimalWindow {
   private HWND? _hwnd;

   public delegate void SizeChangedEventDelegate(int width, int height);
   public event SizeChangedEventDelegate? SizeChanged;


   internal HWND Handle => _hwnd!.Value;


   private MinimalWindow() { }


   public void Show() => Show(_hwnd);


   private void raiseSizeEvent(int width, int height) {
      SizeChanged?.Invoke(width, height);
   }
}
