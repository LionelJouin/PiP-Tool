using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Helpers;
using Helpers.Native;

namespace TestConsole
{
    public class Program
    {

        public static void Main(string[] args)
        {
            HotKey.RegisterHotKey(Keys.P, KeyModifiers.Alt);
            HotKey.HotKeyPressed += HotKeyPressed;

            do
            {
                var t = Console.ReadLine();
                if (t == "y")
                    break;
            } while (true);
        }

        // https://www.codeproject.com/Articles/20651/Capturing-Minimized-Window-A-Kid-s-Trick
        // https://stackoverflow.com/questions/2832217/modify-the-windows-style-of-another-application-using-winapi
        public static void HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            var foregroundWindow = NativeMethods.GetForegroundWindow();
            Console.WriteLine("HotKey : " + GetWindowTitle(foregroundWindow));
            var style = NativeMethods.GetWindowLong(foregroundWindow, NativeConsts.GWL_STYLE);

            // "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --app=https://www.youtube.com/?gl=FR&hl=fr
            NativeMethods.SetWindowLong(foregroundWindow, NativeConsts.GWL_STYLE, (uint)style & ~(uint)NativeEnums.WindowStyles.WS_CAPTION);

            SetWindowOnTop(foregroundWindow);
        }

        public static void SetWindowOnTop(IntPtr window)
        {

            NativeMethods.SetWindowPos(
                    window,
                    (IntPtr)NativeEnums.SpecialWindowHandles.HWND_TOPMOST,
                    0, 0, 0, 0,
                    (int)NativeEnums.SetWindowPosFlags.SWP_NOMOVE | (int)NativeEnums.SetWindowPosFlags.SWP_NOSIZE | (int)NativeEnums.SetWindowPosFlags.SWP_FRAMECHANGED
                    );
        }

        private static string GetWindowTitle(IntPtr window)
        {
            const int nChars = 256;
            var buff = new StringBuilder(nChars);

            if (NativeMethods.GetWindowText(window, buff, nChars) > 0)
            {
                return buff.ToString();
            }
            return null;
        }

    }
}
