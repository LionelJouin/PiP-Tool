using Microsoft.ML.Runtime.Api;

namespace PiP_Tool.MachineLearning.DataModel
{
    public class WindowData
    {

        /// <summary>
        /// Labal: predicted region (format: "Top Left Height Width")
        /// </summary>
        [Column("0", "Label")]
        public string Region;

        /// <summary>
        /// Name of the program
        /// </summary>
        [Column("1", "Program")]
        public string Program;

        /// <summary>
        /// Title of the window
        /// </summary>
        [Column("2", "WindowTitle")]
        public string WindowTitle;

        /// <summary>
        /// Top position of the window
        /// </summary>
        [Column("3", "WindowTop")]
        public float WindowTop;

        /// <summary>
        /// Left position of the window
        /// </summary>
        [Column("4", "WindowLeft")]
        public float WindowLeft;

        /// <summary>
        /// Height of the window
        /// </summary>
        [Column("5", "WindowHeight")]
        public float WindowHeight;

        /// <summary>
        /// Width of the window
        /// </summary>
        [Column("6", "WindowWidth")]
        public float WindowWidth;
        
        public override string ToString()
        {
            return "WindowTitle : " + WindowTitle + ", " +
                   "WindowTop : " + WindowTop + ", " +
                   "WindowLeft : " + WindowLeft + ", " +
                   "WindowHeight : " + WindowHeight + ", " +
                   "WindowWidth : " + WindowWidth;
        }

    }
}

