using ConsoleSample.UIBasics;

namespace ViewPlatform;

public interface IViewPlatformKeyboardEventSource {
   public event EventHandler<KeyPressedEventArgs> KeyPressed;
}
