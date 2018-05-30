using System;
using System.Windows;
using System.Windows.Input;
using Helpers.Native;
using PiP_Tool.ViewModels;
using Point = System.Drawing.Point;

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
            ViewModel = new Selector();
            DataContext = ViewModel;
            InitializeComponent();

            Left = PositionRestriction.X;
            Top = PositionRestriction.Y;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            if (Left < PositionRestriction.X)
                Left = PositionRestriction.X;

            if (Left + Width > PositionRestriction.X + SizeRestriction.Width)
                Left = PositionRestriction.X + SizeRestriction.Width - Width;

            if (Top < PositionRestriction.Y)
                Top = PositionRestriction.Y;

            if (Top + Height > PositionRestriction.Y + SizeRestriction.Height)
                Top = PositionRestriction.Y + SizeRestriction.Height - Height;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (sizeInfo.NewSize.Width > SizeRestriction.Width)
                Width = SizeRestriction.Width;

            if (sizeInfo.NewSize.Height > SizeRestriction.Height)
                Height = SizeRestriction.Height;
        }

    }
}
