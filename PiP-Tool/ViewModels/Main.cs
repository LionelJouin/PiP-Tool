using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using Helpers.Native;
using PiP_Tool.DataModel;
using PiP_Tool.Helpers;
using PiP_Tool.Views;

namespace PiP_Tool.ViewModels
{
    public class Main : BaseViewModel
    {

        public ICommand StartPictureInPicture
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_selectorWindow == null)
                        return;
                    var selectedRegion = _selectorWindow.SelectedRegion;
                    _selectorWindow.Close();
                    var pip = new PictureInPictureWindow(new SelectedWindow(SelectedWindowInfo, selectedRegion));
                    pip.Show();
                    CloseWindow();
                });
            }
        }

        public WindowInfo SelectedWindowInfo
        {
            get => _selectedWindowInfo;
            set
            {
                _selectedWindowInfo = value;
                ShowSelector();
            }

        }
        public CollectionView WindowsList { get; }

        private SelectorWindow _selectorWindow;
        private WindowInfo _selectedWindowInfo;

        public Main()
        {
            var openWindows = GetOpenWindows();
            WindowsList = new CollectionView(openWindows);
        }

        public static List<WindowInfo> GetOpenWindows()
        {
            var shellWindow = NativeMethods.GetShellWindow();
            var windows = new List<WindowInfo>();

            NativeMethods.EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!NativeMethods.IsWindowVisible(hWnd)) return true;

                NativeMethods.DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.Cloaked, out var isCloacked, Marshal.SizeOf(typeof(bool)));
                if (isCloacked) return true;

                var length = NativeMethods.GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length);
                NativeMethods.GetWindowText(hWnd, builder, length + 1);

                //windows[hWnd] = builder.ToString();
                windows.Add(new WindowInfo(hWnd));
                return true;

            }, 0);

            return windows;
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _selectorWindow?.Close();
        }

        private void ShowSelector()
        {
            _selectorWindow?.Close();
            _selectorWindow = new SelectorWindow(SelectedWindowInfo);
            _selectorWindow.Show();
        }

    }
}
