using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CFL_1.CFL_System.SqlServerOrm;
using ShLayouts;

namespace CFL_1.CFLGraphics
{
    public class Form_accueuil : CFLForm
    {
        public Form_accueuil()
        {
            Init();
        }

        public override void BecomeCurrent()
        {}

        public override void GetNotification(DBNotification _notification)
        { /* pas de notification utile à form_accueil */}

        public override bool Save()
        { return true; }
        public override bool NewOne()
        { return true; }
        public override bool DeleteCurrent()
        { return true; }

        public override void Documents()
        {
            throw new NotImplementedException();
        }

        private Label __topLabel;
        private Calendar __calendar;

        //private:

        private void Init()
        {
            __topLabel = new Label()
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
                Content = "CFL",
                FontSize = 36,
                FontWeight = FontWeights.Bold
            };
            
            LinearGradientBrush _gradient = new LinearGradientBrush
            (Colors.Black, Colors.White, new Point(0, 0.5), new Point(1, 0.5));

            __topLabel.Background = _gradient;

            // calendar
            __calendar = new Calendar();

            AddElementToRootLayout(__topLabel);
            AddElementToRootLayout(new Spacer(0, 90));
            AddElementToRootLayout(__calendar);
            AddElementToRootLayout(new Spacer(0, 90));
        }
    }
}
