using System;
using ConsoleSample.UIBasics;


namespace ConsoleSample.AppEvents;

internal record KeyPressInfo(ConsoleKeyInfo KeyData) : IKeyPressInfo;

internal static class ConsoleKeyInfoExtensions {
   internal static IKeyPressInfo ToKeyPressInfo(this ConsoleKeyInfo consoleKeyInfo)
      => new KeyPressInfo(consoleKeyInfo);
}
