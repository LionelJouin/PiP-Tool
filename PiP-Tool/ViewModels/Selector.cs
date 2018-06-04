using System.Windows;
using Helpers.Native;
using PiP_Tool.DataModel;

namespace PiP_Tool.ViewModels
{
    public class Selector : BaseViewModel
    {
        public int WindowTop => _windowInfo.Position.Y;
        public int WindowLeft => _windowInfo.Position.X;
        public int WindowWidth => _windowInfo.Rect.Width;
        public int WindowHeight => _windowInfo.Rect.Height;
        public Thickness CanvasMargin => new Thickness(_windowInfo.Border.Left, _windowInfo.Border.Top, _windowInfo.Border.Right, _windowInfo.Border.Bottom);

        public int MaxHeight => _sizeRestriction.Height;
        public int MaxWidth => _sizeRestriction.Width;
        public int MinHeight => 100;
        public int MinWidth => 100;

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                UpdateBottom();
                NotifyPropertyChanged();
            }
        }
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                UpdateRight();
                NotifyPropertyChanged();
            }
        }
        public int Top
        {
            get => _top;
            set
            {
                _top = value;
                UpdateBottom();
                NotifyPropertyChanged();
            }
        }
        public int Left
        {
            get => _left;
            set
            {
                _left = value;
                UpdateRight();
                NotifyPropertyChanged();
            }
        }
        public int Bottom
        {
            get => _bottom;
            set
            {
                _bottom = value;
                NotifyPropertyChanged();
            }
        }
        public int Right
        {
            get => _right;
            set
            {
                _right = value;
                NotifyPropertyChanged();
            }
        }

        public NativeStructs.Rect SelectedRegion => new NativeStructs.Rect(Left, Top, Width + Left, Top + Height);

        private int _height;
        private int _width;
        private int _top;
        private int _left;
        private int _bottom;
        private int _right;

        private NativeStructs.Rect _sizeRestriction;
        private readonly WindowInfo _windowInfo;

        public Selector(WindowInfo windowInfo)
        {
            _windowInfo = windowInfo;
            _sizeRestriction = windowInfo.Rect - windowInfo.Border;
            Top = 0;
            Left = 0;
            Height = MinHeight;
            Width = MinWidth;
        }

        public Selector(NativeStructs.Rect sizeRestriction)
        {
            _sizeRestriction = sizeRestriction;
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

    }
}
