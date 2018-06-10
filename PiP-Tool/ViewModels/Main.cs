using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using PiP_Tool.DataModel;
using PiP_Tool.Helpers;
using PiP_Tool.Views;

namespace PiP_Tool.ViewModels
{
    public class Main : BaseViewModel
    {

        public ICommand StartPictureInPicture
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_selectorWindow == null)
                        return;
                    var selectedRegion = _selectorWindow.SelectedRegion;
                    _selectorWindow.Close();
                    var pip = new PictureInPictureWindow(new SelectedWindow(SelectedWindowInfo, selectedRegion));
                    pip.Show();
                    CloseWindow();
                });
            }
        }

        public WindowInfo SelectedWindowInfo
        {
            get => _selectedWindowInfo;
            set
            {
                if (_selectedWindowInfo == value)
                    return;
                _selectedWindowInfo = value;
                ShowSelector();
                NotifyPropertyChanged();
            }
        }
        //public CollectionView WindowsList
        //{
        //    get => _windowsList;
        //    set
        //    {
        //        _windowsList = value;
        //        NotifyPropertyChanged();
        //    }
        //}

        //private CollectionView _windowsList;

        public ObservableCollection<WindowInfo> WindowsList
        {
            get => _windowsList;
            set
            {
                _windowsList = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<WindowInfo> _windowsList;
        private SelectorWindow _selectorWindow;
        private WindowInfo _selectedWindowInfo;
        private readonly ProcessList _processList;

        public Main()
        {
            WindowsList = new ObservableCollection<WindowInfo>();

            _processList = new ProcessList();
            _processList.OpenWindowsChanged += OpenWindowsChanged;
            UpdateWindowsList();
        }

        private void UpdateWindowsList()
        {
            var openWindows = _processList.OpenWindows;

            var toAdd = openWindows.Where(x => WindowsList.All(y => x.Handle != y.Handle));
            var toRemove = WindowsList.Where(x => openWindows.All(y => x.Handle != y.Handle));

            foreach (var e in toAdd)
            {
                WindowsList.Add(e);
            }
            foreach (var e in toRemove)
            {
                WindowsList.Remove(e);
            }
        }

        private void OpenWindowsChanged(object sender, EventArgs e)
        {
            UpdateWindowsList();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _processList.Dispose();
            _selectorWindow?.Close();
        }

        private void ShowSelector()
        {
            _selectorWindow?.Close();
            _selectorWindow = new SelectorWindow(SelectedWindowInfo);
            _selectorWindow.Show();
        }

    }
}
