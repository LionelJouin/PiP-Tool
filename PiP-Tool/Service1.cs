using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace PiP_Tool
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

    }
}
