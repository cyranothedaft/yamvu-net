using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;
using WinFormsCounterSample.ViewPlatform;
using yamvu;



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

      ExternalMessageDispatcher externalMessageDispatcher = new();

      WinFormProgram winformProgram = new WinFormProgram();
      MvuPlatformProgram mvuPlatformProgram = new MvuPlatformProgram(externalMessageDispatcher);

      // mvuPlatformProgram.SetComponentContainer(winformProgram.ComponentContainer);

      winformProgram.ExitRequested += mvuPlatformProgram.ExternalExitRequested;

      mvuPlatformProgram.ViewEmitted += winformProgram.ReplaceView;
      mvuPlatformProgram.ProgramExited += winformProgram.InternalExitRequested;

      // winformProgram.AttachFormBehavior(form => mvuPlatformProgram.InjectIntoForm(form));
      // winformProgram.MainFormCloseRequested += Program_ExitRequested;


      async Task runMvuProgram() {
         await mvuPlatformProgram.RunAsync();
         Debug.WriteLine("MVU program terminated");
      }


      Task mvuProgramTask = runMvuProgram();
      RunAndHandleExceptionsAsync(mvuProgramTask);

      Task winformProgramTask = winformProgram.StartAsync();
      RunAndHandleExceptionsAsync(winformProgramTask); // essentially, fire and forget
      Application.Run();
      Debug.WriteLine("Winform program terminated");
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
