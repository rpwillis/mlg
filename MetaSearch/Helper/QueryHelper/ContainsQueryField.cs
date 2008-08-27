using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.WebParts.MetaSearch.Helper.QueryHelper
{
    /// <summary>
    /// Represents a field that uses the contains operator
    /// </summary>
    class ContainsQueryField : QueryFieldBase

    {
        public ContainsQueryField(string fieldInternalName, string searchValue, bool showInViewFields) : base(fieldInternalName, searchValue, showInViewFields) { }
        public override string GetQueryXml()
        {
            return string.Format("<Contains>{0}{1}</Contains>", string.Format(FieldRefTemplateXml, FieldName), string.Format(ValueTemplateXml, QueryValue));
        }
    }
}
