using System.Collections.Generic;
using System.Diagnostics;
using WinAPI;

namespace AppLst
{
    public class WindowsList
    {
        private List<Window> _windows;

        public WindowsList()
        {
            _windows = new List<Window>();
            var procceses = Process.GetProcesses();
            foreach (var process in procceses)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle) && NativeMethods.IsWindowVisible(process.MainWindowHandle))
                {
                    _windows.Add(new Window());
                }
            }
        }

    }
}
