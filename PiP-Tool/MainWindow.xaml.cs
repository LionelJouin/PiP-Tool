using System;
using PiP_Tool.Interfaces;
using PiP_Tool.ViewModels;

namespace PiP_Tool
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public readonly Main ViewModel;

        public MainWindow()
        {
            ViewModel = new Main();
            DataContext = ViewModel;
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is ICloseable closeable)
                    closeable.RequestClose += (_, __) => Close();
                Closing += ViewModel.OnWindowClosing;
            };

        }

    }
}
