using System.ComponentModel.DataAnnotations.Schema;
using MSTD.ShBase;

namespace CFL_1.CFLGraphics.MyControls.GraphEditor
{
    [NotMapped]
    public class GraphRule : Base
    {
        public GraphRule() { }
        public GraphRule(string _acceptableType, int _howmany)
        {
            acceptableType = _acceptableType;
            canaccept = _howmany;
        }

        public string acceptableType { get ; set ; } = "";
        public int canaccept { get ; set ; } = 0;
    }
}
