using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSTD;
using RuntimeExec;

namespace DailySchedule
{
    public class ActionInfo
    {
        public Guid ID{ get; set; } = Guid.NewGuid();

        public ZoneInfo Zone { get; set; }
        public DateTime? Day { get => Zone?.Day; }

        [NotMapped]
        public ActionControl Control { get; set; } = new ActionControl();

        public void Display()
        {
            Control.TextBoxHeure.Text = HeureDisplay.Display();
            Control.TextBoxAction.Text = ActionDisplay.Display();
        }

        public DataDisplay HeureDisplay { get; set; }
        
        public DataDisplay ActionDisplay{ get; set; }

    }
}
