using System;
using System.Runtime.InteropServices;
using System.Text;
using PiP_Tool.Native;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace PiP_Tool.DataModel
{
    public class WindowInfo
    {

        public IntPtr Handle { get; }
        public string Title { get; private set; }
        public Point Position { get; private set; }
        public Size Size { get; private set; }
        public NativeStructs.Rect Rect { get; private set; }
        public NativeStructs.Rect Border { get; set; }

        public bool IsMinimized => (_winInfo.dwStyle & (uint)WindowStyles.WS_MINIMIZE) == (uint)WindowStyles.WS_MINIMIZE;

        private NativeStructs.WINDOWINFO _winInfo;

        public WindowInfo(IntPtr handle)
        {
            Handle = handle;
            RefreshInfo();
        }

        public void RefreshInfo()
        {
            GetSizeAndPosition();
            GetTitle();
            GetWinInfo();
            GetBorder();
        }

        public void SetAsForegroundWindow()
        {
            RefreshInfo();
            if (IsMinimized)
                NativeMethods.ShowWindow(Handle, ShowWindowCommands.Restore);
            RefreshInfo();
            NativeMethods.SetForegroundWindow(Handle);
        }

        private void GetSizeAndPosition()
        {
            if (!NativeMethods.GetWindowRect(Handle, out var rct)) return;
            Rect = rct;
            Position = new Point(rct.Left, rct.Top);
            Size = new Size(rct.Right - rct.Left + 1, rct.Bottom - rct.Top + 1);
        }

        private void GetTitle()
        {
            var length = NativeMethods.GetWindowTextLength(Handle);
            if (length == 0) return;

            var builder = new StringBuilder(length);
            NativeMethods.GetWindowText(Handle, builder, length + 1);
            Title = builder.ToString();
        }

        private void GetWinInfo()
        {
            _winInfo = new NativeStructs.WINDOWINFO();
            _winInfo.cbSize = (uint)Marshal.SizeOf(_winInfo);
            NativeMethods.GetWindowInfo(Handle, ref _winInfo);
        }

        private void GetBorder()
        {
            NativeMethods.DwmGetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.ExtendedFrameBounds, out NativeStructs.Rect frame, Marshal.SizeOf(typeof(NativeStructs.Rect)));
            Border = new NativeStructs.Rect(
                frame.Left - Rect.Left,
                frame.Top - Rect.Top,
                Rect.Right - frame.Right,
                Rect.Bottom - frame.Bottom
            );
        }

        public override bool Equals(object obj)
        {
            return obj is WindowInfo windowInfo && Handle.Equals(windowInfo.Handle);
        }

        protected bool Equals(WindowInfo other)
        {
            return Handle.Equals(other.Handle);
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public static bool operator ==(WindowInfo left, WindowInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WindowInfo left, WindowInfo right)
        {
            return !Equals(left, right);
        }

    }
}
