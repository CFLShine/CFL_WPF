using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using MSTD;
using MSTD.ShBase;
using RuntimeExec;
using ShLayouts;

namespace ObjectEdit
{
    public class ObjectEditControl : VBoxLayout
    {
        public ObjectEditControl(){ }

        public ObjectEditControl(Base obj)
        {
            Object = obj;
        }

        public new void Clear()
        {
            Object = null;
        }

        #region include sub objects

        public bool IncludeSubObjects
        {
            get;
            set;
        }

        #endregion include sub objects

        /// <summary>
        /// objet édité.
        /// set : appèle Build().
        /// </summary>
        public Base Object
        {
            get => __object;
            set
            {
                __object = value;
                Build();
            }
        }

        public void AddPropertyConfig(PropertyConfig prConfig)
        {
            __properties_controls[prConfig.PropertyInfo] = prConfig;
        }

        public void AddPropertyConfig(PropertyInfo prInfo, PropertyEditControl prEditControl)
        {
            PropertyConfig prConfig = new PropertyConfig(prInfo);
            prConfig.PropertyEditControl = prEditControl;
            AddPropertyConfig(prConfig);
        }

        public void AddPropertyConfig(MemberPath path, PropertyEditControl prEditControl)
        {
            AddPropertyConfig(path.LastPropertyInfo, prEditControl);
        }

        #region Exclude

        public void Exclude(REMemberExpression _expr)
        {
            Exclude(_expr.LastMemberName());
        }
        
        public void Exclude(string _propertyName)
        {
            __excludeds.Add(_propertyName);
        }

        private List<string> __excludeds = new List<string>();

        #endregion Exclude

        /// <summary>
        /// Valeur par défaut : 27
        /// </summary>
        public double ControlsHeight 
        { 
            get => __controlsHeight;
            set
            {
                __controlsHeight = value;
            }
        }

        public double LabelsMinimumWidth{ get; set; } = 150;
        public double LabelsMaximumWidth{ get; set; } = double.PositiveInfinity;
        public double ControlsMinimumWidth{ get; set; } = 150;
        public double ControlsMaximumWidth{ get; set; } = double.PositiveInfinity;


        #region Build
        /// <summary>
        /// Peuple le <see cref="EditControl"/>
        /// </summary>
        public void Build()
        {
            Init();

            if(Object != null)
            {
                foreach(PropertyInfo _prInfo in Object.GetType().GetProperties())
                {
                    if(IsElligible(_prInfo))
                    {
                        if(__properties_controls.ContainsKey(_prInfo))
                            AddToCurrentPropertiesEditControl(__properties_controls[_prInfo]);
                        else
                        {
                            Type _t = _prInfo.PropertyType;
                            if(_t == typeof(Base) || _t.IsSubclassOf(typeof(Base)) && IncludeSubObjects)
                            {
                                FinalyseCurrentPropertiesEditControl();
                                
                                Base _obj = _prInfo.GetValue(Object) as Base;

                                if(_obj != null)
                                {
                                    ObjectEditControl _objEditControl = new ObjectEditControl();
                                    _objEditControl.ShowHeader = true;
                                    // todo header background
                                    _objEditControl.Object = _obj;
                                    Add(_objEditControl);
                                }
                            }
                            else
                            {
                                PropertyConfig _prConfig = new PropertyConfig(_prInfo);
                                AddToCurrentPropertiesEditControl(_prConfig);
                            }
                        }
                    }
                }
                FinalyseCurrentPropertiesEditControl();
            }
        }

        private void AddToCurrentPropertiesEditControl(PropertyConfig prConfig)
        {
            if(prConfig.PropertyEditControl != null) // sinon, prConfig crée avec un type de propriété non supporté.
            {
                if(__currentPropertiesEditControl == null)
                {
                    __currentPropertiesEditControl = new PropertiesEditControl();
                    __currentPropertiesEditControl.Object = Object;
                    Add(__currentPropertiesEditControl);
                }
                __currentPropertiesEditControl.Add(prConfig);
            }
        }

        private void FinalyseCurrentPropertiesEditControl()
        {
            if(__currentPropertiesEditControl != null)
            {
                __currentPropertiesEditControl.Build();
                __currentPropertiesEditControl = null;
            }
        }

        private PropertiesEditControl __currentPropertiesEditControl = null;

        private Dictionary<PropertyInfo, PropertyConfig> __properties_controls = new Dictionary<PropertyInfo, PropertyConfig>();

        #endregion Build

        public delegate void ValueChanged(PropertyEditControl control);
        public ValueChanged ValueChangedEvent;

        /// <summary>
        /// Retourne le <see cref="PropertyConfig"/> qui expose la propriété prInfo,
        /// ou null si non trouvé.
        /// </summary>
        public PropertyConfig GetPropertyEditControl(PropertyInfo prInfo)
        {
            PropertyConfig _ctrl = null;
            __properties_controls.TryGetValue(prInfo, out _ctrl);
            return _ctrl;
        }

        protected void OnValueChanged(PropertyEditControl control)
        {
            ValueChangedEvent?.Invoke(control);
        }

        private string GetLabel(PropertyInfo prInfo)
        {
            return PropertyHelper.GetNameToDisplay(prInfo);
        }

        private bool IsElligible(PropertyInfo prInfo)
        {
            Type _t = prInfo.PropertyType;

            DisplayAttribute _att = prInfo.GetCustomAttribute<DisplayAttribute>();
            if(_att != null && _att.GetAutoGenerateField() == false)
            {
                return false;
            }

            if(_t.IsPublic == false
            || prInfo.CanRead == false
            || prInfo.CanWrite == false)
                return false;

            foreach(string _name in __excludeds)
            {
                if(_name == prInfo.Name)
                    return false;
            }

            return true;
        }

        private void Init()
        {
            base.Clear();
            __labelHeader = null;
            if(ShowHeader)
                showHeader();
            else RemoveHeader();
        }

        #region show header

        /// <summary>
        /// get : 
        /// Retourne la valeur qui a été donnée par un set,
        /// ou par défaut le nom du type de <see cref="Object"/>.
        /// </summary>
        public string Header
        {
            get
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
            if(__labelHeader == null)// sinon, déja ajouté
            {
                __labelHeader = new Label()
                { 
                    Height = 27,// ControlsHeight, 
                    Background = Brushes.Gray 
                };
                Insert(0, __labelHeader);
            }
            __labelHeader.Content = Header;
        }

        private void RemoveHeader()
        {
            //Remove(null) est permis
            Remove(__labelHeader);
            __labelHeader = null;
        }

        private Label __labelHeader = null;

        private bool __showHeader = true;

        #endregion show header

        private Base __object = null;

        private double __controlsHeight = 27;

        private string __nameLabel = "";
        

        

    }
}
