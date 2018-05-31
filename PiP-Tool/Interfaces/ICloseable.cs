using System;

namespace PiP_Tool.Interfaces
{
    public interface ICloseable
    {

        event EventHandler<EventArgs> RequestClose;

    }
}
