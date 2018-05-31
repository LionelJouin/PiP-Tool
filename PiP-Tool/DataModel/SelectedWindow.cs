using Helpers.Native;

namespace PiP_Tool.DataModel
{
    public class SelectedWindow
    {

        public WindowInfo WindowInfo { get; private set; }
        public NativeStructs.Rect SelectedRegion { get; set; }

        public float Ratio => WindowInfo.Size.Height > 0 ? SelectedRegion.Width / (float)SelectedRegion.Height : 0;

        public SelectedWindow(WindowInfo windowInfo, NativeStructs.Rect selectedRegion)
        {
            WindowInfo = windowInfo;
            SelectedRegion = new NativeStructs.Rect(
                selectedRegion.Left + WindowInfo.Border.Left,
                selectedRegion.Top + WindowInfo.Border.Top,
                selectedRegion.Right - WindowInfo.Border.Right,
                selectedRegion.Bottom - WindowInfo.Border.Bottom
                );
        }

    }
}
