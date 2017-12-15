using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CFL_1.CFLGraphics.PM_grid
{
    public class PM_GridTextBox : TextBox
    {
        public PM_GridTextBox(PM_GridCell _cell)
        { __cell = _cell; }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            __cell.value = Text;
            if (__cell.grid != null)
                __cell.grid.checkIfRowToAddOrRemove();
        }
        private PM_GridCell __cell;
    }

    public class PM_GridCheckBox : CheckBox
    {
        public PM_GridCheckBox(PM_GridCell _cell)
        { __cell = _cell; }

        protected override void OnClick()
        {
            base.OnClick();
            __cell.value = IsChecked;
        }

        PM_GridCell __cell;
    }

    public enum Pm_alignement
    {
        center,
        left,
        right,
        top,
        bottom
    }

    public enum PM_hAlignement
    {
        center,
        left,
        right
    }

    public enum PM_vAlignement
    {
        center,
        top,
        right
    }

    public enum PM_state
    {
        UNSET,
        TRUE,
        FALSE
    }

    public enum PM_GridCellType
    {
        HORIZONTALHEADER,
        VERTICALHEADER,
        CELLTEXTBOX,
        CELLCHECKBOX,
        CELLBUTTON
    }

    public class PM_GridCellStyle
    {
        public Image image;
        public Pm_alignement imageAlignment;
        public Color backColor;
        public Color foreColor;
        public string stringFormat;
        public HorizontalAlignment contentHorizontalAlignement;
        public VerticalAlignment contentVerticalAlignement;

        public PM_GridCellStyle()
        {
            foreColor = Colors.Black;
            backColor = Colors.White;
            stringFormat = "";
            contentHorizontalAlignement = HorizontalAlignment.Center;
            contentVerticalAlignement = VerticalAlignment.Center;
        }

        public PM_GridCellStyle(Color _backColor, Color _foreColor)
        {
            backColor = _backColor;
            foreColor = _foreColor;
        }

        public PM_GridCellStyle(Color _backColor, Color _foreColor, HorizontalAlignment _contentHAlignement, VerticalAlignment _contentVAlignement)
        {
            backColor = _backColor;
            foreColor = _foreColor;
            contentHorizontalAlignement = _contentHAlignement;
            contentVerticalAlignement = _contentVAlignement;
        }

        public PM_GridCellStyle(Color _backColor, Color _foreColor, HorizontalAlignment _contentHAlignement, VerticalAlignment _contentVAlignement, string _stringFormat)
        {
            backColor = _backColor;
            foreColor = _foreColor;
            contentHorizontalAlignement = _contentHAlignement;
            contentVerticalAlignement = _contentVAlignement;
            stringFormat = _stringFormat;
        }

    }

    public abstract class PM_GridCell
    {
        public PM_GridCell(PM_Grid _grid)
        {
            grid = _grid;
            readOnly = PM_state.UNSET;
        }

        public PM_Grid grid { get; set; }

        public abstract Control show();
        public abstract Control control();
        public PM_state readOnly { get; set; }

        public abstract void applyControl_value();
        public abstract void applyStyle(PM_GridCellStyle _style);
        public abstract void applyIsReadOnly(PM_state _state);

        public abstract void setVisibility(Visibility _visibility);

        public abstract PM_GridCellType type { get; }

        public PM_GridCellStyle style
        { get; set; }

        public object value
        { get { return __value; } set { __value = value; } }

        /// <summary>
        /// Sert à déterminer si une ligne doit être ajoutée lorsque autoAddRow == true.
        /// </summary>
        public abstract bool hasValue();

        public bool setOnGrid { get; set; }

        //protected:
        
        protected object __value;
        protected PM_GridCellStyle __style;
    }

    public abstract class PM_GridCell_typed<T> : PM_GridCell where T : Control
    {
        public PM_GridCell_typed()
        :base(null){ }

        public PM_GridCell_typed(PM_Grid _grid)
            : base(_grid) { }

        public override Control control() { return __control; }

        public override Control show()
        {
            if(__control == null)
                setControl();

            __control.Visibility = Visibility.Visible;
            return __control;
        }

        public override void applyStyle(PM_GridCellStyle _style)
        {
            // assert(_style != null)
            __control.Background = new SolidColorBrush(_style.backColor);
            __control.Foreground = new SolidColorBrush(_style.foreColor);
            __control.HorizontalContentAlignment = _style.contentHorizontalAlignement;
            __control.VerticalContentAlignment = _style.contentVerticalAlignement;
        }

        public override void setVisibility(Visibility _visibility)
        {
            if(__control != null)
                __control.Visibility = _visibility;
        }

        //protected:

        protected abstract void setControl();

        protected Control __control;

        protected bool __controlConnected;
    }

    public class PM_GridCellTextBox : PM_GridCell_typed<PM_GridTextBox>
    {
        public PM_GridCellTextBox(PM_Grid _grid)
            : base(_grid) { }

        public override PM_GridCellType type
        {
            get{ return PM_GridCellType.CELLTEXTBOX; }
        }

        public override void applyControl_value()
        {
            if (value == null)
                ((PM_GridTextBox)__control).Text = "";
            else
                ((PM_GridTextBox)__control).Text = value.ToString();
        }

        public override void applyIsReadOnly(PM_state _state)
        {
            ((PM_GridTextBox)__control).IsReadOnly = (_state == PM_state.TRUE);
        }

        public override bool hasValue()
        { return __control != null && !string.IsNullOrEmpty(((PM_GridTextBox)__control).Text); }

        protected override void setControl()
        {
            __control = new PM_GridTextBox(this);
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            value = ((PM_GridTextBox)__control).Text;
            grid.checkIfRowToAddOrRemove();
        }
    }

    public class PM_GridCellNumeric_int : PM_GridCellTextBox
    {
        public PM_GridCellNumeric_int(PM_Grid _grid)
            : base(_grid) { }

        public override PM_GridCellType type
        {
            get { return PM_GridCellType.CELLTEXTBOX; }
        }

        public override void applyControl_value()
        {
            if (value == null)
                ((PM_GridTextBox)__control).Text = "";
            else
                ((PM_GridTextBox)__control).Text = value.ToString();
        }
    }

    public class PM_GridCellNumeric_double : PM_GridCellTextBox
    {
        public PM_GridCellNumeric_double(PM_Grid _grid)
            : base(_grid) { }

        public override PM_GridCellType type
        {
            get { return PM_GridCellType.CELLTEXTBOX; }
        }

        public override void applyControl_value()
        {
            if (value == null)
                ((PM_GridTextBox)__control).Text = "";
            else
            {
                if (__style != null && !(string.IsNullOrWhiteSpace(__style.stringFormat)))
                    ((PM_GridTextBox)__control).Text = ((double)value).ToString(__style.stringFormat);
                else
                    ((PM_GridTextBox)__control).Text = ((double)value).ToString();
            }
        }
    }

    public class PM_GridCellCheckBox : PM_GridCell_typed<PM_GridCheckBox>
    {
        public PM_GridCellCheckBox(PM_Grid _grid)
            : base(_grid) { }

        public override PM_GridCellType type
        {
            get { return PM_GridCellType.CELLCHECKBOX; }
        }

        public override void applyControl_value()
        {
            ((PM_GridCheckBox)__control).IsChecked = (value == null) ? false : (bool)value;
        }

        /// <summary>
        /// PM_GridCell contient la propriété readOnly mais ne décide pas, à chaque affichage,
        /// d'appliquer son état readOly, ce rôle est laissé à la vue (PM_Grid).
        /// </summary>
        /// <param name="_state"></param>
        public override void applyIsReadOnly(PM_state _state)
        {
            //actual readOnly sera utilisé pour le prochain affichage.
            __actualyReadOnly = _state;
        }

        public override bool hasValue()
        {
            return value != null && (bool)value;
        }

        protected override void setControl()
        {
            __control = new PM_GridCheckBox(this);
        }

        private PM_state __actualyReadOnly = PM_state.UNSET;

    }

    public class PM_GridCellDate : PM_GridCellTextBox
    {
        public PM_GridCellDate(PM_Grid _grid)
            : base(_grid) { }

        public override PM_GridCellType type
        {
            get { return PM_GridCellType.CELLTEXTBOX; }
        }

        public override void applyControl_value()
        {
            if (value == null)
                ((PM_GridTextBox)__control).Text = "";
            else
            {
                if (__style != null && !(string.IsNullOrWhiteSpace(__style.stringFormat)))
                    ((PM_GridTextBox)__control).Text = (((DateTime)value).ToString(__style.stringFormat));
                else
                    ((PM_GridTextBox)__control).Text = ((DateTime)value).ToString("dd/MM/yyyy");
            }
        }
    }

    public class PM_GridCellButton : PM_GridCell_typed<Button>
    {
        public PM_GridCellButton(PM_Grid _grid)
            : base(_grid) { }

        public override PM_GridCellType type
        {
            get { return PM_GridCellType.CELLBUTTON; }
        }

        public override void applyControl_value()
        {
            if (value == null)
                ((Button)__control).Content = "";
            else
                ((Button)__control).Content = (string)value;
        }

        public override void applyIsReadOnly(PM_state _state)
        {}

        public override void applyStyle(PM_GridCellStyle _style)
        {
            base.applyStyle(_style);
        }

        public override bool hasValue()
        {
            return false;
        }

        protected override void setControl()
        {
            __control = new Button();
        }
    }

    public class PM_GridCellHHeader : PM_GridCellButton
    {
        public PM_GridCellHHeader()
            : base(null) { }

        public PM_GridCellHHeader(PM_Grid _grid)
            : base(_grid) { }

        public override PM_GridCellType type
        {
            get { return PM_GridCellType.HORIZONTALHEADER; }
        }
    }

    public class PM_GridCellVHeader : PM_GridCellButton
    {
        public PM_GridCellVHeader()
            : base(null) { }

        public PM_GridCellVHeader(PM_Grid _grid)
            : base(_grid) { }

        public override PM_GridCellType type
        {
            get { return PM_GridCellType.VERTICALHEADER; }
        }
    }

    
}
