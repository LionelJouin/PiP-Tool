using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using PiP_Tool.Common;
using PiP_Tool.Interfaces;

namespace PiP_Tool.ViewModels
{
    public class Main : INotifyPropertyChanged, ICloseable
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> RequestClose;

        public ICommand StartPictureInPicture
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
                    var main = new PictureInPictureWindow(new Point(100, 100), new Size(100, 100));
                    main.Show();
                    CloseWindow();
                });
            }
        }

        public ICommand StartSelector
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var selector = new SelectorWindow();
                    selector.Show();
                    CloseWindow();
                });
            }
        }

        private void CloseWindow()
        {
            var handler = RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
