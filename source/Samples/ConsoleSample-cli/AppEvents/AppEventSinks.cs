using System;
using ConsoleSample.UIBasics;
using Microsoft.Extensions.Logging;



namespace ConsoleSample.AppEvents;


internal class AppEventSinks : IAppEventSink_KeyPressed {
   private readonly ILogger? _uiLogger;
   private readonly Func<IKeyPressInfo, bool> _handleAppKeyPressed;


   public AppEventSinks(Func<IKeyPressInfo, bool> handleAppKeyPressed, ILogger? uiLogger) {
      _handleAppKeyPressed = handleAppKeyPressed;
      _uiLogger            = uiLogger;
   }


   /// <inheritdoc />
   public bool HandleAppKeyPress(IKeyPressInfo keyPressInfo) {
      _uiLogger?.LogTrace("App Event - KeyPressed - handling:  {key}", keyPressInfo.KeyData.DisplayText());
      return _handleAppKeyPressed(keyPressInfo);
   }
}
