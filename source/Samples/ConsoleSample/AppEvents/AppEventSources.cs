using System;
using ConsoleSample.UIBasics;
using Microsoft.Extensions.Logging;



namespace ConsoleSample.AppEvents;

internal class AppEventSources : IAppEventSource_KeyPressed {
   private readonly ILogger? _uiLogger;

   public event Func<IKeyPressInfo, bool>? AppKeyPressed;


   public AppEventSources(ILogger? uiLogger) {
      _uiLogger = uiLogger;
   }


   /// <inheritdoc />
   public bool RaiseAppKeyPressed(IKeyPressInfo keyPressInfo) {
      _uiLogger?.LogTrace("App Event - KeyPressed - raising:   {key}", keyPressInfo.KeyData.DisplayText());
      return AppKeyPressed?.Invoke(keyPressInfo) ?? false;
   }
}
