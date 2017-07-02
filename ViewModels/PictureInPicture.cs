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
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace PiP_Tool.ViewModels
{
    public class PictureInPicture : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _state;

        private ImageSource _imageScreen;
        private DispatcherTimer _timer;

        public ImageSource ImageScreen
        {
            get { return _imageScreen; }
            set
            {
                _imageScreen = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand TestCommand
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
                _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
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
            using (var screenBmp = new Bitmap(
                100,
                100,
                PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
