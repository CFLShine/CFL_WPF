
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CustomControls;
using MSTD;
using MSTD.ShBase;
using RuntimeExec;
using ShLayouts;

namespace ObjectEdit
{
    public abstract class PropertyEditControl : HBoxLayout
    {
        protected PropertyEditControl()
        {
            Init();
        }

        /// <summary>
        /// L'objet de class <see cref="Base"/> édité par le <see cref="ObjectEditControl"/> contenant
        /// ce <see cref="PropertyEditControl"/>.
        /// Appelle <see cref="UpdateControl"/> si <see cref="IsComplete"/> == true.
        /// </summary>
        public Base Object
        {
            get => __object;
            set
            {
                __object = value;
                if(IsComplete())
                    UpdateControl(); 
            }
        }

        public PropertyInfo PropertyInfo
        {
            get => __prInfo;
            set
            {
                __prInfo = value;
                if(IsComplete())
                    UpdateControl();
            }
        }

        public object Value
        {
            get
            {
                if(IsComplete())
                    return PropertyInfo.GetValue(Object);
                return null;
            }

            set
            {
                if(IsComplete())
                {
                    PropertyInfo.SetValue(Object, value);
                    UpdateControl();
                }
            }
        }

        public delegate void ValueChangedByUser(PropertyEditControl control);
        public ValueChangedByUser ValueChangedByUserEvent;

        protected void OnValueChangedByUser()
        {
            ValueChangedByUserEvent?.Invoke(this);
        }

        protected virtual bool IsComplete()
        {
            return Object != null && PropertyInfo != null;
        }

        /// <summary>
        /// Produit le <see cref="BoxLayout"/>.
        /// </summary>
        protected abstract void Init();

        public abstract void UpdateControl();

        private Base __object = null;
        private PropertyInfo __prInfo = null;
    }

    public class PropertyTextControl : PropertyEditControl
    {
        protected override void Init()
        {
            Add(__textBox); 
            __textBox.KeyUp += OnKeyUp;
        }

        public override void UpdateControl()
        {
            if(Value != null)
                __textBox.Text = Value.ToString();
            else
                __textBox.Text = "";
        }

        protected virtual void OnKeyUp(object sender, RoutedEventArgs e)
        {
            Value = __textBox.Text;
            OnValueChangedByUser();
        }

        protected TextBox __textBox = new TextBox(){ Background = Brushes.White };
    }

    public class PropertyBoolControl : PropertyEditControl
    {
        protected override void Init()
        {
            Add(__checkBox); 
            __checkBox.Click += Click;
        }

        protected virtual void Click(object sender, RoutedEventArgs e)
        {
            Value = __checkBox.IsChecked;
            OnValueChangedByUser();
        }

        public override void UpdateControl()
        {
            object _value = Value;
            if(_value != null)
                __checkBox.IsChecked = (bool)_value;
            else
                __checkBox.IsChecked = false;
        }

        private CheckBox __checkBox = new CheckBox();
    }

    public class PropertyIntControl : PropertyTextControl
    {
        protected override void OnKeyUp(object sender, RoutedEventArgs e)
        {
            string _new = __textBox.Text;
            if(!int.TryParse(_new, out int _int))
            {
                Value = 0;
                UpdateControl();
            }
            else Value = _int;
            OnValueChangedByUser();
        }
    }

    public class PropertyLongControl : PropertyTextControl
    {
        protected override void OnKeyUp(object sender, RoutedEventArgs e)
        {
            string _new = __textBox.Text;
            if(!long.TryParse(_new, NumberStyles.Any, CultureInfo.InvariantCulture, out long _long))
            {
                Value = 0;
                UpdateControl();
            }
            else Value = _long;
            OnValueChangedByUser();
        }
    }

    public class PropertyDoubleControl : PropertyTextControl
    {
        protected override void OnKeyUp(object sender, RoutedEventArgs e)
        {
            string _new = __textBox.Text;
            if(!double.TryParse(_new, NumberStyles.Any, CultureInfo.InvariantCulture, out double _double))
            {
                Value = 0;
                UpdateControl();
            }
            else Value = _double;
            OnValueChangedByUser();
        }

