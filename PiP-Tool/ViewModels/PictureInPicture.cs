using System;
using System.Drawing;
using Helpers.Native;
using PiP_Tool.Models;

namespace PiP_Tool.ViewModels
{
    public class PictureInPicture : BaseViewModel
    {

        public WindowInfo SelectedWindow
        {
            get => _selectedWindow;
            set
            {
                if (_selectedWindow != null && _selectedWindow.Handle == IntPtr.Zero)
                    return;

                _selectedWindow = value;

                if (_thumbHandle != IntPtr.Zero)
                    NativeMethods.DwmUnregisterThumbnail(_thumbHandle);

                if (NativeMethods.DwmRegisterThumbnail(_targetHandle, SelectedWindow.Handle, out _thumbHandle) == 0)
                    Update();

                NotifyPropertyChanged();
            }
        }

        public Size WindowSize
        {
            get => _windowSize;
            set
            {
                _windowSize = value;
                NotifyPropertyChanged();
            }
        }

        private Size _windowSize = new Size(100, 100);
        private IntPtr _targetHandle, _thumbHandle;
        private WindowInfo _selectedWindow;

        //NativeStructs.Rect _targetRect;

        //public PictureInPicture(IntPtr hWnd, PictureInPictureWindow.Rect rect)
        //{
        //    _targetHandle = Process.GetCurrentProcess().MainWindowHandle;
        //    _targetRect = rect;
        //    SelectedWindow = hWnd;
        //    a();
        //    ImageScreen = Update();
        //}

        public void Init(IntPtr target, WindowInfo selectedWindow)
        {
            _targetHandle = target;
            SelectedWindow = selectedWindow;
            WindowSize = SelectedWindow.Size;
        }

        public void Update()
        {
            if (_thumbHandle == IntPtr.Zero)
                return;

            NativeMethods.DwmQueryThumbnailSourceSize(_thumbHandle, out NativeStructs.Psize size);
            
            var props = new NativeStructs.DwmThumbnailProperties
            {
                fVisible = true,
                //dwFlags = (int) (DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTSOURCE),
                dwFlags = (int)(DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_OPACITY),
                opacity = 255,
                rcDestination = new NativeStructs.Rect(0, 0, 300, 300),
                rcSource = new NativeStructs.Rect(50, 150, 300, 300)
            };

            //if (size.x < _targetRect.Width)
            //    props.rcDestination.Right = 0;

            //if (size.y < _targetRect.Height)
            //    props.rcDestination.Bottom = 0;

            NativeMethods.DwmUpdateThumbnailProperties(_thumbHandle, ref props);
        }
        
    }
}
