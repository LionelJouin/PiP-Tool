using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace PiP_Tool.ViewModels
{
    public class PictureInPicture : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private bool _state;
        private ImageSource _imageScreen;
        private DispatcherTimer _timer;
        private Point _capturePosition;
        private Size _captureSize;
        private Size _windowSize = new Size(100, 100);
        public double Ratio;

        public ImageSource ImageScreen
        {
            get { return _imageScreen; }
            set
            {
                _imageScreen = value;
                NotifyPropertyChanged();
            }
        }

        public Size WindowSize
        {
            get { return _windowSize; }
            set
            {
                _windowSize = value;
                NotifyPropertyChanged();
            }
        }

        public PictureInPicture(Point position, Size size)
        {
            _capturePosition = position;
            _captureSize = size;
            Ratio = _captureSize.Width / _captureSize.Height;
            WindowSize = new Size(_captureSize.Width, _captureSize.Height);
            // todo max size, min size
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
                _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
                _timer.Tick += Screenshot;
                _timer.Start();
            }
            _state = !_state;
        }

        private void Screenshot(object sender, EventArgs e)
        {
            ImageScreen = CopyScreen();
        }

        private BitmapSource CopyScreen()
        {
            BitmapSource result;
            using (var screenBmp = new Bitmap((int)_captureSize.Width, (int)_captureSize.Height))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen((int)_capturePosition.X, (int)_capturePosition.Y, 0, 0, screenBmp.Size, CopyPixelOperation.SourceCopy);
                    var hBitmap = screenBmp.GetHbitmap();
                    bmpGraphics.Dispose();
                    result = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    DeleteObject(hBitmap);

                }
            }
            GC.Collect();
            return result;
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
