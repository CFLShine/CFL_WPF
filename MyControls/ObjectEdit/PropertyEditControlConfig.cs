

using System.Collections;
using MSTD;

namespace ObjectEdit
{
    public class PropertyClassEditControlConfig
    {
        public PropertyClassEditControlConfig()
        {}

        public DataDisplay DataDisplay
        {
            get => __dataDisplay;
            set
            {
                __dataDisplay = value;
            }
        }

        public IList ObjectsToDisplay
        { 
            get => __objectsToDisplay; 
            set
            {
                __objectsToDisplay = value;
            }
        }

        private IList __objectsToDisplay = null;
        private DataDisplay __dataDisplay = null;
    }
}
