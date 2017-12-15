using System;
using System.Windows.Controls;
using CFL_1.CFL_Data;
using CFL_1.CFL_System.SqlServerOrm;
using Telerik.Windows.Controls;

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
            if(!HasBeenDisplayed())
                Day = DateTime.Now;
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

        /// <summary>
        /// get : retourne la date selectionnée. Si la date est null, set DateTime.Now.  
        /// set : Selectionne la date donnée et affiche le planing pour cette date.
        /// </summary>
        public DateTime? Day
        {
            get
            {
                if(__calandar.SelectedDate == null)
                    Day = DateTime.Now;
                return __calandar.SelectedDate;
            }
            
            set
            {
                if(value == null)
                    value = DateTime.Now;
                __calandar.SelectedDate = value;
                DisplayZones();
            }
        }

        public bool HasBeenDisplayed()
        {
            return __calandar.SelectedDate != null;
        }

        protected virtual void DisplayZones()
        {
            
        }

#pragma warning disable CS0414 // Le champ 'Form_planing_journalier.PageJour' est assigné, mais sa valeur n'est jamais utilisée
        private PageJour __page = null;
#pragma warning restore CS0414 // Le champ 'Form_planing_journalier.PageJour' est assigné, mais sa valeur n'est jamais utilisée

        private void init()
        {
            AddElementToRootLayout(__layoutMain);

            __layoutMenuLeft.MinWidth = 200;
            __layoutMenuLeft.MaxWidth = 300;
            __layoutComments.Height = 100;

            __calandar.Height = 200;
            __calandar.DateSelectionMode = Telerik.Windows.Controls.Calendar.DateSelectionMode.Day;
            __calandar.DisplayMode = Telerik.Windows.Controls.Calendar.DisplayMode.MonthView;
            __calandar.SelectionMode = SelectionMode.Single;

            __layoutMenuLeft.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            __layoutPlaning.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            __layoutComments.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            __layoutMain.Items.Add(__layoutMenuLeft);
            __layoutMain.Items.Add(__layoutPlaning);
            __layoutPlaning.Items.Add(__layoutComments);
            __layoutPlaning.Items.Add(__layoutZones);
            __layoutZones.Items.Add(__layoutAm);
            __layoutZones.Items.Add(__layoutPm);

            __layoutMenuLeft.Items.Add(__calandar);
        }

        #region controls

        RadLayoutControl __layoutMain     = new RadLayoutControl() { Orientation = Orientation.Horizontal };
        RadLayoutControl __layoutPlaning  = new RadLayoutControl() { Orientation = Orientation.Vertical };
        RadLayoutControl __layoutZones    = new RadLayoutControl() { Orientation = Orientation.Horizontal };
        RadLayoutControl __layoutComments = new RadLayoutControl() { Orientation = Orientation.Horizontal };
        RadLayoutControl __layoutMenuLeft = new RadLayoutControl() { Orientation = Orientation.Vertical };
        RadLayoutControl __layoutAm      = new RadLayoutControl() { Orientation = Orientation.Vertical };
        RadLayoutControl __layoutPm      = new RadLayoutControl() { Orientation = Orientation.Vertical };

        private Button __addZone_am = new Button { Content = "Ajouter une zone" };
        private Button __addZone_pm = new Button { Content = "Ajouter une zone" };

        // menu left
        RadCalendar __calandar = new RadCalendar();

        #endregion controls
    
    }
}
