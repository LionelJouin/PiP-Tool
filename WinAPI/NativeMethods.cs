using System;
using System.Runtime.InteropServices;

namespace WinAPI
{
    public class NativeMethods
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

    }
}
