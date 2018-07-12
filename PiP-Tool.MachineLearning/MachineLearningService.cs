using System;
using System.IO;
using System.Linq;
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
        private readonly TaskCompletionSource<bool> _ready;
        private readonly SemaphoreSlim _semaphore;

        #endregion

        /// <summary>
        /// Constructor (Singleton so private)
        /// </summary>
        private MachineLearningService()
        {
            _semaphore = new SemaphoreSlim(1);
            _ready = new TaskCompletionSource<bool>();

            _dataPath = Path.Combine(_folderPath, "Data.csv");
            _modelPath = Path.Combine(_folderPath, "Model.zip");

            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);
        }

        /// <summary>
        /// Init <see cref="_model"/> : read model if exist, create it if not
        /// </summary>
        public void Init()
        {
            if (_ready.Task.IsCompleted)
                return;

            Task.Run(async () =>
            {
                if (!ModelExist)
                    await Train();
                else
                    _model = await PredictionModel.ReadAsync<WindowData, RegionPrediction>(_modelPath);
            }).ContinueWith(obj =>
            {
                _ready.SetResult(true);
            });
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

        /// <summary>
        /// check if prediction is ready and call <see cref="Train()"/>
        /// </summary>
        /// <returns>Task asynchronous method</returns>
        public async Task TrainAsync()
        {
            if (!_ready.Task.IsCompleted)
                await _ready.Task;

            await Train();
        }

        /// <summary>
        /// Train and write in model and set <see cref="_model"/> 
        /// </summary>
        /// <returns>Task asynchronous method</returns>
        private async Task Train()
        {
            try
            {
                CheckDataFile();

                var pipeline = new LearningPipeline {
                        new TextLoader(_dataPath).CreateFrom<WindowData>(separator: ','),
                        new Dictionarizer("Label"),
                        new TextFeaturizer("Program", "Program"),
                        new TextFeaturizer("WindowTitle", "WindowTitle"),
                        new ColumnConcatenator("Features", "Program", "WindowTitle", "WindowTop", "WindowLeft", "WindowHeight", "WindowWidth"),
                        new StochasticDualCoordinateAscentClassifier(),
                        new PredictedLabelColumnOriginalValueConverter {PredictedLabelColumn = "PredictedLabel"}
                };

                await _semaphore.WaitAsync();
                _model = pipeline.Train<WindowData, RegionPrediction>();
                _semaphore.Release();

                await _model.WriteAsync(_modelPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Create WindowData and call <see cref="PredictAsync(WindowData)"/>
        /// </summary>
        /// <param name="program"><see cref="WindowData.Program"/></param>
        /// <param name="windowTitle"><see cref="WindowData.WindowTitle"/></param>
        /// <param name="windowTop"><see cref="WindowData.WindowTop"/></param>
        /// <param name="windowLeft"><see cref="WindowData.WindowLeft"/></param>
        /// <param name="windowHeight"><see cref="WindowData.WindowHeight"/></param>
        /// <param name="windowWidth"><see cref="WindowData.WindowWidth"/></param>
        /// <returns>Task asynchronous method with region predicted</returns>
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

        /// <summary>
        /// Predict region
        /// </summary>
        /// <param name="windowData">Window data to use for the prediction</param>
        /// <returns>Task asynchronous method with region predicted</returns>
        public async Task<RegionPrediction> PredictAsync(WindowData windowData)
        {
            if (!_ready.Task.IsCompleted)
                await _ready.Task;

            await _semaphore.WaitAsync();
            var prediction = _model.Predict(windowData);
            _semaphore.Release();

            prediction.Predicted();

            return prediction;
        }

        /// <summary>
        /// Add new data
        /// </summary>
        /// <param name="region"><see cref="WindowData.Region"/></param>
        /// <param name="program"><see cref="WindowData.Program"/></param>
        /// <param name="windowTitle"><see cref="WindowData.WindowTitle"/></param>
        /// <param name="windowTop"><see cref="WindowData.WindowTop"/></param>
        /// <param name="windowLeft"><see cref="WindowData.WindowLeft"/></param>
        /// <param name="windowHeight"><see cref="WindowData.WindowHeight"/></param>
        /// <param name="windowWidth"><see cref="WindowData.WindowWidth"/></param>
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

        /// <summary>
        /// Check if data file exist, create and add data if not
        /// </summary>
        private void CheckDataFile()
        {
            if (!DataExist)
                File.WriteAllText(_dataPath, "");

            var lineCount = File.ReadLines(_dataPath).Count();
            if (lineCount >= 2)
                return;
            AddData("0 0 100 100", "PiP", "PiP", 0, 0, 100, 100);
            AddData("0 0 100 100", "Tool", "Tool", 0, 0, 200, 200);
            AddData("100 100 200 200", "Test", "Test", 0, 0, 300, 300);
        }

    }
}
