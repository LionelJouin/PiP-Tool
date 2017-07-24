using System;
using System.Windows;
using System.Windows.Input;
using PiP_Tool.ViewModels;

namespace PiP_Tool.Views
{
    /// <summary>
    /// Logique d'interaction pour WindowSelectorWindow.xaml
    /// </summary>
    public partial class WindowSelectorWindow
    {

        public readonly WindowSelector ViewModel;

        public WindowSelectorWindow()
        {
            ViewModel = new WindowSelector();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            ViewModel.OnMouseMove(Mouse.GetPosition(this));
        }

    }
}
