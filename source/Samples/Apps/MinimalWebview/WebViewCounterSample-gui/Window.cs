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


namespace MinimalWebViewCounterSample;

public static class Window {
   // private readonly UiThreadSynchronizationContext _uiThreadSyncCtx;
   // private readonly HWND _hwnd;
   //
   //
   // private Window(UiThreadSynchronizationContext uiThreadSyncCtx, HWND hwnd) {
   //    _uiThreadSyncCtx = uiThreadSyncCtx;
   //    _hwnd            = hwnd;
   // }


   internal static TResult CreateAndShow<TResult>(string windowTitle, uint backgroundColor,
                                      Func<HWND, TResult> afterShowWindowFunc) {
      UiThreadSynchronizationContext? uiThreadSyncCtx = null;

      // ReSharper disable once AccessToModifiedClosure
      // ^-- this is *probably* okay, right?
      HWND hwnd = registerAndCreate(() => uiThreadSyncCtx, 
                                          // () => controller, 
                                          windowTitle, backgroundColor);
      if (hwnd.Value == 0)
         throw new Exception("hwnd not created");

      SynchronizationContext.SetSynchronizationContext(uiThreadSyncCtx = new UiThreadSynchronizationContext(hwnd)); // minimize the time that uiThreadSyncCtx is non-null and not yet set as the synchronization context

      PInvoke.ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_NORMAL);

      return afterShowWindowFunc(hwnd);
   }


   private static HWND registerAndCreate(Func<UiThreadSynchronizationContext?> uiThreadSyncCtxFunc,
                                         // Func<CoreWebView2Controller?> controllerFunc,
                                         string windowTitle, uint backgroundColor) {
      unsafe {

         HBRUSH backgroundBrush = PInvoke.CreateSolidBrush(backgroundColor);
         if (backgroundBrush.IsNull) {
            // fallback to the system background color in case it fails
            backgroundBrush = (HBRUSH)(IntPtr)(SYS_COLOR_INDEX.COLOR_BACKGROUND + 1);
         }

         HINSTANCE hInstance = PInvoke.GetModuleHandle((char*)null);
         ushort classId = registerWindowClass(uiThreadSyncCtxFunc, 
                                              // controllerFunc, 
                                              hInstance, backgroundBrush, windowTitle);
         if (classId == 0)
            throw new Exception("class not registered");

         return createWindow(hInstance, classId, windowTitle);


         static ushort registerWindowClass(Func<UiThreadSynchronizationContext?> uiThreadSyncCtxFunc,
                                           // Func<CoreWebView2Controller?> controllerFunc,
                                           HINSTANCE hInstance, HBRUSH backgroundBrush, string windowTitle) {
            fixed ( char* classNamePtr = windowTitle ) {
               WNDCLASSW wc = new()
                                 {
                                    lpfnWndProc   = (hwnd, msg, wParam, lParam) => WndProc(uiThreadSyncCtxFunc, 
                                                                                           // controllerFunc, 
                                                                                           hwnd, msg, wParam, lParam),
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


   private static LRESULT WndProc(Func<UiThreadSynchronizationContext?> uiThreadSyncCtxFunc,
                                  // Func<CoreWebView2Controller?> controllerFunc,
                                  HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam) {
      switch (msg) {
         // case PInvoke.WM_SIZE:
         //    OnSize(controllerFunc(), hwnd, wParam, GetLowWord(lParam.Value), GetHighWord(lParam.Value));
         //    break;

         case Constants.WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE:
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
  
}