        public override void UpdateControl()
        {
            if(Value != null)
                __textBox.Text = Value.ToString().Replace(',', '.');
            else
                __textBox.Text = "";
        }
    }

    public class PropertyDateControl : PropertyEditControl
    {
        public override void UpdateControl()
        {
            
        }

        protected override void Init()
        {
            Add(__textBox);
        }

        private TextBoxDate __textBox = new TextBoxDate();
    }

    public class PropertyTimeSpanControl : PropertyEditControl
    {
        public override void UpdateControl()
        {
            
        }

        protected override void Init()
        {
            
        }
    }

    public class PropertyObjectSelectionControl : PropertyEditControl
    {
        public PropertyObjectSelectionControl(){ }

        public PropertyObjectSelectionControl(DataDisplay dataDisplay, IList objectsToDisplay)
        {
            DataDisplay = dataDisplay;

            List<Base> _objectsToDisplay = new List<Base>();
            foreach (object _o in objectsToDisplay)
                _objectsToDisplay.Add((Base)_o);
            ObjectsToDisplay = _objectsToDisplay;
        }

        public DataDisplay DataDisplay
        {
            get => __dataDisplay;
            set
            {
                __dataDisplay = value;
                
                __memberExpressions = new List<REMemberExpression>();
                foreach(REExpression _expr in DataDisplay.Elements)
                {
                    if(_expr is REMemberExpression _memberExpression)
                        __memberExpressions.Add(_memberExpression);
                }

                if(IsComplete())
                {
                    PopulateCombo();
                    UpdateControl();
                }
            }
        }

        public List<Base> ObjectsToDisplay
        { 
            get => __objectsToDisplay; 
            set
            {
                __objectsToDisplay = value;
                __currentlyDisplayedObjects = value;
                if(IsComplete())
                {
                    PopulateCombo();
                    UpdateControl();
                }
            }
        }

        protected void OnKeyUp(object sender, RoutedEventArgs e)
        {
            Search();
            PopulateCombo();
            __combobox.IsDropDownOpen = true;
        }

        protected override void Init()
        {
            Add(__combobox);
            __combobox.IsEditable = true;
            __combobox.IsTextSearchEnabled = false;
            __combobox.KeyUp += OnKeyUp;
            __combobox.GotFocus += OnGotFocus;
            __combobox.PreviewMouseDown += OnMouseDown;

            // virtualisation du combobox
            __combobox.ItemsPanel = new ItemsPanelTemplate();
            FrameworkElementFactory stackPanelTemplate = new FrameworkElementFactory(typeof (VirtualizingStackPanel));
            __combobox.ItemsPanel.VisualTree = stackPanelTemplate;
        }

        public override void UpdateControl()
        {
            __combobox.SelectionChanged -= OnSelectionChanged;

            if(__combobox.Items.Count == 0)
                PopulateCombo();

            object _value = Value;
            if(_value != null)
            {
                if(!Value.GetType().IsSubclassOf(typeof(Base)))
                    throw new Exception("La propriété visée par ce control est de type " + _value.GetType().Name + " alors qu'elle devrait être de type Base.");
                
                int _index = IndexOf((Base)_value);
                if(_index > -1)
                    __combobox.SelectedIndex = _index;
            }
            __combobox.SelectionChanged += OnSelectionChanged;
        }

        protected override bool IsComplete()
        {
            return base.IsComplete()
                && DataDisplay != null && ObjectsToDisplay != null && ObjectsToDisplay.Count != 0;
        }

        protected void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if(__combobox.SelectedIndex >= 0 && __combobox.SelectedIndex < __currentlyDisplayedObjects.Count)
                Value = __currentlyDisplayedObjects[__combobox.SelectedIndex]; 
            OnValueChangedByUser();// lors du chargement, OnSelectionChanged n'est pas appelé.
        }

        protected void OnGotFocus(object sender, RoutedEventArgs e)
        {
            __combobox.IsDropDownOpen = true;
        }

