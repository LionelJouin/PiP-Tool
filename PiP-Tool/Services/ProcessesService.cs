using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using PiP_Tool.DataModel;
using PiP_Tool.Native;
using PiP_Tool.Shared;

namespace PiP_Tool.Services
{
    public class ProcessesService : IDisposable
    {

        #region public

        /// <summary>
        /// Gets all open windows exepted <see cref="_excludedWindows"/> 
        /// </summary>
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
        /// <summary>
        /// Gets current foreground window
        /// </summary>
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

        /// <summary>
        /// Gets the instance of the singleton
        /// </summary>
        public static ProcessesService Instance => _instance ?? (_instance = new ProcessesService());

        #endregion

        #region private

        private static ProcessesService _instance;
        private List<IntPtr> _excludedWindows;
        private Hashtable _processes;
        private readonly IntPtr _createDestroyEventhook;
        private readonly NativeMethods.WinEventDelegate _createDestroyEventProc;
        private readonly IntPtr _foregroundEventhook;
        private readonly NativeMethods.WinEventDelegate _foregroundEventProc;

        #endregion

        /// <summary>
        /// Constructor (Singleton so private)
        /// </summary>
        private ProcessesService()
        {
            Logger.Instance.Info("Init processes service");

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

        /// <summary>
        /// Destructor
        /// </summary>
        ~ProcessesService() => Dispose();

        /// <inheritdoc />
        /// <summary>
        /// Dispose: Unhook, singleton
        /// </summary>
        public void Dispose()
        {
            NativeMethods.UnhookWinEvent(_createDestroyEventhook);
            NativeMethods.UnhookWinEvent(_foregroundEventhook);
            _instance = null;
        }

        /// <summary>
        /// Callback function that the system calls in response to event sytem foreground
        /// </summary>
        /// <param name="hWinEventHook">Handle to an event hook function</param>
        /// <param name="eventType">Specifies the event that occurred</param>
        /// <param name="hwnd">Handle to the window that generates the event, or NULL if no window is associated with the event</param>
        /// <param name="idObject">Identifies the object associated with the event</param>
        /// <param name="idChild">Identifies whether the event was triggered by an object or a child element of the object</param>
        /// <param name="dwEventThread">Identifies the thread that generated the event, or the thread that owns the current window</param>
        /// <param name="dwmsEventTime">Specifies the time, in milliseconds, that the event was generated</param>
        private void ForegroundEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0 || eventType != (uint)EventConstants.EVENT_SYSTEM_FOREGROUND)
                return;
            ForegroundWindowChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Callback function that the system calls in response to events object create or destroy
        /// </summary>
        /// <param name="hWinEventHook">Handle to an event hook function</param>
        /// <param name="eventType">Specifies the event that occurred</param>
        /// <param name="hwnd">Handle to the window that generates the event, or NULL if no window is associated with the event</param>
        /// <param name="idObject">Identifies the object associated with the event</param>
        /// <param name="idChild">Identifies whether the event was triggered by an object or a child element of the object</param>
        /// <param name="dwEventThread">Identifies the thread that generated the event, or the thread that owns the current window</param>
        /// <param name="dwmsEventTime">Specifies the time, in milliseconds, that the event was generated</param>
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
                        OpenWindowsChanged?.Invoke(this, new EventArgs());
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
                        OpenWindowsChanged?.Invoke(this, new EventArgs());
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    break;
            }
        }

        /// <summary>
        /// Get all current processes
        /// </summary>
        private void GetProcesses()
        {
            _processes = new Hashtable();
            foreach (var p in Process.GetProcesses())
            {
                if (p.Id != 0)
                    _processes.Add(p.Id, p);
            }
        }

        /// <summary>
        /// Get all windows of this software and add them to excluded list
        /// </summary>
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
