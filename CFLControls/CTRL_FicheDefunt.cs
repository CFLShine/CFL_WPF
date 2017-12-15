using System.Windows.Controls;
using CFL_1.CFL_Data;
using ObjectEdit;
using ShLayouts;

namespace CFL_1.CFLGraphics.CFLControls
{
    public class CTRL_FicheDefunt : VBoxLayout
    {
        public CTRL_FicheDefunt()
        {
            Init();
        }

        public Defunt Defunt
        {
            get => __defunt;
            set
            {
                __defunt = value;
                ShowDefunt();
            }
        }

        private void ShowDefunt()
        {
            __defunt_identite.Object = Defunt.Identite;
            __defunt_naissance.Object = Defunt.Naissance;
        }

        private void Init()
        {
            Add(__mainTabControl);

            __mainTabControl.Items.Add(__tabDefunt);
            __mainTabControl.Items.Add(__tabServices);

            __controlDefunt.TabStripPlacement = Dock.Left;
            __controlServices.TabStripPlacement = Dock.Left;

            __tabDefunt.Content = __controlDefunt;
            __tabServices.Content = __controlServices;

            __controlDefunt.Items.Add(__tabEtatCivil);
            __controlDefunt.Items.Add(__tabDeces);
            __controlDefunt.Items.Add(__tabFiliation);
            __controlDefunt.Items.Add(__tabSituationFamiliale);
            __controlDefunt.Items.Add(__tabPouvoir);

            __controlServices.Items.Add(__tabTransport);
            __controlServices.Items.Add(__tabAdmission);
            __controlServices.Items.Add(__tabSoin);
            __controlServices.Items.Add(__tabCeremonie);
            __controlServices.Items.Add(__tabCremation);
            __controlServices.Items.Add(__tabInhumation);
            __controlServices.Items.Add(__tabRDVOrdo);

            /////////////////// layouts /////////////////////////////

            __tabEtatCivil.Content = __layoutEtatcivil;
            __tabDeces.Content = __layoutDeces;

            /////////////////// controls d'édition //////////////////

            __layoutEtatcivil.Add(__defunt_identite);
            __layoutEtatcivil.Add(__defunt_naissance);

        }

        private Defunt __defunt = null;

        #region controls declaration

        private TabControl __mainTabControl = new TabControl();

        private TabItem __tabDefunt = new TabItem()             { Header = "Défunt" };
        private TabItem __tabServices = new TabItem()           { Header = "Services" };

        private TabControl __controlDefunt = new TabControl();
        private TabControl __controlServices = new TabControl();

        private TabItem __tabEtatCivil = new TabItem()          { Header = "Etat civil" };
        private TabItem __tabDeces = new TabItem()              { Header = "Décès" };
        private TabItem __tabFiliation = new TabItem()          { Header = "Filiation" };
        private TabItem __tabSituationFamiliale = new TabItem() { Header = "Situation familiale" };
        private TabItem __tabPouvoir = new TabItem()            { Header = "Pouvoir" };

        private TabItem __tabTransport = new TabItem()          { Header = "Transport" };
        private TabItem __tabAdmission = new TabItem()          { Header = "Admission" };
        private TabItem __tabSoin = new TabItem()               { Header = "Soin" };
        private TabItem __tabCeremonie = new TabItem()          { Header = "Cérémonie" };
        private TabItem __tabCremation = new TabItem()          { Header = "Crémation" };
        private TabItem __tabInhumation = new TabItem()         { Header = "Inhumation" };
        private TabItem __tabExhumation = new TabItem()         { Header = "Exhumation" };
        private TabItem __tabRDVOrdo = new TabItem()            { Header = "RDV Ordonnateur" };

        private VBoxLayout __layoutEtatcivil = new VBoxLayout();
        private VBoxLayout __layoutDeces = new VBoxLayout();
        private VBoxLayout __layoutFiliation = new VBoxLayout();

        private ObjectEditControl __defunt_identite = new ObjectEditControl();
        private ObjectEditControl __defunt_naissance = new ObjectEditControl();

        #endregion controls declaration
    }
}
