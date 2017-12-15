using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CFL_1.CFLGraphics.MyControls.GraphEditor;
using MSTD.ShBase;
using ObjectEdit;
using Telerik.Windows.Controls;

namespace CFL_1.CFLGraphics.GraphEditor
{
    public class GraphShape : RadDiagramShape
    {
        public GraphShape(ShapeTypeInfo _typeInfo)
        {
            __shapeTypeInfo = _typeInfo;
            BorderBrush = Brushes.Black;
            initContent();
            Update();
            IsManipulationAdornerVisible = false;
        }

        public ShapeTypeInfo TypeInfo
        { 
            get
            { return __shapeTypeInfo ; }
        }

        public Graph Graph
        {
            get
            {
                return Diagram as Graph; 
            }
        }

        public void Update()
        {
            if(TypeInfo != null)
            {
                __label.MinHeight = TypeInfo.Height;
                __label.MinWidth = TypeInfo.Width;

                string _text = TypeInfo.Designation;
                __label.Content = _text;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            string _tooltip = TypeInfo.ToolTip;
            if(!string.IsNullOrWhiteSpace(_tooltip))
                ToolTip = _tooltip;
            else
                ToolTip = null;
        }

        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            if(IsSelected)
                BorderBrush = Brushes.Gold;
            else
                BorderBrush = Brushes.Black;
        }

        protected override void OnPositionChanged(Point oldPosition, Point newPosition)
        {
            base.OnPositionChanged(oldPosition, newPosition);
            TypeInfo.LeftPosition = Position.X;
            TypeInfo.TopPosition = Position.Y;
        }

        private ShapeTypeInfo __shapeTypeInfo;

        #region Content & Edit Component

        public void EditComponent(Base _component)
        {
            if(_component != null)
            {
                __objectEditControl.Object = _component;
                __objectEditControl.Visibility = Visibility.Visible;
                __layout.IsHitTestVisible = true;
            }
                
            else EndEditComponent();
        }

        public void EndEditComponent()
        {
            __objectEditControl.Clear();
            __objectEditControl.Visibility = Visibility.Collapsed;
            __layout.IsHitTestVisible = false;
        }

        private void initContent()
        {
            Content = __layout;
            __layout.Orientation = System.Windows.Controls.Orientation.Horizontal;
            __layout.Items.Add(__label);

            __objectEditControl = new ObjectEditControl();
            __layout.Items.Add(__objectEditControl);

            __label.IsHitTestVisible = false;
            __layout.IsHitTestVisible = false;
        }

        private RadLayoutControl __layout = new RadLayoutControl();
        
        private System.Windows.Controls.Label __label = new System.Windows.Controls.Label();
        private ObjectEditControl __objectEditControl;

        #endregion Content

        #region linkeds

        public ShapeTypeInfo LinkedBy 
        { 
            get
            {
                return TypeInfo.AcceptedBy;
            }
            set
            {
                TypeInfo.AcceptedBy = LinkedBy; 
            }
        }

        public void LinkTo(GraphShape _shape)
        {
            TypeInfo.Accept(_shape.TypeInfo);
            _shape.LinkedBy = this.TypeInfo;
        }

        #endregion linkeds

    }
}
