using System.Windows;
using PiP_Tool.ViewModels;

namespace PiP_Tool
{
    /// <summary>
    /// Logique d'interaction pour SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow : Window
    {
        public SelectorWindow()
        {
            this.DataContext = new Selector();
            InitializeComponent();
        }
    }
}
