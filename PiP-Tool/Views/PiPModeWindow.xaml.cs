using System.Windows.Input;
using PiP_Tool.Interfaces;
using PiP_Tool.Shared;

namespace PiP_Tool.Views
{
    /// <inheritdoc cref="PiPModeWindow" />
    /// <summary>
    /// Logique d'interaction pour PiPModeWindow.xaml
    /// </summary>
    public partial class PiPModeWindow
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public PiPModeWindow()
        {
            InitializeComponent();

            Logger.Instance.Info("   ====== PiPModeWindow ======   ");

            Loaded += (s, e) =>
            {
                if (DataContext is ICloseable closeable)
                    closeable.RequestClose += (_, __) => Close();
            };
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }

    }
}
