using Helpers.Native;
using PiP_Tool.DataModel;
using Selector = PiP_Tool.ViewModels.Selector;

namespace PiP_Tool.Views
{
    /// <summary>
    /// Logique d'interaction pour SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow
    {
        
        public readonly Selector ViewModel;

        public NativeStructs.Rect SelectedRegion => ViewModel.SelectedRegion;

        public SelectorWindow(WindowInfo windowInfo)
        {
            ViewModel = new Selector(windowInfo);
            DataContext = ViewModel;
            InitializeComponent();
        }

    }
}
