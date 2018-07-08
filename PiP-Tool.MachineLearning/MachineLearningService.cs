using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using PiP_Tool.MachineLearning.DataModel;

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
        private const string DataPath = "Data.txt";
        private readonly LearningPipeline _pipeline;

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
        
        /// <summary>
        /// Constructor (Singleton so private)
        /// </summary>
        private MachineLearningService()
        {
            _pipeline = new LearningPipeline
            {
                new TextLoader(DataPath).CreateFrom<WindowData>(separator: ','),
                new Dictionarizer("Label"),
                new TextFeaturizer("Program", "Program"),
                new TextFeaturizer("WindowTitle", "WindowTitle"),
                new ColumnConcatenator("Features", "Program", "WindowTitle"),
                new StochasticDualCoordinateAscentClassifier(),
                new PredictedLabelColumnOriginalValueConverter {PredictedLabelColumn = "PredictedLabel"}
            };
        }

        public RegionPrediction Predict(string program, string windowTitle)
        {
            return Predict(new WindowData
            {
                Program = program,
                WindowTitle = windowTitle
            });
        }

        public RegionPrediction Predict(WindowData windowData)
        {
            var model = _pipeline.Train<WindowData, RegionPrediction>();

            var prediction = model.Predict(windowData);

            prediction.Predicted();

            return prediction;
        }

    }
}
