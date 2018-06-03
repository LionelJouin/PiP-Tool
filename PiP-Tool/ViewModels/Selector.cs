using Helpers.Native;

namespace PiP_Tool.ViewModels
{
    public class Selector : BaseViewModel
    {

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
