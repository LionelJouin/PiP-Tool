using System;
using System.IO;
using System.Threading.Tasks;
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
        private readonly string _dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Data.csv");
        private readonly string _modelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Model.zip");
        private PredictionModel<WindowData, RegionPrediction> _model;

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
            if (!File.Exists(_dataPath))
                File.WriteAllText(_dataPath, "");
        }

        public async Task TrainAsync()
        {
            var pipeline = new LearningPipeline
            {
                new TextLoader(_dataPath).CreateFrom<WindowData>(separator: ','),
                new Dictionarizer("Label"),
                new TextFeaturizer("Program", "Program"),
                new TextFeaturizer("WindowTitle", "WindowTitle"),
                new ColumnConcatenator("Features", "Program", "WindowTitle", "WindowTop", "WindowLeft", "WindowHeight", "WindowWidth"),
                new StochasticDualCoordinateAscentClassifier(),
                new PredictedLabelColumnOriginalValueConverter {PredictedLabelColumn = "PredictedLabel"}
            };
            _model = pipeline.Train<WindowData, RegionPrediction>();

            await _model.WriteAsync(_modelPath);
        }

        public Task<RegionPrediction> Predict(string program, string windowTitle)
        {
            return PredictAsync(new WindowData
            {
                Program = program,
                WindowTitle = windowTitle
            });
        }

        public async Task<RegionPrediction> PredictAsync(WindowData windowData)
        {
            if (_model == null)
            {
                _model = await PredictionModel.ReadAsync<WindowData, RegionPrediction>(_modelPath);
            }
            var prediction = _model.Predict(windowData);

            prediction.Predicted();

            return prediction;
        }

        public void AddData(WindowData windowData)
        {
            var newLine =
                $"{Environment.NewLine}" +
                $"{windowData.Region}," +
                $"{windowData.Program}," +
                $"{windowData.WindowTitle}," +
                $"{windowData.WindowTop}," +
                $"{windowData.WindowLeft}," +
                $"{windowData.WindowHeight}," +
                $"{windowData.WindowWidth}";

            if (!File.Exists(_dataPath))
                File.WriteAllText(_dataPath, "");

            File.AppendAllText(_dataPath, newLine);
        }

    }
}
