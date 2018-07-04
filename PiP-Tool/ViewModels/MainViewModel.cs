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
using PiP_Tool.Services;
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

        /// <summary>
        /// Gets or sets selected window info and call <see cref="ShowCropper"/>
        /// </summary>
        public WindowInfo SelectedWindowInfo
        {
            get => _selectedWindowInfo;
            set
            {
                if (_selectedWindowInfo == value)
                    return;
                _selectedWindowInfo = value;
                ShowCropper();
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets windows list
        /// </summary>
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
        private CropperWindow _cropperWindow;
        private WindowInfo _selectedWindowInfo;

        #endregion

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel()
        {
            StartPipCommand = new RelayCommand(StartPipCommandExecute);
            QuitCommand = new RelayCommand(QuitCommandExecute);
            ClosingCommand = new RelayCommand(ClosingCommandExecute);

            WindowsList = new ObservableCollection<WindowInfo>();

            ProcessesService.Instance.OpenWindowsChanged += (s, e) => UpdateWindowsList();
            ProcessesService.Instance.ForegroundWindowChanged += ForegroundWindowChanged;
            UpdateWindowsList();
        }

        /// <summary>
        /// Update <see cref="WindowsList"/> with <see cref="ProcessesService.OpenWindows"/> 
        /// </summary>
        private void UpdateWindowsList()
        {
            var openWindows = ProcessesService.Instance.OpenWindows;

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

        /// <summary>
        /// Close old cropper if exist, and open new with <see cref="SelectedWindowInfo"/>
        /// </summary>
        private void ShowCropper()
        {
            _cropperWindow?.Close();
            _cropperWindow = new CropperWindow();
            MessengerInstance.Send(SelectedWindowInfo);
            _cropperWindow.Show();
        }

        /// <summary>
        /// Callback in <see cref="StartPipCommandExecute"/>. Show <see cref="PiPModeWindow"/> and send selected window
        /// </summary>
        /// <param name="selectedRegion"></param>
        private void StartPip(NativeStructs.Rect selectedRegion)
        {
            var pip = new PiPModeWindow();
            MessengerInstance.Send(new SelectedWindow(SelectedWindowInfo, selectedRegion));
            pip.Show();
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when foreground window changed. Change <see cref="SelectedWindowInfo"/>
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event arguments</param>
        private void ForegroundWindowChanged(object sender, EventArgs e)
        {
            UpdateWindowsList();
            var foregroundWindow = ProcessesService.Instance.ForegroundWindow;
            if (foregroundWindow != null)
                SelectedWindowInfo = foregroundWindow;
        }

        #region commands

        /// <summary>
        /// Executed on click on change selected window button. Send message with <see cref="StartPip"/> callback
        /// </summary>
        private void StartPipCommandExecute()
        {
            MessengerInstance.Send<Action<NativeStructs.Rect>>(StartPip);
        }

        /// <summary>
        /// Executed on click on quit button
        /// </summary>
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

        /// <summary>
        /// Executed when the window is closing. Dispose <see cref="ProcessesService"/>. Close <see cref="CropperWindow"/>
        /// </summary>
        private void ClosingCommandExecute()
        {
            ProcessesService.Instance.Dispose();
            _cropperWindow?.Close();
        }

        #endregion

    }
}
