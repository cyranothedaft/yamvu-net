using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CounterSample.AppCore;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;
using WinFormsCounterSample.ViewPlatform;



namespace WinFormsCounterSample.gui;

internal static class EntryPoint {
   /// <summary>
   ///  The main entry point for the application.
   /// </summary>
   [STAThread]
   static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());

      const LogLevel minimumLogLevel = LogLevel.Trace;
      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                                  .SetMinimumLevel(minimumLogLevel));

      WinFormProgram winformProgram = new WinFormProgram();
      MvuPlatformProgram mvuPlatformProgram = new MvuPlatformProgram();

      // mvuPlatformProgram.SetComponentContainer(winformProgram.ComponentContainer);

      winformProgram.MainFormCloseRequested += mvuPlatformProgram.ExternalExitRequested;

      mvuPlatformProgram.ViewEmitted += winformProgram.ReplaceView;

      // winformProgram.AttachFormBehavior(form => mvuPlatformProgram.InjectIntoForm(form));
      // winformProgram.MainFormCloseRequested += Program_ExitRequested;

      Task mvuProgramTask = mvuPlatformProgram.StartAsync();
      RunAndHandleExceptionsAsync(mvuProgramTask); // essentially, fire and forget

      Task winformProgramTask = winformProgram.StartAsync();
      RunAndHandleExceptionsAsync(winformProgramTask); // essentially, fire and forget
      Application.Run();
   }


   private static void Program_ExitRequested(object? sender, EventArgs e) {
      // TODO?
   }


   private static async void RunAndHandleExceptionsAsync(Task task) {
      try {
         // Force this to yield to the caller, so Application.Run will be executing
         await Task.Yield();
         await task;
      }
      catch (Exception ex) {
         throw;
         //=== ...log the exception, show an error to the user, etc.
         Application.Exit();
      }
   }
}
