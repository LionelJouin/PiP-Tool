using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PiP_Tool.DataModel;
using PiP_Tool.Interfaces;
using PiP_Tool.Native;
using PiP_Tool.Views;

namespace PiP_Tool.ViewModels
{
    public class MainViewModel : ViewModelBase, ICloseable
    {

        #region public

        public event EventHandler<EventArgs> RequestClose;

        public ICommand StartPipCommand { get; }
        public ICommand QuitCommand { get; }
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
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<WindowInfo> WindowsList
        {
            get => _windowsList;
            set
            {
                _windowsList = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region private

        private ObservableCollection<WindowInfo> _windowsList;
        private SelectorWindow _selectorWindow;
        private WindowInfo _selectedWindowInfo;
        private readonly ProcessList _processList;

        #endregion

        public MainViewModel()
        {
            StartPipCommand = new RelayCommand(StartPipCommandExecute);
            QuitCommand = new RelayCommand(QuitCommandExecute);
            ClosingCommand = new RelayCommand(ClosingCommandExecute);

            WindowsList = new ObservableCollection<WindowInfo>();

            _processList = new ProcessList();
            _processList.OpenWindowsChanged += OpenWindowsChanged;
            _processList.ForegroundWindowChanged += ForegroundWindowChanged;
            UpdateWindowsList();
        }

        private void UpdateWindowsList()
        {
            var openWindows = _processList.OpenWindows;

            var toAdd = openWindows.Where(x => WindowsList.All(y => x != y));
            var toRemove = WindowsList.Where(x => openWindows.All(y => x != y)).ToList();

            foreach (var e in toAdd)
            {
                WindowsList.Add(e);
            }
            for (var index = 0; index < toRemove.Count; index++)
            {
                WindowsList.Remove(toRemove[index]);
            }
        }

        private void ShowSelector()
        {
            _selectorWindow?.Close();
            _selectorWindow = new SelectorWindow();
            MessengerInstance.Send(SelectedWindowInfo);
            _selectorWindow.Show();
            _selectorWindow.Activate();
        }

        private void StartPip(NativeStructs.Rect selectedRegion)
        {
            var pip = new PictureInPictureWindow();
            MessengerInstance.Send(new SelectedWindow(SelectedWindowInfo, selectedRegion));
            pip.Show();
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void OpenWindowsChanged(object sender, EventArgs e)
        {
            UpdateWindowsList();
        }

        private void ForegroundWindowChanged(object sender, EventArgs e)
        {
            UpdateWindowsList();
            var foregroundWindow = _processList.ForegroundWindow;
            if (foregroundWindow != null)
                SelectedWindowInfo = foregroundWindow;
        }

        #region commands

        private void StartPipCommandExecute()
        {
            MessengerInstance.Send<Action<NativeStructs.Rect>>(StartPip);
        }

        private void QuitCommandExecute()
        {
            var windowsList = Application.Current.Windows.Cast<Window>();
            foreach (var window in windowsList)
            {
                if (window.DataContext != this)
                    window.Close();
            }
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void ClosingCommandExecute()
        {
            _processList.Dispose();
            _selectorWindow?.Close();
        }

        #endregion

    }
}
