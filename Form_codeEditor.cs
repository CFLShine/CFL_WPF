using System;

using CFL_1.CFL_System.DB; // utile pour hériter de CFL_form et implémenter 
                           //la fonction abstraite getNotification(DB_notification _notification)

using System.Windows.Controls;
using CFL_1.CFL_System.Compil;
using CFL_1.CFL_System.Compil_and_script;
using System.Windows;
using CFL_1.CFL_Data;
using CFL_1.CFL_System;
using MyControls.Editor;
using CFL_1.CFL_System.SqlServerOrm;
using ShLayouts;

namespace CFL_1.CFLGraphics.Editor
{
    public class Form_codeEditor : CFLForm
    {
        public Form_codeEditor()
        {
            init();
        }

        public override void BecomeCurrent()
        {
            addCFL_formsTreesEditors();
        }

        public override bool DeleteCurrent()
        {
            throw new NotImplementedException();
        }

        public override void Documents()
        {
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

        private void buttonEvaluateClicked(object sender, RoutedEventArgs e)
        {
            // TODO passer un object selectionné par l'utilisateur
            Personne _user = new Personne();
            ((Compiler)__compiler).run(_user);
            string _name = _user.Identite.Nom;
        }

        private void buttonShowTreeClicked(object sender, RoutedEventArgs e)
        {
            __syntaxTreeVisualizer.Visibility = 
                (__syntaxTreeVisualizer.Visibility == Visibility.Visible) ? 
                 Visibility.Hidden : Visibility.Visible;
        }

        private void init()
        {
            // compilation

            __compiler = new Compiler(true);
            __ctrl_userCommunication = new ctrl_userCommunication(__codeEditor);
            __codeEditor.init(__compiler, __syntaxTreeVisualizer, __ctrl_userCommunication);
            

            HBoxLayout _layoutMain = new HBoxLayout();
            AddElementToRootLayout(__layoutTop);
            AddElementToRootLayout(_layoutMain);

            // menu

            __layoutTop.Add(__buttonShowTree);
            __buttonShowTree.Click += buttonShowTreeClicked;
            __layoutTop.Add(__buttonEvaluate);
            __buttonEvaluate.Click += buttonEvaluateClicked;

            //
            
            __layoutLeft.MaxWidth = 120;

            _layoutMain.Add(__layoutLeft);
            _layoutMain.Add(__layoutCenter);
            _layoutMain.Add(__layoutRight);

            __layoutCenter.Add(__codeEditor);

            __syntaxTreeVisualizer.Visibility = Visibility.Hidden;
            __layoutRight.Add(__syntaxTreeVisualizer);
            
            __layoutRight.Add(__layoutTreesObjectEditors);
            
            HBoxLayout _layout_bottomCenter = new HBoxLayout() { MaxHeight = 200 };
            _layout_bottomCenter.Add(__ctrl_userCommunication);
            __layoutCenter.Add(_layout_bottomCenter);
        }

        private void addCFL_formsTreesEditors()
        {
            __layoutTreesObjectEditors.Clear();

            foreach(CFLForm _form in CFLForms.instance.forms)
            {
                //ctrl_CFL_form_treeEditor _tree = new ctrl_CFL_form_treeEditor(true, false);
                //_tree.edit(_form, _form.Name);
                //__layoutTreesObjectEditors.Add(_tree);
            }
        }

        CodeCompiler __compiler;
        
        private ctrl_codeEditor __codeEditor = new ctrl_codeEditor();
        private ctrl_userCommunication __ctrl_userCommunication;
        private SyntaxTreeVisualiser __syntaxTreeVisualizer = new SyntaxTreeVisualiser();
        
        private HBoxLayout __layoutTop  = new HBoxLayout() { Height = 27 };
        private VBoxLayout __layoutLeft = new VBoxLayout();
        private VBoxLayout __layoutCenter = new VBoxLayout();
        private VBoxLayout __layoutRight = new VBoxLayout();
        private VBoxLayout __layoutTreesObjectEditors = new VBoxLayout();

        private Button __buttonEvaluate = new Button() { Content = "Evaluer", Height = 27 };
        private Button __buttonShowTree = new Button() { Content = "Arbre", Height = 27 };
    }

}
