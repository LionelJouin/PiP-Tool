using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace PiP_Tool.ViewModels
{
    public class Selector : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private Point _selectorBoxPosition = new Point(100, 200);
        private Size _selectorBoxSize = new Size(50, 300);

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

        public void SetCursor(Point mousePosition)
        {
            var left = _selectorBoxPosition.X;
            var top = _selectorBoxPosition.Y;
            var right = left + _selectorBoxSize.Width;
            var bottom = top + _selectorBoxSize.Height;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
