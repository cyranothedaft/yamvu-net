using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WelterKit.Std.StaticUtilities;
using WelterKit.Testing;
using yamvu.core.Primitives;



namespace yamvu.Testing;

public static class MvuAssert {

   public static void UpdateResultsAreEqual<TModel, TCmd>((TModel model, TCmd[] commands) expected,
                                                          (TModel model, TCmd[] commands) actual)
         where TCmd : IMvuCommand {
      Assert.AreEqual(expected.model, actual.model, "Model");
      SequenceAssert.AreEqual(expected.commands, actual.commands,
                              assertAreEqualAction: CommandsAreEqual, "Commands");
   }


   public static void CommandsAreEqual<TCmd>(TCmd expected, TCmd actual, string? message = null)
         where TCmd : IMvuCommand {
      if (      expected is IMvuMessageCommand expectedMessageCommand
               && actual is IMvuMessageCommand actualMessageCommand)
         MessagesAreEqual(expectedMessageCommand.Message,
                            actualMessageCommand.Message);

      else if ( expected is IMvuEffectCommand expectedEffectCommand
               && actual is IMvuEffectCommand actualEffectCommand)
         EffectsAreEqual(expectedEffectCommand.Effect,
                           actualEffectCommand.Effect);

      else {
         Assert.Fail("Types don't match - expected<{0}>, actual<{1}>",
                     expected.GetType().GetFriendlyName(),
                     actual.GetType().GetFriendlyName());
      }
   }


   public static void MessagesAreEqual(IMvuMessage expected, IMvuMessage actual, string? message = null) {
      Assert.AreEqual(expected, actual, message);
   }


   public static void EffectsAreEqual(IMvuEffect expected, IMvuEffect actual, string? message = null) {
      Assert.AreEqual(expected, actual, message);
   }
}
