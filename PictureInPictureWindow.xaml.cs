using System.Windows;
using PiP_Tool.ViewModels;

namespace PiP_Tool
{
    /// <summary>
    /// Logique d'interaction pour PictureInPictureWindow.xaml
    /// </summary>
    public partial class PictureInPictureWindow
    {
        public PictureInPictureWindow(Point position, Size size)
        {
            DataContext = new PictureInPicture(position, size);
            InitializeComponent();
        }
    }
}
