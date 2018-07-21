using System.Windows.Input;
using PiP_Tool.Interfaces;

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

            Loaded += (s, e) =>
            {
                if (DataContext is ICloseable closeable)
                    closeable.RequestClose += (_, __) => Close();
            };
        }

        /// <summary>
        /// Drag this window
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }

    }
}
