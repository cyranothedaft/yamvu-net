using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WelterKit.Testing;



namespace yamvu.Testing;

public static class MvuAssert {

   public static void AreEqual<TModel, TCmd>((TModel model, TCmd[] commands) expected,
                                             (TModel model, TCmd[] commands) actual) {
      Assert.AreEqual(expected.model, actual.model, "Model");
      SequenceAssert.AreEqual(expected.commands, actual.commands, 
                              assertAreEqualAction: Assert.AreEqual, "Model");
   }
}
