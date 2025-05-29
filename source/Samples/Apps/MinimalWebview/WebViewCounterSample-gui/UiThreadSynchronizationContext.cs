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

// based on this very good Stephen Toub article: https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps/
internal sealed class UiThreadSynchronizationContext : SynchronizationContext {
   private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> m_queue = new();
   private readonly HWND hwnd;


   public UiThreadSynchronizationContext(HWND hwnd)
         : base() {
      this.hwnd = hwnd;
   }


   public override void Post(SendOrPostCallback d, object state) {
      m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
      PInvoke.PostMessage(hwnd, Constants.WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE, 0, 0);
   }


   public override void Send(SendOrPostCallback d, object state) {
      m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
      PInvoke.SendMessage(hwnd, Constants.WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE, 0, 0);
   }


   public void RunAvailableWorkOnCurrentThread() {
      while (m_queue.TryTake(out KeyValuePair<SendOrPostCallback, object> workItem))
         workItem.Key(workItem.Value);
   }


   public void Complete() {
      m_queue.CompleteAdding();
   }
}
