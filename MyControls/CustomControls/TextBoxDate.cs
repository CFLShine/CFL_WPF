using System;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace CustomControls
{
    /// <summary>
    /// 01/01/2017
    /// </summary>
    public class TextBoxDate : MaskedTextBox
    {
        public TextBoxDate()
        {
            Mask = "00/00/0000";
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SelectAll();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab)
                AutoCompleteDate();
            else if(e.Key == Key.Delete)
                Text = Text.Insert(SelectionStart, "_");
            base.OnKeyDown(e);
        }

        public int Day
        { 
            get=> __day;
            set
            {
                __day = value;
                Text= Text.Remove(0, 2).Insert(0, DayToString());
            }
        }

        public int Month
        { 
            get =>__month; 
            set
            {
                __month = value;
                Text = Text.Remove(3,2).Insert(3,MonthToString());
            }
        }

        public int Year
        {
            get => __year;
            set
            {
                __year = value;
                Text = Text.Remove(6,4).Insert(6, YearToString());
            }
        }

        private void AutoCompleteDate()
        {

        }

        private string DayToString()
        {
            return string.Format("{0:00}", __day);
        }

        private string MonthToString()
        {
            return string.Format("{0:00}", __month);
        }

        private string YearToString()
        {
            return string.Format("{0:00000}", __year);
        }

        private int __day = 0;
        private int __month = 0;
        private int __year = 0;
    }
}
