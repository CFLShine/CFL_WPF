using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CFL_1.CFLGraphics;
using CFL_1.CFL_System;

namespace CFL_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;

            initForms();
            initControls();
            currentForm = CFLForms.instance.form_accueil;
        }

        #region currentForm
        /// <summary>
        /// _form devient la form visible.
        /// </summary>
        /// <param name="_form"></param>
        public CFLForm currentForm
        {
            set
            {
                if (__currentForm != null)
                    __mainPanel.Children.Remove(__currentForm) ;
                __currentForm = value ;
                __mainPanel.Children.Add(__currentForm) ;
            }
        }

        CFLForm __currentForm;

        #endregion currentForm

        public Label __labelConnectionDB;

        #region private controls

        private DockPanel __mainPanel;
        private StackPanel __leftPanel;

        private TabControl __tabControl;

        private TabItem __tabItemAccueil;
        private TabItem __tabItemConnection ;
        private TabItem __tabItemConfiguration;
        private TabItem __tabItemPersonnel;
        private TabItem __tabItemPlannings;
        private TabItem __tabItemCF;
        private TabItem __tabItemThanato;
        private TabItem __tabItemFiches;
        private TabItem __tabItemDivers;
        private TabItem __tabItemDev;

        private StackPanel __panelConfiguration;
        private StackPanel __panelPersonnel;
        private StackPanel __panelPlannings;
        private StackPanel __panelCF;
        private StackPanel __panelThanato;
        private StackPanel __panelFiches;
        private StackPanel __panelDivers;
        private StackPanel __panelDev;

        private Button __buttonConfigConnectionDB;
        private Button __buttonConfigDocuments;

        private Button __buttonPersonnelPresence;

        private Button __buttonPlanningsPf;
        private Button __buttonPlanningsCrema;
        private Button __buttonPlanningsAstreintes;
        
        private Button __buttonCfAdmissions;
        private Button __buttonChambresFune;

        private Button __buttonThanatoSoins;

        private Button __buttonFichesDefunts;
        private Button __buttonFichesCommunes;

        private Button __buttonDiversRepertoire;
        private Button __buttonDiversMessagerie;

        private Button __buttonCodeEdit;
        private Button __buttonTest;
        private Button __buttonTestGraph;
        private Button __buttonTestPlanningsJournaliers;

        #endregion private controls

        #region leftPanel events

        private void button_jourSuivant_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_jourPrecedent_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        // Le TabControl Menu général.
        #region tabControl events

        private void tab_accueil_gotFocus(object sender, RoutedEventArgs e)
        {
            CFLForms.instance.currentForm = CFLForms.instance.form_accueil;
        }

        // Config
        private void buttonConfigConnectionDB_Click(object sender, RoutedEventArgs e)
        {
            CFLForms.instance.currentForm = CFLForms.instance.form_config_connection;
        }

        private void buttonConfigPersonnel_Click(object sender, RoutedEventArgs e)
        {
            CFLForms.instance.currentForm = CFLForms.instance.form_user ;
        }

        private void buttonConfigAutorisations_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonConfigDocuments_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonConfigLieux_Click(object sender, RoutedEventArgs e)
        {
        }

        // Personnel
        private void buttonPersonnelDonnees_Click(object sender, RoutedEventArgs e)
        { }

        private void buttonPersonnelPresences_Click(object sender, RoutedEventArgs e)
        { }

        // Plannings
        private void buttonPlanningsPf_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonPlanningsCrema_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonPlanningsAstreintes_Click(object sender, RoutedEventArgs e)
        {

        }

        // Cf
        private void buttonAdmissions_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCasesSalons_Click(object sender, RoutedEventArgs e)
        {

        }

        // Thanato
        private void buttonThanatoSoins_Click(object sender, RoutedEventArgs e)
        {

        }

        // Fiches
        private void buttonFichesDefunts_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonFichesCommune_Click(object sender, RoutedEventArgs e)
        { }

        // Divers
        private void buttonRepertoire_Click(object sender, RoutedEventArgs e)
        { }

        private void buttonMessagerie_Click(object sender, RoutedEventArgs e)
        { }

        private void buttonCodeEdit_Click(object sender, RoutedEventArgs e)
        {
            CFLForms.instance.currentForm = CFLForms.instance.form_codeEditor;
        }

        // Test
        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            CFLForms.instance.currentForm = CFLForms.instance.form_test;
        }

        private void buttonTestGraph_Click(object sender, RoutedEventArgs e)
        {
            CFLForms.instance.currentForm = CFLForms.instance.form_config_entreprise;
        }

        private void buttonTestPlaningsJournaliers_Click(object sender, RoutedEventArgs e)
        {
            CFLForms.instance.currentForm = CFLForms.instance.form_planingJournalier;
        }

        #endregion

        private void initForms()
        {
            CFLForms.instance.mainWindow = this;
            CFLForms.instance.initForms();
        }

        private void initControls()
        {
            __mainPanel = new DockPanel();
            __mainPanel.Background = Brushes.DarkGray;
            this.AddChild(__mainPanel);

            /////////////////// menu gauche /////////////////////////////////
            __leftPanel = new StackPanel();
            DockPanel.SetDock(__leftPanel, Dock.Left);
            __mainPanel.Children.Add(__leftPanel);

            __labelConnectionDB = new Label();
            __labelConnectionDB.Content = "Connection DB";
            __labelConnectionDB.Background = Brushes.Red;
            __labelConnectionDB.FontWeight = FontWeights.Bold;
            __leftPanel.Children.Add(__labelConnectionDB);

            //////////////////// tabControl /////////////////////////////////

            __tabControl = new TabControl();
            __tabControl.VerticalAlignment = VerticalAlignment.Stretch;

            // tabItems
            __tabItemAccueil       = new TabItem();
            __tabItemConnection    = new TabItem();
            __tabItemConfiguration = new TabItem();
            __tabItemPersonnel     = new TabItem();
            __tabItemPlannings     = new TabItem();
            __tabItemCF            = new TabItem();
            __tabItemThanato       = new TabItem(); 
            __tabItemFiches        = new TabItem();
            __tabItemDivers        = new TabItem();
            __tabItemDev           = new TabItem();

            __tabItemAccueil.GotFocus += tab_accueil_gotFocus;
            
            __tabItemAccueil.Header       = "Accueil";
            __tabItemConnection.Header    = "Connection";
            __tabItemConfiguration.Header = "Configuration";
            __tabItemPersonnel.Header     = "Personnel";
            __tabItemPlannings.Header     = "Plannings";
            __tabItemCF.Header            = "Chambre funéraire";
            __tabItemThanato.Header       = "Thanatopraxie";
            __tabItemFiches.Header        = "Fiches";
            __tabItemDivers.Header        = "Divers";
            __tabItemDev.Header           = "Dev.";

            // buttons
            __buttonConfigConnectionDB       = new Button(){ Content = "Connection DB" };
            __buttonConfigDocuments          = new Button(){ Content = "Documents" } ;
                                                       
            __buttonPersonnelPresence        = new Button(){ Content = "Présences" } ;
                                                            
            __buttonPlanningsPf              = new Button(){ Content = "planings PF" } ;
            __buttonPlanningsCrema           = new Button(){ Content = "Planings crématoriums" } ;
            __buttonPlanningsAstreintes      = new Button(){ Content = "Planings astreintes" } ;
                                             
            __buttonCfAdmissions             = new Button(){ Content = "Admissions" } ;
            __buttonChambresFune             = new Button(){ Content = "Chambres funéraires" };
                                                            
            __buttonThanatoSoins             = new Button(){ Content = "soins" };
                                                            
            __buttonFichesDefunts            = new Button(){ Content = "Défunts" };
            __buttonFichesCommunes           = new Button(){ Content = "Communes" };
                                                            
            __buttonDiversRepertoire         = new Button() ;
            __buttonDiversMessagerie         = new Button() ;
                                                            
            __buttonCodeEdit                 = new Button() ;
            __buttonTest                     = new Button() ;
            __buttonTestGraph                = new Button(){ Content = "Test Graph" };
            __buttonTestPlanningsJournaliers = new Button(){ Content = "Test Plannings journaliers" };

            __buttonDiversRepertoire.Content    = "Répertoire" ;
            __buttonDiversMessagerie.Content    = "Messagerie" ;

            __buttonCodeEdit.Content            = "Code" ;
            __buttonTest.Content                = "Test" ;

            __buttonConfigConnectionDB.Margin       = new Thickness(1, 0, 1, 0);
            __buttonConfigDocuments.Margin          = new Thickness(1, 0, 1, 0);
                                                                               
            __buttonPersonnelPresence.Margin        = new Thickness(1, 0, 1, 0);

            __buttonPlanningsPf.Margin              = new Thickness(1, 0, 1, 0);
            __buttonPlanningsCrema.Margin           = new Thickness(1, 0, 1, 0);
            __buttonPlanningsAstreintes.Margin      = new Thickness(1, 0, 1, 0);
                                                                               
            __buttonCfAdmissions.Margin             = new Thickness(1, 0, 1, 0);
            __buttonChambresFune.Margin             = new Thickness(1, 0, 1, 0);
                                                                               
            __buttonThanatoSoins.Margin             = new Thickness(1, 0, 1, 0);
                                                                               
            __buttonFichesDefunts.Margin            = new Thickness(1, 0, 1, 0);
            __buttonFichesCommunes.Margin           = new Thickness(1, 0, 1, 0);
                                                                               
            __buttonDiversRepertoire.Margin         = new Thickness(1, 0, 1, 0);
            __buttonDiversMessagerie.Margin         = new Thickness(1, 0, 1, 0);
                                                                                
            __buttonCodeEdit.Margin                 = new Thickness(1, 0, 1, 0);
            __buttonTest.Margin                     = new Thickness(1, 0, 1, 0);
            __buttonTestGraph.Margin                = new Thickness(1, 0, 1, 0);
            __buttonTestPlanningsJournaliers.Margin = new Thickness(1, 0, 1, 0);

            __buttonConfigConnectionDB.Click       += buttonConfigConnectionDB_Click;
            __buttonConfigDocuments.Click          += buttonConfigDocuments_Click;

            __buttonPersonnelPresence.Click        += buttonPersonnelPresences_Click ;
                                                   
            __buttonPlanningsPf.Click              += buttonPlanningsPf_Click ;
            __buttonPlanningsCrema.Click           += buttonPlanningsCrema_Click;
                                                   
            __buttonCfAdmissions.Click             += buttonAdmissions_Click;
            __buttonChambresFune.Click             += buttonCasesSalons_Click;
                                                   
            __buttonThanatoSoins.Click             += buttonThanatoSoins_Click;
                                                   
            __buttonFichesDefunts.Click            += buttonFichesDefunts_Click;
            __buttonFichesCommunes.Click           += buttonFichesCommune_Click;
                                                   
            __buttonDiversRepertoire.Click         += buttonRepertoire_Click;
            __buttonDiversMessagerie.Click         += buttonMessagerie_Click;
                                                   
            __buttonCodeEdit.Click                 += buttonCodeEdit_Click;
            __buttonTest.Click                     += buttonTest_Click;
            __buttonTestGraph.Click                += buttonTestGraph_Click;
            __buttonTestPlanningsJournaliers.Click += buttonTestPlaningsJournaliers_Click;

            // panels
            __panelConfiguration = new StackPanel();
            __panelPersonnel     = new StackPanel();
            __panelPlannings     = new StackPanel();
            __panelCF            = new StackPanel();
            __panelThanato       = new StackPanel();
            __panelFiches        = new StackPanel();
            __panelDivers        = new StackPanel();
            __panelDev           = new StackPanel();

            __panelConfiguration.Orientation = Orientation.Horizontal;
            __panelPersonnel.Orientation     = Orientation.Horizontal;
            __panelPlannings.Orientation     = Orientation.Horizontal;
            __panelCF.Orientation            = Orientation.Horizontal;
            __panelThanato.Orientation       = Orientation.Horizontal;
            __panelFiches.Orientation        = Orientation.Horizontal;
            __panelDivers.Orientation        = Orientation.Horizontal;
            __panelDev.Orientation           = Orientation.Horizontal;

            __panelConfiguration.HorizontalAlignment = HorizontalAlignment.Stretch;
            __panelPersonnel.HorizontalAlignment     = HorizontalAlignment.Stretch;
            __panelPlannings.HorizontalAlignment     = HorizontalAlignment.Stretch;
            __panelCF.HorizontalAlignment            = HorizontalAlignment.Stretch;
            __panelThanato.HorizontalAlignment       = HorizontalAlignment.Stretch;
            __panelFiches.HorizontalAlignment        = HorizontalAlignment.Stretch;
            __panelDivers.HorizontalAlignment        = HorizontalAlignment.Stretch;
            __panelDev.HorizontalAlignment           = HorizontalAlignment.Stretch;

            __panelConfiguration.Children.Add(__buttonConfigConnectionDB);
            __panelConfiguration.Children.Add(__buttonConfigDocuments);

            __panelPersonnel.Children.Add(__buttonPersonnelPresence);

            __panelPlannings.Children.Add(__buttonPlanningsPf);
            __panelPlannings.Children.Add(__buttonPlanningsCrema);
            __panelPlannings.Children.Add(__buttonPlanningsAstreintes);

            __panelCF.Children.Add(__buttonCfAdmissions);
            __panelCF.Children.Add(__buttonChambresFune);

            __panelThanato.Children.Add(__buttonThanatoSoins);

            __panelFiches.Children.Add(__buttonFichesDefunts);
            __panelFiches.Children.Add(__buttonFichesCommunes);

            __panelDivers.Children.Add(__buttonDiversRepertoire);
            __panelDivers.Children.Add(__buttonDiversMessagerie);

            __panelDev.Children.Add(__buttonCodeEdit);
            __panelDev.Children.Add(__buttonTest);
            __panelDev.Children.Add(__buttonTestGraph) ;
            __panelDev.Children.Add(__buttonTestPlanningsJournaliers);

            __tabItemConfiguration.Content = __panelConfiguration;
            __tabItemPersonnel.Content     = __panelPersonnel;
            __tabItemPlannings.Content     = __panelPlannings;
            __tabItemCF.Content            = __panelCF;
            __tabItemThanato.Content       = __panelThanato;
            __tabItemFiches.Content        = __panelFiches;
            __tabItemDivers.Content        = __panelDivers;
            __tabItemDev.Content           = __panelDev;

            __tabControl.Items.Add(__tabItemAccueil);
            __tabControl.Items.Add(__tabItemConnection);
            __tabControl.Items.Add(__tabItemConfiguration);
            __tabControl.Items.Add(__tabItemPersonnel);
            __tabControl.Items.Add(__tabItemPlannings);
            __tabControl.Items.Add(__tabItemCF);
            __tabControl.Items.Add(__tabItemThanato);
            __tabControl.Items.Add(__tabItemFiches);
            __tabControl.Items.Add(__tabItemDivers);
            __tabControl.Items.Add(__tabItemDev);
            
            DockPanel.SetDock(this.__tabControl, Dock.Top);
            __mainPanel.Children.Add(__tabControl);
                        
        }
    }
}
