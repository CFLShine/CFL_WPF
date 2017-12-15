using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Media;

namespace CFL_1.CFLGraphics.PM_grid
{   
    public enum PM_ColumnType
    {
        STRING,
        INT,
        DOUBLE,
        BOOL,
        BUTTON,
        DATE
    }

    public class PM_GridColumn
    {
        public PM_GridColumn(PM_ColumnType _type, double _width)
        {
            type = _type;
            width = _width;
            visibility = Visibility.Visible;
            readOnly = PM_state.UNSET;
        }

        public PM_ColumnType type;

        public PM_GridCellHHeader header;

        public double width
        { get; set; }

        public PM_GridCellStyle style { get; set; }

        public Visibility visibility { get; set; }

        public PM_state readOnly { get; set; }

        public PM_GridCell newCell(PM_Grid _grid)
        {
            PM_GridCell _cell;
            switch (type)
            {
                case PM_ColumnType.STRING:
                    _cell =  new PM_GridCellTextBox(_grid);
                    break;
                case PM_ColumnType.INT:
                    _cell = new PM_GridCellNumeric_int(_grid);
                    break;
                case PM_ColumnType.DOUBLE:
                    _cell = new PM_GridCellNumeric_double(_grid);
                    break;
                case PM_ColumnType.BOOL:
                    _cell = new PM_GridCellCheckBox(_grid);
                    break;
                case PM_ColumnType.BUTTON:
                    _cell = new PM_GridCellButton(_grid);
                    break;
                case PM_ColumnType.DATE:
                    _cell = new PM_GridCellDate(_grid);
                    break;
                default:
                    return null;
            }
            return _cell;
        }

    }

    public class PM_GridRow
    {
        public PM_GridRow(int _columns, double _height)
        {
            __cells = new PM_GridCell[_columns];
            height = _height;
            readOnly = PM_state.UNSET;
            visibility = Visibility.Visible;
        }

        public PM_Grid grid
        {
            get { return __grid; }
            set
            {
                __grid = value;
                for(int _i = 0; _i < __cells.Length; _i++)
                {
                    PM_GridCell _cell = cell(_i);
                    if (_cell != null)
                        _cell.grid = __grid;
                }
            }
        }

        public PM_GridCellVHeader header
        { get; set; }

        public double height
        { get; set; }

        public PM_GridCell cell(int _col)
        {
            Contract.Requires(_col >= 0 && _col < cells.Length);
            return __cells[_col];
        }

        public PM_GridCell[] cells { get { return __cells; } }

        public void setCell(PM_GridCell _cell, int _col)
        {
            Contract.Requires(_col >= 0 && _col < cells.Length);
            if (_cell != null)
                _cell.grid = grid;
            __cells[_col] = _cell;
        }

        public Visibility visibility { get; set; }

        public PM_GridCellStyle style { get; set; }

        public PM_state readOnly { get; set; }

        public bool hasValues()
        {
            for(int _i = 0; _i < __cells.Length; _i++)
            {
                PM_GridCell _cell = __cells[_i];
                if (_cell != null && _cell.hasValue())
                    return true;
            }
            return false;
        }

        // private:
        PM_GridCell[] __cells;
        PM_Grid __grid;
    }

    class RowCompareAsc_string : IComparer
    {
        public RowCompareAsc_string(int _column)
        {
            __column = _column;
        }

        public int Compare(object x, object y)
        {
            return string.Compare(value((PM_GridRow)x), value((PM_GridRow)y));
        }

        string value(PM_GridRow _row)
        {
            Contract.Requires(_row != null);
            if (__column >= 0 && __column <_row.cells.Length)
            {
                PM_GridCell _cell = _row.cell(__column);
                if (_cell != null)
                    return (string)_cell.value;
                else
                    return "";
            }
            else
                throw new Exception("__column out of range.");
        }

        private int __column;
    }

    class RowCompareDesc_string : IComparer
    {
        public RowCompareDesc_string(int _column)
        {
            __column = _column;
        }

        public int Compare(object x, object y)
        {
            return string.Compare(value((PM_GridRow)y), value((PM_GridRow)x));
        }

        string value(PM_GridRow _row)
        {
            Contract.Requires(_row != null);
            if (__column >= 0 && __column <_row.cells.Length)
            {
                PM_GridCell _cell = _row.cell(__column);
                if (_cell != null)
                    return (string)_cell.value;
                else
                    return "";
            }
            else
                throw new Exception("__column out of range.");
        }

        private int __column;
    }

