using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using PiP_Tool.Models;
using PiP_Tool.ViewModels;

namespace PiP_Tool.Views
{
    /// <inheritdoc cref="PictureInPictureWindow" />
    /// <summary>
    /// Logique d'interaction pour PictureInPictureWindow.xaml
    /// </summary>
    public partial class PictureInPictureWindow
    {

        private readonly WindowInteropHelper _wih;
        private readonly PictureInPicture _pictureInPicture;

        public PictureInPictureWindow(SelectedWindow selectedWindow)
        {
            InitializeComponent();

            _wih = new WindowInteropHelper(this);
            _pictureInPicture = new PictureInPicture();
            DataContext = _pictureInPicture;
            Loaded += (s, e) => _pictureInPicture.Init(_wih.Handle, selectedWindow);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (sizeInfo.WidthChanged)
            {
                Width = sizeInfo.NewSize.Height * _pictureInPicture.Ratio;
            }
            else
            {
                Height = sizeInfo.NewSize.Width * _pictureInPicture.Ratio;
            }
        }

    }
}
