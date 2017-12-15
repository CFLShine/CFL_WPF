using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CFL_1.CFLGraphics.PM_grid
{
    
    public class PM_GridEventArg 
    {
        public PM_GridEventArg(int _x, int _y, PM_GridCellType _type)
        {
            x = _x;
            y = _y;
            type = _type;
        }

        public int x
        { get; set; }
        public int y
        { get; set; }
        public PM_GridCellType type
        { get; set; }
    }

    public class PM_Point
    {
        PM_Point(int _col, int _row)
        {
            column = _col;
            row = _row;
        }

        int column { get; set; }
        int row { get; set; }
    }

    public class PM_Size
    {
        public PM_Size(double _height, double _width)
        {
            height = _height;
            width = _width;
        }
        public double height { get; set; }
        public double width { get; set; }
    }

    public class PM_rectangle
    {
        public int top;
        public int bottom;
        public int left;
        public int right;

        public bool givenValues = false;
    }

    public enum PM_GridSelectionModeType
    {
        NONE,
        ROW,
        COLUMN,
        CELL,
    }

    public enum PM_GridScrollBars
    {
        NONE,
        BOTH,
        VERTICAL,
        HORIZONTAL
    }

    public class PM_Grid : UserControl
    {
        public PM_Grid()
        {
            init();
            defaultRowHeight = 25;
        }

        public double defaultRowHeight
        {
            get { return __defaultRowHeight; }
            set
            {
                __defaultRowHeight = value;
                if (__model == null)
                    __model = new PM_GridModel(this);
                __model.defaultRowHeight = __defaultRowHeight;
            }
        }

        /// <summary>
        /// Equivalent à rowCount = 0.
        /// </summary>
        void clearRows()
        {
            __model.clearRows();
            render();
        }

        /// <summary>
        /// Indique que des changements vont être effectués, 
        /// bloque les mises à jour de la grille jusqu'au prochain appèle de endChanges().
        /// <para>Améliore les performances dans le cas de plusieurs changements à effectuer.</para>
        /// </summary>
        public void beginChanges()
        {
            __noRender = true;
        }

        /// <summary>
        /// Indique que les changements son terminés et déclanche une mise à jour de la grille.
        /// </summary>
        public void endChanges()
        {
            __noRender = false;
            render();
        }

        /// <summary>
        /// Ne provoque pas la mise à jour de la grille.
        /// </summary>
        /// <param name="_model"></param>
        public void setModel(PM_GridModel _model)
        {
            __model = _model;
            render();
        }

        /// <summary>
        /// set columnCount efface le contenu de la grille.
        /// </summary>
        public int columnCount
        {
            get
            {
                return __model.columnCount;
            }
            set
            {
                Contract.Requires(value >= 0);
                __model.columnCount = value;
                render();//efface les lignes affichées.
            }
        }

        /// <summary>
        /// set rowCout efface le contenu de la grille.
        /// </summary>
        public int rowCount
        {
            get
            {
                return __model.rowCount;
            }
            set
            {
                __model.rowCount = value;
                render();// efface les lignes affichées.
            }
        }

        public object value(int _columnIndex, int _rowIndex)
        {
            return __model.value(_columnIndex, _rowIndex);
        }

        public void value(int _col, int _row, object _value)
        {
            PM_GridCell _cell = __model.cell(_col, _row);
            if (_cell == null)
                return;
            _cell.value = _value;
            updateCell(new cellInfo(_cell, _col, _row));
        }

        public void setColumnHeaderValues(object[] _values)
        {
            bool _noRender = __noRender;
            __noRender = true;

            for(int _i = 0; _i < _values.Length; _i++)
            {
                if (_i < columnCount)
                    setColumnHeaderValue(_i, _values[_i]);
                else
                    return;
            }
            __noRender = _noRender;
            render();
        }

        public void setColumnHeaderValue(int _columnIndex, object _value)
        {
            PM_GridColumn _column = __model.column(_columnIndex);
            if (_column.header == null)
                _column.header = new PM_GridCellHHeader(this);
            _column.header.value = _value;
        }

        /// <summary>
        /// Retourne la ligne correspondante à _rowIndex,
        /// ou null si _rowIndex n'est pas valide.
        /// <para>Toute modification directement sur row n'entraine pas la mise à jour
        /// de la grille.</para>
        /// </summary>
        /// <param name="_rowIndex"></param>
        /// <returns></returns>
        public PM_GridRow row(int _rowIndex)
        {
            return __model.row(_rowIndex);
        }

        /// <summary>
        /// Retourne l'index de la première ligne dans laquelle _value a été trouvé dans la colonne _columnIndex,
        /// ou -1 si non trouvé.
        /// </summary>
        /// <param name="_columnIndex">Colonne dans laquelle chercher</param>
        /// <param name="_value">Valeur à trouver</param>
        /// <returns></returns>
        public int indexOf(int _columnIndex, object _value)
        {
            return __model.indexOf(_columnIndex, _value);
        }

        #region allow add rows
        public bool allowAddRows
        {
            get{ return __allowAddRows; }
            set
            {
                __allowAddRows = value;
                if (__allowAddRows)
                    checkIfRowToAddOrRemove();
            }
        }

        public void checkIfRowToAddOrRemove()
        {
            if (automaticAppendRow())
                render();
            else
                if (removeEmptyLastRows())
                render();
        }

        private bool __allowAddRows;

        /// <summary>
        /// Ajoute une ligne si la dernière ligne contient des valeurs non vides.
        /// Retourne true si une ligne est ajoutée.
        /// </summary>
        private bool automaticAppendRow()
        {
            if(allowAddRows && (rowCount == 0 || __model.hasValues(rowCount -1)))
            {
                __model.appendRow(defaultRowHeight);
                return true;
            }
            return false; 
        }

        private bool removeEmptyLastRows()
        {
            if (!allowAddRows)
                return false;

            bool _removed = false;

            while(rowCount > 1)
            {
                PM_GridRow _row = row(rowCount - 1);
                PM_GridRow _rowbefore = row(rowCount - 2);
                if (_row != null && _rowbefore != null && !_row.hasValues() && !_rowbefore.hasValues())
                {
                    __model.removeRow(rowCount - 1);
                    _removed = true;
                }
                else break;
            }
            return _removed;    
        }

        #endregion allow add rows

        #region scroll

        public bool scrollTo_vertical(int _rowIndex)
        {
            if (_rowIndex < 0 || _rowIndex >= rowCount)
                return false;
            __vScrollBar.Value = _rowIndex;
            render();
            return true;
        }

        public bool scrollTo_horizontal(int _columnIndex)
        {
            if (_columnIndex < 0 || _columnIndex >= columnCount)
                return false;
            __hScrollBar.Value = _columnIndex;
            render();
            return true;
        }

        #endregion

        #region set style

        public void setStyle(PM_GridCellStyle _style)
        {
            __model.style = _style;
            updateCells();
        }

        public void setColumnStyle(int _columnIndex, PM_GridCellStyle _style)
        {
            __model.setColumnStyle(_columnIndex, _style);
            updateCells();
        }

        public void setRowStyle(int _rowIndex, PM_GridCellStyle _style)
        {
            __model.setRowStyle(_rowIndex, _style);
            updateCells();
        }

        public void setCellStyle(int _columnIndex, int _rowIndex, PM_GridCellStyle _style)
        {
            cellInfo _cellInfo = new cellInfo(__model.row(_rowIndex).cell(_columnIndex), _columnIndex, _rowIndex);
            _cellInfo.cell.style = _style;
            updateCell(_cellInfo);    
        }

        public void setHorizontalHeaderStyle(PM_GridCellStyle _style)
        {
            __model.horizontalHeaderStyle = _style;
            updateCells();
        }

        public void setVerticalHeaderStyle(PM_GridCellStyle _style)
        {
            __model.verticalHeaderStyle = _style;
            updateCells();
        }

        public void setColumnHeaderStyle(int _columnIndex, PM_GridCellStyle _style)
        {
            cellInfo _cellInfo = new cellInfo(__model.column(_columnIndex).header, _columnIndex, 0);
            _cellInfo.cell.style = _style;
            updateCell(_cellInfo);
        }

        public void setRowHeaderStyle(int _rowIndex, PM_GridCellStyle _style)
        {
            cellInfo _cellInfo = new cellInfo(__model.row(_rowIndex).header, 0, _rowIndex);
            _cellInfo.cell.style = _style;
            updateCell(_cellInfo);
        }

        #endregion

        #region set readOnly
        public void setReadOnly(PM_state _readOnly)
        {
            __model.readOnly = _readOnly;
            updateCells();
        }

        public void setColumnReadOnly(int _columnIndex, PM_state _state)
        {
            __model.setColumnReadOnly(_columnIndex, _state);
            updateCells();
        }

        public void setRowReadOnly(int _rowIndex, PM_state _state)
        {
            __model.setRowReadOnly(_rowIndex, _state);
            updateCells();
        }

        public void setCellReadOnly(int _columnIndex, int _rowIndex, PM_state _state)
        {
            cellInfo _cellInfo = new cellInfo(__model.row(_rowIndex).cell(_columnIndex), _columnIndex, _rowIndex);
            _cellInfo.cell.readOnly = _state;
            updateCell(_cellInfo); 
        }

        #endregion

        #region set visibility

        public void setColumnVisibility(int _columnIndex, bool _visible)
        {
            if (_visible)
                __model.setColumnVisibility(_columnIndex, Visibility.Visible);
            else
                __model.setColumnVisibility(_columnIndex, Visibility.Hidden);
            render();
        }

        public void setRowVisibility(int _rowIndex, bool _visible)
        {
            if (_visible)
                __model.setRowVisibility(_rowIndex, Visibility.Visible);
            else
                __model.setRowVisibility(_rowIndex, Visibility.Hidden);
            render();
        }

        public void setHorizontalHeaderVisible(bool _visible)
        {
            __hHeaderVisible = _visible;
        }

        public void setVerticalHeaderVisible(bool _visible)
        {
            __vHeaderVisible = _visible;
        }

        #endregion

        #region setColumn

        public void setColumn_string(int _col, double _width)
        {
            Contract.Requires(_col >= 0 && _col < columnCount);
            __model.setColumn(_col, PM_ColumnType.STRING, _width);
        }

        public void setColumn_int(int _col, double _width)
        {
            Contract.Requires(_col >= 0 && _col < columnCount);
            __model.setColumn(_col, PM_ColumnType.INT, _width);
        }

        public void setColumn_double(int _col, double _width)
        {
            Contract.Requires(_col >= 0 && _col < columnCount);
            __model.setColumn(_col, PM_ColumnType.DOUBLE, _width);
        }

        public void setColumn_bool(int _col, double _width)
        {
            Contract.Requires(_col >= 0 && _col < columnCount);
            __model.setColumn(_col, PM_ColumnType.BOOL, _width);
        }

        public void setColumn_button(int _col, double _width)
        {
            Contract.Requires(_col >= 0 && _col < columnCount);
            __model.setColumn(_col, PM_ColumnType.BUTTON, _width);
        }

        public void setColumn_date(int _col, double _width)
        {
            Contract.Requires(_col >= 0 && _col < columnCount);
            __model.setColumn(_col, PM_ColumnType.DATE, _width);
        }

        #endregion

        #region append and remove row

        public void appendRow()
        {
            appendRow(defaultRowHeight);
        }

        public void appendRow(double _height)
        {
            __model.appendRow(_height);
            __vScrollBar.Maximum = rowCount;
            render();
        }

        public void appendRow(double _height, PM_GridCellVHeader _header)
        {
            PM_GridRow _row = new PM_GridRow(columnCount, _height);
            _row.header = _header;
            __vScrollBar.Maximum = rowCount;
            render();
        }

        public void removeRow(int _rowIndex)
        {
            Contract.Requires(_rowIndex >= 0 && _rowIndex < rowCount);

            __model.removeRow(_rowIndex);
            render();
        }

        #endregion

        #region sort

        public bool sortOnHorizontalHeaderClicked
        {
            get
            {
                return __sortOnHHeaderClicked;
            }
            set
            {
                if (value == __sortOnHHeaderClicked)
                    return;
                __sortOnHHeaderClicked = value;
                if (__sortOnHHeaderClicked)
                    horizontalHeaderClicked += sortByColumn;
                else
                    horizontalHeaderClicked -= sortByColumn;
            }
        }

        public void sortByColumn_ascending(int _columnIndex)
        {
            __model.sortByColumn_ascending(_columnIndex);
            render();
        }

        public void sortByColumn_descending(int _columnIndex)
        {
            __model.sortByColumn_descending(_columnIndex);
            render();
        }

        private bool __sortOnHHeaderClicked;
        private bool __descending;

        private void sortByColumn(int _columnIndex)
        {
            if(__descending)
            {
                sortByColumn_descending(_columnIndex);
                __descending = false;
            }
            else
            {
                sortByColumn_ascending(_columnIndex);
                __descending = true;
            }
        }

        #endregion

        public delegate void HorizontalHeaderClicked(int _col);
        public delegate void VerticalHeaderClicked(int _row);
        public delegate void CellClicked(int _col, int _row);
        public HorizontalHeaderClicked horizontalHeaderClicked;
        public VerticalHeaderClicked verticalHeaderClicked;

        /// <summary>
        /// cellClicked(int column, int row)
        /// </summary>
        public CellClicked cellClicked;
 

        public Color backGroundColor
        {
            get
            {
                return __backGroundColor;
            }
            set
            {
                __backGroundColor = value;
                if(__grid != null)
                    __grid.Background = new SolidColorBrush(value);
            }
        }

        public bool showGridLines
        {
            get
            {
                return __showGridLines;
            }
            set
            {
                __showGridLines = value;
                if(__grid != null)
                    __grid.ShowGridLines = __showGridLines;
            }
        }

        //protected:
        protected virtual void OnCellClicked(object _sender, RoutedEventArgs _e)
        {
            if (_sender == null || cellClicked == null)
                return;
            PM_GridEventArg _pos = (PM_GridEventArg)(((Control)_sender).Tag);
            if (_pos != null)
            {
                switch (_pos.type)
                {
                    case PM_GridCellType.HORIZONTALHEADER:
                        horizontalHeaderClicked?.Invoke(_pos.x);
                        break;
                    case PM_GridCellType.VERTICALHEADER:
                        verticalHeaderClicked?.Invoke(_pos.y);
                        break;
                    default:
                        cellClicked?.Invoke(_pos.x, _pos.y);
                        break;
                }
            }
        }

        //private:

        #region render

        public void render()
        {
            if (__noRender)
                return;
            initMainPanel();
            renderContent();
        }

        void initMainPanel()
        {
            if (__mainPanel != null)
            {
                __mainPanel.Children.Clear();
                this.Content = null;
            }
            __mainPanel = new DockPanel();
            this.AddChild(__mainPanel);

            double _gridHeight = this.ActualHeight;
            double _gridWidth = this.ActualWidth;

            dimScrollBars();

            if (__showVScrollBar)
            {
                DockPanel.SetDock(__vScrollBar, Dock.Right);
                __vScrollBar.Visibility = Visibility.Visible;
                __mainPanel.Children.Add(__vScrollBar);
                _gridWidth -= __scrollBarsWidth;
            }
            else
                __vScrollBar.Visibility = Visibility.Hidden;

            if (__showHScrollBar)
            {
                DockPanel.SetDock(__hScrollBar, Dock.Bottom);
                __mainPanel.Children.Add(__hScrollBar);
                __hScrollBar.Visibility = Visibility.Visible;
                _gridHeight -= __scrollBarsWidth;
            }
            else
                __hScrollBar.Visibility = Visibility.Hidden;

            if (_gridWidth < 0)
                _gridWidth = 0;
            if (_gridHeight < 0)
                _gridHeight = 0;

            if(__grid != null)
                __grid.Children.Clear();
                
            __grid = new Grid();
            __grid.Height = _gridHeight;
            __grid.Width = _gridWidth;
            __grid.ShowGridLines = __showGridLines;
            if(__backGroundColor != null)
                __grid.Background = new SolidColorBrush(__backGroundColor);

            __mainPanel.LastChildFill = true;
            __mainPanel.Children.Add(__grid);
        }

        class cellInfo
        {
            public cellInfo(PM_GridCell _cell, int _colIndex, int _rowIndex)
            {
                cell = _cell;
                colIndex = _colIndex;
                rowIndex = _rowIndex;
            }
            public PM_GridCell cell;
            public int colIndex;
            public int rowIndex;
        }

        List<cellInfo> __displayedCells;

        private void renderContent()
        {
            double _heightdelta = 0;
            double _widthdelta = 0;
            PM_GridRow _pmRow;
            PM_GridColumn _pmColumn;
            int _firstVisualColumn = 0;
            int _visualRow = 0;
            int _visualColumn = 0;

            PM_Size _size = drawableSurface();

            bool _showVHeaders = (__vHeaderVisible == true && __vHeadersWidth > 0);
            bool _showHHeaders = (__hHeaderVisible == true && __hHeadearsHeight > 0);

            bool _columnsDefinitionsDisplayed = false;

            detachDisplayedControls();

            if (_showVHeaders)
            {
                _widthdelta = addColumnDefinition(__vHeadersWidth);
                _firstVisualColumn = 1;
            }

            if(_showHHeaders)
            {
                _heightdelta = addRowDefinition(__hHeadearsHeight);
                _visualRow = 1;
            }

            _visualColumn = _firstVisualColumn;
            
            for (int _i = (int)__vScrollBar.Value; _i < rowCount; _i++)
            {
                if (_heightdelta <= _size.height)
                {
                    _pmRow = __model.row(_i);
                    if (_pmRow == null || _pmRow.visibility == Visibility.Hidden)
                        continue;

                    _heightdelta += addRowDefinition(_pmRow.height);
                    if (_showVHeaders)
                    {
                        _pmRow.header = header(_pmRow.header, _i);
                        setControlToGrid(0, _visualRow, new cellInfo(_pmRow.header,0, _i));
                    }

                    for (int _j = (int)__hScrollBar.Value; _j < columnCount; _j++)
                    {
                        if (_widthdelta <= _size.width)
                        {
                            _pmColumn = __model.column(_j);
                            if (_pmColumn == null || _pmColumn.visibility == Visibility.Hidden)
                                continue;

                            if (!_columnsDefinitionsDisplayed)
                            {
                                addColumnDefinition(_pmColumn.width);
                                if (_showHHeaders)
                                {
                                    _pmColumn.header = header(_pmColumn.header, _j);
                                    setControlToGrid(_visualColumn, 0, new cellInfo(_pmColumn.header, _j, 0));
                                }
                            }
                         
                            PM_GridCell _cell = _pmRow.cell(_j);
                            setControlToGrid(_visualColumn, _visualRow, new cellInfo(_cell, _j, _i));

                            _widthdelta += _pmColumn.width;
                            ++_visualColumn;
                        }
                        else
                            break;
                    }
                    _columnsDefinitionsDisplayed = true;
                    
                    _visualColumn = _firstVisualColumn;
                    ++_visualRow;

                    _widthdelta = (_showHHeaders) ? __vHeadersWidth : 0;
                }
            }
            _showVHeaders = false;
        }

        T header<T>(T _header, int _index) where T : PM_GridCell_typed<Button>, new()
        {
            if (_header == null)
                _header = new T();
            if (_header.value == null)
                _header.value = _index.ToString();
            return _header;
        }

        double addColumnDefinition(double _width)
        {
            ColumnDefinition _columnDefinition = new ColumnDefinition();
            _columnDefinition.Width = new GridLength(_width);
            __grid.ColumnDefinitions.Add(_columnDefinition);
            return _width;
        }

        double addRowDefinition(double _height)
        {
            RowDefinition _rowDefinition = new RowDefinition();
            _rowDefinition.Height = new GridLength(_height);
            __grid.RowDefinitions.Add(_rowDefinition);
            return _height;
        }

        void setControlToGrid(int _visualCol, int _visualRow, cellInfo _cellInfo)
        {
            if (_cellInfo.cell == null)
                return;
            //init control if null, connect control events
            _cellInfo.cell.show();
            
            Control _control = _cellInfo.cell.control();
            if (_control == null)
                return;

            applyProperties(_cellInfo);

            Grid.SetColumn(_control, _visualCol);
            Grid.SetRow(_control, _visualRow);
            __grid.Children.Add(_control);
            _cellInfo.cell.setOnGrid = true;
            __displayedCells.Add(_cellInfo);
            
            _control.Tag = new PM_GridEventArg(_cellInfo.colIndex, _cellInfo.rowIndex, _cellInfo.cell.type);

            switch (_cellInfo.cell.type)
            {
                case PM_GridCellType.HORIZONTALHEADER:
                    ((Button)_control).Click += OnCellClicked;
                    break;
                case PM_GridCellType.VERTICALHEADER:
                    ((Button)_control).Click += OnCellClicked;
                    break;
                case PM_GridCellType.CELLTEXTBOX:
                    ((TextBox)_control).PreviewMouseDown += OnCellClicked;
                    break;
                case PM_GridCellType.CELLCHECKBOX:
                    ((CheckBox)_control).Click += OnCellClicked;
                    break;
                case PM_GridCellType.CELLBUTTON:
                    ((Button)_control).Click += OnCellClicked;
                    break;
                default:
                    break;
            }
        }

        void applyProperties(cellInfo _cellInfo)
        {
            PM_GridColumn _column = __model.column(_cellInfo.colIndex);
            PM_GridRow _row = __model.row(_cellInfo.rowIndex);

            _cellInfo.cell.applyControl_value();

            if(_cellInfo.cell.type == PM_GridCellType.HORIZONTALHEADER)
            {
                applyHorizontalHeaderProperties(_cellInfo);
                return;
            }

            if(_cellInfo.cell.type == PM_GridCellType.VERTICALHEADER)
            {
                applyVerticalHeaderProperties(_cellInfo);
                return;
            }

            // readOnly
            if (_cellInfo.cell.readOnly != PM_state.UNSET)
                _cellInfo.cell.applyIsReadOnly(_cellInfo.cell.readOnly);
            else
                if (_row.readOnly != PM_state.UNSET)
                _cellInfo.cell.applyIsReadOnly(_row.readOnly);
            else
                if (_column.readOnly != PM_state.UNSET)
                _cellInfo.cell.applyIsReadOnly(_column.readOnly);
            else
                _cellInfo.cell.applyIsReadOnly(__model.readOnly);

            // style
            if (_cellInfo.cell.style != null)
                _cellInfo.cell.applyStyle(_cellInfo.cell.style);
            else
                if (_row.style != null)
                _cellInfo.cell.applyStyle(_row.style);
            else
                if (_column.style != null)
                _cellInfo.cell.applyStyle(_column.style);
            else
                _cellInfo.cell.applyStyle(__model.style);

        }

        void applyHorizontalHeaderProperties(cellInfo _cellInfo)
        {
            if (_cellInfo.cell.style != null)
                _cellInfo.cell.applyStyle(_cellInfo.cell.style);
            else
                if(__model.horizontalHeaderStyle != null)
                _cellInfo.cell.applyStyle(__model.horizontalHeaderStyle);
            else
                _cellInfo.cell.applyStyle(__model.style);
        }

        void applyVerticalHeaderProperties(cellInfo _cellInfo)
        {
            if (_cellInfo.cell.style != null)
                _cellInfo.cell.applyStyle(_cellInfo.cell.style);
            else
                if (__model.verticalHeaderStyle != null)
                _cellInfo.cell.applyStyle(__model.verticalHeaderStyle);
            else
                _cellInfo.cell.applyStyle(__model.style);
        }

        void detachDisplayedControls()
        {
            for(int _i = 0; _i < __displayedCells.Count; _i++)
            {
                PM_GridCell _cell= __displayedCells[_i].cell;
                _cell.setOnGrid = false;
                Control _control = _cell.control();
                _control.Visibility = Visibility.Hidden;

                switch (((PM_GridEventArg)_control.Tag).type)
                {
                    case PM_GridCellType.HORIZONTALHEADER:
                        ((Button)_control).Click -= OnCellClicked;
                        break;
                    case PM_GridCellType.VERTICALHEADER:
                        ((Button)_control).Click -= OnCellClicked;
                        break;
                    case PM_GridCellType.CELLTEXTBOX:
                        ((TextBox)_control).PreviewMouseDown -= OnCellClicked;
                        break;
                    case PM_GridCellType.CELLCHECKBOX:
                        ((CheckBox)_control).Click -= OnCellClicked;
                        break;
                    case PM_GridCellType.CELLBUTTON:
                        ((Button)_control).Click -= OnCellClicked;
                        break;
                    default:
                        break;
                }
            }
        }

        PM_Size drawableSurface()
        {
            PM_Size _s = new PM_Size(this.ActualHeight, this.ActualWidth);
            if (__showVScrollBar)
                _s.width -= __scrollBarsWidth;
            if (__showHScrollBar)
                _s.height -= __scrollBarsWidth;
            return _s;
        }

        #endregion render

        #region show_scrollbars

        bool __showVScrollBar = false;
        bool __showHScrollBar = false;
        void dimScrollBars()
        {
            switch (__scrollBars)
            {
                case PM_GridScrollBars.NONE:
                    break;
                case PM_GridScrollBars.BOTH:
                    __showVScrollBar = showVScrollBar();
                    __showHScrollBar = showHScrollBar();
                    break;
                case PM_GridScrollBars.VERTICAL:
                    __showVScrollBar = showVScrollBar();
                    break;
                case PM_GridScrollBars.HORIZONTAL:
                    __showHScrollBar = showHScrollBar();
                    break;
                default:
                    break;
            }
            if (!__showVScrollBar)
                __vScrollBar.Value = 0;
            if (!__showHScrollBar)
                __hScrollBar.Value = 0;
            __vScrollBar.Maximum = rowCount;
            __hScrollBar.Maximum = columnCount;
        }

        bool showVScrollBar()
        {
            double _n = 0;
            double _h = this.ActualHeight;
            for (int _i = 0; _i < rowCount; _i++)
            {
                PM_GridRow _row = __model.row(_i);
                if(_row != null)
                {
                    _n += __model.row(_i).height;
                    if (_n > _h)
                        return true;
                }
            }
            return false;
        }

        bool showHScrollBar()
        {
            double _n = 0;
            double _w = this.ActualWidth;
            for (int _i = 0; _i < columnCount; _i++)
            {
                PM_GridColumn _column = __model.column(_i);
                if(_column != null)
                {
                    _n += __model.column(_i).width;
                    if (_n > _w)
                        return true;
                }
            }
            return false;
        }
        
        #endregion

        #region updateCells

        private void updateCell(cellInfo _cellInfo)
        {
            if(!__noRender && _cellInfo.cell.setOnGrid)
            {
                _cellInfo.cell.show();
                applyProperties(_cellInfo);
            }
        }

        private void updateCells()
        {
            foreach (cellInfo _cellInfo in __displayedCells)
            {
                _cellInfo.cell.show();
                applyProperties(_cellInfo);
            }
        }

        #endregion

        
        private void scroll(object sender, ScrollEventArgs _e)
        {
            render();
        }

        private void sizeChanged(object sender, SizeChangedEventArgs _e)
        {
            render();
        }

        #region private variables

        PM_GridModel __model;

        private DockPanel __mainPanel;

        Grid __grid;
        Color __backGroundColor;
        Color __linesColor;
        bool __showGridLines;

        ScrollBar __vScrollBar;
        ScrollBar __hScrollBar;
        int __scrollBarsWidth;

        double __hHeadearsHeight;
        double __vHeadersWidth;
        bool __hHeaderVisible;
        bool __vHeaderVisible;

        bool __noRender;

        double __defaultRowHeight;
        
#pragma warning disable CS0169 // Le champ 'PM_Grid.__selectionMode' n'est jamais utilisé
        PM_GridSelectionModeType __selectionMode;
#pragma warning restore CS0169 // Le champ 'PM_Grid.__selectionMode' n'est jamais utilisé
        PM_GridScrollBars __scrollBars;

        #endregion

        void init()
        {
            __model = new PM_GridModel(this);

            __noRender = false;
            
            __displayedCells = new List<cellInfo>();

            __hHeadearsHeight = 20;
            __vHeadersWidth   = 50;
            __hHeaderVisible = true;
            __vHeaderVisible = true;

            __linesColor = Colors.Black;
            __backGroundColor = Colors.LightGray;

            __vScrollBar  = new ScrollBar();
            __hScrollBar  = new ScrollBar();

            __vScrollBar.Orientation = Orientation.Vertical;
            __vScrollBar.Width = __scrollBarsWidth;
            __vScrollBar.SmallChange = 1;

            __hScrollBar.Orientation = Orientation.Horizontal;
            __hScrollBar.Height = __scrollBarsWidth;
            __hScrollBar.SmallChange = 1;

            __scrollBars = PM_GridScrollBars.BOTH;
            __scrollBarsWidth = 15;

            __vScrollBar.Scroll += scroll;
            __hScrollBar.Scroll += scroll;

            initMainPanel();

            this.SizeChanged += sizeChanged;
            
        }
    }
}
