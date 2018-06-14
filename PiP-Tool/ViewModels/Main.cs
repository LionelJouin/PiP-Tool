using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using PiP_Tool.DataModel;
using PiP_Tool.Helpers;
using PiP_Tool.Views;

namespace PiP_Tool.ViewModels
{
    public class Main : BaseViewModel
    {

        #region public

        public ICommand StartPipCommand { get; }
        public ICommand LoadedCommand { get; }
        public ICommand ClosingCommand { get; }

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
        public ObservableCollection<WindowInfo> WindowsList
        {
            get => _windowsList;
            set
            {
                _windowsList = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region private

        private ObservableCollection<WindowInfo> _windowsList;
        private SelectorWindow _selectorWindow;
        private WindowInfo _selectedWindowInfo;
        private readonly ProcessList _processList;

        #endregion

        public Main()
        {
            StartPipCommand = new RelayCommand(StartPipCommandExecute);
            LoadedCommand = new RelayCommand(LoadedCommandExecute);
            ClosingCommand = new RelayCommand(ClosingCommandExecute);

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

        private void ShowSelector()
        {
            _selectorWindow?.Close();
            _selectorWindow = new SelectorWindow(SelectedWindowInfo);
            _selectorWindow.Show();
        }

        private void OpenWindowsChanged(object sender, EventArgs e)
        {
            UpdateWindowsList();
        }

        #region commands

        private void StartPipCommandExecute()
        {
            if (_selectorWindow == null)
                return;
            var selectedRegion = _selectorWindow.SelectedRegion;
            _selectorWindow.Close();
            var pip = new PictureInPictureWindow(new SelectedWindow(SelectedWindowInfo, selectedRegion));
            pip.Show();
            CloseWindow();
        }

        private void LoadedCommandExecute()
        {
            var a = new WindowInteropHelper(Application.Current.MainWindow).Handle;
        }

        private void ClosingCommandExecute()
        {
            _processList.Dispose();
            _selectorWindow?.Close();
        }

        #endregion

    }
}
