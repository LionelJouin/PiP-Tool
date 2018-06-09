using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
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
                _selectedWindowInfo = value;
                ShowSelector();
            }

        }
        public CollectionView WindowsList
        {
            get => _windowsList;
            set
            {
                _windowsList = value;
                NotifyPropertyChanged();
            }
        }

        private CollectionView _windowsList;

        private SelectorWindow _selectorWindow;
        private WindowInfo _selectedWindowInfo;
        private readonly ProcessList _processList;

        public Main()
        {
            _processList = new ProcessList();
            //_processList.OpenWindowsChanged += OpenWindowsChanged;
            SetWindowsList();
        }

        private void SetWindowsList()
        {
            var sortedList = _processList.OpenWindows.OrderBy(x => x.Title);
            WindowsList = new CollectionView(sortedList);
        }

        private void OpenWindowsChanged(object sender, EventArgs e)
        {
            SetWindowsList();
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
