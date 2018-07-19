using PiP_Tool.Interfaces;
using PiP_Tool.Shared;

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

            Logger.Instance.Info("   ====== CropperWindow ======   ");

            Loaded += (s, e) =>
            {
                if (DataContext is ICloseable closeable)
                    closeable.RequestClose += (_, __) => Close();
            };
        }

    }
}
