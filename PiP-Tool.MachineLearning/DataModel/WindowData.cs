using Microsoft.ML.Runtime.Api;

namespace PiP_Tool.MachineLearning.DataModel
{
    public class WindowData
    {

        [Column("0", "Label")]
        public float Width;

        [Column("1", "Program")]
        public string Program;

        [Column("2", "WindowTitle")]
        public string WindowTitle;

    }
}

