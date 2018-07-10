using Microsoft.ML.Runtime.Api;

namespace PiP_Tool.MachineLearning.DataModel
{
    public class RegionPrediction
    {

        #region public

        [ColumnName("PredictedLabel")]
        public string Region;

        [ColumnName("Score")]
        public float[] Score;

        [ColumnName("WindowTop")]
        public float WindowTop;

        [ColumnName("WindowLeft")]
        public float WindowLeft;

        [ColumnName("WindowHeight")]
        public float WindowHeight;

        [ColumnName("WindowWidth")]
        public float WindowWidth;

        [NoColumn]
        public int Top;
        [NoColumn]
        public int Left;
        [NoColumn]
        public int Height;
        [NoColumn]
        public int Width;

        #endregion

        #region private

        private const char Delimiter = ' ';

        #endregion

        public void Predicted()
        {
            if (Region.Split(Delimiter).Length != 4)
                return;

            Top = int.Parse(Region.Split(Delimiter)[0]);
            Left = int.Parse(Region.Split(Delimiter)[1]);
            Height = int.Parse(Region.Split(Delimiter)[2]);
            Width = int.Parse(Region.Split(Delimiter)[3]);

            if (Width + Left > WindowWidth)
                Width = (int)(WindowWidth - Left);

            if (Height + Top > WindowHeight)
                Height = (int)(WindowHeight - Top);
        }

    }
}