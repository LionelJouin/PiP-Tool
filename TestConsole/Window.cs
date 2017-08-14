using System;
using System.Text;
using Helpers.Native;

namespace TestConsole
{
    public class Window
    {

        private readonly IntPtr _window;
        private readonly int _defaultWindowLong;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">Handle of window</param>
        public Window(IntPtr window)
        {
            _window = window;
            _defaultWindowLong = NativeMethods.GetWindowLong(_window, NativeConsts.GWL_STYLE);
        }

        /// <summary>
        /// Remove border and set on top
        /// </summary>
        public void SetWindowPiP(bool statePiP = true)
        {
            SetWindowStyle(statePiP);
            SetWindowOnTop(statePiP);
        }

        /// <summary>
        /// Remove border of the window
        /// </summary>
        private void SetWindowStyle(bool statePiP = true)
        {
            var style = (uint)_defaultWindowLong;

            if (statePiP)
                style = style & ~(uint)NativeEnums.WindowStyles.WS_CAPTION;

            NativeMethods.SetWindowLong(
                _window,
                NativeConsts.GWL_STYLE,
                style
                );
        }

        /// <summary>
        /// Set window on top of others windows
        /// </summary>
        private void SetWindowOnTop(bool statePiP = true)
        {
            var pos = (IntPtr) NativeEnums.SpecialWindowHandles.HWND_TOP;
            if (statePiP)
                pos = (IntPtr)NativeEnums.SpecialWindowHandles.HWND_TOPMOST;
            NativeMethods.SetWindowPos(
                    _window,
                    pos,
                    0, 0, 0, 0,
                    (int)NativeEnums.SetWindowPosFlags.SWP_NOMOVE | (int)NativeEnums.SetWindowPosFlags.SWP_NOSIZE | (int)NativeEnums.SetWindowPosFlags.SWP_FRAMECHANGED
                    );
        }

        /// <summary>
        /// Get title of the window
        /// </summary>
        /// <returns></returns>
        public string GetWindowTitle()
        {
            const int nChars = 256;
            var buff = new StringBuilder(nChars);

            if (NativeMethods.GetWindowText(_window, buff, nChars) > 0)
                return buff.ToString();

            return null;
        }

    }
}
