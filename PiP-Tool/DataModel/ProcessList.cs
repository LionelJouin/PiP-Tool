using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Helpers.Native;

namespace PiP_Tool.DataModel
{
    public class ProcessList
    {

        public List<WindowInfo> OpenWindows;
        public event EventHandler OpenWindowsChanged;

        private Process[] _processes;
        private readonly IntPtr _winEventhook;
        private readonly NativeMethods.WinEventDelegate _winEventProc;

        public ProcessList()
        {
            _processes = Process.GetProcesses();
            SetOpenWindows();

            _winEventProc = WinEventProc;
            _winEventhook = NativeMethods.SetWinEventHook(
                (uint)EventConstants.EVENT_OBJECT_CREATE,
                (uint)EventConstants.EVENT_OBJECT_DESTROY,
                IntPtr.Zero,
                _winEventProc,
                0, 0,
                (uint)EventConstants.WINEVENT_OUTOFCONTEXT | (uint)EventConstants.WINEVENT_SKIPOWNPROCESS
            );
        }

        public void Dispose()
        {
            NativeMethods.UnhookWinEvent(_winEventhook);
        }

        private void SetOpenWindows()
        {
            OpenWindows = GetOpenWindows();
            OnOpenWindowsChanged();

        }

        private List<WindowInfo> GetOpenWindows()
        {
            var shellWindow = NativeMethods.GetShellWindow();
            var windows = new List<WindowInfo>();

            NativeMethods.EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!NativeMethods.IsWindowVisible(hWnd)) return true;

                NativeMethods.DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.Cloaked, out var isCloacked, Marshal.SizeOf(typeof(bool)));
                if (isCloacked) return true;

                var length = NativeMethods.GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length);
                NativeMethods.GetWindowText(hWnd, builder, length + 1);

                windows.Add(new WindowInfo(hWnd));
                return true;
            }, 0);

            return windows;
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
                return;

            switch (eventType)
            {
                case (uint)EventConstants.EVENT_OBJECT_CREATE:

                    NativeMethods.GetWindowThreadProcessId(hwnd, out var processId);
                    if (processId == 0) return;

                    try
                    {
                        var p = Process.GetProcessById((int)processId);

                        if (_processes.FirstOrDefault(x => x.Id == p.Id) != null)
                            return;

                        _processes = Process.GetProcesses();
                        SetOpenWindows();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    break;

                case (uint)EventConstants.EVENT_OBJECT_DESTROY:

                    try
                    {
                        if (_processes.FirstOrDefault(x => x.MainWindowHandle == hwnd) == null) return;
                        _processes = Process.GetProcesses();
                        SetOpenWindows();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    break;
            }
        }

        public void OnOpenWindowsChanged()
        {
            OpenWindowsChanged?.Invoke(this, new EventArgs());
        }

    }
}
