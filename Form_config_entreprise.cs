
using System;
using CFL_1.CFL_System.DB;
using CFL_1.CFLGraphics.GraphEditor;
using CFL_1.CFLGraphics.MyControls.GraphEditor;
using CFL_1.CFL_Data;
using CFL_1.CFL_System.SqlServerOrm;
using SqlOrm;

namespace CFL_1.CFLGraphics
{
    class Form_config_entreprise : CFLForm
    {
        public Form_config_entreprise()
        {
            Init();
            buttonNew = true;
            buttonSave = true;
        }

        public override void BecomeCurrent()
        {
            if(__graphControl.Project == null)
                LoadProject();
            if(__graphControl.Project == null)
                NewOne();
        }

        public override bool DeleteCurrent()
        {
            throw new NotImplementedException();
        }

        public override void Documents()
        {
            throw new NotImplementedException();
        }
        
        public override void GetNotification(DBNotification _notification)
        {
            //TODO
            if(!_notification.IsSentBySelf)
            {
                 
            }
        }

        public override bool NewOne()
        {
            GraphProject _project = new GraphProject();
            _project.ProjectName = "Configuration entreprise";
            __graphControl.Project = _project;
            return true;
        }

        public override bool Save()
        {
            DBContext_CFL.instance.GetOrAttach(__graphControl.Project);
            return DBContext_CFL.instance.SaveChanges();
        }

        private void LoadProject()
        {
            GraphProject _project = null;

            DBContext_CFL dbContext = DBContext_CFL.instance;
            
            _project = new DBLoader<GraphProject>
                (dbContext.Connection, dbContext).First
                (new DBSelect<GraphProject>("*").Where("ProjectName = 'Configuration entreprise'"));

            if(_project != null)
            {
                foreach(ShapeTypeInfo _shapeInfo in _project.ShapeInfos)
                {
                    __graphControl.TypesDescription.CompleteShapeTypeInfo(_shapeInfo);
                }
            }
            
            __graphControl.Project = _project;
        }

