using System;
using System.Collections.Generic;
using System.Diagnostics;
using Helpers.Native;
using PiP_Tool.Models;
using Point = System.Windows.Point;

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

        public void OnMouseMove(Point point)
        {
            foreach (var window in _windows)
            {
                //if (window.Size.Contains(point))
                //    Console.WriteLine(window.Title);
            }
        }


    }
}
