using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CFL_1.CFL_Data;
using CFL_1.CFL_Data.CFL_DataManaging;
using CFL_1.CFL_System;
using CFL_1.CFL_System.DB;
using CFL_1.CFL_System.SqlServerOrm;
using ShLayouts;

namespace CFL_1.CFLGraphics
{
    // TODO fonctions de l'utilisateur.
    public class Form_user : CFLForm
    {
        public Form_user()
        {
            init();
            buttonNew = true;
            buttonSave = true;
            buttonDelete = true;
            buttonDocuments = true;
        }

        public override void BecomeCurrent()
        {
            load();//la liste des utilisateurs.
            if (__current == null)
                initNewOne();
        }

        /// <summary>
        /// Si la notification concerne les utilisateurs, recharge la grille des uitilisateurs.
        /// Si _currentUser fait partie des utilisateurs affectés, le recharge, le reconnecte aux controls.
        /// </summary>
        public override void GetNotification(DBNotification _notification)
        {
            
        }

        public override bool NewOne()
        {
            //TODO changed
            if(__current != null /*&& __current.changed == true*/)
            {
                MessageBoxResult _result = MessageBox.Show("Voulez-vous sauvegarder l'utilisateur en cours de saisie ?", "", MessageBoxButton.YesNoCancel);
                switch (_result)
                {
                    case MessageBoxResult.None:
                        return false;
                    case MessageBoxResult.Cancel:
                        return false;
                    case MessageBoxResult.Yes:
                        Save();
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        break;
                }
            }
            initNewOne();

            return true;
        }

        /// <summary>
        /// Si les users ne sont pas chargés dans <seealso cref="CFLGlobalData"/>, en réclame le chargement.
        /// Charge la grille des users.
        /// </summary>
        public bool load()
        {
            CFLGlobalData.instance.LoadUsers();
            //gridUsers.load();
            return true;
        }

        //TODO
        public override bool Save()
        { return false; }

        public override bool DeleteCurrent()
        {
            throw new NotImplementedException();
        }

        public override void Documents()
        {
            throw new NotImplementedException();
        }

        public Personne currentUser
        {
            get
            { 
                if(__current == null)
                    __current = new Personne();
                return __current; 
            }
            set 
            { __current = value ;}
        }

        #region public controls

        //public ctrl_select_users gridUsers
        //{
        //    get
        //    {
        //        return __gridUsers;
        //    }
        //}
        //private ctrl_select_users __gridUsers;
        
        public TextBox       nom = new TextBox();
        public TextBox       prenom = new TextBox();
        public TextBox       adress1 = new TextBox();
        public TextBox       adress2 = new TextBox();
        //public ctrl_receiver commune = new ctrl_receiver(0, new int[] { 1 });
        public TextBox       telFixPerso = new TextBox();
        public TextBox       telPortPerso = new TextBox();
        public TextBox       telFixPro = new TextBox();
        public TextBox       telPortPro = new TextBox();
        public TextBox       fax = new TextBox();
        public TextBox       mail = new TextBox();
        public TextBox       pass = new TextBox();

        #endregion public controls

        #region private controls

        private VBoxLayout __layoutUser = new VBoxLayout();
        private VBoxLayout __layoutGridUsers = new VBoxLayout();

        private HBoxLayout __layoutBotom = new HBoxLayout();

        #endregion private controls
        //private:

        Personne __current;
        InterControlsData __interControlsData;

        private void connectCurrent()
        {
            if (__current == null)
                return;
            
            if (__interControlsData == null)
                __interControlsData = new InterControlsData();

            __interControlsData.connect(nom,          __current.Identite, () => __current.Identite.Nom);
            __interControlsData.connect(prenom,       __current.Identite, () =>__current.Identite.Prenom);
            __interControlsData.connect(adress1,      __current.Coordonnees, () =>__current.Coordonnees.adress1);
            __interControlsData.connect(adress2,      __current.Coordonnees, () =>__current.Coordonnees.adress2);
            //__interControlsData.connect(commune,      __current.coordonnees, () =>__current.coordonnees.commune);
            __interControlsData.connect(fax,          __current.Contacts, () =>__current.Contacts.fax);
            __interControlsData.connect(mail,         __current.Contacts, () =>__current.Contacts.mail);
            __interControlsData.connect(telFixPerso,  __current.Contacts, () =>__current.Contacts.telFixPerso);
            __interControlsData.connect(telFixPro,    __current.Contacts, () =>__current.Contacts.telFixPro);
            __interControlsData.connect(telPortPerso, __current.Contacts, () =>__current.Contacts.telPortPerso);
            __interControlsData.connect(telPortPro,   __current.Contacts, () =>__current.Contacts.telPortPro);
            
        }

        private void gridUsersClicked(int _column, int row)
        {
            //__current = __gridUsers.sellectedUser;
            connectCurrent();
        }

        private void init()
        {

            FormLayout __formLayoutgridUser = new FormLayout();

            __formLayoutgridUser.Add("nom", nom, 25);
            __formLayoutgridUser.Add("prénom", prenom, 25);
            __formLayoutgridUser.Add("adresse", adress1, 25);
            __formLayoutgridUser.Add("", adress2, 25);
            //__formLayoutgridUser.Add("commune", commune, 25);
            __formLayoutgridUser.Add("tel fix perso", telFixPerso, 25);
            __formLayoutgridUser.Add("tel portable perso", telPortPerso, 25);
            __formLayoutgridUser.Add("tel fix proffessionnel", telFixPro, 25);
            __formLayoutgridUser.Add("tel portable proffessionnel", telPortPro, 25);
            __formLayoutgridUser.Add("fax", fax, 25);
            __formLayoutgridUser.Add("mail", mail, 25);
            __formLayoutgridUser.Add("pass", pass, 25);

            TextBox tb = new TextBox() ;
            
             GroupBox _groupBox_user;
            _groupBox_user = new GroupBox() { Header = "utilisateur" };
            _groupBox_user.MinWidth = 250;

            _groupBox_user.Content = __formLayoutgridUser;
            _groupBox_user.MinHeight = __formLayoutgridUser.MinHeight + 23;
            
            // gridUsers

            //__gridUsers = new ctrl_select_users();
            GroupBox _groupBoxGridUsers = new GroupBox() { Header = "Utilisateurs" };
            //_groupBoxGridUsers.Content = gridUsers;
            //__gridUsers.Background = Brushes.BurlyWood;
            //__gridUsers.grid.cellClicked += gridUsersClicked;

            // fonctions

            AddElementToRootLayout(__layoutTop);
            AddElementToRootLayout(__layoutBotom);

            __layoutTop.Add(_groupBox_user);
            __layoutTop.Add(_groupBoxGridUsers);
        }

        HBoxLayout __layoutTop = new HBoxLayout();

        /// <summary>
        /// new UserCFL et connectCurrent
        /// </summary>
        private void initNewOne()
        {
            __current = new Personne();
            connectCurrent();
        }
    }
}
