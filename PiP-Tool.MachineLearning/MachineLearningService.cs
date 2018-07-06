namespace PiP_Tool.MachineLearning
{
    public class MachineLearningService
    {

        #region public

        /// <summary>
        /// Gets the instance of the singleton
        /// </summary>
        public static MachineLearningService Instance => _instance ?? (_instance = new MachineLearningService());

        #endregion

        #region private

        private static MachineLearningService _instance;

        #endregion

        /// <summary>
        /// Destructor
        /// </summary>
        ~MachineLearningService() => Dispose();

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }

    }
}