        protected void OnMouseDown(object sender, RoutedEventArgs e)
        {
            if(__combobox.IsKeyboardFocusWithin)
                __combobox.IsDropDownOpen = true;
        }

        private void PopulateCombo()
        {
            List<string> _itemsToDisplay = new List<string>();
            if(__currentlyDisplayedObjects != null)
            {
                foreach(Base _toDisplay in __currentlyDisplayedObjects)
                {
                    _itemsToDisplay.Add(Item(_toDisplay));
                }
            }
            __combobox.ItemsSource = _itemsToDisplay;
            __combobox.SelectedIndex = -1;
        }

        private int IndexOf(Base _base)
        {
            if(_base == null || __currentlyDisplayedObjects == null)
                return -1;

            int _i = 0;
            foreach(Base _b in __currentlyDisplayedObjects)
            {
                if(_b != null && _b.ID == _base.ID)
                    return _i;
                ++_i;
            }
            return -1;
        }

        #region Search

        private void Search()
        {
            __currentlyDisplayedObjects = new List<Base>();

            if(string.IsNullOrWhiteSpace(__combobox.Text))
            {
                __currentlyDisplayedObjects.AddRange(ObjectsToDisplay);
            }
            else
            {
                SelectByPertinence _pertinenceFinder = new SelectByPertinence();
                _pertinenceFinder.Seach(__combobox.Text, ObjectsToDisplay, __memberExpressions);
                
                __currentlyDisplayedObjects.AddRange(_pertinenceFinder.ListPertinence1);
                __currentlyDisplayedObjects.AddRange(_pertinenceFinder.ListPertinence2);
                __currentlyDisplayedObjects.AddRange(_pertinenceFinder.ListPertinence3);
                __currentlyDisplayedObjects.AddRange(_pertinenceFinder.ListPertinence4);
                __currentlyDisplayedObjects.AddRange(_pertinenceFinder.ListPertinence5);
                __currentlyDisplayedObjects.AddRange(_pertinenceFinder.ListPertinence6);
            }
        }

        private string Item(Base _base)
        {
            DataDisplay.Update(_base);
            return DataDisplay.Display();
        }

        #endregion Search

        // Les objets dont les membres désignés par les REMemberExpression sont à afficher dans le combobox.
        private List<Base> __objectsToDisplay = null;

        // Au départ, identique à __objectsToDisplay,
        // après une recherche suite à un caractère tapé, __currentlyDisplayedObjects devient le résultat de la recherche.
        private List<Base> __currentlyDisplayedObjects = null;

        private DataDisplay __dataDisplay = null;

        private List<REMemberExpression> __memberExpressions = null;

        ComboBox __combobox = new ComboBox();
    }

    public class PropertyEnumClontrol : PropertyEditControl
    {
        public override void UpdateControl()
        {
            __combobox.SelectionChanged -= OnSelectionChanged;

            if(__combobox.Items.Count == 0)
                PopulateCombo();
            object _value = Value;

            int _i = 0;
            foreach(object _item in __combobox.Items)
            {
                if((int)_item == (int)_value)
                {
                    __combobox.SelectedIndex = _i;
                    return;
                }
                ++_i;
            }

            __combobox.SelectionChanged += OnSelectionChanged;
        }

        protected void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            Value = __combobox.SelectedItem;
            OnValueChangedByUser();// lors du chargement, OnSelectionChanged n'est pas appelé.
        }

        protected override void Init()
        {
            Add(__combobox);
            __combobox.IsEditable = false;

        }

        private void PopulateCombo()
        {
            __combobox.ItemsSource = GetValues();
        }

        private List<object> GetValues()
        {
            List<object> _l = new List<object>();
            if(PropertyInfo != null)
            {
                Type _enumType = PropertyInfo.PropertyType;
                IEnumerable<FieldInfo> fields = _enumType.GetFields().Where( x => x.IsLiteral );
                foreach( FieldInfo field in fields )
                {
                    _l.Add(field.GetValue( _enumType ));
                }
            }
            return _l;
        }

        ComboBox __combobox = new ComboBox();
    }

}