        private void Init()
        {
            double _smallHeight = 25;
            double _smallWidth = 60;

            double _mediumHeight = 40;
            double _mediumWidth = 90;

            GraphTypesDescription _descriptions = new GraphTypesDescription();
            
            ShapeTypeInfo _entreprise = _descriptions.AddType("Entreprise", "entreprise");
            _entreprise.IsRootType = true;

            ShapeTypeInfo _accueil = _descriptions.AddType("Accueil", "accueil");
            _accueil.Height = _mediumHeight;
            _accueil.Width = _mediumWidth;

            ShapeTypeInfo _centreFune = _descriptions.AddType("Centre funéraire", "centrefuneraire");

            ShapeTypeInfo _pf = _descriptions.AddType("Pompe funèbre", "pompefunebre");

            ShapeTypeInfo _chambreFune = _descriptions.AddType("Chambre funéraire", "chambrefuneraire");
                ShapeTypeInfo _salon = _descriptions.AddType("Salon", "salon");
                _salon.Height = _smallHeight;
                _salon.Width = _smallWidth;
                ShapeTypeInfo _case = _descriptions.AddType("Case", "case");
                _case.Height = _smallHeight;
                _case.Width = _smallWidth;
                ShapeTypeInfo _salleSoins = _descriptions.AddType("Salle de soins", "sallesoins");
                _salleSoins.Height = _smallHeight;
                _salleSoins.Width = _smallWidth;

            ShapeTypeInfo _crema = _descriptions.AddType("Crématorium", "crematorium");
            ShapeTypeInfo _four = _descriptions.AddType("Four", "four");
            _four.Height = _smallHeight;
            _four.Width = _smallWidth;

            ShapeTypeInfo _serviceCimetiere = _descriptions.AddType("Service cimetière", "servicecimetiere");

            ShapeTypeInfo _salleCeremonie = _descriptions.AddType("Salle de cérémonie", "salleceremonie");
            _salleCeremonie.Height = _smallHeight;
            _salleCeremonie.Width = _smallWidth;

            ShapeTypeInfo _personnelAccueil = _descriptions.AddType("Personnel accueil", "personnel_accueil");
            _personnelAccueil.Height = _mediumHeight;
            _personnelAccueil.Width = _mediumWidth;
            ShapeTypeInfo _personnelPf = _descriptions.AddType("Personnel pompe funèbre", "personnel_pompefunebre");
            _personnelPf.Height = _mediumHeight;
            _personnelPf.Width = _mediumWidth;
            ShapeTypeInfo _personnelCrema = _descriptions.AddType("Personnel crématorium", "personnel_crematorium");
            _personnelCrema.Height = _mediumHeight;
            _personnelCrema.Width = _mediumWidth;
            ShapeTypeInfo _personnelChambreFune = _descriptions.AddType("Personnel chambre funéraire", "personnel_chambrefuneraire");
            _personnelChambreFune.Height = _mediumHeight;
            _personnelChambreFune.Width = _mediumWidth;
            ShapeTypeInfo _personnelCimetiere = _descriptions.AddType("Personnel cimetière", "personnel_cimetiere");
            _personnelCimetiere.Height = _mediumHeight;
            _personnelCimetiere.Width = _mediumWidth;

            ShapeTypeInfo _metier = _descriptions.AddType("Métier", "metier");
            _metier.Height = _mediumHeight;
            _metier.Width = _mediumWidth;

            ShapeTypeInfo _utilisateur = _descriptions.AddType("Utilisateur", "utilisateur");
            _utilisateur.IsReusable = true;
            _utilisateur.Height = _smallHeight;
            _utilisateur.Width = _smallWidth;

            ShapeTypeInfo _planingJournalier = _descriptions.AddType("Planing journalier", "planing_jours");
            _planingJournalier.Height = _smallHeight;
            _planingJournalier.Width = _smallWidth;

            ShapeTypeInfo _zone = _descriptions.AddType("Zone", "zone");
            _zone.Height = _smallHeight;
            _zone.Width = _smallWidth;
            
            ShapeTypeInfo _action = _descriptions.AddType("Action", "action");
            _action.Height = _smallHeight;
            _action.Width = _smallWidth;

            //// Rules

            _descriptions.SetRule(_entreprise,
                _accueil,
                _centreFune,
                _pf,
                _crema,
                _chambreFune,
                _serviceCimetiere
                );

            _descriptions.SetRule(_centreFune,
                _accueil, 1);
            _descriptions.SetRule(_centreFune,
                _pf,
                _crema,
                _chambreFune
                );

            _descriptions.SetRule(_pf, 
                _accueil, 1);
            _descriptions.SetRule(_pf,
                _personnelPf, 1);
            // pour permetre de configurer les autorisations relatives au planing pf
            _descriptions.SetRule(_pf,
                _planingJournalier, 1);

            _descriptions.SetRule(_chambreFune,
                _accueil, 1);
            _descriptions.SetRule(_chambreFune, 
                _personnelChambreFune, 1);
            _descriptions.SetRule(_chambreFune,
                _salon,
                _case);

            _descriptions.SetRule(_crema,
                _accueil, 1);
            _descriptions.SetRule(_crema,
                _personnelCrema, 1);
            _descriptions.SetRule(_crema,
                _four);
            _descriptions.SetRule(_crema,
                _planingJournalier, 1);

            _descriptions.SetRule(_serviceCimetiere,
                _personnelCimetiere, 1);
            
            _descriptions.SetRule(_accueil,
                _personnelAccueil, 1);

            _descriptions.SetRule(_personnelAccueil,
                _metier);
            
            _descriptions.SetRule(_personnelPf,
                _metier);
            
            _descriptions.SetRule(_personnelChambreFune,
                _metier);

            _descriptions.SetRule(_personnelCrema,
                _metier);

            _descriptions.SetRule(_personnelCimetiere,
                _metier);

            _descriptions.SetRule(_metier,
                _utilisateur);

            _descriptions.SetRule(_planingJournalier,
                _zone);

            _descriptions.SetRule(_zone,
                _action);

            #region Data

            _planingJournalier.ClaimComponentToEdit(_utilisateur, typeof(Autorisation));

            _personnelAccueil.ClaimComponentToEdit(_utilisateur, typeof(Autorisation));
            _personnelPf.ClaimComponentToEdit(_utilisateur, typeof(Autorisation));
            _personnelChambreFune.ClaimComponentToEdit(_utilisateur, typeof(Autorisation));
            _personnelCrema.ClaimComponentToEdit(_utilisateur, typeof(Autorisation));
            _personnelCimetiere.ClaimComponentToEdit(_utilisateur, typeof(Autorisation));

            _entreprise.AddEditableComponent(new Entreprise());

            _pf.AddEditableComponent(new Pf());

            _crema.AddEditableComponent(new Crematorium());
            _four.AddEditableComponent(new Four());

            _chambreFune.AddEditableComponent(new ChambreFuneraire());

            _metier.AddEditableComponent(new Metier());

            _utilisateur.AddEditableComponent(new Utilisateur());

            _salon.AddEditableComponent(new Salon());
            _case.AddEditableComponent(new CaseRefrigeree());


            _planingJournalier.AddEditableComponent(new PlaningJournalier());
            _zone.AddEditableComponent(new ZoneInfo());
            _action.AddEditableComponent(new ZoneAction());

            #endregion Data

            #region Show tooltips

            Entreprise _ent = _entreprise.ComponentOfType<Entreprise>();
            _entreprise.AddToolTip(_ent, () => _ent.raisonSociale.nom);

            Utilisateur __utilisateur = _utilisateur.ComponentOfType<Utilisateur>();
            _utilisateur.AddToolTip(__utilisateur, () => __utilisateur.Identite.Prenom); 
            _utilisateur.AddToolTip(__utilisateur, () => __utilisateur.Identite.Nom);

            Metier _met = _metier.ComponentOfType<Metier>();
            _metier.AddToolTip(_met, () => _met.Intitule);

            #endregion Show tooltips

            #region Events

            _metier.OnAcceptShapeTypeInfo += delegate(ShapeTypeInfo _acceptor, ShapeTypeInfo _accepted) 
            {
                if(_accepted.TypeName == _utilisateur.TypeName)
                {
                    Metier _mt = _acceptor.ComponentOfType<Metier>();
                    Utilisateur _ut = _accepted.ComponentOfType<Utilisateur>();
                    if(!_mt.Utilisateurs.Contains(_ut))
                        _mt.Utilisateurs.Add(_ut);
                }
            };

            _metier.OnRemoveShape += delegate(ShapeTypeInfo _acceptor, ShapeTypeInfo _removed)
            {
                Metier _mt = _acceptor.ComponentOfType<Metier>();
                if(_removed.TypeName == _utilisateur.TypeName)
                {
                    Utilisateur _ut = _removed.ComponentOfType<Utilisateur>();
                    int _index = _mt.Utilisateurs.IndexOf(_ut);
                    if(_index != -1)
                        _mt.Utilisateurs.RemoveAt(_index);
                }
            };

            _chambreFune.OnAcceptShapeTypeInfo += delegate(ShapeTypeInfo _acceptor, ShapeTypeInfo _accepted)
            { 
                if(_accepted.TypeName == _salon.TypeName)
                {
                    ChambreFuneraire _cf = _acceptor.ComponentOfType<ChambreFuneraire>();
                    _cf.salons.Add(_accepted.ComponentOfType<Salon>());
                }
                else
                if(_accepted.TypeName == _case.TypeName)
                {
                    ChambreFuneraire _cf = _acceptor.ComponentOfType<ChambreFuneraire>();
                    _cf.cases.Add(_accepted.ComponentOfType<CaseRefrigeree>());
                }
            };

            _chambreFune.OnRemoveShape += delegate(ShapeTypeInfo _acceptor, ShapeTypeInfo _removed)
            {
                if(_removed.TypeName == _salon.TypeName)
                {
                    ChambreFuneraire _cf = _acceptor.ComponentOfType<ChambreFuneraire>();
                    _cf.salons.Remove(_removed.ComponentOfType<Salon>());
                }
                else
                if(_removed.TypeName == _case.TypeName)
                {
                    ChambreFuneraire _cf = _acceptor.ComponentOfType<ChambreFuneraire>();
                    _cf.cases.Remove(_removed.ComponentOfType<CaseRefrigeree>());
                }
            };

            _planingJournalier.OnAcceptShapeTypeInfo += delegate(ShapeTypeInfo _acceptor, ShapeTypeInfo _accepted)
            {
                if(_accepted.TypeName == _zone.TypeName)
                {
                    PlaningJournalier _pl = _acceptor.ComponentOfType<PlaningJournalier>();
                    if(_pl.pageModel == null)
                        _pl.pageModel = new PageJour();
                    _pl.pageModel.zones.Add(_zone.ComponentOfType<ZoneInfo>());
                }
            };

            _planingJournalier.OnRemoveShape += delegate(ShapeTypeInfo _acceptor, ShapeTypeInfo _removed)
            {
                if(_removed.TypeName == _zone.TypeName)
                {
                    PlaningJournalier _pl = _acceptor.ComponentOfType<PlaningJournalier>();
                    ZoneInfo _removedZone = _removed.ComponentOfType<ZoneInfo>();
                    _pl.pageModel.zones.Remove(_removedZone);
                }
            };

            _zone.OnAcceptShapeTypeInfo += delegate(ShapeTypeInfo _acceptor, ShapeTypeInfo _accepted)
            {
                if(_accepted.TypeName == _action.TypeName)
                {
                    ZoneInfo _zoneInfo = _acceptor.ComponentOfType<ZoneInfo>();
                    _zoneInfo.actions.Add(_accepted.ComponentOfType<ZoneAction>());
                }
            };   
            
            _zone.OnRemoveShape += delegate(ShapeTypeInfo _acceptor, ShapeTypeInfo _removed)
            {
                if(_removed.TypeName == _action.TypeName)
                {
                    ZoneInfo _zoneInfo = _acceptor.ComponentOfType<ZoneInfo>();
                    _zoneInfo.actions.Remove(_removed.ComponentOfType<ZoneAction>());
                }
            };

            #endregion Events

            __graphControl = new GraphControl(_descriptions);

            AddElementToRootLayout(__graphControl);
        }

        private GraphControl __graphControl;
    }
}
