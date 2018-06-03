using Helpers.Native;
using Point = System.Drawing.Point;
using Selector = PiP_Tool.ViewModels.Selector;

namespace PiP_Tool.Views
{
    /// <summary>
    /// Logique d'interaction pour SelectorWindow.xaml
    /// </summary>
    public partial class SelectorWindow
    {

        //public NativeStructs.Rect SizeRestriction;
        //public Point PositionRestriction;
        public readonly Selector ViewModel;

        public NativeStructs.Rect SelectedRegion => ViewModel.SelectedRegion;

        public SelectorWindow(NativeStructs.Rect sizeRestriction, Point positionRestriction)
        {
            //SizeRestriction = sizeRestriction;
            //PositionRestriction = positionRestriction;
            ViewModel = new Selector(sizeRestriction);
            DataContext = ViewModel;
            InitializeComponent();

            Height = sizeRestriction.Height;
            Width = sizeRestriction.Width;
            Left = positionRestriction.X;
            Top = positionRestriction.Y;
        }

    }
}
