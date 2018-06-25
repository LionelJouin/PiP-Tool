using PiP_Tool.Interfaces;

namespace PiP_Tool.Views
{
    /// <summary>
    /// Logique d'interaction pour SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow
    {

        public SelectorWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is ICloseable closeable)
                    closeable.RequestClose += (_, __) => Close();
            };
        }

    }
}
