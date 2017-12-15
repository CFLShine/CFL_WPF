using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ShLayouts
{
    public enum ELEMENTTYPE
    {
        LAYOUT,
        SPACER,
        FIXEDSPACER,
        GLUE,
        ANY
    }

    public abstract class CellInfo
    {
        public CellInfo(BoxLayoutModel model, FrameworkElement e)
        {
            Element = e;
            Model = model;
        }

        public DefinitionBase Cell { get; set; }

        public BoxLayoutModel Model { get; private set; }

        public ELEMENTTYPE ELEMENTTYPE { get; private set; }

        public FrameworkElement Element
        {
            get => __element;
            set
            {
                __element = value??throw new ArgumentNullException("value");

                if(__element is BoxLayout)
                    ELEMENTTYPE = ELEMENTTYPE.LAYOUT;
                else
                if(__element is Spacer)
                    ELEMENTTYPE = ELEMENTTYPE.SPACER;
                else
                if(__element is FixedSpacer)
                    ELEMENTTYPE = ELEMENTTYPE.FIXEDSPACER;
                else
                    ELEMENTTYPE = ELEMENTTYPE.ANY;
            }
        }

        public abstract double MinOrientedSize { get; set; }
        public abstract double MaxOrientedSize { get; set; }

        public abstract double PerpendicularElementMinSize { get;}
        public abstract double PerpendicularElementMaxSize { get;}

        public abstract void UpdateOrientedSizes();
        
        private FrameworkElement __element = null;
    }

    public class VCellInfo : CellInfo
    {
        public VCellInfo(BoxLayoutModel model, FrameworkElement e)
            : base(model, e){ }

        public override double MinOrientedSize
        {
            get => ((VCell)Cell).MinHeight;
            set => ((VCell)Cell).MinHeight = value;
        }
        public override double MaxOrientedSize
        {
            get => ((VCell)Cell).MaxHeight;
            set => ((VCell)Cell).MaxHeight = value;
        }
        public override double PerpendicularElementMinSize
        {
            get
            {
                if(Element is Spacer)
                    return 0;
                return MesureHelper.MinWidth(Element);
            }
        }
        public override double PerpendicularElementMaxSize
        {
            get
            {
                if(Element is Spacer)
                    return 0;
                return MesureHelper.MaxWidth(Element);
            }
        }

        public override void UpdateOrientedSizes()
        {
            if(Element is Spacer _spacer)
            {
                MinOrientedSize = _spacer.MinSpace;
                MaxOrientedSize = _spacer.MaxSpace;
            }
            else
            {
                MinOrientedSize = MesureHelper.MinHeight(Element);
                MaxOrientedSize = MesureHelper.MaxHeight(Element);
            }
        }
    }

    public class HCellInfo : CellInfo
    {
        public HCellInfo(BoxLayoutModel model, FrameworkElement e)
            : base(model, e){ }

        public override double MinOrientedSize
        {
            get => ((HCell)Cell).MinWidth;
            set => ((HCell)Cell).MinWidth = value;
        }
        public override double MaxOrientedSize
        {
            get => ((HCell)Cell).MaxWidth;
            set => ((HCell)Cell).MaxWidth = value;
        }
        public override double PerpendicularElementMinSize
        {
            get
            {
                if(Element is Spacer)
                    return 0;
                return MesureHelper.MinHeight(Element);
            }
        }
        public override double PerpendicularElementMaxSize
        {
            get
            {
                if(Element is Spacer)
                    return 0;
                return MesureHelper.MaxHeight(Element);
            }
        }

        public override void UpdateOrientedSizes()
        {
            if(Element is Spacer _spacer)
            {
                MinOrientedSize = _spacer.MinSpace;
                MaxOrientedSize = _spacer.MaxSpace;
            }
            else
            {
                MinOrientedSize = MesureHelper.MinWidth(Element);
                MaxOrientedSize = MesureHelper.MaxWidth(Element);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    public class HCell : ColumnDefinition
    {
        public HCell(CellInfo cellInfo)
        {
            CellInfo = cellInfo;
            CellInfo.Cell = this;
        }

        public CellInfo CellInfo { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VCell : RowDefinition
    {
        public VCell(CellInfo cellInfo)
        {
            CellInfo = cellInfo;
            CellInfo.Cell = this;
        }

        public CellInfo CellInfo { get; private set; }
    }
}
