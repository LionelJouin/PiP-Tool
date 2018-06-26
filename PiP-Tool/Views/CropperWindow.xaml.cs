using PiP_Tool.Interfaces;

namespace PiP_Tool.Views
{
    /// <summary>
    /// Logique d'interaction pour CropperWindow.xaml
    /// </summary>
    public partial class CropperWindow
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public CropperWindow()
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
