using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ShLayouts;

namespace DailySchedule
{
    public class ActionControl : HBoxLayout
    {
        public ActionControl()
        {
            Init();
        }

        protected void Init()
        {
            Add(TextBoxHeure);
            Add(TextBoxAction);
        }

#pragma warning disable CS0414 // Le champ 'ActionControl.__actionInfo' est assigné, mais sa valeur n'est jamais utilisée
        private ActionInfo __actionInfo = null;
#pragma warning restore CS0414 // Le champ 'ActionControl.__actionInfo' est assigné, mais sa valeur n'est jamais utilisée

        public TextBox TextBoxHeure = new TextBox() { MaxWidth = 40, IsReadOnly = true };
        public TextBox TextBoxAction = new TextBox() { IsReadOnly = true };
    }
}
