using System;
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
        private bool _dragging = false;
        private Point _mouseDownPosition = new Point(0, 0);

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
            None, Body, TL, TR, BR, BL, L, R, T, B
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
                if (mousePosition.Y - top < margin) return Hit.TL;
                if (bottom - mousePosition.Y < margin) return Hit.BL;
                return Hit.L;
            }
            if (right - mousePosition.X < margin)
            {
                if (mousePosition.Y - top < margin) return Hit.TR;
                if (bottom - mousePosition.Y < margin) return Hit.BR;
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
                case Hit.TL:
                case Hit.BR:
                    cursor = Cursors.SizeNWSE;
                    break;
                case Hit.BL:
                case Hit.TR:
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
            _mouseDownPosition.X = mousePosition.X;
            _mouseDownPosition.Y = mousePosition.X;
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
                //var offsetX = _mouseDownPosition.X - SelectorBoxPosition.X;
                //var offsetY = _mouseDownPosition.Y - SelectorBoxPosition.Y;
                SelectorBoxPosition = new Point(mousePosition.X, mousePosition.Y);
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
