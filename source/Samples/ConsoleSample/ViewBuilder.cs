using System;
using ConsoleSample.Mvu;
using ConsoleSample.PlatAgnAppCore;
using Microsoft.Extensions.Logging;
using yamvu.core;



namespace ConsoleSample;

internal class ViewBuilder {
   public static View BuildInitialView()
      => new View(View.StaticHeader,
                  "(no key has been pressed",
                  fillCharArray('.'));
/*
      char[,] initialViewChars =
            {
               { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', },
               { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', },
               { '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', },
               { '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', },
               { '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', },
               { '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', },
               { '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', },
               { '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', },
               { '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', },
               { '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', },
            };
 */
   
   public static View BuildFromModel(MvuMessageDispatchDelegate dispatch, Model model, ProgramEventSources eventSources, ILogger? uilogger)
      => new View(View.StaticHeader,
                  $"(status)",
                  fillCharArray(model.Char));


   private static char[,] createEmptyCharArray()
      => new char[View.Width, View.Height];


   private static char[,] fillCharArray(char keyChar)
      => fillCharArray(createEmptyCharArray(), keyChar);


   private static char[,] fillCharArray(char[,] array, char keyChar) {
      int rows = array.GetLowerBound(0),
          cols = array.GetLowerBound(1);
      for (int r = 0; r < rows; ++r)
      for (int c = 0; c < cols; ++c)
         array[r, c] = keyChar;
      return array;
   }
}
