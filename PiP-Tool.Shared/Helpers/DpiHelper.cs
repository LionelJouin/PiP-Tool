using System;
using System.Drawing;
using PiP_Tool.Native;

namespace PiP_Tool.Shared.Helpers
{
    public static class DpiHelper
    {

        /// <summary>
        /// Get monitor dpi for a window
        /// </summary>
        /// <param name="hwnd">handle of the window</param>
        /// <param name="dpiX">out dpi X (horizontal)</param>
        /// <param name="dpiY">out dpi Y (vertical)</param>
        public static void GetDpi(IntPtr hwnd, out float dpiX, out float dpiY)
        {
            try
            {
                var handle = NativeMethods.MonitorFromWindow(hwnd, MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);
                NativeMethods.GetDpiForMonitor(handle, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var x, out var y);

                dpiX = x / 96f;
                dpiY = y / 96f;
            }
            catch (Exception)
            {
                dpiX = 1;
                dpiY = 1;
            }
        }

        /// <summary>
        /// Get dpi of primary monitor
        /// </summary>
        /// <param name="dpiX">out dpi X (horizontal)</param>
        /// <param name="dpiY">out dpi Y (vertical)</param>
        public static void GetPrimaryDpi(out float dpiX, out float dpiY)
        {
            var g = Graphics.FromHwnd(IntPtr.Zero);

            dpiX = g.DpiX / 96f;
            dpiY = g.DpiY / 96f;
        }

    }
}