    class RowCompareAsc_int : IComparer
    {
        public RowCompareAsc_int(int _column)
        {
            __column = _column;
        }

        public int Compare(object x, object y)
        {
            int _v1 = value((PM_GridRow)x);
            int _v2 = value((PM_GridRow)y);

            if (_v1 == _v2)
                return 0;
            if (_v1 > _v2)
                return 1;
            return -1;
        }

        int value(PM_GridRow _row)
        {
            return (int)((_row.cell(__column).value));
        }

        private int __column;
    }

    class RowCompareDesc_int : IComparer
    {
        public RowCompareDesc_int(int _column)
        {
            __column = _column;
        }

        public int Compare(object x, object y)
        {
            int _v1 = value((PM_GridRow)x);
            int _v2 = value((PM_GridRow)y);

            if (_v1 == _v2)
                return 0;
            if (_v1 < _v2)
                return 1;
            return -1;
        }

        int value(PM_GridRow _row)
        {
            return (int)((_row.cell(__column).value));
        }

        private int __column;
    }

    class RowCompareAsc_double : IComparer
    {
        public RowCompareAsc_double(int _column)
        {
            __column = _column;
        }

        public int Compare(object x, object y)
        {
            double _v1 = value((PM_GridRow)x);
            double _v2 = value((PM_GridRow)y);

            if (_v1 == _v2)
                return 0;
            if (_v1 > _v2)
                return 1;
            return -1;
        }

        double value(PM_GridRow _row)
        {
            return (double)((_row.cell(__column).value));
        }

        private int __column;
    }

    class RowCompareDesc_double : IComparer
    {
        public RowCompareDesc_double(int _column)
        {
            __column = _column;
        }

        public int Compare(object x, object y)
        {
            double _v1 = value((PM_GridRow)x);
            double _v2 = value((PM_GridRow)y);

            if (_v1 == _v2)
                return 0;
            if (_v1 < _v2)
                return 1;
            return -1;
        }

        double value(PM_GridRow _row)
        {
            return (double)((_row.cell(__column).value));
        }

        private int __column;
    }

    class RowCompareAsc_date : IComparer
    {
        public RowCompareAsc_date(int _column)
        {
            __column = _column;
        }

        public int Compare(object x, object y)
        {
            DateTime _v1 = value((PM_GridRow)x);
            DateTime _v2 = value((PM_GridRow)y);

            if (_v1 == _v2)
                return 0;
            if (_v1 > _v2)
                return 1;
            return -1;
        }

        DateTime value(PM_GridRow _row)
        {
            return (DateTime)((_row.cell(__column).value));
        }

        private int __column;
    }

    class RowCompareDesc_date : IComparer
    {
        public RowCompareDesc_date(int _column)
        {
            __column = _column;
        }

        public int Compare(object x, object y)
        {
            DateTime _v1 = value((PM_GridRow)x);
            DateTime _v2 = value((PM_GridRow)y);

            if (_v1 == _v2)
                return 0;
            if (_v1 < _v2)
                return 1;
            return -1;
        }

        DateTime value(PM_GridRow _row)
        {
            return (DateTime)((_row.cell(__column).value));
        }

        private int __column;
    }

    public class PM_GridModel
    {
        public PM_GridModel(PM_Grid _grid)
        {
            __grid = _grid;
            readOnly = PM_state.FALSE;
            style = new PM_GridCellStyle();
            style.backColor = Colors.LightGray;
            style.foreColor = Colors.Black;
            style.contentHorizontalAlignement = HorizontalAlignment.Center;
            style.contentVerticalAlignement = VerticalAlignment.Center;
        }

        public PM_Grid grid
        {
            get { return __grid; }
            set
            {
                __grid = value;
                for(int _i = 0; _i < rowCount; _i++)
                {
                    PM_GridRow _row = row(_i);
                    if (_row != null)
                        _row.grid = __grid;
                }
            }
        }

        public double defaultRowHeight
        {
            get
            {
                if (__defaultRowHeight == 0 && grid != null)
                    return grid.defaultRowHeight;
                return __defaultRowHeight;
            }
            set
            {
                __defaultRowHeight = value;
            }
        }

        /// <summary>
        /// réserve une capacité de n colonnes non instanciées.
        /// rowCount est réinitialisé à 0.
        /// Les cellules existantes sont suprimées.
        /// </summary>
        public int columnCount
        {
            get
            {
                if (__columns == null)
                    return 0;
                return __columns.Length;
            }
            set
            {
                __columns = new PM_GridColumn[value];
                rowCount = 0;
            }
        }

