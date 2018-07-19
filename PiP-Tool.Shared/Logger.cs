using System.Reflection;
using log4net;
using log4net.Config;

namespace PiP_Tool.Shared
{
    public class Logger
    {

        #region public

        public static Logger Instance => _instance ?? (_instance = new Logger());

        #endregion

        #region private

        private static Logger _instance;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        private Logger()
        {
            GlobalContext.Properties["LogFileName"] = Constants.LogPath;
            XmlConfigurator.Configure();
        }

        public void Debug(string message)
        {
#if DEBUG
            _log.Debug(message);
#endif
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }

        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Fatal(string message)
        {
            _log.Fatal(message);
        }

    }
}
