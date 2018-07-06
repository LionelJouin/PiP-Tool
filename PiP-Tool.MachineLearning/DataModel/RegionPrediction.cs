using Microsoft.ML.Runtime.Api;

namespace PiP_Tool.MachineLearning.DataModel
{
    public class RegionPrediction
    {

        [ColumnName("PredictedLabel")] public float Width;

        [ColumnName("Score")] public float[] Score;

    }
}