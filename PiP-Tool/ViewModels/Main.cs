using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Helpers.Native;
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

        private IDictionary<IntPtr, string> _openWindows;
        private IntPtr _windowToPip;

        public Main()
        {
            _openWindows = NativeMethods.GetOpenWindows();
            foreach (var window in _openWindows)
            {
                var handle = window.Key;
                var title = window.Value;

                Console.WriteLine("{0}: {1}", handle, title);
            }
            //_windowToPip = _openWindows.FirstOrDefault(x => x.Value.Contains("Bloc-notes")).Key;
        }

        private void CloseWindow()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

    }
}
