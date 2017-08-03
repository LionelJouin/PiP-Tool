using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Native
{
    public class NativeConsts
    {

        // http://www.pinvoke.net/default.aspx/coredll.setwindowlong
        // http://www.jasinskionline.com/windowsapi/ref/s/setwindowlong.html
        public const int GWL_EXSTYLE = -20;
        public const int GWL_HINSTANCE = -6;
        public const int GWL_HWNDPARENT = -8;
        public const int GWL_ID = -12;
        public const int GWL_STYLE = -16;
        public const int GWL_USERDATA = -21;
        public const int GWL_WNDPROC = -4;
        public const int DWL_DLGPROC = 4;
        public const int DWL_MSGRESULT = 0;
        public const int DWL_USER = 8;

    }
}
