using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using static NativeMethods;

public static class MouseHook
{
    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_RBUTTONUP = 0x0205;
    private const int WM_MOUSEMOVE = 0x0200;

    private static IntPtr hookId = IntPtr.Zero;
    private static NativeMethods.LowLevelMouseProc hookCallback;

    public static event EventHandler<Point> MouseMove;
    public static event EventHandler<MouseClickEventArgs> MouseClickAction;

    public static void Start()
    {
        hookCallback = MouseHookCallback;
        using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            hookId = NativeMethods.SetWindowsHookEx(WH_MOUSE_LL, hookCallback, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    public static void Stop()
    {
        NativeMethods.UnhookWindowsHookEx(hookId);
    }

    private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if(nCode >= 0)
        {
            if (wParam == (IntPtr)WM_MOUSEMOVE)
            {
                // Marshal the lParam to get mouse coordinates
                var hookStruct = (NativeMethods.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MSLLHOOKSTRUCT));

                // Raise the MouseMove event with the mouse coordinates
                MouseMove?.Invoke(null, new Point(hookStruct.pt.x, hookStruct.pt.y));
            }
            else
            {
                int msg = wParam.ToInt32();
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                switch (msg)
                {
                    case WM_LBUTTONDOWN:
                        MouseClickAction?.Invoke(null, new MouseClickEventArgs(MouseAction.LMBDown)); 
                        break;
                    case WM_LBUTTONUP:
                        MouseClickAction?.Invoke(null, new MouseClickEventArgs(MouseAction.LMBUp));
                        break;
                    case WM_RBUTTONDOWN:
                        MouseClickAction?.Invoke(null, new MouseClickEventArgs(MouseAction.RMBDown));
                        break;
                    case WM_RBUTTONUP:
                        MouseClickAction?.Invoke(null, new MouseClickEventArgs(MouseAction.RMBUp));
                        break;
                }
            }
        }

        return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);
    }
}

public class MouseClickEventArgs : EventArgs
{
    public MouseAction Action { get; }

    public MouseClickEventArgs(MouseAction action)
    {
        Action = action;
    }
}

public enum MouseAction
{
    LMBDown,
    RMBDown,
    LMBUp,
    RMBUp
}

internal static class NativeMethods
{
    public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
