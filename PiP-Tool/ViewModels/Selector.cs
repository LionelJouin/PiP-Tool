using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PiP_Tool.ViewModels
{
    public class Selector : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _dragging;
        private Hit _hit;
        private Point _selectorBoxPosition = new Point(500, 300);
        private Size _selectorBoxSize = new Size(100, 300);
        private Point _lastMousePosition = new Point(0, 0);
        private readonly double _screenWidth = SystemParameters.PrimaryScreenWidth;
        private readonly double _screenHeight = SystemParameters.PrimaryScreenHeight;

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
            _lastMousePosition = mousePosition;
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
                _hit = GetHit(mousePosition);
                SetCursor(_hit);
            }
            else
            {
                var offsetX = mousePosition.X - _lastMousePosition.X;
                var offsetY = mousePosition.Y - _lastMousePosition.Y;
                
                var newX = _selectorBoxPosition.X;
                var newY = _selectorBoxPosition.Y;
                var newWidth = _selectorBoxSize.Width;
                var newHeight = _selectorBoxSize.Height;



                if (newX <= 0)
                {
                    newX = 0;
                }
                if (newY <= 0)
                {
                    newY = 0;
                }
                if (newWidth > _screenWidth)
                {
                    newWidth = _screenWidth;
                }
                if (newHeight > _screenHeight)
                {
                    newHeight = _screenHeight;
                }
                if (newX + newWidth >= _screenWidth)
                {
                    newX -= newX + newWidth - _screenWidth;
                }
                if (newY + newHeight >= _screenHeight)
                {
                    newY -= newY + newHeight - _screenHeight;
                }

                switch (_hit)
                {
                    case Hit.Body:
                        newX += offsetX;
                        newY += offsetY;
                        break;
                    case Hit.Tl:
                        newX += offsetX;
                        newY += offsetY;
                        newWidth -= offsetX;
                        newHeight -= offsetY;
                        break;
                    case Hit.Tr:
                        newY += offsetY;
                        newWidth += offsetX;
                        newHeight -= offsetY;
                        break;
                    case Hit.Br:
                        newWidth += offsetX;
                        newHeight += offsetY;
                        break;
                    case Hit.Bl:
                        newX += offsetX;
                        newWidth -= offsetX;
                        newHeight += offsetY;
                        break;
                    case Hit.L:
                        newX += offsetX;
                        newWidth -= offsetX;
                        break;
                    case Hit.R:
                        newWidth += offsetX;
                        break;
                    case Hit.B:
                        newHeight += offsetY;
                        break;
                    case Hit.T:
                        newY += offsetY;
                        newHeight -= offsetY;
                        break;
                }

                if (!(newWidth > 50) || !(newHeight > 50)) return;
                SelectorBoxPosition = new Point(newX, newY);
                SelectorBoxSize = new Size(newWidth, newHeight);
                _lastMousePosition = mousePosition;
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
