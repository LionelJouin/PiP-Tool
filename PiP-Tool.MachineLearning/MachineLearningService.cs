using System;
using System.IO;
using System.Threading;
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

        public bool DataExist => Directory.Exists(_folderPath) && File.Exists(_dataPath);
        public bool ModelExist => Directory.Exists(_folderPath) && File.Exists(_modelPath);

        #endregion

        #region private

        private static MachineLearningService _instance;
        private readonly string _folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PiP-Tool");
        private readonly string _dataPath;
        private readonly string _modelPath;
        private PredictionModel<WindowData, RegionPrediction> _model;

        #endregion

        /// <summary>
        /// Constructor (Singleton so private)
        /// </summary>
        private MachineLearningService()
        {
            _dataPath = Path.Combine(_folderPath, "Data.csv");
            _modelPath = Path.Combine(_folderPath, "Model.zip");

            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);

            if (!DataExist)
                File.WriteAllText(_dataPath, "");
        }

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

        public async Task TrainAsync()
        {
            try
            {
                var pipeline = new LearningPipeline {
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<RegionPrediction> PredictAsync(string program, string windowTitle, float windowTop, float windowLeft, float windowHeight, float windowWidth)
        {
            return await PredictAsync(new WindowData
            {
                Program = program,
                WindowTitle = windowTitle,
                WindowTop = windowTop,
                WindowLeft = windowLeft,
                WindowHeight = windowHeight,
                WindowWidth = windowWidth
            });
        }

        public async Task<RegionPrediction> PredictAsync(WindowData windowData)
        {
            if (!ModelExist)
                await TrainAsync();

            if (_model == null)
                _model = await PredictionModel.ReadAsync<WindowData, RegionPrediction>(_modelPath);

            var prediction = _model.Predict(windowData);
            prediction.Predicted();

            return prediction;
        }

        public void AddData(string region, string program, string windowTitle, float windowTop, float windowLeft, float windowHeight, float windowWidth)
        {
            var newLine =
                $"{Environment.NewLine}" +
                $"{region}," +
                $"{program}," +
                $"{windowTitle}," +
                $"{windowTop}," +
                $"{windowLeft}," +
                $"{windowHeight}," +
                $"{windowWidth}";

            if (!File.Exists(_dataPath))
                File.WriteAllText(_dataPath, "");

            File.AppendAllText(_dataPath, newLine);
        }

    }
}
