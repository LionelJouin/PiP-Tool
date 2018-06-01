using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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

        public NativeStructs.Rect SizeRestriction;
        public Point PositionRestriction;
        public readonly Selector ViewModel;

        public SelectorWindow(NativeStructs.Rect sizeRestriction, Point positionRestriction)
        {
            SizeRestriction = sizeRestriction;
            PositionRestriction = positionRestriction;
            ViewModel = new Selector(sizeRestriction);
            DataContext = ViewModel;
            InitializeComponent();

            Height = sizeRestriction.Height;
            Width = sizeRestriction.Width;
            Left = PositionRestriction.X;
            Top = PositionRestriction.Y;
        }

    }
}
