using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using Helpers.Native;
using PiP_Tool.Common;
using PiP_Tool.Interfaces;
using PiP_Tool.Models;
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
                    var main = new PictureInPictureWindow(new SelectedWindow(SelectedWindow, SelectedRegion));
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
                    var selector = new SelectorWindow();
                    selector.Show();
                    CloseWindow();
                });
            }
        }

        public ICommand StartWindowSelector
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var windowSelector = new WindowSelectorWindow();
                    windowSelector.Show();
                    CloseWindow();
                });
            }
        }
        
        public WindowInfo SelectedWindow { get; set; }
        public NativeStructs.Rect SelectedRegion => new NativeStructs.Rect(0, 0, SelectedWindow.Size.Width, SelectedWindow.Size.Height);

        public CollectionView WindowsList { get; }

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
