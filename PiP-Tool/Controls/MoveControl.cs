using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PiP_Tool.Controls
{
    public class MoveControl : Thumb
    {

        public MoveControl()
        {
            DragDelta += MoveDragDelta;
        }

        private void MoveDragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as ContentControl;

            if (designerItem == null)
                return;

            var left = Canvas.GetLeft(designerItem) + e.HorizontalChange;
            if (left < 0)
                left = 0;
            else if (left + designerItem.ActualWidth > designerItem.MaxWidth)
                left = designerItem.MaxWidth - designerItem.ActualWidth;

            var top = Canvas.GetTop(designerItem) + e.VerticalChange;
            if (top < 0)
                top = 0;
            else if (top + designerItem.ActualHeight > designerItem.MaxHeight)
                top = designerItem.MaxHeight - designerItem.ActualHeight;

            Canvas.SetLeft(designerItem, left);
            Canvas.SetTop(designerItem, top);
        }

    }
}
