using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using PiP_Tool.DataModel;
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
        public Popup Popup;

        public PictureInPictureWindow(SelectedWindow selectedWindow)
        {
            InitializeComponent();

            _wih = new WindowInteropHelper(this);
            _pictureInPicture = new PictureInPicture();
            DataContext = _pictureInPicture;

            Loaded += (s, e) => _pictureInPicture.Init(_wih.Handle, selectedWindow);

            var element = FindName("TopbarPopup");
            if (element is Popup)
                Popup = element as Popup;
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

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            PopupPosition();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Popup.IsOpen = true;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!Popup.IsMouseOver)
                Popup.IsOpen = false;
        }
        private void OpenPopup(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = true;
        }
        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            if (!IsMouseOver)
                Popup.IsOpen = false;
        }

        private void PopupPosition()
        {
            var offset = Popup.HorizontalOffset;
            Popup.HorizontalOffset = offset + 1;
            Popup.HorizontalOffset = offset;
        }

    }
}
