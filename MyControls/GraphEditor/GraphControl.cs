using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using CFL_1.CFLGraphics.MyControls.GraphEditor;
using Telerik.Windows.Controls.Diagrams.Extensions;
using Telerik.Windows.Controls;
using MSTD.ShBase;
using ObjectEdit;
using ShLayouts;

namespace CFL_1.CFLGraphics.GraphEditor
{
    public class GraphControl : VBoxLayout
    {
        public GraphControl(GraphTypesDescription _descriptions)
        {
            init();
            TypesDescription = _descriptions;
        }

        public GraphTypesDescription TypesDescription
        {
            get
            {
                return __typesDescription;
            }
            set
            {
                __typesDescription = value;
                SetShapesOnLayoutShapes();
            }
        }

        public GraphProject Project
        {
            get { return __graph.Project; }
            set 
            { 
                __graph.Project = value; 
                CurrentShape = null;
                __navigationPane.RefreshThumbnail();
            }
        }

        #region layout shapes

        public GraphShape CurrentShape
        {
            get { return __currentShape; }
            set
            {
                __currentShape = value;
                
                SetShapesOnLayoutShapes() ;
                SetShapesOnLayoutReusables();
                EditCurrent();
            }
        }

        private void SetShapesOnLayoutShapes()
        {
            List<string> _types = null;

            if(__graph.Shapes.Count == 0)
                _types = new List<string>() { __typesDescription.RootType().TypeName };
            else
                _types = __currentShape?.TypeInfo.CanAcceptList();

            __layoutShapes.Clear();
            __layoutShapes.MaxWidth = 150;

            if (_types == null)
                return;

            foreach (string _type in _types)
            {
                GraphShape _shape = __typesDescription.ShapeFactory(_type);
                _shape.Height = _shape.TypeInfo.Height + 10;
                if(_shape.TypeInfo.Width + 10 > __layoutShapes.MaxWidth)
                    __layoutShapes.MaxWidth = _shape.TypeInfo.Width + 10;
                __layoutShapes.Add(_shape);
            }
        }

        private void SetShapesOnLayoutReusables()
        {
            __layoutReusableShapes.Clear();
            __layoutReusableShapes.MaxWidth = 150;
            foreach(ShapeTypeInfo _type in ReusableShapesTypeInfos())
            {
                GraphShape _shape = new GraphShape(_type);
                _shape.Height = _type.Height + 10;
                if(_type.Width + 10 > __layoutReusableShapes.MaxWidth)
                    __layoutReusableShapes.MaxWidth = _type.Width + 10;
                __layoutReusableShapes.Add(_shape);
            }
        }

        private List<ShapeTypeInfo> ReusableShapesTypeInfos()
        {
            List<ShapeTypeInfo> _reusables = new List<ShapeTypeInfo>();
            if(CurrentShape == null)
                return _reusables;
            foreach(GraphShape _shape in __graph.Shapes)
            {
                if(_shape.TypeInfo.IsReusable && _shape != CurrentShape
                && CurrentShape.TypeInfo.CanAccept(_shape.TypeInfo.TypeName)
                && CurrentShape.TypeInfo.AcceptedShapes.Contains(_shape.TypeInfo) == false)
                    _reusables.Add(_shape.TypeInfo);
            }
            return _reusables;
        }

        private GraphShape ShapeUnderMouse()
        {
            foreach(object _o in __layoutShapes.Children)
            {
                if (_o is GraphShape _shape && _shape.IsMouseOver)
                    return _shape;
            }

            foreach(object _o in __layoutReusableShapes.Children)
            {
                if (_o is GraphShape _shape && _shape.IsMouseOver)
                    return _shape;
            }

            return null;
        }

        #endregion layout shapes

