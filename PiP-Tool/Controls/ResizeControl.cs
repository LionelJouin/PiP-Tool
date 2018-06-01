using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PiP_Tool.Controls
{
    public class ResizeControl : Thumb
    {

        public ResizeControl()
        {
            DragDelta += ResizeDragDelta;
        }

        private void ResizeDragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;

            if (designerItem == null)
                return;

            double deltaVertical, deltaHorizontal;

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                    if (e.VerticalChange <= 0 || e.VerticalChange > 0 && Canvas.GetTop(designerItem) + designerItem.ActualHeight < designerItem.MaxHeight)
                        designerItem.Height -= deltaVertical;
                    break;
                case VerticalAlignment.Top:
                    deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                    if (e.VerticalChange >= 0 || e.VerticalChange < 0 && Canvas.GetTop(designerItem) > 0)
                    {
                        Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + deltaVertical);
                        designerItem.Height -= deltaVertical;
                    }
                    break;
            }

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                    if (e.HorizontalChange >= 0 || e.HorizontalChange < 0 && Canvas.GetLeft(designerItem) > 0)
                    {
                        Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + deltaHorizontal);
                        designerItem.Width -= deltaHorizontal;
                    }
                    break;
                case HorizontalAlignment.Right:
                    deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                    if (e.HorizontalChange <= 0 || e.HorizontalChange > 0 && Canvas.GetLeft(designerItem) + designerItem.ActualWidth < designerItem.MaxWidth)
                        designerItem.Width -= deltaHorizontal;
                    break;
            }

            e.Handled = true;
        }

    }
}
