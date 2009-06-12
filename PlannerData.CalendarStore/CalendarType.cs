using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.Helper.CalendarStore
{
    /// <summary>Indicates the type of calendar.</summary>
    [Serializable]
    public enum CalendarType
    { 
        /// <summary>A calendar in Exchange.</summary>
        Exchange,
        /// <summary>A calendar in SharePoint.</summary>
        SharePoint
    }
}
