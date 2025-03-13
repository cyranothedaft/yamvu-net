using System;
using ConsoleSample.Mvu.Messages;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.core.Primitives;



namespace ConsoleSample.Mvu;

public class ProgramUpdate : IMvuProgramUpdatable<Model> {
   private readonly ILogger? _logger;


   public ProgramUpdate(ILogger? logger = null) {
      _logger = logger;
   }


   // MVU init


   public (Model initialModel, IMvuCommand[] initialCommands) Init()
      => ( new Model(Char: '-', Time: null),
           [ ] );


   // MVU update


   public (Model newModel, IMvuCommand[] commands) Update(MvuCommandDispatchDelegate dispatch, Model currentModel, IMvuMessage message)
      => message switch
         {
            Request_UpdateTimeMessage => Request_UpdateTimeMessage.Handle(dispatch, currentModel),
            SetCharMessage msg => SetCharMessage.Handle(currentModel, msg.Char),
            SetTimeMessage msg => SetTimeMessage.Handle(currentModel, msg.Time),

            // ignore all others - including Request_Quit
            _ => ProgramUpdateHelper.IgnoreMessage(Program.Info, currentModel, message, _logger)
         };
}
