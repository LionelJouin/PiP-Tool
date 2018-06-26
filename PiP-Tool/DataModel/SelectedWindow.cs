using PiP_Tool.Native;

namespace PiP_Tool.DataModel
{
    public class SelectedWindow
    {

        #region public

        public WindowInfo WindowInfo { get; }
        public NativeStructs.Rect SelectedRegion { get; set; }

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
            SelectedRegion = new NativeStructs.Rect(
                selectedRegion.Left + WindowInfo.Border.Left,
                selectedRegion.Top + WindowInfo.Border.Top,
                selectedRegion.Right + WindowInfo.Border.Left,
                selectedRegion.Bottom + WindowInfo.Border.Top
                );
        }

    }
}
