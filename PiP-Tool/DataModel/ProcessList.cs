using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Helpers.Native;

namespace PiP_Tool.DataModel
{
    public class ProcessList : IDisposable
    {

        #region public

        public List<WindowInfo> OpenWindows
        {
            get
            {
                var shellWindow = NativeMethods.GetShellWindow();
                var windows = new List<WindowInfo>();

                SetExcludedProcesses();

                NativeMethods.EnumWindows(delegate (IntPtr hWnd, int lParam)
                {
                    if (_excludedWindows.Contains(hWnd)) return true;
                    if (hWnd == shellWindow) return true;
                    if (!NativeMethods.IsWindowVisible(hWnd)) return true;

                    NativeMethods.DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.Cloaked, out bool isCloacked, Marshal.SizeOf(typeof(bool)));
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
        }
        public WindowInfo ForegroundWindow
        {
            get
            {
                var foregroundWindow = NativeMethods.GetForegroundWindow();
                return OpenWindows.FirstOrDefault(x => x.Handle == foregroundWindow);
            }
        }
        public event EventHandler OpenWindowsChanged;
        public event EventHandler ForegroundWindowChanged;

        #endregion

        #region private

        private List<IntPtr> _excludedWindows;
        private Hashtable _processes;
        private readonly IntPtr _createDestroyEventhook;
        private readonly NativeMethods.WinEventDelegate _createDestroyEventProc;
        private readonly IntPtr _foregroundEventhook;
        private readonly NativeMethods.WinEventDelegate _foregroundEventProc;

        #endregion

        public ProcessList()
        {
            _excludedWindows = new List<IntPtr>();
            GetProcesses();

            _createDestroyEventProc = CreateDestroyEventProc;
            _createDestroyEventhook = NativeMethods.SetWinEventHook(
                (uint)EventConstants.EVENT_OBJECT_CREATE,
                (uint)EventConstants.EVENT_OBJECT_DESTROY,
                IntPtr.Zero,
                _createDestroyEventProc,
                0, 0,
                (uint)EventConstants.WINEVENT_OUTOFCONTEXT | (uint)EventConstants.WINEVENT_SKIPOWNPROCESS
            );

            _foregroundEventProc = ForegroundEventProc;
            _foregroundEventhook = NativeMethods.SetWinEventHook(
                (uint)EventConstants.EVENT_SYSTEM_FOREGROUND,
                (uint)EventConstants.EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero,
                _foregroundEventProc,
                0, 0,
                (uint)EventConstants.WINEVENT_OUTOFCONTEXT | (uint)EventConstants.WINEVENT_SKIPOWNPROCESS
            );
        }

        ~ProcessList() => Dispose();

        public void Dispose()
        {
            NativeMethods.UnhookWinEvent(_createDestroyEventhook);
            NativeMethods.UnhookWinEvent(_foregroundEventhook);
        }

        private void ForegroundEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0 || eventType != (uint)EventConstants.EVENT_SYSTEM_FOREGROUND)
                return;
            OnForegroundWindowChanged();
        }

        private void GetProcesses()
        {
            _processes = new Hashtable();
            foreach (var p in Process.GetProcesses())
            {
                if (p.Id != 0)
                    _processes.Add(p.Id, p);
            }
        }

        private void CreateDestroyEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
                return;

            if (hwnd == IntPtr.Zero)
                return;

            NativeMethods.GetWindowThreadProcessId(hwnd, out var processId);
            if (processId == 0)
                return;

            switch (eventType)
            {
                case (uint)EventConstants.EVENT_OBJECT_CREATE:
                    try
                    {
                        if (_processes.ContainsKey((int)processId))
                            return;
                        
                        var p = Process.GetProcessById((int)processId);
                        _processes.Add(p.Id, p);
                        OnOpenWindowsChanged();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    break;
                case (uint)EventConstants.EVENT_OBJECT_DESTROY:
                    try
                    {
                        if (!_processes.ContainsKey((int)processId))
                            return;

                        var p = Process.GetProcessById((int)processId);
                        _processes.Remove(p.Id);
                        OnOpenWindowsChanged();
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

        public void OnForegroundWindowChanged()
        {
            ForegroundWindowChanged?.Invoke(this, new EventArgs());
        }

        private void SetExcludedProcesses()
        {
            var windowsList = Application.Current.Windows.Cast<Window>();
            _excludedWindows = new List<IntPtr>();
            foreach (var window in windowsList)
            {
                var handle = new WindowInteropHelper(window).Handle;
                _excludedWindows.Add(handle);
            }
        }

    }
}