        #region drag and drop

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            __oldMousePos = e.GetPosition(null);
            ToDragAndDrop = ShapeUnderMouse();
            if(ToDragAndDrop != null)
            {
                GraphShape _new = __typesDescription.ShapeFactory(ToDragAndDrop.TypeInfo.TypeName);
                if(ToDragAndDrop.TypeInfo.IsOnGraph == true)
                {
                    _new.TypeInfo.EditableComponents.Clear();
                    foreach(Base _component in ToDragAndDrop.TypeInfo.EditableComponents)
                    {
                        _new.TypeInfo.AddEditableComponent(_component);
                    }
                }
                ToDragAndDrop = _new;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (ToDragAndDrop == null)
                return;
            Vector _diff = __oldMousePos - e.GetPosition(null);

            if (e.LeftButton == MouseButtonState.Pressed
            && (Math.Abs(_diff.X) > SystemParameters.MinimumHorizontalDragDistance
                || Math.Abs(_diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                DataObject _dragData = new DataObject("shape", ToDragAndDrop);
                DragDrop.DoDragDrop(__layoutShapes, _dragData, DragDropEffects.Move);
            }
        }
        Point __oldMousePos;
        public GraphShape ToDragAndDrop
        {
            get;
            private set;
        }

        private void OnDropOnGraph(object sender, System.Windows.DragEventArgs e)
        {
            CurrentShape = (GraphShape)__graph.SelectedItem;
        }

        #endregion drag and drop

        #region edition

        private void EditCurrent()
        {
            __layoutEdition.Items.Clear();

            if(CurrentShape == null)
            {
                __layoutEdition.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Editable components
                foreach(Base _component in CurrentShape.TypeInfo.EditableComponents)
                {
                    ObjectEditControl _dataform = new ObjectEditControl();
                    _dataform.Object = _component;
                    _dataform.ShowHeader = true;
                    __layoutEdition.Items.Add(_dataform);
                }
                __layoutEdition.Visibility = Visibility.Visible;

                foreach(GraphShape _shape in __graph.Shapes)
                {
                    if(_shape != CurrentShape)
                    {
                        Base _claimedComponent = CurrentShape.TypeInfo.GetClaimedComponentBy(_shape.TypeInfo);
                        _shape.EditComponent(_claimedComponent);
                    }
                }
            }
        }

        #endregion edition

        private void init()
        {
            __graph  = new Graph(this) { Background = Brushes.White, IsBackgroundSurfaceVisible = true };
            __navigationPane = new RadDiagramNavigationPane();
            __navigationPane.VerticalAlignment = VerticalAlignment.Stretch;
            __navigationPane.IsExpanded = true;
            __navigationPane.Diagram = __graph;

            Add(__layoutMenu);
            __layoutMenu.Background = Brushes.LightGray;

            Add(__layoutMain);

            __layoutShapes.HorizontalAlignment = HorizontalAlignment.Left;
            __layoutReusableShapes.HorizontalAlignment = HorizontalAlignment.Left;
            __layoutMain.Items.Add(__layoutShapes);
            __layoutMain.Items.Add(__layoutReusableShapes);
            __layoutMain.Items.Add(__graph);

            __layoutEdition.VerticalAlignment = VerticalAlignment.Stretch;

            __layoutRight.MaxWidth = 275;
            __layoutRight.Orientation = System.Windows.Controls.Orientation.Vertical;
            __layoutRight.HorizontalAlignment = HorizontalAlignment.Right;
            __layoutMain.Items.Add(__layoutRight);
            __layoutRight.Items.Add(__navigationPane);
            __layoutRight.Items.Add(__layoutEdition);
            
            __graph.AllowDrop = true;
            __graph.IsConnectorsManipulationEnabled = false;
            __graph.SelectionChanged += OnGraphSelectionChanged;
            __graph.Drop += OnDropOnGraph;
        }

        private void OnGraphSelectionChanged(object sender, RoutedEventArgs e)
        {
            CurrentShape = __graph.SelectedItem as GraphShape;
        }

        GraphShape __currentShape;

        RadLayoutControl __layoutMain = new RadLayoutControl();
        HBoxLayout __layoutMenu = new HBoxLayout() { MaxHeight = 30 };
        VBoxLayout __layoutShapes = new VBoxLayout() { MaxWidth = 150 , Background = Brushes.Moccasin };
        VBoxLayout __layoutReusableShapes = new VBoxLayout() { MaxWidth = 150 , Background = Brushes.OldLace };
        RadLayoutControl __layoutRight = new RadLayoutControl();
        RadLayoutControl __layoutEdition = new RadLayoutControl()
        { Orientation = System.Windows.Controls.Orientation.Vertical };

        private GraphTypesDescription __typesDescription;
        private Graph __graph;
        private RadDiagramNavigationPane __navigationPane;
    }
}
