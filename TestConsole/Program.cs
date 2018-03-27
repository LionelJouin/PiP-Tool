using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Helpers;
using Helpers.Native;

namespace TestConsole
{
    public class Program
    {

        private static List<Window> _windows;

        public static void Main(string[] args)
        {
            _windows = new List<Window>();

            // ALT + P
            HotKey.RegisterHotKey(Keys.P, KeyModifiers.Alt);
            HotKey.HotKeyPressed += HotKeyPressed;

            do
            {
                var t = Console.ReadLine();
                if (t == "y")
                    break;
            } while (true);

            foreach (var window in _windows)
            {
                window.SetWindowPiP(false);
            }
        }

        // https://www.codeproject.com/Articles/20651/Capturing-Minimized-Window-A-Kid-s-Trick
        // https://stackoverflow.com/questions/2832217/modify-the-windows-style-of-another-application-using-winapi
        public static void HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            var foregroundWindow = NativeMethods.GetForegroundWindow();
            var windowInPipMode = _windows.Any(x => x.Equals(foregroundWindow));
            if (windowInPipMode)
            {
                var window = _windows.First(x => x.Equals(foregroundWindow));
                window.SetWindowPiP(false);
                _windows.Remove(window);
            }
            else
            {
                var window = new Window(foregroundWindow);
                _windows.Add(window);
                window.SetWindowPiP();
            }
        }

    }
}
