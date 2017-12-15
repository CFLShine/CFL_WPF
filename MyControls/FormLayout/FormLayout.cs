using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ShLayouts
{
    public class FormLayout : Grid
    {
        public FormLayout()
        {
            init();
        }

        public virtual void Clear()
        {
            Children.Clear();
            ColumnDefinitions.Clear();
            RowDefinitions.Clear();
            init();
        }

        public void Add(Label _label, FrameworkElement _element)
        {
            int _count = RowDefinitions.Count;
            RowDefinition _row = new RowDefinition();
            RowDefinitions.Add(_row);

            SetRow(_label, _count);
            SetColumn(_label, 0);
            SetRow(_element, _count); 
            SetColumn(_element, 1);

            double _minHeight = MesureHelper.MinHeight(_label, _element);

            if(!double.IsNaN(_minHeight))
                MinHeight += _minHeight;
            //double _maxHeight = MesureHelper.MaxHeightNotInfinity(_label, _element);
            
            //if(!double.IsNaN(_minHeight))
            //    _row.MinHeight = _minHeight;

            //if(!double.IsNaN(_maxHeight))
            //    _row.MaxHeight = _maxHeight;

            Children.Add(_label);
            Children.Add(_element);
        }

        public void Add(string _label, FrameworkElement _element, double height = double.NaN)
        {
            Label _l = new Label() { Content = _label };
            if(!double.IsNaN(height))
                _l.Height = height;

            Add(_l, _element);
        }

        public int Count
        {
            get
            {
                return RowDefinitions.Count;
            }
        }

        public IEnumerable<FrameworkElement> Elements()
        {
            foreach(UIElement _element in Children)
            {
                if(Grid.GetColumn(_element) == 1)
                    yield return _element as FrameworkElement;
            }

        }

        public void FitHeightToContent()
        {
            MaxHeight = MinHeight;
        }

        private void init()
        {
            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
            VerticalAlignment = VerticalAlignment.Top;
            MinHeight = 0;
        }
    }
}
