using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using MSTD;
using MSTD.ShBase;
using ShLayouts;

namespace ObjectEdit
{
    /// <summary>
    /// <see cref="PropertyConfig"/> associe une propriété à un <see cref="PropertyEditControl"/>.
    /// Les trois façon d'obtenir un control pour la propriété sont
    /// set <see cref="PropertyConfig.PropertyEditControl"/>,
    /// set <see cref="PropertyConfig.ControlType"/>,
    /// ou <see cref="PropertyConfig.Build"/>.
    /// </summary>
    public class PropertyConfig
    {
        public PropertyConfig(PropertyInfo prInfo)
        {
            PropertyInfo = prInfo;
        }

        public PropertyInfo PropertyInfo{ get; private set; }

        public PropertyEditControl PropertyEditControl
        { 
            get
            {
                Build();
                return __control;
            }
            set
            {
                __control = value;
                if(__control == null)
                    __controlType = null;
                else
                {
                    __controlType = __control.GetType();
                    __control.PropertyInfo = PropertyInfo;
                }
            }
        }

        public Type ControlType
        {
            get => __controlType;
            set
            {
                __controlType = value;
                Build();
            }
        }

        public string Label
        {
            get
            {
                if(string.IsNullOrWhiteSpace(__label))
                    __label = PropertyHelper.GetNameToDisplay(PropertyInfo);
                return __label;
            }
            set => __label = value;
        }

        public void Build()
        {
            if(__control == null)
            {
                if(__controlType != null)
                    __control = (PropertyEditControl)(Activator.CreateInstance(__controlType));
                else
                {
                    Type _t = PropertyInfo.PropertyType;
                    if(_t.IsEnum)
                        __control = new PropertyEnumClontrol();
                    else if(_t == typeof(Base) || _t.IsSubclassOf(typeof(Base)))
                        __control = new PropertyObjectSelectionControl();
                    else if(__propertiesTypes_controlsTypes.ContainsKey(_t))
                        __control = (PropertyEditControl)(Activator.CreateInstance(__propertiesTypes_controlsTypes[_t]));
                }
            }
            if(__control != null)
                __control.PropertyInfo = PropertyInfo;
        }

        private PropertyEditControl __control = null;
        private Type __controlType = null;

        private string __label = "";

        private static Dictionary<Type, Type> __propertiesTypes_controlsTypes = new Dictionary<Type, Type>()
        {
            { typeof(string)    , typeof(PropertyTextControl)     },
            { typeof(bool)      , typeof(PropertyBoolControl)     },
            { typeof(int)       , typeof(PropertyIntControl)      },
            { typeof(long)      , typeof(PropertyLongControl)     },
            { typeof(double)    , typeof(PropertyDoubleControl)   },
            { typeof(DateTime?) , typeof(PropertyDateControl)     },
            { typeof(TimeSpan?) , typeof(PropertyTimeSpanControl) },
            { typeof(Enum)      , typeof(PropertyEnumClontrol)    }
        };
    }

    public class PropertiesEditControl : VBoxLayout
    {
        /// <summary>
        /// Donne obj à tous les <see cref="PropertyEditControl"/> présents dans ce <see cref="PropertiesEditControl"/>.
        /// L'utilisation de cette propriété suppose que toutes les propriétés éditées dans ce <see cref="PropertiesEditControl"/>
        /// soient les propriétés d'un même type de classe.
        /// </summary>
        public Base Object
        {
            get => __object;

            set
            {
                __object = value;

                foreach(PropertyConfig _prConfig in __propertiesConfigs)
                {
                    PropertyEditControl _ctrl = _prConfig.PropertyEditControl;
                    if(_ctrl != null)
                        _prConfig.PropertyEditControl.Object = __object;
                }
            }
        }

        private Base __object;

