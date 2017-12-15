
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using MSTD;
using MSTD.ShBase;
using RuntimeExec;

namespace CFL_1.CFLGraphics.MyControls.GraphEditor
{
    public class ShapeTypeInfo : Base
    {
        public ShapeTypeInfo(){ }

        public ShapeTypeInfo(string _designation, string _typename)
        {
            Designation = _designation;
            TypeName = _typename;
        }

        public ShapeTypeInfo(ShapeTypeInfo _typeInfo)
        {
            foreach(Base _editableComponent in _typeInfo.EditableComponents)
            {
                EditableComponents.Add(_editableComponent);
            }
            foreach(ClassClaimer _claimer in _typeInfo.ClaimersForEdit)
            {
                ClaimersForEdit.Add(_claimer);
            }
        }

        public string TypeName { get ; set ; }

        public double LeftPosition { get ; set ; } = 0;

        public double TopPosition { get ; set ; } = 0;

        public bool IsOnGraph {  get ; set ; }

        public string Designation { get; set; }

        public bool IsRootType { get; set; } = false;
        
        public bool IsReusable { get ; set ; } = false;

        public double Height { get; set; } = 60;
        public double Width { get; set; } = 120;

        #region Show tooltip
        
        public void AddToolTip(Base _component, Expression<Func<object>> _memberExpression)
        {
            PropertyHoldersForToolTip.Add(new REMemberExpression(new REClassObject(_component), _memberExpression));
        }

        public void AddToolTip(REMemberExpression _memberExpression)
        {
            Base _component = null;
            foreach(Base _base in EditableComponents)
            {
                if(_base.GetType().Name == _memberExpression.ParentTypeName)
                    _component = _base;
            }
            PropertyHoldersForToolTip.Add(new REMemberExpression(new REClassObject(_component), _memberExpression.Display()));
        }

        public string ToolTip
        {
            get
            {
                string _toolTip = "";
                foreach(REMemberExpression _memberExpression in PropertyHoldersForToolTip)
                {
                    Base _component = BaseHelper.ComponentOfType(this, _memberExpression.ParentTypeName);
                    _memberExpression.Parent = new REClassObject(_component);
                    object _value = _memberExpression.CValue;
                    if(_value != null)
                        _toolTip += _value.ToString() + " ";
                }
                return _toolTip;
            }
        }

        public List<REMemberExpression> PropertyHoldersForToolTip = new List<REMemberExpression>();

        #endregion Show tooltip

        /// <summary>
        /// Retourne le premier composant de type T trouvé dans EditableCoponents.
        /// </summary>
        public T ComponentOfType<T>() where T : Base
        {
            foreach(Base _base in EditableComponents)
            {
                if(_base.GetType() == typeof(T))
                    return (T)_base;
            }
            return null;
        }

        #region Accept

        public List<ShapeTypeInfo> AcceptedShapes { get; set; } = new List<ShapeTypeInfo>();

        public ShapeTypeInfo AcceptedBy { get ; set ; }

        public List<GraphRule> Rules()
        {
            return __rules;
        }

        private List<GraphRule> __rules = new List<GraphRule>();

        public void SetRule(string _acceptableType, int _howmany)
        {
            __rules.Add(new GraphRule(_acceptableType, _howmany));
        }

        public void SetRule(GraphRule _rule)
        {
            __rules.Add(_rule);
        }

        public bool CanAccept (string _typename )
        {
            GraphRule _rule = Rule(_typename);
            if(_rule == null)
                return false;

            return _rule.canaccept == -1 || _rule.canaccept > AcceptedsOfType(_typename);
        }

        public void Accept(ShapeTypeInfo _toAccept)
        {
            AcceptedShapes.Add(_toAccept);
            _toAccept.AcceptedBy = this;
            OnAcceptShapeTypeInfo?.Invoke(this, _toAccept);
        }

        /// <summary>
        /// Retire _typeInfo de <see cref="AcceptedShapes"/>,
        /// Invoque <see cref="OnRemoveShape"/>.
        /// </summary>
        public void Remove(ShapeTypeInfo _typeInfo)
        {
            int _index = AcceptedShapes.IndexOf(_typeInfo);
            if(_index != -1)
                AcceptedShapes.RemoveAt(_index);
            OnRemoveShape?.Invoke(this, _typeInfo);
        }

        public List<string> CanAcceptList()
        {
            List<string> _l = new List<string>();
            foreach(GraphRule _rule in __rules)
            {
                string _type = _rule.acceptableType;
                if(CanAccept(_type))
                    _l.Add(_type);
            }
            return _l;
        }

        public GraphRule Rule(string _typename)
        {
            foreach(GraphRule _rule in __rules)
            {
                if(_rule.acceptableType == _typename)
                    return _rule;
            }
            return null;
        }

        public int AcceptedsOfType(string _typename)
        {
            int _accepteds = 0;
            foreach(ShapeTypeInfo _accepted in AcceptedShapes)
            {
                if(_accepted.TypeName == _typename)
                    ++_accepteds;
            }
            return _accepteds;
        }

        #endregion Accept

        #region Events
       
        public delegate void OnAccept(ShapeTypeInfo _this, ShapeTypeInfo _toAccept);
        public OnAccept OnAcceptShapeTypeInfo;
        
        public delegate void OnRemove(ShapeTypeInfo _this, ShapeTypeInfo _removed);
        public OnRemove OnRemoveShape;

        #endregion Events

        #region Components for edition on GraphControl

        public Base AddEditableComponent(Base _component)
        {
            EditableComponents.Add(_component);
            return _component;
        }

        public List<Base> EditableComponents { get; set; } = new List<Base>();

        #endregion Components for edition on GraphControl

        #region Claim component to edition

        [NotMapped]
        public List<ClassClaimer> ClaimersForEdit { get ; set ; } = new List<ClassClaimer>();
        public void ClaimComponentToEdit(ShapeTypeInfo _toType, Type _componentType) 
        {
            ClaimersForEdit.Add(new ClassClaimer(_toType.TypeName, _componentType.Name));
        }
        public void ClaimComponentToEdit(ClassClaimer _claimer)
        {
            ClaimersForEdit.Add(_claimer);
        }

        /// <summary>
        /// Retourne le type du Component que ce GraphTypeInfo réclame à _type
        /// </summary>
        public Type ClaimsComponentTo(ShapeTypeInfo _type)
        {
            foreach(ClassClaimer _claimer in ClaimersForEdit)
            {
                if(_claimer.fromType == _type.TypeName)
                    return SolutionClasses.Type(_claimer.claimedType);
            }
            return null;
        }

        public List<Base> ClaimedComponents { get ; set ; } = new List<Base>();
        public List<ShapeTypeInfo> ClaimedBy { get ; set ; } = new List<ShapeTypeInfo>();

        /// <summary>
        /// Retourne l'objet réclamé par une autre Shape.
        /// </summary>
        public Base GetClaimedComponentBy(ShapeTypeInfo _by)
        {
            for(int _i = 0 ; _i < ClaimedBy.Count ; _i++)
            {
                if(ClaimedBy[_i] == _by)
                    return ClaimedComponents[_i];
            }

            Type _t = _by.ClaimsComponentTo(this);
            if(_t == null)
                return null;
            
            Base _newcomponent =  SolutionClasses.Factory(_t.Name);
            ClaimedComponents.Add(_newcomponent);
            ClaimedBy.Add(_by);
            return _newcomponent;
        }

        #endregion Claim component to edition

    }
}
