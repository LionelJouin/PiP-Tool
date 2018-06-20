using System.Windows;
using System.Windows.Input;
using PiP_Tool.Interfaces;

namespace PiP_Tool.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private bool _dragging;
        private Point _anchorPoint;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is ICloseable closeable)
                    closeable.RequestClose += (_, __) => Close();
            };
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_dragging)
                return;
            Left = Left + e.GetPosition(this).X - _anchorPoint.X;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _anchorPoint = e.GetPosition(this);
            _dragging = true;
            CaptureMouse();
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!_dragging)
                return;
            ReleaseMouseCapture();
            _dragging = false;
            e.Handled = true;
        }

    }
}
