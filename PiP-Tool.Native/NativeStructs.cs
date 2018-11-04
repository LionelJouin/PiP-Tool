using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace PiP_Tool.Native
{
    public static class NativeStructs
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public Rect rcWindow;
            public Rect rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(bool? filler) : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
            {
                cbSize = (uint)(Marshal.SizeOf(typeof(WINDOWINFO)));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DwmThumbnailProperties
        {
            public int dwFlags;
            public Rect rcDestination;
            public Rect rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Psize
        {
            public int x;
            public int y;
        }

        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left, Top, Right, Bottom;

            public Rect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public Rect(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X
            {
                get => Left;
                set { Right -= (Left - value); Left = value; }
            }

            public int Y
            {
                get => Top;
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height
            {
                get => Bottom - Top;
                set => Bottom = value + Top;
            }

            public int Width
            {
                get => Right - Left;
                set => Right = value + Left;
            }

            public Point Location
            {
                get => new Point(Left, Top);
                set { X = value.X; Y = value.Y; }
            }

            public Size Size
            {
                get => new Size(Width, Height);
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator Rectangle(Rect r)
            {
                return new Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator Rect(Rectangle r)
            {
                return new Rect(r);
            }

            public static bool operator ==(Rect r1, Rect r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(Rect r1, Rect r2)
            {
                return !r1.Equals(r2);
            }

            public static Rect operator -(Rect r1, Rect r2)
            {
                return new Rectangle(r1.Left, r1.Top, r1.Width - (r2.Left + r2.Right), r1.Height - (r2.Top + r2.Bottom));
            }

            public static Rect operator *(Rect r1,  float f)
            {
                return new Rectangle(r1.Left, r1.Top, (int) (r1.Width * f), (int) (r1.Height * f));
            }

            public static Rect operator /(Rect r1, float f)
            {
                return new Rectangle(r1.Left, r1.Top, (int)(r1.Width / f), (int)(r1.Height / f));
            }

            public bool Equals(Rect r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is Rect)
                    return Equals((Rect)obj);
                else if (obj is Rectangle)
                    return Equals(new Rect((Rectangle)obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((Rectangle)this).GetHashCode();
            }

            public override string ToString()
            {
                return String.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }

    }
}
