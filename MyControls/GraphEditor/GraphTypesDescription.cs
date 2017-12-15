using System.Collections.Generic;
using CFL_1.CFL_System.MSTD;
using CFL_1.CFLGraphics.MyControls.GraphEditor;
using MSTD;
using MSTD.ShBase;
using RuntimeExec;

namespace CFL_1.CFLGraphics.GraphEditor
{
    public class GraphTypesDescription
    {
        /// <summary>
        /// _name est le nom qui apparaitra sur la GraphShape
        /// </summary>
        public ShapeTypeInfo AddType(string _designation, string _typename)
        {
            ShapeTypeInfo _type = new ShapeTypeInfo(_designation, _typename);
            __types.Add(_type);
            return _type;
        }

        public void SetRule(ShapeTypeInfo _type, params ShapeTypeInfo[] _acceptables)
        {
            foreach(ShapeTypeInfo _t in _acceptables)
                _type.SetRule(_t.TypeName, -1);
        }

        public void SetRule(ShapeTypeInfo _type, ShapeTypeInfo _acceptable, int _howMany)
        {
            _type.SetRule(_acceptable.TypeName, _howMany);
        }

        public ShapeTypeInfo RootType()
        {
            foreach(ShapeTypeInfo _type in __types)
            {
                if(_type.IsRootType)
                    return _type;
            }
            return null;
        }

        private ShapeTypeInfo GetTypeInfo(string _name)
        {
            foreach(ShapeTypeInfo _type in __types)
            {
                if(_type.TypeName == _name)
                    return _type;
            }
            return null;
        }

        public GraphShape ShapeFactory(string _typename)
        {
            ShapeTypeInfo _type = GetTypeInfo(_typename);
            if(_type == null)
                return null;
            
            ShapeTypeInfo _new = new ShapeTypeInfo(_type.Designation, _type.TypeName);
            
            foreach(Base _component in  _type.EditableComponents)
            {
                _new.AddEditableComponent((Base)(TypeHelper.NewInstance(_component)));
            }
            CompleteShapeTypeInfo(_new);
            return new GraphShape(_new);
        }

        /// <summary>
        /// Complète un ShapeTypeInfo en lui donnant ce que l'on peut retrouver
        /// depuis ce modèle et qu'il n'est pas utile de sauvegarder, ex les règles, IsReusable, ...
        /// </summary>
        /// <param name="_type"></param>
        public void CompleteShapeTypeInfo(ShapeTypeInfo _type)
        {
            ShapeTypeInfo _model = GetTypeInfo(_type.TypeName);
            _type.IsReusable = _model.IsReusable;
            _type.IsRootType = _model.IsRootType;
            _type.Height = _model.Height;
            _type.Width = _model.Width;

            foreach(GraphRule _rule in _model.Rules())
                _type.SetRule(_rule);
            foreach(ClassClaimer _claimer in _model.ClaimersForEdit)
                _type.ClaimComponentToEdit(_claimer);
            _type.OnAcceptShapeTypeInfo += _model.OnAcceptShapeTypeInfo;

            foreach(REMemberExpression _memberExpression in _model.PropertyHoldersForToolTip)
                _type.AddToolTip(_memberExpression);
        }

        private List<ShapeTypeInfo> __types = new List<ShapeTypeInfo>();

    }
}
