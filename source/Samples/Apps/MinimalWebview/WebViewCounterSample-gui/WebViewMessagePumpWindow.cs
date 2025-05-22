using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.Extensions.Logging;



namespace MinimalWebView;


/// <summary>
/// copied and adapted from:  https://github.com/rgwood/MinimalWebView
/// </summary>
public static class WebViewMessagePumpWindow {

   private const uint WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE = PInvoke.WM_USER + 1;

   private const string StaticFileDirectory = "wwwroot";
   private const string VirtualHostName = "minimalwebview.example";


   public static int Run(string windowTitle, uint backgroundColor, ILogger? logger = null) {
      UiThreadSynchronizationContext? uiThreadSyncCtx = null;
      CoreWebView2Controller? controller = null;

      // v-- this is *probably* okay, right?
      // ReSharper disable once AccessToModifiedClosure
      HWND hwnd = registerAndCreateWindow(() => uiThreadSyncCtx, () => controller, windowTitle, backgroundColor);
      if (hwnd.Value == 0)
         throw new Exception("hwnd not created");

      PInvoke.ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_NORMAL);

      SynchronizationContext.SetSynchronizationContext(uiThreadSyncCtx = new UiThreadSynchronizationContext(hwnd)); // minimize the time that uiThreadSyncCtx is non-null and not yet set as the synchronization context

      // Start initializing WebView2 in a fire-and-forget manner. Errors will be handled in the initialization function
      _ = createCoreWebView2Async(hwnd, newcontroller => { controller = newcontroller;}, logger);

