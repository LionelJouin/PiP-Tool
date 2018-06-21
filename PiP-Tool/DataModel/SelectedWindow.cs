using Helpers.Native;

namespace PiP_Tool.DataModel
{
    public class SelectedWindow
    {

        public WindowInfo WindowInfo { get; }
        public NativeStructs.Rect SelectedRegion { get; set; }

        public float Ratio => WindowInfo.Size.Height > 0 ? SelectedRegion.Width / (float)SelectedRegion.Height : 0;
        public float RatioHeightByWidth => WindowInfo.Size.Width > 0 ? SelectedRegion.Height / (float)SelectedRegion.Width : 0;

        public SelectedWindow(WindowInfo windowInfo, NativeStructs.Rect selectedRegion)
        {
            WindowInfo = windowInfo;
            SelectedRegion = new NativeStructs.Rect(
                selectedRegion.Left + WindowInfo.Border.Left,
                selectedRegion.Top + WindowInfo.Border.Top,
                selectedRegion.Right + WindowInfo.Border.Left,
                selectedRegion.Bottom + WindowInfo.Border.Top
                );
        }

    }
}
