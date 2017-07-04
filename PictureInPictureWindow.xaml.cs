using PiP_Tool.ViewModels;

namespace PiP_Tool
{
    /// <summary>
    /// Logique d'interaction pour PictureInPictureWindow.xaml
    /// </summary>
    public partial class PictureInPictureWindow
    {
        public PictureInPictureWindow()
        {
            DataContext = new PictureInPicture();
            InitializeComponent();
        }
    }
}
