using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using Helpers.Native;
using PiP_Tool.DataModel;
using PiP_Tool.Helpers;
using PiP_Tool.Interfaces;
using PiP_Tool.Views;

namespace PiP_Tool.ViewModels
{
    public class Main : BaseViewModel, ICloseable
    {

        public event EventHandler<EventArgs> RequestClose;

        public ICommand StartPictureInPicture
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var selectedRegion = _selectorWindow.SelectedRegion;
                    Console.WriteLine("koukou " + selectedRegion);
                    _selectorWindow.Close();
                    var main = new PictureInPictureWindow(new SelectedWindow(SelectedWindowInfo, selectedRegion));
                    main.Show();
                    CloseWindow();
                });
            }
        }

        public ICommand StartSelector
        {
            get
            {
                return new RelayCommand(() =>
                {
                    //_selectedWindow = new SelectedWindow(SelectedWindowInfo, SelectedRegion);
                    _selectorWindow = new SelectorWindow(SelectedWindowInfo.Rect, SelectedWindowInfo.Position);
                    _selectorWindow.Show();
                    //CloseWindow();
                });
            }
        }


        public WindowInfo SelectedWindowInfo { get; set; }
        //public NativeStructs.Rect SelectedRegion => new NativeStructs.Rect(0, 0, SelectedWindowInfo.Size.Width, SelectedWindowInfo.Size.Height);

        public CollectionView WindowsList { get; }

        private SelectorWindow _selectorWindow;
        //private SelectedWindow _selectedWindow { get; set; }

        public Main()
        {
            var openWindows = GetOpenWindows();
            WindowsList = new CollectionView(openWindows);
        }

        // https://stackoverflow.com/questions/43927156/enumwindows-returns-closed-windows-store-applications
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

        private void CloseWindow()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

    }
}
