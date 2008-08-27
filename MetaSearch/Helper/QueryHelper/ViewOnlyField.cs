using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.WebParts.MetaSearch.Helper.QueryHelper
{
    /// <summary>
    /// Represents a field that appear in the quiery results only.
    /// </summary>
    class ViewOnlyField :QueryFieldBase
    {

        public ViewOnlyField(string fieldInternalName):base(fieldInternalName,"",true){}
        
      
    }
}
