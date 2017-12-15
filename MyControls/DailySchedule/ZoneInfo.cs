

using System;
using System.ComponentModel.DataAnnotations.Schema;
using RuntimeExec;
using SqlOrm;

namespace DailySchedule
{
    public class ZoneInfo
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        public DaylyScheduleInfo DaylyScheduleInfo { get; set; }
        public SheetInfo Sheet { get; set; }
        public DateTime? Day { get => Sheet?.Day; }

        [NotMapped]
        public ZoneControl Control { get; set; } = new ZoneControl();

        public void Display()
        {
            Control.Clear();

            ZonePatern _patern = DaylyScheduleInfo.ZonePatern;
            if(_patern != null)
            {
                foreach(ActionInfo _action in _patern.ActionsInfo)
                {

                }
            }
        }

        /// <summary>
        /// L'objet de classe qui sera le sujet de chaque <see cref="ActionInfo.HeureDisplay"/> et <see cref="ActionInfo.ActionDisplay"/>
        /// </summary>
        public REClassObject Subject { get; set; } = null;

    }
}
