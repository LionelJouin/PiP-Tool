using System.Linq;
using Microsoft.ML.Runtime.Api;
using Constants = PiP_Tool.Shared.Constants;

namespace PiP_Tool.MachineLearning.DataModel
{
    public class RegionPrediction
    {

        #region public

        /// <summary>
        /// <see cref="WindowData.Region"/>  
        /// </summary>
        [ColumnName("PredictedLabel")]
        public string Region;

        /// <summary>
        /// Score of the prediction
        /// </summary>
        [ColumnName("Score")]
        public float[] Score;

        /// <summary>
        /// <see cref="WindowData.WindowTop"/>  
        /// </summary>
        [ColumnName("WindowTop")]
        public float WindowTop;

        /// <summary>
        /// <see cref="WindowData.WindowLeft"/>  
        /// </summary>
        [ColumnName("WindowLeft")]
        public float WindowLeft;

        /// <summary>
        /// <see cref="WindowData.WindowHeight"/>  
        /// </summary>
        [ColumnName("WindowHeight")]
        public float WindowHeight;

        /// <summary>
        /// <see cref="WindowData.WindowWidth"/>  
        /// </summary>
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

        public float PredictionScore => Score.Max();

        #endregion

        #region private

        private const char Delimiter = ' ';

        #endregion

        /// <summary>
        /// Adjust Height and Width against the Window Height and Width 
        /// </summary>
        public void Predicted()
        {
            if (Region.Split(Delimiter).Length != 4)
                return;

            Top = int.Parse(Region.Split(Delimiter)[0]);
            Left = int.Parse(Region.Split(Delimiter)[1]);
            Height = int.Parse(Region.Split(Delimiter)[2]);
            Width = int.Parse(Region.Split(Delimiter)[3]);
        }
        
        public override string ToString()
        {
            return "Top : " + Top + ", " +
                   "Left : " + Left + ", " +
                   "Height : " + Height + ", " +
                   "Width : " + Width;
        }

    }
}