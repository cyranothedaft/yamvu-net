using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Windows.Win32;
using Windows.Win32.Foundation;


namespace yamvu.Extensions.WebView.Library.Window;

// based on this very good Stephen Toub article: https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps/
internal sealed class UiThreadSynchronizationContext : SynchronizationContext {
   private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> _queue = new();
   private readonly HWND _hwnd;


   public UiThreadSynchronizationContext(HWND hwnd) {
      _hwnd = hwnd;
   }


   public override void Post(SendOrPostCallback d, object? state) {
      _queue.Add(new KeyValuePair<SendOrPostCallback, object?>(d, state));
      PInvoke.PostMessage(_hwnd, Constants.WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE, 0, 0);
   }


   public override void Send(SendOrPostCallback d, object? state) {
      _queue.Add(new KeyValuePair<SendOrPostCallback, object?>(d, state));
      PInvoke.SendMessage(_hwnd, Constants.WM_SYNCHRONIZATIONCONTEXT_WORK_AVAILABLE, 0, 0);
   }


   public void RunAvailableWorkOnCurrentThread() {
      while (_queue.TryTake(out KeyValuePair<SendOrPostCallback, object?> workItem))
         workItem.Key(workItem.Value);
   }


   public void Complete() {
      _queue.CompleteAdding();
   }
}
