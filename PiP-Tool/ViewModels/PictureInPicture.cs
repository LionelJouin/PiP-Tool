using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Helpers.Native;
using PiP_Tool.DataModel;
using PiP_Tool.Interfaces;
using PiP_Tool.Views;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Drawing.Point;

namespace PiP_Tool.ViewModels
{
    public class PictureInPicture : ViewModelBase, ICloseable
    {

        #region public

        public const int MinSize = 100;
        public const float DefaultSizePercentage = 0.25f;

        public event EventHandler<EventArgs> RequestClose;

        public ICommand LoadedCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand ChangeSelectedWindowCommand { get; }
        public ICommand SizeChangedCommand { get; }
        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }

        public int MinHeight
        {
            get => _minHeight;
            set
            {
                _minHeight = value;
                Update();
                RaisePropertyChanged();
            }
        }
        public int MinWidth
        {
            get => _minWidth;
            set
            {
                _minWidth = value;
                Update();
                RaisePropertyChanged();
            }
        }
        public int Top
        {
            get => _top;
            set
            {
                _top = value;
                Update();
                RaisePropertyChanged();
            }
        }
        public int Left
        {
            get => _left;
            set
            {
                _left = value;
                Update();
                RaisePropertyChanged();
            }
        }
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                Update();
                RaisePropertyChanged();
            }
        }
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                Update();
                RaisePropertyChanged();
            }
        }
        public float Ratio { get; private set; }
        public Visibility TopBarVisibility
        {
            get => _topBarVisibility;
            set
            {
                _topBarVisibility = value;
                Update();
                RaisePropertyChanged();
            }
        }

        public bool TopBarIsVisible => TopBarVisibility == Visibility.Visible;

        #endregion

        #region private

        private const int TopBarHeight = 30;

        private int _heightOffset;
        private Visibility _topBarVisibility;
        private bool _renderSizeEventDisabled;
        private int _minHeight;
        private int _minWidth;
        private int _top;
        private int _left;
        private int _height;
        private int _width;
        private IntPtr _targetHandle, _thumbHandle;
        private SelectedWindow _selectedWindow;

        #endregion

        public PictureInPicture()
        {
            LoadedCommand = new RelayCommand(LoadedCommandExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute);
            ChangeSelectedWindowCommand = new RelayCommand(ChangeSelectedWindowCommandExecute);
            SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(SizeChangedCommandExecute);
            MouseEnterCommand = new RelayCommand<MouseEventArgs>(MouseEnterCommandExecute);
            MouseLeaveCommand = new RelayCommand<MouseEventArgs>(MouseLeaveCommandExecute);

            MessengerInstance.Register<SelectedWindow>(this, InitSelectedWindow);

            MinHeight = MinSize;
            MinWidth = MinSize;
        }

        private void InitSelectedWindow(SelectedWindow selectedWindow)
        {
            MessengerInstance.Unregister<SelectedWindow>(this);
            _selectedWindow = selectedWindow;
            _renderSizeEventDisabled = true;
            TopBarVisibility = Visibility.Hidden;
            _heightOffset = 0;
            Ratio = _selectedWindow.Ratio;
            Height = _selectedWindow.SelectedRegion.Height;
            Width = _selectedWindow.SelectedRegion.Width;
            Top = 200;
            Left = 200;

            // set Min size
            if (Height < Width)
                MinWidth = MinSize * (int)Ratio;
            else if (Width < Height)
                MinHeight = MinSize * (int)_selectedWindow.RatioHeightByWidth;

            // set Default size
            var resolution = Screen.PrimaryScreen.Bounds;
            if (Height > resolution.Height * DefaultSizePercentage)
            {
                Height = (int)(resolution.Height * DefaultSizePercentage);
                Width = Convert.ToInt32(Height * Ratio);
            }
            if (Width > resolution.Width * DefaultSizePercentage)
            {
                Width = (int)(resolution.Width * DefaultSizePercentage);
                Height = Convert.ToInt32(Width * _selectedWindow.RatioHeightByWidth);
            }

            _renderSizeEventDisabled = false;

            Init();
        }

        private void Init()
        {
            if (_selectedWindow == null || _selectedWindow.WindowInfo.Handle == IntPtr.Zero || _targetHandle == IntPtr.Zero)
                return;

            if (_thumbHandle != IntPtr.Zero)
                NativeMethods.DwmUnregisterThumbnail(_thumbHandle);

            if (NativeMethods.DwmRegisterThumbnail(_targetHandle, _selectedWindow.WindowInfo.Handle, out _thumbHandle) == 0)
                Update();
        }

        private void Update()
        {
            if (_thumbHandle == IntPtr.Zero)
                return;

            var dest = new NativeStructs.Rect(0, _heightOffset, _width, _height);

            var props = new NativeStructs.DwmThumbnailProperties
            {
                fVisible = true,
                dwFlags = (int)(DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTSOURCE),
                opacity = 255,
                rcDestination = dest,
                rcSource = _selectedWindow.SelectedRegion
            };

            NativeMethods.DwmUpdateThumbnailProperties(_thumbHandle, ref props);
        }

        #region commands

        private void LoadedCommandExecute()
        {
            var windowsList = Application.Current.Windows.Cast<Window>();
            var thisWindow = windowsList.FirstOrDefault(x => x.DataContext == this);
            if (thisWindow != null)
                _targetHandle = new WindowInteropHelper(thisWindow).Handle;
            Init();
        }

        private void CloseCommandExecute()
        {
            MessengerInstance.Unregister<SelectedWindow>(this);
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void ChangeSelectedWindowCommandExecute()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void SizeChangedCommandExecute(SizeChangedEventArgs sizeInfo)
        {
            if (_renderSizeEventDisabled)
                return;
            var topBarHeight = 0;
            if (TopBarIsVisible)
                topBarHeight = TopBarHeight;
            if (sizeInfo.WidthChanged)
            {
                var width = Convert.ToInt32((sizeInfo.NewSize.Height - topBarHeight) * Ratio);
                if (width < MinWidth)
                    width = MinWidth;
                Width = width;
            }
            else
            {
                var height = Convert.ToInt32(sizeInfo.NewSize.Width * Ratio + topBarHeight);
                if (height < MinHeight)
                    height = MinHeight;
                Height = height;
            }
        }

        private void MouseEnterCommandExecute(MouseEventArgs e)
        {
            if (TopBarIsVisible)
                return;
            _renderSizeEventDisabled = true;
            TopBarVisibility = Visibility.Visible;
            _heightOffset = TopBarHeight;
            Top = Top - TopBarHeight;
            Height = Height + TopBarHeight;
            MinHeight = MinHeight + TopBarHeight;
            _renderSizeEventDisabled = false;
            e.Handled = true;
        }

        private void MouseLeaveCommandExecute(MouseEventArgs e)
        {
            // Prevent OnMouseEnter, OnMouseLeave loop
            Thread.Sleep(50);
            NativeMethods.GetCursorPos(out var p);
            var r = new Rectangle(Convert.ToInt32(Left), Convert.ToInt32(Top), Convert.ToInt32(Width), Convert.ToInt32(Height));
            var pa = new Point(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));

            if (!TopBarIsVisible || r.Contains(pa))
                return;
            TopBarVisibility = Visibility.Hidden;
            _renderSizeEventDisabled = true;
            _heightOffset = 0;
            Top = Top + TopBarHeight;
            MinHeight = MinHeight - TopBarHeight;
            Height = Height - TopBarHeight;
            _renderSizeEventDisabled = false;
            e.Handled = true;
        }

        #endregion

    }
}