        /// <summary>
        /// get : retourne __rows.Length
        /// set : réserve une capacité de n lignes non instanciées.
        /// Les cellules existantes sont suprimées(pas les headers horizontaux).
        /// </summary>
        public int rowCount
        {
            get
            {
                if (__rows == null)
                    return 0;
                return __rows.Length;
            }
            set
            {
                __rows = new PM_GridRow[value];
                for (int _i = 0; _i < value; _i++)
                    setRow(_i, new PM_GridRow(columnCount, defaultRowHeight));
            }
        }

        /// <summary>
        /// Equivalent à rowCount = 0;
        /// </summary>
        public void clearRows()
        {
            rowCount = 0;
        }

        /// <summary>
        /// Pose un model de colonne à l'index _index.
        /// L'espace columnCount > _index doit avoir été réservé.
        /// </summary>
        public void setColumn(int _index, PM_ColumnType _type, double _width)
        {
            Contract.Requires(_index >= 0 && _index < columnCount);
            PM_GridColumn _column = new PM_GridColumn(_type, _width);
            setColumn(_index, _column);
        }

        /// <summary>
        /// Ajoute une ligne, la peuple de cellules selon les types définis par chaque colonne.
        /// <para>Si la capacité prédéfinie pour le nombre de ligne est ici augmentée si nécessaire,
        /// mais, pour les performances,  il est préférable d'avoir prévu un <c>rowCount</c> suffisant.</para>
        /// Il faut donc avoir définit les colonnes (<c>setColumn</c>) avant d'utiliser <c>setRow</c>.
        /// </summary>
        /// <param name="_height"></param>
        public void appendRow(double _height)
        {
            if (__rows == null)
                __rows = new PM_GridRow[1];
            else
            {
                Array.Resize(ref __rows, __rows.Length + 1);
            }
            setRow(rowCount -1, new PM_GridRow(columnCount, _height));
        }

        public void removeRow(int _rowIndex)
        {
            Contract.Requires(_rowIndex >= 0 && _rowIndex < rowCount);

            PM_GridRow[] _rows = new PM_GridRow[rowCount - 1];

            int _n = 0;
            for (int _i = 0; _i < rowCount; _i++)
            {
                if(_i != _rowIndex)
                {
                    PM_GridRow _row = __rows[_i];
                    _rows[_n] = _row;
                    ++_n;
                }
            }
            __rows = _rows;
        }

        public PM_GridColumn column(int _index)
        {
            if (_index < __columns.Length && _index >= 0)
                return __columns[_index];
            return null;
        }

        public PM_GridRow row(int _index)
        {
            if (_index < __rows.Length && _index >= 0)
                return __rows[_index];
            return null;
        }

        public PM_GridCell cell(int _columnIndex, int _rowIndex)
        {
            PM_GridRow _row = row(_rowIndex);
            return (_row == null) ? null : _row.cell(_columnIndex);
        }

        public void setRowVisibility(int _index, Visibility _visibility)
        {
            PM_GridRow _row = row(_index);
            if (_row == null)
                return;
            _row.visibility = _visibility;
        }

        public void setColumnVisibility(int _index, Visibility _visibility)
        {
            PM_GridColumn _column = column(_index);
            if (_column == null)
                return;
            _column.visibility = _visibility;
        }

        public void setColumnStyle(int _index, PM_GridCellStyle _style)
        {
            PM_GridColumn _column = column(_index);
            if (_column == null)
                return;
            _column.style = _style;
        }

        public void setRowStyle(int _index, PM_GridCellStyle _style)
        {
            PM_GridRow _row = row(_index);
            if (_row == null)
                return;
            _row.style = _style;
        }

        public PM_state readOnly { get; set; }

        public PM_GridCellStyle style
        {
            get
            {
                if (__style == null)
                    __style = new PM_GridCellStyle();
                return __style;
            }
            set
            {
                __style = value;
            }
        }

        public PM_GridCellStyle horizontalHeaderStyle{get;set;}

        public PM_GridCellStyle verticalHeaderStyle { get; set; }

        public void setRowReadOnly(int _index, PM_state _readOnly)
        {
            PM_GridRow _row = row(_index);
            if (_row == null)
                return;
            _row.readOnly = _readOnly;
        }

        public void setColumnReadOnly(int _index, PM_state _readOnly)
        {
            PM_GridColumn _column = column(_index);
            if (_column == null)
                return;
            _column.readOnly = _readOnly;
        }

        public PM_GridRow[] rows()
        {
            return __rows;
        }

