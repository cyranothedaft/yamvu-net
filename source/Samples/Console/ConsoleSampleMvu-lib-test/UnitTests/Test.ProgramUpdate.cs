using CounterMvu;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Effects;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Mvu.Program;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yamvu.core.Primitives;
using yamvu.Testing;



namespace CounterSample.AppCore.Mvu_test.UnitTests;

[TestClass]
public sealed class Test_ProgramUpdate {

   private static readonly ILogger TestLogger = new LoggerForTesting();


   [TestMethod]
   public void Init() {
      MvuAssert.UpdateResultsAreEqual((new Model(Counter: 0), []),
                                      new ProgramUpdate(TestLogger).Init());
   }


   [TestMethod]
   public void Update_Request_QuitMessage_FromInitial() {
      testUpdate(message          : MvuMessages.Request_Quit(),
                 inputModel       : new Model(Counter: 0),
                 model_expected   : new Model(Counter: 0),
                 commands_expected: [ ]);
   }


   [TestMethod]
   public void Update_Request_QuitMessage_FromNumber() {
      testUpdate(message          : MvuMessages.Request_Quit(),
                 inputModel       : new Model(Counter: 42),
                 model_expected   : new Model(Counter: 42),
                 commands_expected: [ ]);
   }


   [TestMethod]
   public void Update_IncrementCounter_FromInitial() {
      testUpdate(message          : MvuMessages.IncrementCounter(42),
                 inputModel       : new Model(Counter:  0),
                 model_expected   : new Model(Counter: 42),
                 commands_expected: [ ]);
   }


   [TestMethod]
   public void Update_IncrementCounter_FromNumber() {
      testUpdate(message          : MvuMessages.IncrementCounter(42),
                 inputModel       : new Model(Counter: 100),
                 model_expected   : new Model(Counter: 142),
                 commands_expected: [ ]);
   }


   [TestMethod]
   public void Update_Request_Increment1_FromInitial() {
      testUpdate(message          : MvuMessages.Request_Increment1(),
                 inputModel       : new Model(Counter: 0),
                 model_expected   : new Model(Counter: 0),
                 commands_expected: [ MvuMessages.IncrementCounter(1).AsCommand() ]);
   }


   [TestMethod]
   public void Update_Request_Increment1_FromNumber() {
      testUpdate(message          : MvuMessages.Request_Increment1(),
                 inputModel       : new Model(Counter: 0),
                 model_expected   : new Model(Counter: 0),
                 commands_expected: [ MvuMessages.IncrementCounter(1).AsCommand() ]);
   }


   [TestMethod]
   public void Update_Request_IncrementRandom_FromInitial() {
      testUpdate(message          : MvuMessages.Request_IncrementRandom(),
                 inputModel       : new Model(Counter: 0),
                 model_expected   : new Model(Counter: 0),
                 commands_expected: [ new GenerateRandomNumberEffect().AsCommandWithResultHandler((int _) => { }) ]);
   }


   [TestMethod]
   public void Update_Request_IncrementRandom_FromNumber() {
      testUpdate(message          : MvuMessages.Request_IncrementRandom(),
                 inputModel       : new Model(Counter: 100),
                 model_expected   : new Model(Counter: 100),
                 commands_expected: [ new GenerateRandomNumberEffect().AsCommandWithResultHandler((int _) => { }) ]);
   }



   private static void testUpdate(Message message, Model inputModel, Model model_expected, IMvuCommand[] commands_expected) {
      // ARRANGE, ACT
      var actualResult = new ProgramUpdate(TestLogger)
                         .Update(NullDispatcher, inputModel, message);

      // ASSERT
      MvuAssert.UpdateResultsAreEqual((model_expected, commands_expected), actualResult);


      static void NullDispatcher(IMvuCommand command) { }
   }

}
