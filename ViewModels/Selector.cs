using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PiP_Tool.ViewModels
{
    public class Selector : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private Point _selectorBoxPosition = new Point(500, 300);
        private Size _selectorBoxSize = new Size(100, 300);
        private Point _draggingOffset = new Point(0, 0);
        private bool _dragging;

        public Point SelectorBoxPosition
        {
            get { return _selectorBoxPosition; }
            set
            {
                _selectorBoxPosition = value;
                NotifyPropertyChanged();
            }
        }

        public Size SelectorBoxSize
        {
            get { return _selectorBoxSize; }
            set
            {
                _selectorBoxSize = value;
                NotifyPropertyChanged();
            }
        }

        public enum Hit
        {
            None, Body, Tl, Tr, Br, Bl, L, R, T, B
        }

        public Hit GetHit(Point mousePosition)
        {
            var left = _selectorBoxPosition.X;
            var top = _selectorBoxPosition.Y;
            var right = left + _selectorBoxSize.Width;
            var bottom = top + _selectorBoxSize.Height;
            if (mousePosition.X < left) return Hit.None;
            if (mousePosition.X > right) return Hit.None;
            if (mousePosition.Y < top) return Hit.None;
            if (mousePosition.Y > bottom) return Hit.None;

            const int margin = 10;
            if (mousePosition.X - left < margin)
            {
                if (mousePosition.Y - top < margin) return Hit.Tl;
                if (bottom - mousePosition.Y < margin) return Hit.Bl;
                return Hit.L;
            }
            if (right - mousePosition.X < margin)
            {
                if (mousePosition.Y - top < margin) return Hit.Tr;
                if (bottom - mousePosition.Y < margin) return Hit.Br;
                return Hit.R;
            }
            if (mousePosition.Y - top < margin) return Hit.T;
            if (bottom - mousePosition.Y < margin) return Hit.B;
            return Hit.Body;
        }

        public void SetCursor(Hit hit)
        {
            var cursor = Cursors.Arrow;

            switch (hit)
            {
                case Hit.None:
                    cursor = Cursors.Arrow;
                    break;
                case Hit.Body:
                    cursor = Cursors.ScrollAll;
                    break;
                case Hit.Tl:
                case Hit.Br:
                    cursor = Cursors.SizeNWSE;
                    break;
                case Hit.Bl:
                case Hit.Tr:
                    cursor = Cursors.SizeNESW;
                    break;
                case Hit.T:
                case Hit.B:
                    cursor = Cursors.SizeNS;
                    break;
                case Hit.L:
                case Hit.R:
                    cursor = Cursors.SizeWE;
                    break;
            }

            if (Mouse.OverrideCursor != cursor)
                Mouse.OverrideCursor = cursor;
        }

        public void MouseDown(Point mousePosition)
        {
            _draggingOffset.X = SelectorBoxPosition.X - mousePosition.X;
            _draggingOffset.Y = SelectorBoxPosition.Y - mousePosition.Y;
            _dragging = true;
        }

        public void MouseUp()
        {
            _dragging = false;
        }

        public void MouseMove(Point mousePosition)
        {
            if (_dragging == false)
            {
                SetCursor(GetHit(mousePosition));
            }
            else
            {
                SelectorBoxPosition = new Point(mousePosition.X + _draggingOffset.X, mousePosition.Y + _draggingOffset.Y);
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
