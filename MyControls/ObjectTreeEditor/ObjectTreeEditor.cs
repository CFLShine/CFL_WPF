using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using ShLayouts;

namespace MyControls
{
    // méthode à appliquer sur __object lorsque le text de __textbox change.
    public delegate void SetValueMethod(ref object _object, string _s);

    /// <summary>
    /// Control héritant de TreeView permetant de visualiser un object, ses propriétes et champs et d'en éditer les valeurs.
    /// La propriété <see cref="Type[] typesToShow"/> permet de restreindre aux types spécifiés les membres à montrer dans l'arbre.
    /// 
    /// </summary>
    public class ObjectTreeEditor : TreeView
    {
        public ObjectTreeEditor(bool _editable, bool _includePrimitivesInRender)
        {
            __editable = _editable;
            __includePrimitives = _includePrimitivesInRender;
        }

        public void edit(object _object, string _name)
        {
            Items.Clear();
            Items.Add(new item_object_value(this, _object, _name, __editable));
        }

        #region set value methods

        public void AddMethod(Type _type, SetValueMethod _method)
        {
            __methods[_type] = _method;
        }

        public SetValueMethod GetMethod(Type _type)
        {
            __methods.TryGetValue(_type, out SetValueMethod _method);
            return _method;
        }

        public void AddMethods_primitives()
        {
            __methods[typeof(bool)] = method_bool;
            __methods[typeof(byte)] = method_byte;
            __methods[typeof(int)] = method_int;
            __methods[typeof(uint)] = method_uint;
            __methods[typeof(short)] = method_short;
            __methods[typeof(ushort)] = method_ushort;
            __methods[typeof(long)] = method_long;
            __methods[typeof(ulong)] = method_ulong;
            __methods[typeof(float)] = method_float;
            __methods[typeof(double)] = method_double;
            __methods[typeof(char)] = method_char;
            __methods[typeof(decimal)] = method_decimal;
            __methods[typeof(string)] = method_string;
        }

        private void method_bool(ref object _object, string _value)
        {
            bool.TryParse(_value, out bool _r);
            _object = _r;
        }
        private void method_byte(ref object _object, string _value)
        {
            byte.TryParse(_value, out byte _r);
            _object = _r;
        }
        private void method_int(ref object _object, string _value)
        {
            int.TryParse(_value, out int _r);
            _object = _r;
        }
        private void method_uint(ref object _object, string _value)
        {
            uint.TryParse(_value, out uint _r);
            _object = _r;
        }
        private void method_short(ref object _object, string _value)
        {
            short.TryParse(_value, out short _r);
            _object = _r;
        }
        private void method_ushort(ref object _object, string _value)
        {
            ushort.TryParse(_value, out ushort _r);
            _object = _r;
        }
        private void method_long(ref object _object, string _value)
        {
            long.TryParse(_value, out long _r);
            _object = _r;
        }
        private void method_ulong(ref object _object, string _value)
        {
            ulong.TryParse(_value, out ulong _r);
            _object = _r;
        }
        private void method_float(ref object _object, string _value)
        {
            float.TryParse(_value, out float _r);
            _object = _r;
        }
        private void method_double(ref object _object, string _value)
        {
            double.TryParse(_value, out double _r);
            _object = _r;
        }
        private void method_char(ref object _object, string _value)
        {
            char.TryParse(_value, out char _r);
            _object = _r;
        }
        private void method_decimal(ref object _object, string _value)
        {
            decimal.TryParse(_value, out decimal _r);
            _object = _r;
        }
        private void method_string(ref object _object, string _value)
        {
            _object = _value;
        }

        #endregion set value methods

        #region types to show

        /// <summary>
        /// Types non primitifs.
        /// Seuls les champs et propriétés instances de classe de ces types, ou dérivés,
        /// seront montrés dans l'arbre.
        /// 
        /// </summary>
        public Type[] typesToShow
        {
            get 
            { return __typesToShow; }
            set
            { __typesToShow = value; }
        }

        public bool isToBeShown(Type _type)
        {
            if(_type.IsPrimitive || _type == typeof(string))
                return __includePrimitives;

            if(__typesToShow == null)
                return false;
            foreach(Type _t in __typesToShow)
            {
                if(_type == _t || _type.IsSubclassOf(_t))
                    return true;
            }
            return false;
        }

        #endregion types to show

        private bool __includePrimitives = true;
        private bool __editable;
        private Dictionary<Type, SetValueMethod> __methods = new Dictionary<Type, SetValueMethod>();
        private Type[] __typesToShow;
    }


    public class item_object_value : TreeViewItem
    {
        /// <summary>
        /// Constructeur pour un item éditable.
        /// </summary>
        public item_object_value(ObjectTreeEditor _objectEditor, object _handledObject, string _name, bool _editable) 
        {
            init();
            __objectEditor = _objectEditor ?? throw new ArgumentNullException("_objectEditor");
            __editable = _editable;
            setObject(_handledObject, _name);
        }

        public object handledObject
        { 
            get 
            {  return __object ; } 
        }

        public bool editable
        {
            get { return __editable; }
            set
            {
                __editable = value;
                __textBox.IsReadOnly = !__editable;
            }
        }

        public void setObject(object _object, string _name)
        { 
            if(_object == null)
            {
                __label.Content = _name;
                __textBox.Text = "null";
            }
            else
            {
                __label.Content = _object.GetType().Name + " " + _name;
                __textBox.Text = _object.ToString();
            }
            
            // __object est assigné après __textBox.Text pour que 
            // textBox_TextChanged n'ait pas d'effet.
            __object = _object;

            makeChildren();
        }

        private bool IsClass(object _object)
        {
            return (_object != null 
                && (_object.GetType().IsClass == true));
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(__editable && __object != null)
            { 
                SetValueMethod _method = __objectEditor.GetMethod(__object.GetType());
                if(_method != null)
                    _method.Invoke(ref __object, __textBox.Text);
            }
        }

        private void makeChildren()
        {
            if(__object == null)
                return ;
            Type _type = __object.GetType();
            
            if(_type.IsNotPublic 
            || _type.IsPrimitive 
            || __object is string
            )
                return ;
            
            // class

            foreach(PropertyInfo _propertyInfo in _type.GetProperties().Where(p=> p.GetIndexParameters().Length == 0))
            {
                if(!__objectEditor.isToBeShown(_propertyInfo.PropertyType))
                    continue;
                object _child = _propertyInfo.GetValue(__object);
                newItem(_child, _propertyInfo.Name);
            }

            foreach(FieldInfo _fieldInfo in _type.GetFields())
            {
                if(!__objectEditor.isToBeShown(_fieldInfo.FieldType))
                    continue;
                object _child = _fieldInfo.GetValue(__object);
                newItem(_child, _fieldInfo.Name);
            }

        }

        private void newItem(object _child, string _name)
        {
            Items.Add( new item_object_value(__objectEditor, _child, _name, __editable));
        }

        private void init()
        {
            HBoxLayout __layout = new HBoxLayout();
            __layout.Add(__label);
            __layout.Add(__textBox);
            __textBox.TextChanged += TextBox_TextChanged;

            Header = __layout;
        }

        ObjectTreeEditor __objectEditor;
        private object __object;
        bool __editable = false;
        //

        private Label __label = new Label();
        private TextBox __textBox = new TextBox();
    }

}
