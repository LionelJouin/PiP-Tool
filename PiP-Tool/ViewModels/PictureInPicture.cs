using System;
using System.Windows.Input;
using Helpers.Native;
using PiP_Tool.DataModel;
using PiP_Tool.Helpers;
using PiP_Tool.Views;

namespace PiP_Tool.ViewModels
{
    public class PictureInPicture : BaseViewModel
    {

        #region public

        public ICommand CloseCommand { get; }
        public ICommand ChangeSelectedWindowCommand { get; }

        public SelectedWindow SelectedWindow
        {
            get => _selectedWindow;
            set
            {
                if (_selectedWindow != null && _selectedWindow.WindowInfo.Handle == IntPtr.Zero)
                    return;

                _selectedWindow = value;

                if (_thumbHandle != IntPtr.Zero)
                    NativeMethods.DwmUnregisterThumbnail(_thumbHandle);

                if (NativeMethods.DwmRegisterThumbnail(_targetHandle, SelectedWindow.WindowInfo.Handle, out _thumbHandle) == 0)
                    Update();

                NotifyPropertyChanged();
            }
        }

        public int HeightOffset;
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

        #endregion

        #region private

        private int _height;
        private int _width;
        private IntPtr _targetHandle, _thumbHandle;
        private SelectedWindow _selectedWindow;

        #endregion

        public PictureInPicture()
        {
            CloseCommand = new RelayCommand(CloseWindow);
            ChangeSelectedWindowCommand = new RelayCommand(ChangeSelectedWindowCommandExecute);
        }

        public void Init(IntPtr target, SelectedWindow selectedWindow)
        {
            _targetHandle = target;
            SelectedWindow = selectedWindow;
            Height = SelectedWindow.SelectedRegion.Height;
            Width = SelectedWindow.SelectedRegion.Width;
            Ratio = SelectedWindow.Ratio;
        }

        public void Update()
        {
            if (_thumbHandle == IntPtr.Zero)
                return;

            var dest = new NativeStructs.Rect(0, HeightOffset, _width, _height);

            var props = new NativeStructs.DwmThumbnailProperties
            {
                fVisible = true,
                dwFlags = (int)(DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTSOURCE),
                opacity = 255,
                rcDestination = dest,
                rcSource = SelectedWindow.SelectedRegion
            };

            NativeMethods.DwmUpdateThumbnailProperties(_thumbHandle, ref props);
        }

        #region commands

        private void ChangeSelectedWindowCommandExecute()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            CloseWindow();
        }

        #endregion
    }
}
