using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Helpers.Native;
using PiP_Tool.DataModel;
using PiP_Tool.Interfaces;
using PiP_Tool.ViewModels;
using Point = System.Drawing.Point;

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

        private const int TopBarHeight = 100;
        private bool _renderSizeEventEnabled;
        private readonly Grid _topBar;

        public PictureInPictureWindow(SelectedWindow selectedWindow)
        {
            InitializeComponent();

            _wih = new WindowInteropHelper(this);
            _pictureInPicture = new PictureInPicture();
            DataContext = _pictureInPicture;

            _topBar = FindName("TopBar") as Grid;

            Loaded += (s, e) =>
            {
                _pictureInPicture.Init(_wih.Handle, selectedWindow);
                if (DataContext is ICloseable)
                {
                    (DataContext as ICloseable).RequestClose += (_, __) => Close();
                }
            };
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (_renderSizeEventEnabled)
            {
                _renderSizeEventEnabled = false;
                return;
            }
            var topBarHeight = 0;
            if (_topBar.IsVisible)
                topBarHeight = TopBarHeight;
            if (sizeInfo.WidthChanged)
            {
                Width = (sizeInfo.NewSize.Height - topBarHeight) * _pictureInPicture.Ratio;
            }
            else
            {
                Height = sizeInfo.NewSize.Width * _pictureInPicture.Ratio + topBarHeight;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (_topBar.IsVisible)
                return;
            _renderSizeEventEnabled = true;
            _topBar.Visibility = Visibility.Visible;
            _pictureInPicture.SetOffset(TopBarHeight);
            Top -= TopBarHeight;
            Height = ActualHeight + TopBarHeight;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // Prevent OnMouseEnter, OnMouseLeave loop
            Thread.Sleep(50);
            NativeMethods.GetCursorPos(out var p);
            var r = new Rectangle(Convert.ToInt32(Left), Convert.ToInt32(Top), Convert.ToInt32(Width), Convert.ToInt32(Height));
            var pa = new Point(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));

            if (!_topBar.IsVisible || r.Contains(pa))
                return;
            _topBar.Visibility = Visibility.Hidden;
            _renderSizeEventEnabled = true;
            _pictureInPicture.SetOffset(0);
            Top += TopBarHeight;
            Height = ActualHeight - TopBarHeight;
        }

    }
}
