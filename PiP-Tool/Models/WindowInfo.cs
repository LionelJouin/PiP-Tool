using System;
using System.Windows;
using Helpers.Native;

namespace PiP_Tool.Models
{
    public class WindowInfo
    {

        public IntPtr Handle { get; set; }
        public Rect PositionSize { get; set; }
        public Point Position { get; set; }
        public Size Size { get; set; }
        public int ZIndex { get; set; }

        public WindowInfo(IntPtr handle)
        {
            Handle = handle;
            Size = new Size(0, 0);
            Position = new Point(0, 0);

            NativeStructs.Rect rct;
            if (NativeMethods.GetWindowRect(handle, out rct))
            {
                Position = new Point(rct.Left, rct.Top);
                Size = new Size(rct.Right - rct.Left + 1, rct.Bottom - rct.Top + 1);
                PositionSize = new Rect
                {
                    Size = new Size(100, 100),
                    Location = Position
                };
            }
        }

    }
}