      int exitCode = runMessagePump(logger);
      return exitCode;
   }


   private static HWND registerAndCreateWindow(Func<UiThreadSynchronizationContext?> uiThreadSyncCtxFunc, Func<CoreWebView2Controller?> controllerFunc,
                                               string windowTitle, uint backgroundColor) {
      unsafe {

         HBRUSH backgroundBrush = PInvoke.CreateSolidBrush(backgroundColor);
         if (backgroundBrush.IsNull) {
            // fallback to the system background color in case it fails
            backgroundBrush = (HBRUSH)(IntPtr)(SYS_COLOR_INDEX.COLOR_BACKGROUND + 1);
         }

         HINSTANCE hInstance = PInvoke.GetModuleHandle((char*)null);
         ushort classId = registerWindowClass(uiThreadSyncCtxFunc, controllerFunc, hInstance, backgroundBrush, windowTitle);
         if (classId == 0)
            throw new Exception("class not registered");

         return createWindow(hInstance, classId, windowTitle);


         static ushort registerWindowClass(Func<UiThreadSynchronizationContext?> uiThreadSyncCtxFunc, Func<CoreWebView2Controller?> controllerFunc,
                                           HINSTANCE hInstance, HBRUSH backgroundBrush, string windowTitle) {
            fixed ( char* classNamePtr = windowTitle ) {
               WNDCLASSW wc = new()
                                 {
                                    lpfnWndProc   = (hwnd, msg, wParam, lParam) => WndProc(uiThreadSyncCtxFunc, controllerFunc, hwnd, msg, wParam, lParam),
                                    lpszClassName = classNamePtr,
                                    hInstance     = hInstance,
                                    hbrBackground = backgroundBrush,
                                    style         = WNDCLASS_STYLES.CS_VREDRAW | WNDCLASS_STYLES.CS_HREDRAW
                                 };
               return PInvoke.RegisterClass(wc);
            }
         }

         static HWND createWindow(HINSTANCE hInstance, ushort classId, string windowTitle) {
            fixed ( char* windowNamePtr = $"{windowTitle} {Assembly.GetExecutingAssembly().GetName().Version}" ) {
               return PInvoke.CreateWindowEx(
                                             0,
                                             (char*)classId,
                                             windowNamePtr,
                                             WINDOW_STYLE.WS_OVERLAPPEDWINDOW,
                                             PInvoke.CW_USEDEFAULT, PInvoke.CW_USEDEFAULT, 600, 500,
                                             new HWND(),
                                             new HMENU(),
                                             hInstance,
                                             null);
            }
         }
      }
   }


   private static async Task createCoreWebView2Async(HWND hwnd, Action<CoreWebView2Controller> setControllerAction, ILogger? logger) {
      try {
         logger?.LogDebug("Initializing WebView2");
         var environment = await CoreWebView2Environment.CreateAsync(null, null, null);
         CoreWebView2Controller controller = await environment.CreateCoreWebView2ControllerAsync(hwnd);
         setControllerAction(controller);

         controller.DefaultBackgroundColor          =  Color.Transparent; // avoids flash of white when page first renders
         controller.CoreWebView2.WebMessageReceived += (sender, args) => CoreWebView2_WebMessageReceived(controller, sender, args);
         controller.CoreWebView2.SetVirtualHostNameToFolderMapping(VirtualHostName, StaticFileDirectory, CoreWebView2HostResourceAccessKind.Allow);
         PInvoke.GetClientRect(hwnd, out var hwndRect);
         controller.Bounds    = new Rectangle(0, 0, hwndRect.right, hwndRect.bottom);
         controller.IsVisible = true;
         controller.CoreWebView2.Navigate($"https://{VirtualHostName}/index.html");

         logger?.LogDebug("WebView2 initialization succeeded.");
      }
      catch (WebView2RuntimeNotFoundException) {
         var result = PInvoke.MessageBox(hwnd, "WebView2 runtime not installed.\r\n" +
                                               "Download and install:\r\n"           +
                                               "https://developer.microsoft.com/en-us/microsoft-edge/webview2?form=MA13LH",
                                         "Error", MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONERROR);

         if (result == MESSAGEBOX_RESULT.IDYES) {
            //TODO: download WV2 bootstrapper from https://go.microsoft.com/fwlink/p/?LinkId=2124703 and run it
         }

         Environment.Exit(1);
      }
      catch (Exception ex) {
         PInvoke.MessageBox(hwnd, $"Failed to initialize WebView2:{Environment.NewLine}{ex}", "Error", MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONERROR);
         Environment.Exit(1);
      }
   }


   private static int runMessagePump(ILogger? logger) {
      logger?.LogDebug("Starting message pump");
      MSG msg;
      while (PInvoke.GetMessage(out msg,
                                new HWND(), // TODO ???
                                0, 0)) {
         PInvoke.TranslateMessage(msg);
         PInvoke.DispatchMessage(msg);
      }

      return (int)msg.wParam.Value;
   }


   private static LRESULT WndProc(Func<UiThreadSynchronizationContext?> uiThreadSyncCtxFunc, Func<CoreWebView2Controller?> controllerFunc,
                                  HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam) {
      switch (msg) {
         case PInvoke.WM_SIZE:
            OnSize(controllerFunc(), hwnd, wParam, GetLowWord(lParam.Value), GetHighWord(lParam.Value));
            break;

         case WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE:
            uiThreadSyncCtxFunc()
                 ?.RunAvailableWorkOnCurrentThread();
            break;

         case PInvoke.WM_CLOSE:
            PInvoke.PostQuitMessage(0);
            break;
      }

      return PInvoke.DefWindowProc(hwnd, msg, wParam, lParam);
   }


   private static void OnSize(CoreWebView2Controller? controller, HWND hwnd, WPARAM wParam, int width, int height) {
      if (controller != null)
         controller.Bounds = new Rectangle(0, 0, width, height);
   }


   private static async void CoreWebView2_WebMessageReceived(CoreWebView2Controller controller, object sender, CoreWebView2WebMessageReceivedEventArgs e) {
      var webMessage = e.TryGetWebMessageAsString();
      if (string.IsNullOrEmpty(webMessage))
         return;

      // // simulate moving some slow operation to a background thread
      // await Task.Run(() => Thread.Sleep(200));

      // this will blow up if not run on the UI thread, so the SynchronizationContext needs to have been wired up correctly
      await controller.CoreWebView2.ExecuteScriptAsync($"replaceView('New View: {webMessage}')");
   }


   private static int GetLowWord(nint value) {
      uint xy = (uint)value;
      int x = unchecked( (short)xy );
      return x;
   }


   private static int GetHighWord(nint value) {
      uint xy = (uint)value;
      int y = unchecked( (short)(xy >> 16) );
      return y;
   }



   // based on this very good Stephen Toub article: https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps/
   private sealed class UiThreadSynchronizationContext : SynchronizationContext {
      private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> m_queue = new();
      private readonly HWND hwnd;


      public UiThreadSynchronizationContext(HWND hwnd)
            : base() {
         this.hwnd = hwnd;
      }


      public override void Post(SendOrPostCallback d, object state) {
         m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
         PInvoke.PostMessage(hwnd, WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE, 0, 0);
      }


      public override void Send(SendOrPostCallback d, object state) {
         m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
         PInvoke.SendMessage(hwnd, WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE, 0, 0);
      }


      public void RunAvailableWorkOnCurrentThread() {
         while (m_queue.TryTake(out KeyValuePair<SendOrPostCallback, object> workItem))
            workItem.Key(workItem.Value);
      }


      public void Complete() {
         m_queue.CompleteAdding();
      }
   }
}
