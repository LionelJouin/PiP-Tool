using System.Windows.Input;
using PiP_Tool.ViewModels;

namespace PiP_Tool
{
    /// <summary>
    /// Logique d'interaction pour SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow
    {

        private readonly Selector _viewModel;

        public SelectorWindow()
        {
            _viewModel = new Selector();
            DataContext = _viewModel;
            InitializeComponent();
        }

        private new void MouseDown(object sender, MouseEventArgs e)
        {
        }

        private new void MouseUp(object sender, MouseEventArgs e)
        {
            _viewModel.SetCursor(Mouse.GetPosition(CanvasMain));
        }

        private new void MouseMove(object sender, MouseEventArgs e)
        {
        }

    }
}
