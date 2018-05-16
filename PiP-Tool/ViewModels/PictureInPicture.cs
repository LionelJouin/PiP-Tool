using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Helpers.Native;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Size = System.Drawing.Size;

namespace PiP_Tool.ViewModels
{
    public class PictureInPicture : BaseViewModel
    {
        private const int Ips = 60;

        private bool _state;
        private DispatcherTimer _timer;
        private ImageSource _imageScreen;
        private Size _windowSize = new Size(100, 100);
        private readonly IntPtr _hWnd;
        public double Ratio { get; private set; }

        public ImageSource ImageScreen
        {
            get => _imageScreen;
            set
            {
                _imageScreen = value;
                NotifyPropertyChanged();
            }
        }

        public Size WindowSize
        {
            get => _windowSize;
            set
            {
                _windowSize = value;
                NotifyPropertyChanged();
            }
        }

        public PictureInPicture(IntPtr hWnd)
        {
            _hWnd = hWnd;
            Video();
        }

        private void Video()
        {
            if (_state)
            {
                _timer.Stop();
            }
            else
            {
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1 / Ips) };
                _timer.Tick += Screenshot;
                _timer.Start();
            }
            _state = !_state;
        }

        private void Screenshot(object sender, EventArgs e)
        {
            ImageScreen = ImageSourceForBitmap(PrintWindow(_hWnd));
        }

        public Bitmap PrintWindow(IntPtr hwnd)
        {
            NativeMethods.GetWindowRect(hwnd, out var rc);

            Ratio = rc.Width / rc.Height;

            var bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            var gfxBmp = Graphics.FromImage(bmp);
            var hdcBitmap = gfxBmp.GetHdc();

            NativeMethods.PrintWindow(hwnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }

        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { NativeMethods.DeleteObject(handle); }
        }

    }
}
