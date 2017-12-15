using System.ComponentModel.DataAnnotations.Schema;
using MSTD.ShBase;

namespace CFL_1.CFLGraphics.MyControls.GraphEditor
{
    [NotMapped]
    public class ClassClaimer : Base
    {
        public ClassClaimer() { }
        public ClassClaimer(string _fromtype, string _claimedtype)
        {
            fromType = _fromtype;
            claimedType = _claimedtype;
        }
        public string fromType { get ; set ; }
        public string claimedType { get ; set ; }
    }
}
