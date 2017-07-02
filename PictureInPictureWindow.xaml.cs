using System.Windows;
using PiP_Tool.ViewModels;

namespace PiP_Tool
{
    /// <summary>
    /// Logique d'interaction pour PictureInPictureWindow.xaml
    /// </summary>
    public partial class PictureInPictureWindow : Window
    {
        public PictureInPictureWindow()
        {
            this.DataContext = new PictureInPicture();
            InitializeComponent();
        }
    }
}
