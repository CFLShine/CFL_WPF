using System;
using System.Collections.Generic;

namespace DailySchedule
{
    public class SheetInfo
    {
        public Guid ID { get; set; }

        public DateTime? Day { get; set; }

        public List<ZoneInfo> ZonesPM { get; set; } = new List<ZoneInfo>();
        public List<ZoneInfo> ZonesAM { get; set; } = new List<ZoneInfo>();
    }
}
