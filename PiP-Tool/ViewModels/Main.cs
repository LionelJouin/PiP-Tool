using System;
using System.Windows;
using System.Windows.Input;
using PiP_Tool.Common;
using PiP_Tool.Interfaces;
using PiP_Tool.Views;

namespace PiP_Tool.ViewModels
{
    public class Main : BaseViewModel, ICloseable
    {
        
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

        public ICommand StartWindowSelector
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var windowSelector = new WindowSelectorWindow();
                    windowSelector.Show();
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

    }
}
