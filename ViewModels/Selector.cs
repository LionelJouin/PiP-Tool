using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PiP_Tool.Common;

namespace PiP_Tool.ViewModels
{
    public class Selector : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand MouseDownCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                });
            }
        }

        public ICommand MouseUpCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Console.WriteLine("koukou");
                });
            }
        }
        public ICommand MouseMoveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                });
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
