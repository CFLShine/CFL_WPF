
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ShLayouts
{
    public class BoxLayoutModel
    {
        public BoxLayoutModel(BoxLayout layout)
        {
            Layout = layout;
        }

        #region public methods

        public bool IsPerpendicularMinimized
        {
            get => __isPerpendicularMinimized;
            set
            {
                __isPerpendicularMinimized = value;
                SetMinMaxLayoutSizes();
            }
        }

        public void Add(FrameworkElement e)
        {
            Insert(Count, e);
        }

        public void Insert(int index, FrameworkElement e)
        {
            switch (Layout.Orientation)
            {
                case Orientation.Horizontal:
                    InsertHorizontal(index, e);
                    break;
                case Orientation.Vertical:
                    InsertVertical(index, e);
                    break;
            }
            
            if(e is Spacer _spacer && double.IsPositiveInfinity(_spacer.MaxSpace))
                ++ SpacersInfinite; 
        }

        public void Remove(FrameworkElement e)
        {
            int _index = IndexOf(e);
            if(RemoveAt(_index))
            {
                if(e is Spacer _spacer && double.IsPositiveInfinity(_spacer.MaxSpace))
                    -- SpacersInfinite;
                Update();
            }
        }

        public void Clear()
        {
            Layout.Children.Clear();
            //UpDate();
        }

        public int Count
        {
            get
            {
                return Layout.Children.Count;
            }
        }

        public int IndexOf(FrameworkElement e)
        {
            return Layout.Children.IndexOf(e);
        }

        #endregion public methods

        double TotalOrientedElementsMinSize   { get; set; } = 0;

        double GreatestPerpendicularElementMinSize { get; set; } = 0;
        double GreatestPerpendicularElementMaxSize { get; set; } = 0;

        private BoxLayout Layout 
        { 
            get => __layout; 
            set
            {
                __layout = value;
            }
        }

        private RowDefinitionCollection Column
        {
            get => Layout?.RowDefinitions;
        }
        private ColumnDefinitionCollection Row
        {
            get => Layout?.ColumnDefinitions;
        }
        
        private IEnumerable<CellInfo> Cells()
        {
            switch (Layout.Orientation)
            {
                case Orientation.Horizontal:
                    foreach(ColumnDefinition _column in Row)
                    {
                        yield return ((HCell)_column).CellInfo;
                    }
                    break;
                case Orientation.Vertical:
                    foreach(RowDefinition _row in Column)
                    {
                        yield return ((VCell)_row).CellInfo;
                    }
                    break;
            }
        }

        private CellInfo VCellInfoAt(int index)
        {
            if(index < 0 || index >= Column.Count)
                return null;
            return ((VCell)Column[index]).CellInfo;
        }

        private CellInfo HCellInfoAt(int index)
        {
            if(index < 0 || index >= Row.Count)
                return null;
            return ((HCell)Row[index]).CellInfo;
        }

        private VCell InsertVertical(int index, FrameworkElement e)
        {
            VCellInfo _cellInfo = new VCellInfo(this, e);
            VCell _vcell = new VCell(_cellInfo);

            Layout.RowDefinitions.Insert(index, _vcell);
            Grid.SetColumn(e, 0);
            Grid.SetRow(e,index);
            Layout.Children.Insert(index, e);

            Update();

            return _vcell;
        }

        private HCell InsertHorizontal(int index, FrameworkElement e)
        {
            HCellInfo _cellInfo = new HCellInfo(this, e);
            HCell _hcell = new HCell(_cellInfo);

            Layout.ColumnDefinitions.Insert(index, _hcell);
            Grid.SetColumn(e, index);
            Grid.SetRow(e, 0);
            Layout.Children.Insert(index, e);

            Update();

            return _hcell;
        }

        private bool RemoveAt(int index)
        {
            if(index < 0 || index >= Layout.Children.Count)
                return false;
            Layout.Children.RemoveAt(index);

            CellInfo _cellInfo = null;

            switch (Layout.Orientation)
            {
                case Orientation.Horizontal:
                    _cellInfo =((HCell)Row[index]).CellInfo;
                    Row.RemoveAt(index);
                    break;
                case Orientation.Vertical:
                    _cellInfo = ((VCell)Column[index]).CellInfo;
                    Column.RemoveAt(index);
                    break;
            }

            return true;
        }

        private int SpacersInfinite { get; set; }

        public bool HasSpacersInfinite
        { 
            get => SpacersInfinite > 0; 
        }

        #region Update

        public void Update()
        {
            UpdateSizes();
            
            SetMinMaxLayoutSizes();
           
            if(Layout.Parent is BoxLayout _layout)
                _layout.Model.Update();
        }

        private void UpdateSizes()
        {
            TotalOrientedElementsMinSize = 0;
            GreatestPerpendicularElementMinSize = 0;
            GreatestPerpendicularElementMaxSize = 0;

            foreach(CellInfo _cellInfo in Cells())
            {
                _cellInfo.UpdateOrientedSizes();

                TotalOrientedElementsMinSize += _cellInfo.MinOrientedSize;
                
                double _perpendicularElementMinSize = _cellInfo.PerpendicularElementMinSize;
                double _perpendicularElementMaxSize = _cellInfo.PerpendicularElementMaxSize;

                if(MSTD.DoubleHelper.Smaller(GreatestPerpendicularElementMinSize, _perpendicularElementMinSize))
                    GreatestPerpendicularElementMinSize = _perpendicularElementMinSize;

                if(
                     !double.IsPositiveInfinity(_perpendicularElementMaxSize)
                  && MSTD.DoubleHelper.Smaller(GreatestPerpendicularElementMaxSize, _perpendicularElementMaxSize)
                  )
                    GreatestPerpendicularElementMaxSize = _perpendicularElementMaxSize;
            }
        }

        private void SetMinMaxLayoutSizes()
        {
            if(Layout.Orientation == Orientation.Horizontal)
            {
                Layout.MinWidth = TotalOrientedElementsMinSize;
                Layout.MinHeight = GreatestPerpendicularElementMinSize;

                if(IsPerpendicularMinimized)
                {
                    if(GreatestPerpendicularElementMaxSize != 0)
                        Layout.MaxHeight = GreatestPerpendicularElementMaxSize;
                    else
                        Layout.MaxHeight = GreatestPerpendicularElementMinSize;
                }
                else
                    Layout.MaxHeight = double.PositiveInfinity;
            }
            else // Orientation.Vertical
            {
                Layout.MinHeight = TotalOrientedElementsMinSize;
                Layout.MinWidth = GreatestPerpendicularElementMinSize;

                if(IsPerpendicularMinimized)
                {
                    if(GreatestPerpendicularElementMaxSize != 0)
                        Layout.MaxWidth = GreatestPerpendicularElementMaxSize;
                    else
                        Layout.MaxWidth = GreatestPerpendicularElementMinSize;
                }
                else
                    Layout.MaxWidth = double.PositiveInfinity;
            }
        }

        #endregion Update

        private BoxLayout __layout = null;
        private bool __isPerpendicularMinimized = false;
    }
    
}
