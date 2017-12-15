using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CFL_1.CFL_System.SqlServerOrm;
using CFL_1.CFL_Data.Defunts;
using MSTD;
using RuntimeExec;
using System.Collections.Generic;
using ObjectEdit;
using CFL_1.CFL_System;
using MSTD.ShBase;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CFL_1.CFL_System.MSTD;
using CFL_1.CFL_System.DB;
using CFL_1.CFL_Data;
using SqlOrm;
using Npgsql;
using Xceed.Wpf.Toolkit;
using CustomControls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using CFL_1.CFLGraphics.CFLControls;

namespace CFL_1.CFLGraphics
{
    public class Form_test : CFLForm
    {
        public Form_test()
        { init(); }

        #region CFLForm methods
        public override void BecomeCurrent()
        {}

        public override void GetNotification(DBNotification _notification)
        {
            //
        }

        public override bool Save()
        { return false; }
        public override bool NewOne()
        { return false; }
        public override bool DeleteCurrent()
        { return false; }
        public override void Documents()
        {}

        #endregion CFLForm methods

        private Button buttonExe;
        private Button buttonB;

        private void buttonExe_Click(object sender, RoutedEventArgs e)
        {
            DBContext_CFL _context = DBContext_CFL.instance;

            Defunt dft = null;

            //var select = new DBSelect<Defunt>("*")
            //    .Where
            //    (
            //        new MemberPath(()=>dft.Identite), "=", new DBSelect().From("identite").Where
                    
            //        (
            //            new MemberPath(()=>((Identite)null).Nom), "=", new DBValue("DUPONT3"),
            //            "or", new MemberPath(()=>dft.Identite.Nom), "=", new DBValue("DUPONT2")
            //        ),

            //        "or",
                    
            //        new MemberPath(()=>dft.Pouvoir), "=", new DBSelect().From("pouvoir").Where
            //        (
            //            new MemberPath(()=>dft.Pouvoir.Identite), "=", new DBSelect().From("identite").Where
            //            (
            //                new MemberPath(()=>((Identite)null).Nom), "==", new DBValue("POUVOIR4"),
            //                "and", new MemberPath(()=>((Defunt)null).Pouvoir.Identite.Prenom), "==", new DBValue("Pouvoir4")
            //            )
            //        )
            //     )
            //    ;

            DBSelect _select = new DBSelect("*").From("defunt")
                              .Where
                              (
                                    new DBOnList(()=>((Defunt)null).OperationsFuneraires)
                                    .Contains
                                    (
                                        new DBSelect().From("inhumation").Where
                                        (
                                            new MemberPath(()=>((Inhumation)null).date), "=", new DBValue(new DateTime(2017, 12, 6))
                                        )
                                    )
                
                              );
                        
            string query = _select.Query();

            List<Defunt> dfts = new DBLoader<Defunt>(_context.Connection, _context).IncludeCascade().ToList(_select);


        }

        private void RowQuery()
        {
            DBContext_CFL _context = DBContext_CFL.instance;
            var _con = _context.Connection.Connection;

            string query = "SELECT * FROM defunt;";
            NpgsqlCommand _command = new NpgsqlCommand(query);
            _command.Connection = _con;
            NpgsqlDataReader _reader =  _command.ExecuteReader();
            
            using(_reader)
            {
                while (_reader.Read())
                {
                    int n = _reader.FieldCount;
                }
            }
        }

        private void PopulateDB()
        {
            DBContext_CFL _context = DBContext_CFL.instance;

            Defunt dft = null;

            for(int _i = 0; _i < 10 ; _i++)
            {
                dft = new Defunt();
                dft.Identite.Nom = "DUPONT" + _i.ToString();
                dft.Identite.Prenom = "Jean"+_i.ToString();
                dft.Pouvoir.Qualite = Qualite.Fils;
                dft.Pouvoir.Identite.Nom = "POUVOIR" + _i.ToString();
                dft.Pouvoir.Identite.Prenom = "Pouvoir" + _i.ToString();
                _context.GetOrAttach(dft);
            }
            _context.SaveChanges();

        }

        private void buttonB_Click(object sender, RoutedEventArgs e)
        {
            Defunt _defunt = Dft;
        }

        Defunt Dft = new Defunt();
        ObjectEditControl objEdit = new ObjectEditControl();

        void init()
        {
            buttonExe = new Button() { Content = "exe" };
            buttonExe.Width = 150;
            buttonExe.Click += buttonExe_Click;
            AddElementToLayoutMenuTop(buttonExe);

            buttonB = new Button() { Content = "exe 2" };
            buttonB.Width = 150;
            buttonB.Click += buttonB_Click;
            AddElementToLayoutMenuTop(buttonB);

            TextBoxDate maskedTextBox = new TextBoxDate();
            maskedTextBox.Mask = "00/00/0000";
            AddElementToLayoutMenuTop(maskedTextBox);

            
            List<Commune> _l = null;
            Gate.Load.Communes(ref _l);

            Dft.Identite.Nom = "MARIE";
            Dft.Identite.Prenom = "Pascal";
            Dft.Coordonnees.adress1 = "route de l'église";

            Dft.Pouvoir.Qualite = Qualite.Fille;
           
            
            //objEdit.SetConfigFor(
            //    typeof(Commune), 
            //    _l, 
            //    new DataDisplay(new REMemberExpression(typeof(Commune), ()=>((Commune)null).nom),
            //                    new REValue(" "),
            //                    new REMemberExpression(typeof(Commune), ()=>((Commune)null).codePost))
            //                    );

            //objEdit.Object = Dft.Pouvoir;

            //PropertyGrid prGrid = new PropertyGrid();
            //AddElementToRootLayout(prGrid);
            //prGrid.SelectedObject = Dft.Coordonnees;

            CTRL_FicheDefunt _ficheDefunt = new CTRL_FicheDefunt();
            AddElementToRootLayout(_ficheDefunt);
            _ficheDefunt.Defunt = Dft;
        }
    }
    
}
