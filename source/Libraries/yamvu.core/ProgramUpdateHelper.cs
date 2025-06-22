using System;
using Microsoft.Extensions.Logging;
using yamvu.core.Primitives;



namespace yamvu.core;

public static class ProgramUpdateHelper {
   public static (TModel newModel, IMvuCommand[] commands) IgnoreMessage<TModel>(ProgramInfo programInfo, TModel currentModel, IMvuMessage message, ILogger? logger, bool warn = false) {
      if (warn)
         logger?.LogWarning("[{programName}] ignoring message: {message}", programInfo.Name, message);
      else
         logger?.LogDebug("[{programName}] ignoring message: {message}", programInfo.Name, message);

      return (currentModel, MvuCommands.None);
   }
}
