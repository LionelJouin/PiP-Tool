using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PiP_Tool.Interfaces;

namespace PiP_Tool.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged, ICloseable
    {

        public event EventHandler<EventArgs> RequestClose;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void CloseWindow()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

    }
}