        /// <summary>
        /// Ajoute la <see cref="PropertyConfig"/>,
        /// ou remplace une <see cref="PropertyConfig"/> pour la même propriété.
        /// </summary>
        public void Add(PropertyConfig propertyConfig)
        {
            int _index = IndexOf(propertyConfig.PropertyInfo);
            if(_index == -1)
                __propertiesConfigs.Add(propertyConfig);
            else
            {
                __propertiesConfigs.RemoveAt(_index);
                __propertiesConfigs.Insert(_index, propertyConfig);
            }

            PropertyEditControl _ctrl = propertyConfig.PropertyEditControl;
            if(_ctrl != null && Object != null)
                _ctrl.Object = Object;
        }

        /// <summary>
        /// Retourne le <see cref="PropertyConfig"/> pour la propriété prInfo,
        /// ou null si non trouvé.
        /// </summary>
        public PropertyConfig GetPropertyConfig(PropertyInfo prInfo)
        {
            foreach(PropertyConfig prConfig in __propertiesConfigs)
            {
                if(prConfig.PropertyInfo == prInfo)
                    return prConfig;
            }
            return null;
        }

        /// <summary>
        /// Retourne la position du <see cref="PropertyConfig"/> pour la propriété prInfo,
        /// ou -1 si non trouvé.
        /// </summary>
        public int IndexOf(PropertyInfo prInfo)
        {
            for(int _i = 0; _i < __propertiesConfigs.Count; _i++)
            {
                if(__propertiesConfigs[_i].PropertyInfo == prInfo)
                    return _i;
            }
            return -1;
        }

        #region Rows min and max height

        public void SetRowsMinMaxHeight(double minHeight, double maxHeight)
        {
            __rowsMinHeight = minHeight;
            __rowsMaxHeight = maxHeight;
            Build();
        }

        private double __rowsMinHeight = 27;
        private double __rowsMaxHeight = double.PositiveInfinity;

        #endregion Rows min and max height

        #region Init and Build

        public void Build()
        {
            Init();
            
            foreach(PropertyConfig prConfig in __propertiesConfigs)
            {
                __formLayout.Add(prConfig.Label, prConfig.PropertyEditControl);
            }
        }

        private void Init()
        {
            if(ShowHeader)
                showHeader();
            Remove(__formLayout);

            __formLayout = new FormLayout();

            Add(__formLayout);
        }

        private FormLayout __formLayout = null;

        #endregion Init and Build

        public delegate void ValueChanged(PropertyEditControl control);
        public ValueChanged ValueChangedEvent;

        #region Show header

        public Brush HeaderBackground
        {
            get => __headerBackGround;

            set
            {
                __headerBackGround = value;
                if(__labelHeader != null)
                    __labelHeader.Background = __headerBackGround;
            }
        }

        /// <summary>
        /// get : 
        /// Retourne la valeur qui a été donnée par un set,
        /// ou par défaut le nom du type de <see cref="Object"/>.
        /// </summary>
        public string Header
        {
            get// =>(!string.IsNullOrWhiteSpace(__nameLabel))? __nameLabel : Object.GetType().Name;
            {
                if(!string.IsNullOrWhiteSpace(__nameLabel))
                    return __nameLabel;
                if(Object != null)
                    return Object.GetType().Name;
                return "";
            }
            private set => __nameLabel = value;
        }

        public bool ShowHeader
        {
            get => __showHeader;
            set
            {
                __showHeader = value;
                if(__showHeader)
                    showHeader();
                else
                    RemoveHeader();
            }
        }

        private void showHeader()
        {
            RemoveHeader();
            __labelHeader = new Label()
            { 
                Height = 27,// ControlsHeight, 
                Background = __headerBackGround,
                Content = Header
            };
            Insert(0, __labelHeader);
        }

        private void RemoveHeader()
        {
            //Remove(null) est permis
            Remove(__labelHeader);
            __labelHeader = null;
        }

        private Label __labelHeader = null;
        private string __nameLabel = "";
        private bool __showHeader = false;
        private Brush __headerBackGround = Brushes.LightGray;

        #endregion Show header

        private List<PropertyConfig> __propertiesConfigs = new List<PropertyConfig>();

        private Type __objectType = null;

       
    }
}
