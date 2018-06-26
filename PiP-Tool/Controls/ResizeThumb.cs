using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PiP_Tool.Controls
{
    public class ResizeThumb : Thumb
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public ResizeThumb()
        {
            DragDelta += ResizeDragDelta;
        }

        /// <summary>
        /// Resize event handler
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event arguments</param>
        private void ResizeDragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;

            if (designerItem == null)
                return;

            double deltaVertical, deltaHorizontal;

            switch (VerticalAlignment)
            {
                // Resize by the bottom
                case VerticalAlignment.Bottom:
                    deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                    if (e.VerticalChange <= 0 || e.VerticalChange > 0 &&
                        Canvas.GetTop(designerItem) + designerItem.ActualHeight < designerItem.MaxHeight)
                    {
                        var height = designerItem.Height - deltaVertical;
                        if (Canvas.GetTop(designerItem) + height > designerItem.MaxHeight)
                            height = designerItem.MaxHeight - Canvas.GetTop(designerItem);
                        designerItem.Height = height;
                    }
                    break;
                // Resize by the top
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
                // Resize by the left
                case HorizontalAlignment.Left:
                    deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                    if (e.HorizontalChange >= 0 || e.HorizontalChange < 0 && Canvas.GetLeft(designerItem) > 0)
                    {
                        Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + deltaHorizontal);
                        designerItem.Width -= deltaHorizontal;
                    }
                    break;
                // Resize by the right
                case HorizontalAlignment.Right:
                    deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                    if (e.HorizontalChange <= 0 || e.HorizontalChange > 0 &&
                        Canvas.GetLeft(designerItem) + designerItem.ActualWidth < designerItem.MaxWidth)
                    {
                        var width = designerItem.Width - deltaHorizontal;
                        if (Canvas.GetLeft(designerItem) + width > designerItem.MaxWidth)
                            width = designerItem.MaxWidth - Canvas.GetLeft(designerItem);
                        designerItem.Width = width;
                    }
                    break;
            }

            e.Handled = true;
        }

    }
}
