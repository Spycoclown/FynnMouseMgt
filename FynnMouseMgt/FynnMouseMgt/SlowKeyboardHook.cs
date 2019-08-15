using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace FynnMouseMgt
{
  public static class SlowHotKeyManager
  {
    public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

    public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
    {
      _windowReadyEvent.WaitOne();
      int id = System.Threading.Interlocked.Increment(ref _id);
      _wnd.Invoke(new RegisterHotKeyDelegate(RegisterHotKeyInternal), _hwnd, id, (uint)modifiers, (uint)key);
      return id;
    }

    public static void UnregisterHotKey(int id)
    {
      _wnd.Invoke(new UnRegisterHotKeyDelegate(UnRegisterHotKeyInternal), _hwnd, id);
    }

    delegate void RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);
    delegate void UnRegisterHotKeyDelegate(IntPtr hwnd, int id);

    private static void RegisterHotKeyInternal(IntPtr hwnd, int id, uint modifiers, uint key)
    {      
      RegisterHotKey(hwnd, id, modifiers, key);      
    }

    private static void UnRegisterHotKeyInternal(IntPtr hwnd, int id)
    {
      UnregisterHotKey(_hwnd, id);
    }    

    private static void OnHotKeyPressed(HotKeyEventArgs e)
    {
      if (SlowHotKeyManager.HotKeyPressed != null)
      {
        SlowHotKeyManager.HotKeyPressed(null, e);
      }
    }

    private static volatile MessageWindow _wnd;
    private static volatile IntPtr _hwnd;
    private static ManualResetEvent _windowReadyEvent = new ManualResetEvent(false);
    static SlowHotKeyManager()
    {
      Thread messageLoop = new Thread(delegate()
        {
          Application.Run(new MessageWindow());
        });
      messageLoop.Name = "MessageLoopThread";
      messageLoop.IsBackground = true;
      messageLoop.Start();      
    }

    private class MessageWindow : Form
    {
      public MessageWindow()
      {
        _wnd = this;
        _hwnd = this.Handle;
        _windowReadyEvent.Set();
      }

      protected override void WndProc(ref Message m)
      {
        if (m.Msg == WM_HOTKEY)
        {
          HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
          SlowHotKeyManager.OnHotKeyPressed(e);
        }

        base.WndProc(ref m);
      }

      protected override void SetVisibleCore(bool value)
      {
        // Ensure the window never becomes visible
        base.SetVisibleCore(false);
      }

      private const int WM_HOTKEY = 0x312;
    }

    [DllImport("user32", SetLastError=true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private static int _id = 0;
  }
}