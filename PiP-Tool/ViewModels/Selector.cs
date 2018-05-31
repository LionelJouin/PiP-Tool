using Helpers.Native;

namespace PiP_Tool.ViewModels
{
    public class Selector : BaseViewModel
    {

        public int MaxHeight => _sizeRestriction.Height;
        public int MaxWidth => _sizeRestriction.Width;

        private NativeStructs.Rect _sizeRestriction;

        public Selector(NativeStructs.Rect sizeRestriction)
        {
            _sizeRestriction = sizeRestriction;
        }

        //public int Height
        //{
        //    get => _selectedWindow.SelectedRegion.Height;
        //    set
        //    {
        //        _selectedWindow.SelectedRegion = new NativeStructs.Rect(
        //            _selectedWindow.SelectedRegion.Left,
        //            _selectedWindow.SelectedRegion.Top,
        //            _selectedWindow.SelectedRegion.Right,
        //            _selectedWindow.SelectedRegion.Bottom
        //        );
        //        _selectedWindow.SelectedRegion = new NativeStructs.Rect(
        //            _selectedWindow.SelectedRegion.Left,
        //            _selectedWindow.SelectedRegion.Top,
        //            _selectedWindow.SelectedRegion.Right,
        //            _selectedWindow.SelectedRegion.Bottom
        //        )
        //        {
        //            Height = value
        //        };
        //        NotifyPropertyChanged();
        //    }
        //}
        //public int Width
        //{
        //    get => _selectedWindow.SelectedRegion.Width;
        //    set
        //    {
        //        _selectedWindow.SelectedRegion = new NativeStructs.Rect(
        //            _selectedWindow.SelectedRegion.Left,
        //            _selectedWindow.SelectedRegion.Top,
        //            _selectedWindow.SelectedRegion.Right,
        //            _selectedWindow.SelectedRegion.Bottom
        //        )
        //        {
        //            Width = value
        //        };
        //        NotifyPropertyChanged();
        //    }
        //}

        //private SelectedWindow _selectedWindow;

        //public Selector(SelectedWindow selectedWindow)
        //{
        //    _selectedWindow = selectedWindow;
        //}

    }
}
