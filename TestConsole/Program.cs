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

        public static void HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            Console.WriteLine("HotKey : " + GetActiveWindowTitle());
        }

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var Buff = new StringBuilder(nChars);
            var foregroundWindow = NativeMethods.GetForegroundWindow();

            if (NativeMethods.GetWindowText(foregroundWindow, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

    }
}