        public PM_GridColumn[] columns()
        {
            return __columns;
        }

        public void sortByColumn_ascending(int _columnIndex)
        {
            switch (column(_columnIndex).type)
            {
                case PM_ColumnType.STRING:
                    Array.Sort(__rows, 0, rowCount, new RowCompareAsc_string(_columnIndex));
                    break;
                case PM_ColumnType.INT:
                    Array.Sort(__rows, 0, rowCount, new RowCompareAsc_int(_columnIndex));
                    break;
                case PM_ColumnType.DOUBLE:
                    Array.Sort(__rows, 0, rowCount, new RowCompareAsc_double(_columnIndex));
                    break;
                case PM_ColumnType.DATE:
                    Array.Sort(__rows, 0, rowCount, new RowCompareAsc_date(_columnIndex));
                    break;
                default:
                    break;
            }
        }

        public void sortByColumn_descending(int _columnIndex)
        {
            Contract.Requires(_columnIndex >= 0 && _columnIndex < columnCount);

            PM_GridColumn _column = column(_columnIndex);
            if(_column != null)
            {
                switch (_column.type)
                {
                    case PM_ColumnType.STRING:
                        Array.Sort(__rows, 0, rowCount, new RowCompareDesc_string(_columnIndex));
                        break;
                    case PM_ColumnType.INT:
                        Array.Sort(__rows, 0, rowCount, new RowCompareDesc_int(_columnIndex));
                        break;
                    case PM_ColumnType.DOUBLE:
                        Array.Sort(__rows, 0, rowCount, new RowCompareDesc_double(_columnIndex));
                        break;
                    case PM_ColumnType.DATE:
                        Array.Sort(__rows, 0, rowCount, new RowCompareDesc_date(_columnIndex));
                        break;
                    default:
                        break;
                }
            }
            else
                throw new Exception("column(_columnIndex null.");
        }

        public void value(int _columnIndex, int _rowIndex, object _value)
        {
            PM_GridRow _row = row(_rowIndex);
            if (_row == null)
                return;
            PM_GridCell _cell = _row.cell(_columnIndex);
            if (_cell != null)
                _cell.value = _value;
        }

        public object value(int _columnIndex, int _rowIndex)
        {
            PM_GridCell _cell = cell(_columnIndex, _rowIndex);
            return (_cell == null) ? null : _cell.value;
        }

        public int indexOf(int _columnIndex, object _value)
        {
            Contract.Requires(_columnIndex > 0 && _columnIndex < columnCount);
            if (_value == null)
                return 1;

            if (_columnIndex >= columnCount)
                return -1;
            PM_GridRow _row;
            PM_GridCell _cell;

            for (int _i = 0; _i < __rows.Length; _i++)
            {
                _row = __rows[_i];
                if(_row != null)
                {
                    _cell = _row.cell(_columnIndex);
                    if (_cell != null && _cell.value != null)
                        // ToString(), sinon, ce sont les références de _value et _cell.value qui sont comparées.
                        if (_cell.value.ToString() == _value.ToString())
                            return _i;
                }
            }
            return -1;
        }

        public bool hasValues(int _rowIndex)
        {
            Contract.Requires(_rowIndex >= 0 && _rowIndex < rowCount);
            PM_GridRow _row = row(_rowIndex);
            return _row != null && _row.hasValues();
        }

        //private:
        PM_GridColumn[] __columns;
        PM_GridRow[] __rows;

        PM_Grid __grid;

        /// <summary>
        /// Pose un model de colonne à l'index _index.
        /// L'espace columnCount > _index doit avoir été réservé.
        /// </summary>
        private void setColumn(int _index, PM_GridColumn _column)
        {
            __columns[_index] = _column;
        }

        /// <summary>
        /// Construit une ligne peuplée de ses cellules de type selon chaque colonne,
        /// met à jour __biggestRowIndex si _index supérieur à __biggestRowIndex et donne grid à la nouvelle ligne _row.
        /// </summary>
        private void setRow(int _index, PM_GridRow _row)
        {
            Contract.Requires(_index >= 0 && _index < __rows.Length);
            Contract.Requires(_row != null);
            _row.grid = grid;

            for (int _i = 0; _i < columnCount; _i++)
            {
                if (__columns[_i] != null)
                    _row.setCell(__columns[_i].newCell(grid), _i);
                else
                    throw new Exception("__columnns[_i] null");
            }
            __rows[_index] = _row;
        }

        private PM_GridCellStyle __style;
        private double __defaultRowHeight;
    }
}
