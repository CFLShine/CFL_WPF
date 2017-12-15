
using CFL_1.CFLGraphics.MyControls.GraphEditor;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace CFL_1.CFLGraphics.GraphEditor
{
    public class Graph : RadDiagram
    {
        public Graph(GraphControl _graphControl)
        {
            Background = Brushes.White;
            graphControl = _graphControl;
           
        }

        GraphControl graphControl { get ; set ; }

        public GraphProject Project
        {
            get
            {
                return __project;
            }

            set
            {
                __project = value;
                Clear();
                
                if(__project != null)
                {
                    foreach(ShapeTypeInfo _shapeInfo in __project.ShapeInfos)
                    {
                        GraphShape _shape = new GraphShape(_shapeInfo);
                        AddShapeOnGraph(_shape, _shapeInfo.LeftPosition, _shapeInfo.TopPosition);
                    }

                    foreach(GraphShape _shape in GetShapes())
                    {
                        foreach(ShapeTypeInfo _typeInfo in _shape.TypeInfo.AcceptedShapes)
                        {
                            Connect(_shape, Shape(_typeInfo));
                        }
                    }
                }
            }
        }
        private GraphProject __project;

        double abs(double _v)
        {
            if(_v < 0)
                return -_v;
            else return _v;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if(SelectionCount > 1)
                return;

            base.OnDrop(e);
            if (e.Data.GetData("shape") is GraphShape _shape)
            {
                Point _p = e.GetPosition(this);
                
                GraphShape _newShape = graphControl.ToDragAndDrop;
                Point _point = AddShapeOnGraph(_newShape, _p.X, _p.Y);
                _shape.TypeInfo.LeftPosition = _p.X;
                _shape.TypeInfo.TopPosition = _p.Y;
                Project.AddShape(_shape.TypeInfo);

                if(SelectedItem != null && SelectedItem is GraphShape _current)
                {
                    Connect(_current, _newShape);
                    _current.LinkTo(_newShape);
                }
                SelectedItem = _newShape;
            }
        }

        private void Connect(GraphShape _source, GraphShape _target)
        {
            RadDiagramConnection _connection = new RadDiagramConnection();
            _connection.AllowDelete = false;
            _connection.AllowCut = false;
            _connection.AllowCopy = false;
            _connection.ConnectionType = Telerik.Windows.Diagrams.Core.ConnectionType.Bezier;
            _connection.Source = _source;
            _connection.Target = _target;
            AddConnection(_connection);
        }

        /// <summary>
        /// Ajoute une GraphShape sur le graph à la position indiquée par _left, _top.
        /// </summary>
        public Point AddShapeOnGraph(GraphShape _shape, double _left, double _top)
        {
            double _zoom = Zoom ;
                
            _left = (_left + Viewport.Left);
            _top = (_top + Viewport.Top);
            AddShape(_shape, new Point(_left, _top));
            _shape.TypeInfo.IsOnGraph = true;
            return new Point(_left, _top);
        }

        protected override void OnDeleteCommandExecutedOverride(object sender, ExecutedRoutedEventArgs e)
        {
            GraphShape _selected = SelectedItem as GraphShape;
            RemoveShapeFromGraph(_selected);

            base.OnDeleteCommandExecutedOverride(sender, e);
        }

        private List<GraphShape> GetShapes()
        {
            List<GraphShape> _shapes = new List<GraphShape>();
            foreach(var _item in Shapes)
            {
                if (_item is GraphShape)
                    _shapes.Add((GraphShape)_item);
            }
            return _shapes;
        }

        private GraphShape Shape(ShapeTypeInfo _typeInfo)
        {
            foreach(GraphShape _shape in GetShapes())
            {
                if (_shape.TypeInfo == _typeInfo)
                    return _shape;
            }
            return null;
        }

        #region Remove

        /// <summary>
        /// Retire la ShapeTypeInfo de _shape du GraphProject.
        /// Suprime physiquement la shape du graph, ainsi que ses connections entrantes (lignes),
        /// appèle _shape.LinkedBy.TypeInfo.Remove(_shape.TypeInfo),
        /// puis suprime en cascade les shapes qui lui sont liées en connection sortantes.
        /// </summary>
        private void RemoveShapeFromGraph(GraphShape _shape)
        {
            if(_shape != null)
            {
                Project.ShapeInfos.Remove(_shape.TypeInfo);

                List<RadDiagramConnection> _connectionsToRemove = new List<RadDiagramConnection>() ;
                foreach(RadDiagramConnection _connection in _shape.IncomingLinks)
                    _connectionsToRemove.Add(_connection);
                foreach(RadDiagramConnection _connection in _connectionsToRemove)
                    RemoveConnection(_connection);

                SelectedItem = _shape;
                Delete();

                // 
                if(_shape.LinkedBy != null)
                    _shape.LinkedBy.Remove(_shape.TypeInfo);
                //

                // supression en cascade des shapes liée en connection sortante
                ShapeTypeInfo[] _accepteds = _shape.TypeInfo.AcceptedShapes.ToArray();
                foreach(ShapeTypeInfo _typeInfo in _accepteds)
                {
                    RemoveShapeFromGraph(Shape(_typeInfo));
                }
            }
            graphControl.CurrentShape = null;
        }
        

        #endregion Remove

        private int SelectionCount
        {
            get
            {
                int _i = 0;
                foreach(var _shape in SelectedItems)
                {
                    if(_shape is GraphShape)
                        ++_i;
                }
                return _i;
            }
        }

    }
}
