using System.Collections.Generic;
using ConsoleSampleMvu.Mvu;
using ConsoleSampleMvu.Mvu.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WelterKit.Testing;
using yamvu.core.Primitives;
using yamvu.Testing;



namespace ConsoleSampleMvu_lib_test.UnitTests;

[TestClass]
public sealed class Test_ProgramUpdate {

   private static readonly ILogger TestLogger = new LoggerForTesting();
   private static void NullDispatcher(IMvuCommand command) { }


   [TestMethod]
   public void Init() {
      MvuAssert.AreEqual((new Model(KeyChar: null), []),
                         new ProgramUpdate(TestLogger).Init());
   }


   [TestMethod]
   public void Update_FromNull_Request_QuitMessage() {
      testUpdate(message          : MvuMessages.Request_Quit(),
                 inputModel       : new Model(KeyChar: null),
                 model_expected   : new Model(KeyChar: null),
                 commands_expected: [ ]);
   }


   [TestMethod]
   public void Update_FromChar_Request_QuitMessage() {
      testUpdate(message          : MvuMessages.Request_Quit(),
                 inputModel       : new Model(KeyChar: 'A'),
                 model_expected   : new Model(KeyChar: 'A'),
                 commands_expected: [ ]);
   }



   private static void testUpdate(Message message, Model inputModel, Model model_expected, IEnumerable<IMvuCommand> commands_expected) {
      (Model model_actual, IMvuCommand[] commands_actual) = new ProgramUpdate(TestLogger)
                                                            .Update(NullDispatcher, inputModel, message);
      Assert.AreEqual(model_expected, model_actual, "Model");
      SequenceAssert.AreEqual(commands_expected, commands_actual,
                              assertAreEqualAction: Assert.AreEqual, "Commands");
   }

}
