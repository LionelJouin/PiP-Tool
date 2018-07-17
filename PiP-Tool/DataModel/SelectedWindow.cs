using PiP_Tool.Helpers;
using PiP_Tool.Native;

namespace PiP_Tool.DataModel
{
    public class SelectedWindow
    {

        #region public

        public WindowInfo WindowInfo { get; }
        public NativeStructs.Rect SelectedRegion { get; set; }

        public NativeStructs.Rect SelectedRegionNoBorder => new NativeStructs.Rect(
            SelectedRegion.Left - WindowInfo.Border.Left,
            SelectedRegion.Top - WindowInfo.Border.Top,
            SelectedRegion.Right - WindowInfo.Border.Left,
            SelectedRegion.Bottom - WindowInfo.Border.Top
        );
        /// <summary>
        /// Gets ratio width / height
        /// </summary>
        public float Ratio => WindowInfo.Size.Height > 0 ? SelectedRegion.Width / (float)SelectedRegion.Height : 0;
        /// <summary>
        /// Gets ratio height / width
        /// </summary>
        public float RatioHeightByWidth => WindowInfo.Size.Width > 0 ? SelectedRegion.Height / (float)SelectedRegion.Width : 0;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="windowInfo">Selected window</param>
        /// <param name="selectedRegion">Selected region</param>
        public SelectedWindow(WindowInfo windowInfo, NativeStructs.Rect selectedRegion)
        {
            WindowInfo = windowInfo;

            selectedRegion = new NativeStructs.Rect(
                (int) (selectedRegion.Left * ScaleHelper.ScalingFactor),
                (int) (selectedRegion.Top * ScaleHelper.ScalingFactor),
                (int) (selectedRegion.Right * ScaleHelper.ScalingFactor),
                (int) (selectedRegion.Bottom * ScaleHelper.ScalingFactor)
            );

            SelectedRegion = new NativeStructs.Rect(
                selectedRegion.Left + WindowInfo.Border.Left,
                selectedRegion.Top + WindowInfo.Border.Top,
                selectedRegion.Right + WindowInfo.Border.Left,
                selectedRegion.Bottom + WindowInfo.Border.Top
                );
        }

    }
}
