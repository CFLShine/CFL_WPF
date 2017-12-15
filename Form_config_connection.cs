using System.Windows;
using System.Windows.Controls;
using CFL_1.CFL_Data;
using System;
using CFL_1.CFL_System.SqlServerOrm;
using CFL_1.CFL_System;
using ShLayouts;

namespace CFL_1.CFLGraphics
{
    public class Form_config_connection : CFLForm
    {
        public Form_config_connection()
        {
            init();
            buttonSave = true;
        }

        public override void BecomeCurrent()
        {
            load();
        }

        public override void GetNotification(DBNotification _notification)
        {
            // 
        }

        public void load()
        {
            CFLConfig _config = new CFLConfig();
            
            Gate.Load.config(ref _config);
            ip.Text = _config.Hostname;
            userName.Text = _config.Username;
            pass.Text = _config.Password;
            dbName.Text = _config.Dbname;
        }

        public override bool Save()
        {
            //TODO 
            return false;
        }

        public override bool NewOne()
        { return true; }
        public override bool DeleteCurrent()
        { return true; }

        public override void Documents()
        {
            throw new NotImplementedException();
        }
        
        private void button_clear_Click(object sender, RoutedEventArgs e)
        {
            textbox_sql.Clear();
        }

        private void init()
        {
            HBoxLayout _layoutTop = new HBoxLayout();
            AddElementToRootLayout(_layoutTop);

            groupBoxConfigDB.Header = "Base de données";
            groupBoxConfigDB.Content = _layoutFormConfigDB;
            _layoutTop.Add(groupBoxConfigDB);

            _layoutFormConfigDB.Add("ip", ip, 25);
            _layoutFormConfigDB.Add("Nom utilisateur", userName, 25);
            _layoutFormConfigDB.Add("Mot de pass", pass, 25);
            _layoutFormConfigDB.Add("Nom DB", dbName, 25);

            dbName.Text = "cfl";

            HBoxLayout _layoutBottom = new HBoxLayout();
            AddElementToRootLayout(_layoutBottom);

            // buttons

            VBoxLayout __layoutLeft = new VBoxLayout() ;
            
            _layoutBottom.Add(__layoutLeft);

            // textbox_sql

            textbox_sql = new TextBox();
            textbox_sql.AcceptsReturn = true;

            _layoutBottom.Add(textbox_sql);
        }

        public TextBox ip = new TextBox();
        public TextBox userName = new TextBox();
        public TextBox pass = new TextBox();
        public TextBox dbName = new TextBox();

        public TextBox textbox_sql = new TextBox();

        private GroupBox groupBoxConfigDB = new GroupBox();
        private FormLayout _layoutFormConfigDB = new FormLayout();

    }
}
