using Microsoft.ML.Runtime.Api;

namespace PiP_Tool.MachineLearning.DataModel
{
    public class WindowData
    {

        [Column("0", "Label")]
        public string Region;

        [Column("1", "Program")]
        public string Program;

        [Column("2", "WindowTitle")]
        public string WindowTitle;

        [Column("3", "WindowTop")]
        public float WindowTop;

        [Column("4", "WindowLeft")]
        public float WindowLeft;

        [Column("5", "WindowHeight")]
        public float WindowHeight;

        [Column("6", "WindowWidth")]
        public float WindowWidth;

    }
}

