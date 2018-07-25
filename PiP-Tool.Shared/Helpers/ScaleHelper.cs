using System;
using System.Drawing;
using PiP_Tool.Native;

namespace PiP_Tool.Shared.Helpers
{
    public class ScaleHelper
    {

        /// <summary>
        /// Get scale setting (Display settings on Windows)
        /// https://stackoverflow.com/questions/5977445/how-to-get-windows-display-settings
        /// </summary>
        public static float ScalingFactor
        {
            get
            {
                var g = Graphics.FromHwnd(IntPtr.Zero);
                var desktop = g.GetHdc();
                var logicalScreenHeight = NativeMethods.GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
                var physicalScreenHeight = NativeMethods.GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

                var screenScalingFactor = physicalScreenHeight / (float)logicalScreenHeight;

                return screenScalingFactor;
            }
        }

    }
}
