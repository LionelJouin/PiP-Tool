using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Helpers.Native;

namespace PiP_Tool.Models
{
    public class WindowInfo
    {

        public IntPtr Handle { get; set; }
        public string Title { get; private set; }
        public Point Position { get; private set; }
        public Size Size { get; private set; }
        public NativeStructs.Rect Rect { get; private set; }

        public float Ratio => Size.Height > 0 ? Size.Width / (float)Size.Height : 0;
        public bool IsMinimized => (_winInfo.dwStyle & (uint)WindowStyles.WS_MINIMIZE) == (uint)WindowStyles.WS_MINIMIZE;

        private NativeStructs.WINDOWINFO _winInfo;
        private Point _savedPosition;

        public WindowInfo(IntPtr handle)
        {
            Handle = handle;
            SetSizeAndPosition();
            SetTitle();
            SetWinInfo();
        }

        private void SetSizeAndPosition()
        {
            if (!NativeMethods.GetWindowRect(Handle, out var rct)) return;
            Rect = rct;
            Position = new Point(rct.Left, rct.Top);
            Size = new Size(rct.Right - rct.Left + 1, rct.Bottom - rct.Top + 1);
        }

        private void SetTitle()
        {
            var length = NativeMethods.GetWindowTextLength(Handle);
            if (length == 0) return;

            var builder = new StringBuilder(length);
            NativeMethods.GetWindowText(Handle, builder, length + 1);
            Title = builder.ToString();
        }

        private void SetWinInfo()
        {
            _winInfo = new NativeStructs.WINDOWINFO();
            _winInfo.cbSize = (uint)Marshal.SizeOf(_winInfo);
            NativeMethods.GetWindowInfo(Handle, ref _winInfo);
        }

    }
}
