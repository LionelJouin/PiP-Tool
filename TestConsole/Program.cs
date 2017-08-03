using System;
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

        // https://stackoverflow.com/questions/2832217/modify-the-windows-style-of-another-application-using-winapi
        public static void HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            var foregroundWindow = NativeMethods.GetForegroundWindow();
            Console.WriteLine("HotKey : " + GetWindowTitle(foregroundWindow));
            var style = NativeMethods.GetWindowLong(foregroundWindow, NativeConsts.GWL_STYLE);

            SetWindowOnTop(foregroundWindow);
            NativeMethods.SetWindowLongPtr(foregroundWindow, NativeConsts.GWL_STYLE, (style & ~NativeEnums.WindowStyles.WS_CAPTION));
        }

        public static void SetWindowOnTop(IntPtr window)
        {

            NativeMethods.SetWindowPos(
                window,
                (IntPtr)NativeEnums.SpecialWindowHandles.HWND_TOPMOST,
                    0, 0, 0, 0,
                    (int)NativeEnums.SetWindowPosFlags.SWP_NOMOVE | (int)NativeEnums.SetWindowPosFlags.SWP_NOSIZE
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
