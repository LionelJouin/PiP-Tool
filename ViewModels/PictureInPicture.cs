using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PiP_Tool.Common;

namespace PiP_Tool.ViewModels
{
    public class PictureInPicture : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _state;

        private ImageSource _imageScreen;
        private DispatcherTimer _timer;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public ImageSource ImageScreen
        {
            get { return _imageScreen; }
            set
            {
                _imageScreen = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand CaptureCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Video();
                });
            }
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

        private static BitmapSource CopyScreen()
        {
            BitmapSource result;
            using (var screenBmp = new Bitmap(100, 100))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size, CopyPixelOperation.SourceCopy);
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
