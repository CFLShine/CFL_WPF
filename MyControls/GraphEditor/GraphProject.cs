using System.Collections.Generic;
using MSTD.ShBase;

namespace CFL_1.CFLGraphics.MyControls.GraphEditor
{
    public class GraphProject : Base
    {
        public string ProjectName { get ; set ; }

        public List<ShapeTypeInfo> ShapeInfos{ get; set; } = new List<ShapeTypeInfo>();

        public void AddShape(ShapeTypeInfo _shape)
        {
            ShapeInfos.Add(_shape);
        }
    }
}
