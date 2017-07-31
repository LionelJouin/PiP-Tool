using System;
using System.Windows.Forms;
using PiP_Tool;

namespace TestConsole
{
    public class Program
    {

        public static void Main(string[] args)
        {
            HotKey.RegisterHotKey(Keys.P, KeyModifiers.Alt);
            HotKey.HotKeyPressed += HotKeyManager_HotKeyPressed;

            do
            {
                var t = Console.ReadLine();
                if (t == "y")
                    break;
            } while (true);
        }

        public static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            Console.WriteLine("Hit me!");
        }

    }
}
