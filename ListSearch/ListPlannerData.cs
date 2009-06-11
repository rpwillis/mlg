using System;
using System.Text;

namespace MLG2007.Helper.ListSearch
{
    /// <summary>
    /// Summary description for ListPlannerData.
    /// </summary>
    public class ListPlannerData: ListData
    {
        /// <summary>Initializes a new instance of <see cref="ListPlannerData"/>.</summary>
        public ListPlannerData()
        {
        }

        /// <summary>See <see cref="ListData.BuildQuery"/>.</summary>
        protected override string BuildQuery()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<Where>");
            builder.Append("<Or>");
            builder.Append("<And>");
                builder.Append("<Leq><FieldRef Name=\"EventDate\"/><Value Type=\"DateTime\">");
                builder.Append(Values[0].ToString());
                builder.Append("Z</Value></Leq>");
                builder.Append("<Geq><FieldRef Name=\"EventDate\"/><Value Type=\"DateTime\">");
                builder.Append(Values[1].ToString());
                builder.Append("Z</Value></Geq>");
            builder.Append("</And>");
            builder.Append("<And>");
                builder.Append("<Leq><FieldRef Name=\"EventDate\"/><Value Type=\"DateTime\">");
                builder.Append(Values[0].ToString());
                builder.Append("Z</Value></Leq>");
                builder.Append("<Geq><FieldRef Name=\"EndDate\"/><Value Type=\"DateTime\">");
                builder.Append(Values[1].ToString());
                builder.Append("Z</Value></Geq>");
            builder.Append("</And>");
            builder.Append("</Or>");
            builder.Append("</Where>");
            return builder.ToString();
        }
    }
}
