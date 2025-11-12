using System;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;


namespace TuiCounterSample.ui;

static class EntryPoint {
   private static void Main(string[] args) {

      // Override the default configuration for the application to use the Light theme
      //ConfigurationManager.RuntimeConfig = """{ "Theme": "Light" }""";
      ConfigurationManager.Enable(ConfigLocations.All);



      Application.Run<ExampleWindow>().Dispose();

      // Before the application exits, reset Terminal.Gui for clean shutdown
      Application.Shutdown();

      // To see this output on the screen it must be done after shutdown,
      // which restores the previous screen.
      Console.WriteLine($"Username: {ExampleWindow.UserName}");
   }
}
