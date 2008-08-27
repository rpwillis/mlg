using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.WebParts.MetaSearch.Helper.QueryHelper
{
    /// <summary>
    /// Represents the fieds taht uses the Eq operator
    /// </summary>
    internal class EqualsQueryField : QueryFieldBase
    {
        public EqualsQueryField(string fieldInternalName, string searchValue, bool showInViewFields) : base(fieldInternalName, searchValue, showInViewFields) { }
        
        public override string GetQueryXml()
        {
            return string.Format("<Eq>{0}{1}</Eq>", string.Format(FieldRefTemplateXml, FieldName), string.Format(ValueTemplateXml, QueryValue));
        }
       

    }
}
