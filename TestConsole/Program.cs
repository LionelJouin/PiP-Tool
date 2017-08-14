using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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

        // https://stackoverflow.com/questions/2832217/modify-the-windows-style-of-another-application-using-winapi
        public static void HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            var foregroundWindow = NativeMethods.GetForegroundWindow();
            var window = new Window(foregroundWindow);
            _windows.Add(window);
            window.SetWindowPiP();
        }

    }
}
