using System;
using System.Collections.Generic;

namespace DailySchedule
{
    /// <summary>
    /// <see cref="ZonePatern"/> est la classe qui servira
    /// de patron à une <see cref="ZoneInfo"/> pour se peupler 
    /// de <see cref="ActionInfo"/>.
    /// Elle participe donc à la configuration d'un <see cref="DaylySchduleControl"/> 
    /// car c'est ici que nous diront à un agenda ce qu'il doit afficher.
    /// </summary>
    public class ZonePatern
    {
        public List<ActionInfo> ActionsInfo;

        
    }
}
