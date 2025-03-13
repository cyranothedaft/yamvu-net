using System;
using Microsoft.Extensions.Logging;
using yamvu.core.Primitives;



namespace yamvu.core;

public static class ProgramUpdateHelper {
   public static (TModel newModel, IMvuCommand[] commands) IgnoreMessage<TModel>(ProgramInfo programInfo, TModel currentModel, IMvuMessage message, ILogger? logger) {
      logger?.LogDebug("[{programName}] ignoring message: {message}", programInfo.Name, message);
      return (currentModel, MvuCommands.None);
   }
}
