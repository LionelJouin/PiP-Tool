using PiP_Tool.Interfaces;
using PiP_Tool.ViewModels;

namespace PiP_Tool
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            DataContext = new Main();
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is ICloseable)
                {
                    (DataContext as ICloseable).RequestClose += (_, __) => Close();
                }
            };
        }

    }
}
