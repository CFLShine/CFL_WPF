using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CFL_1.CFL_System.DB;
using CFL_1.CFL_System.SqlServerOrm;
using ShLayouts;

namespace CFL_1.CFLGraphics
{
    /// <summary>
    /// <see cref="CFLForm"/> hérite de <see cref="ScrollViewer"/> dont la visibilité des ScrollBar 
    /// est mise à Auto.
    /// Son contenu est un <see cref="HBoxLayout"/> pour le menu Top, et un <see cref="VBoxLayout"/> pour le reste.
    /// La fonction <see cref="AddElementToRootLayout(FrameworkElement)"/> permet d'ajouter un élément à <see cref="RootLayout"/>,
    /// Lafonction <see cref="AddElementToLayoutMenuTop(FrameworkElement)"/> permet d'ajouter un élément à <see cref="MenuTopLayout"/>.
    /// </summary>
    public abstract class CFLForm : ScrollViewer
    {
        public CFLForm()
        {
            Name = GetType().Name; 
            init();
            CFLDBConnection.instance.NotificationEvent += GetNotification;
        }

        #region CFL_form functions

        public abstract bool Save();
        public abstract bool NewOne();
        public abstract bool DeleteCurrent();
        public abstract void Documents();

        #endregion CFL_form functions

        /// <summary>
        /// becomeCurrent est appelé par <seealso cref="MainWindow.setCurrentForm(CFLForm)"/> 
        /// </summary>
        public abstract void BecomeCurrent();

        /// <summary>
        /// Lorsqu'une notification est reçue par <seealso cref="CFLDBConnection"/>, cette notification est passée 
        /// aux classes system concernées(ex <seealso cref="CFL_global"/>) 
        /// et à <seealso cref="CFL_forms"/> qui la propage à toutes les CFL_form.
        /// </summary>
        /// <param name="_notification"></param>
        public abstract void GetNotification(DBNotification _notification);
        
        #region Menu Top

        public HBoxLayout MenuTopLayout
        {
            get => __layoutMenuTop;
            private set => __layoutMenuTop = value;
        }

        public void AddElementToLayoutMenuTop(FrameworkElement _e)
        {
            _e.Visibility = Visibility.Visible;
            __layoutMenuTop.Insert(__layoutMenuTop.Count - 1 ,_e);
        }

        /// <summary>
        /// Conditionne la visibilité du bouton Nouveau.
        /// </summary>
        public bool buttonNew
        {
            get { return __buttonNew != null && __buttonNew.Visibility == Visibility.Visible; }
            set
            {
                if(value)
                {
                    if(__buttonNew == null)
                    {
                        __buttonNew = new Button() {  Content = "Nouveau", Margin = new Thickness(3, 3, 3, 3), Height = 25, Width = 100, Visibility = Visibility.Collapsed  } ;
                        AddElementToLayoutMenuTop(__buttonNew);
                        __buttonNew.Click += newOne ;
                    }
                    else
                        __buttonNew.Visibility = Visibility.Visible ;
                }
                else
                {
                    if(__buttonNew != null)
                        __buttonNew.Visibility = Visibility.Hidden ;
                }
            }
        }

        /// <summary>
        /// Conditionne la visibilité du bouton Enregistrer.
        /// </summary>
        public bool buttonSave
        {
            get { return __buttonSave != null && __buttonSave.Visibility == Visibility.Visible; }
            set
            {
                if(value)
                {
                    if(__buttonSave == null)
                    {
                        __buttonSave = new Button() { Content = "Enregistrer", Margin = new Thickness(3, 3, 3, 3), Height = 25,Width = 100, Visibility = Visibility.Collapsed } ;
                        AddElementToLayoutMenuTop(__buttonSave);
                        __buttonSave.Click += save ;
                    }
                    else
                        __buttonSave.Visibility = Visibility.Visible ;
                }
                else
                {
                    if(__buttonSave == null)
                        __buttonSave.Visibility = Visibility.Hidden ;
                }
            }
        }

        /// <summary>
        /// Conditionne la visibilité du bouton Suprimer.
        /// </summary>
        public bool buttonDelete
        {
            get { return __buttonDelete!= null &&  __buttonDelete.Visibility == Visibility.Visible; }
            set
            {
                if(value)
                {
                    if(__buttonDelete == null)
                    {
                        __buttonDelete = new Button() { Content = "Suprimer", Margin = new Thickness(3, 3, 3, 3), Height = 25, Width = 100, Visibility = Visibility.Collapsed  } ;
                        AddElementToLayoutMenuTop(__buttonDelete);
                        __buttonDelete.Click += deleteCurrent ;
                    }
                    else
                        __buttonDelete.Visibility = Visibility.Visible ;
                }
                else
                {
                    if(__buttonDelete != null)
                        __buttonDelete.Visibility = Visibility.Hidden ;
                }
            }
        }
        
        /// <summary>
        /// Conditionne la visibilité du bouton Documents.
        /// </summary>
        public bool buttonDocuments
        {
            get { return __buttonDocuments.Visibility == Visibility.Visible; }
            set
            {
                if(value)
                {
                    if(__buttonDocuments == null)
                    {
                        __buttonDocuments = new Button() { Content = "Documents", Margin = new Thickness(3, 3, 3, 3), Height = 25, Width = 100, Visibility = Visibility.Collapsed  } ;
                        AddElementToLayoutMenuTop(__buttonDocuments);
                        __buttonDocuments.Click += documents ; 
                    }
                }
                else
                {
                    if(__buttonDocuments != null)
                        __buttonDocuments.Visibility = Visibility.Hidden ;
                }
            }
        }

        private void save(object sender, RoutedEventArgs e)
        { Save() ; }
        private void newOne(object sender, RoutedEventArgs e)
        { NewOne() ;  }
        private void deleteCurrent(object sender, RoutedEventArgs e)
        { DeleteCurrent() ; }
        private void documents(object sender, RoutedEventArgs e)
        { Documents() ; }

        private void showButton(Button _button, bool _flag)
        {
            _button.Visibility = (_flag) ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion Menu Top

        #region Root Layout

        public VBoxLayout RootLayout
        {
            get => __rootLayout;
            private set => __rootLayout = value;
        }

        /// <summary>
        /// Ajoute un élément au layout racine ( un layout orienté vertical).
        /// </summary>
        protected void AddElementToRootLayout(FrameworkElement _element)
        {
            __rootLayout.Add(_element) ;
        }

        #endregion Root Layout

        private void init()
        {
            Background = Brushes.Gray ;
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            __rootLayout = new VBoxLayout();
            Content = __rootLayout ;

            __layoutMenuTop = new HBoxLayout() { Height = 30, Background = Brushes.Beige };
            __layoutMenuTop.Add(new Spacer());
            
            //__rootLayout.Add(new Glue(0));
            __rootLayout.Add(__layoutMenuTop);
        }

        private VBoxLayout __rootLayout = null ;
        private HBoxLayout __layoutMenuTop = null;

        Button __buttonSave      = null;
        Button __buttonNew       = null;
        Button __buttonDelete    = null;
        Button __buttonDocuments = null;

    }
}
