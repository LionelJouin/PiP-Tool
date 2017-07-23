using System.Collections.Generic;
using System.Diagnostics;
using Helpers.Native;
using PiP_Tool.Models;

namespace PiP_Tool.ViewModels
{
    public class WindowSelector : BaseViewModel
    {

        private List<WindowInfo> _windows;

        public WindowSelector()
        {
            _windows = new List<WindowInfo>();
            var procceses = Process.GetProcesses();
            foreach (var process in procceses)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle) && NativeMethods.IsWindowVisible(process.MainWindowHandle))
                {
                    _windows.Add(new WindowInfo(process.MainWindowHandle));
                }
            }
        }
        
    }
}
