using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
using PiP_Tool.MachineLearning.DataModel;
using PiP_Tool.Shared;

namespace PiP_Tool.MachineLearning
{
    public class MachineLearningService
    {

        #region public

        /// <summary>
        /// Gets the instance of the singleton
        /// </summary>
        public static MachineLearningService Instance => _instance ?? (_instance = new MachineLearningService());

        public bool DataExist => Directory.Exists(Constants.FolderPath) && File.Exists(Constants.DataPath);
        public bool ModelExist => Directory.Exists(Constants.FolderPath) && File.Exists(Constants.ModelPath);

        #endregion

        #region private

        private static MachineLearningService _instance;

        private PredictionModel<WindowData, RegionPrediction> _model;
        private readonly TaskCompletionSource<bool> _ready;
        private readonly SemaphoreSlim _semaphore;

        #endregion

        /// <summary>
        /// Constructor (Singleton so private)
        /// </summary>
        private MachineLearningService()
        {
            Logger.Instance.Info("ML : Init machine learning service");

            _semaphore = new SemaphoreSlim(1);
            _ready = new TaskCompletionSource<bool>();

            if (!Directory.Exists(Constants.FolderPath))
                Directory.CreateDirectory(Constants.FolderPath);
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
                    try
                    {
                        _model = await PredictionModel.ReadAsync<WindowData, RegionPrediction>(Constants.ModelPath);
                    }
                    catch (Exception)
                    {
                        File.Delete(Constants.ModelPath);
                        await Train();
                    }
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
                Logger.Instance.Info("ML : Training model");
                CheckDataFile();

                var pipeline = new LearningPipeline {
                        new TextLoader(Constants.DataPath).CreateFrom<WindowData>(separator: ','),
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

                await _model.WriteAsync(Constants.ModelPath);

                Logger.Instance.Info("ML : Model trained");
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

            Logger.Instance.Info("ML : Predicted : " + prediction + " From " + windowData);

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

            Logger.Instance.Info("ML : Add new data");

            var newLine =
                $"{Environment.NewLine}" +
                $"{region}," +
                $"{program}," +
                $"{windowTitle}," +
                $"{windowTop}," +
                $"{windowLeft}," +
                $"{windowHeight}," +
                $"{windowWidth}";

            if (!File.Exists(Constants.DataPath))
                File.WriteAllText(Constants.DataPath, "");

            File.AppendAllText(Constants.DataPath, newLine);
        }

        /// <summary>
        /// Check if data file exist, create and add data if not
        /// </summary>
        private void CheckDataFile()
        {
            if (!DataExist)
            {
                Logger.Instance.Warn("ML : " + DataExist + " doesn't exist");
                File.WriteAllText(Constants.DataPath, "");
            }

            var lineCount = File.ReadLines(Constants.DataPath).Count();
            if (lineCount >= 3)
                return;
            Logger.Instance.Warn("ML : No or not enough data");
            AddData("0 0 100 100", "PiP", "PiP", 0, 0, 100, 100);
            AddData("0 0 100 100", "Tool", "Tool", 0, 0, 200, 200);
            AddData("100 100 200 200", "Test", "Test", 0, 0, 300, 300);
        }

    }
}
