using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Helpers.Native;
using PiP_Tool.DataModel;
using PiP_Tool.Interfaces;

namespace PiP_Tool.ViewModels
{
    public class Selector : ViewModelBase, ICloseable
    {

        #region public

        public ICommand ClosingCommand { get; }

        public event EventHandler<EventArgs> RequestClose;

        public NativeStructs.Rect SelectedRegion => new NativeStructs.Rect(Left, Top, Width + Left, Top + Height);

        public int WindowTop
        {
            get => _windowTop;
            set
            {
                _windowTop = value;
                RaisePropertyChanged();
            }
        }
        public int WindowLeft
        {
            get => _windowLeft;
            set
            {
                _windowLeft = value;
                RaisePropertyChanged();
            }
        }
        public int WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                RaisePropertyChanged();
            }
        }
        public int WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                RaisePropertyChanged();
            }
        }
        public Thickness CanvasMargin
        {
            get => _canvasMargin;
            set
            {
                _canvasMargin = value;
                RaisePropertyChanged();
            }
        }

        public int MaxHeight
        {
            get => _maxHeight;
            set
            {
                _maxHeight = value;
                UpdateBottom();
                RaisePropertyChanged();
            }
        }
        public int MaxWidth
        {
            get => _maxWidth;
            set
            {
                _maxWidth = value;
                UpdateRight();
                RaisePropertyChanged();
            }
        }
        public int MinHeight
        {
            get => _minHeight;
            set
            {
                _minHeight = value;
                RaisePropertyChanged();
            }
        }
        public int MinWidth
        {
            get => _minWidth;
            set
            {
                _minWidth = value;
                RaisePropertyChanged();
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                UpdateBottom();
                RaisePropertyChanged();
            }
        }
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                UpdateRight();
                RaisePropertyChanged();
            }
        }
        public int Top
        {
            get => _top;
            set
            {
                _top = value;
                UpdateBottom();
                RaisePropertyChanged();
            }
        }
        public int Left
        {
            get => _left;
            set
            {
                _left = value;
                UpdateRight();
                RaisePropertyChanged();
            }
        }
        public int Bottom
        {
            get => _bottom;
            set
            {
                _bottom = value;
                RaisePropertyChanged();
            }
        }
        public int Right
        {
            get => _right;
            set
            {
                _right = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region private

        private int _windowTop;
        private int _windowLeft;
        private int _windowWidth;
        private int _windowHeight;
        private Thickness _canvasMargin;

        private int _maxHeight;
        private int _maxWidth;
        private int _minHeight;
        private int _minWidth;

        private int _height;
        private int _width;
        private int _top;
        private int _left;
        private int _bottom;
        private int _right;

        private NativeStructs.Rect _sizeRestriction;
        private WindowInfo _windowInfo;

        #endregion

        public Selector()
        {
            ClosingCommand = new RelayCommand(ClosingCommandExecute);
            MessengerInstance.Register<WindowInfo>(this, Init);
            MessengerInstance.Register<Action<NativeStructs.Rect>>(this, StartPip);
        }

        private void Init(WindowInfo windowInfo)
        {
            MessengerInstance.Unregister<WindowInfo>(this);
            _windowInfo = windowInfo;
            _windowInfo.SetAsForegroundWindow();
            _sizeRestriction = _windowInfo.Rect - _windowInfo.Border;

            WindowTop = _windowInfo.Position.Y;
            WindowLeft = _windowInfo.Position.X;
            WindowWidth = _windowInfo.Rect.Width;
            WindowHeight = _windowInfo.Rect.Height;
            CanvasMargin = new Thickness(_windowInfo.Border.Left, _windowInfo.Border.Top, _windowInfo.Border.Right, _windowInfo.Border.Bottom);

            MaxHeight = _sizeRestriction.Height;
            MaxWidth = _sizeRestriction.Width;
            MinHeight = 100;
            MinWidth = 100;

            Top = 0;
            Left = 0;
            Height = MinHeight;
            Width = MinWidth;
        }

        private void UpdateBottom()
        {
            Bottom = MaxHeight - (Top + Height);
        }

        private void UpdateRight()
        {
            Right = MaxWidth - (Left + Width);
        }

        private void StartPip(Action<NativeStructs.Rect> cb)
        {
            MessengerInstance.Unregister<Action<NativeStructs.Rect>>(this);
            cb(SelectedRegion);
        }

        #region commands

        private void ClosingCommandExecute()
        {
            MessengerInstance.Unregister<WindowInfo>(this);
            MessengerInstance.Unregister<Action<NativeStructs.Rect>>(this);
        }

        #endregion

    }
}
