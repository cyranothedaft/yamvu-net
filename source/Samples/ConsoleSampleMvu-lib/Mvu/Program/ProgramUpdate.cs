using System;
using CounterSample.AppCore.Mvu.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.core.Primitives;


namespace CounterSample.AppCore.Mvu.Program;

public class ProgramUpdate : IMvuProgramUpdatable<Model> {
   private readonly ILogger? _logger;


   public ProgramUpdate(ILogger? logger = null) {
      _logger = logger;
   }


   // MVU init


   public (Model initialModel, IMvuCommand[] initialCommands) Init()
      => ( new Model( 0 ),
           [ ] );


   // MVU update


   public (Model newModel, IMvuCommand[] commands) Update(MvuCommandDispatchDelegate dispatch, Model currentModel, IMvuMessage message)
      => message switch
         {
            Request_Increment1Message          => Request_Increment1Message      .Handle(dispatch, currentModel),
            Request_IncrementRandomMessage     => Request_IncrementRandomMessage .Handle(dispatch, currentModel),
            IncrementCounterMessage        msg => IncrementCounterMessage        .Handle(          currentModel, msg.Increment),

            // ignore all others - including Request_Quit
            _ => ProgramUpdateHelper.IgnoreMessage(Program.Info, currentModel, message, _logger)
         };
}
