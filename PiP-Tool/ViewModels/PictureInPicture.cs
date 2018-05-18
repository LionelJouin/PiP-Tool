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
        
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                Update();
                NotifyPropertyChanged();
            }
        }
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                Update();
                NotifyPropertyChanged();
            }
        }

        public float Ratio { get; private set; }
        
        private int _height;
        private int _width;
        private IntPtr _targetHandle, _thumbHandle;
        private WindowInfo _selectedWindow;

        public void Init(IntPtr target, WindowInfo selectedWindow)
        {
            _targetHandle = target;
            SelectedWindow = selectedWindow;
            Height = SelectedWindow.Size.Height;
            Width = SelectedWindow.Size.Width;
            Ratio = SelectedWindow.Ratio;
        }

        public void Update()
        {
            if (_thumbHandle == IntPtr.Zero)
                return;

            NativeMethods.DwmQueryThumbnailSourceSize(_thumbHandle, out var size);

            //var source = new NativeStructs.Rect(SelectedWindow.Position.X, SelectedWindow.Position.Y, SelectedWindow.Size.Width, SelectedWindow.Size.Height);
            //var dest = new NativeStructs.Rect(SelectedWindow.Position.X, SelectedWindow.Position.Y, SelectedWindow.Size.Width, SelectedWindow.Size.Height);
            var source = new NativeStructs.Rect(0, 0, SelectedWindow.Size.Width, SelectedWindow.Size.Height);
            var dest = new NativeStructs.Rect(0, 0, _width, _height);

            var props = new NativeStructs.DwmThumbnailProperties
            {
                fVisible = true,
                //dwFlags = (int) (DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTSOURCE),
                dwFlags = (int)(DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_OPACITY),
                opacity = 255,
                rcDestination = dest,
                rcSource = source
            };

            //if (size.x < _targetRect.Width)
            //    props.rcDestination.Right = 0;

            //if (size.y < _targetRect.Height)
            //    props.rcDestination.Bottom = 0;

            NativeMethods.DwmUpdateThumbnailProperties(_thumbHandle, ref props);
        }

    }
}
