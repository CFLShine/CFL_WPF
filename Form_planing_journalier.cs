using System;
using System.Windows.Controls;
using CFL_1.CFL_Data;
using CFL_1.CFL_System.SqlServerOrm;

namespace CFL_1.CFLGraphics
{
    public class Form_planing_journalier : CFLForm
    {
        public Form_planing_journalier()
        {
            init();
        }

        #region CFL_form functions
        
        public override void BecomeCurrent()
        {
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
            
        }

        public override bool NewOne()
        {
            throw new NotImplementedException();
        }

        public override bool Save()
        {
            throw new NotImplementedException();
        }
        
        #endregion CFL_form functions


#pragma warning disable CS0414 // Le champ 'Form_planing_journalier.PageJour' est assigné, mais sa valeur n'est jamais utilisée
        private PageJour __page = null;
#pragma warning restore CS0414 // Le champ 'Form_planing_journalier.PageJour' est assigné, mais sa valeur n'est jamais utilisée

        private void init()
        {
        }

        #region controls

        private Button __addZone_am = new Button { Content = "Ajouter une zone" };
        private Button __addZone_pm = new Button { Content = "Ajouter une zone" };

        // menu left

        #endregion controls
    
    }
}
