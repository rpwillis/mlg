

/*Date: 06-03-2007
* Checked In by:   Mohamed Yehia
* Description  :   Corrected the query comilation to allow more than two fields - Resolved issue # 136916.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.WebParts.MetaSearch.Helper.QueryHelper
{
    /// <summary>
    /// Builds CAML query depedning on the fields
    /// </summary>
    internal class QueryManager
    {

        private const string WhereStartTag = "<Where>";
        private const string WhereEndTag = "</Where>";
        private const string AndStartTag = "<And>";
        private const string AndEndTag = "</And>";

        List<QueryFieldBase> fields;
        List<QueryFieldBase> queryFields;

        string viewFields;

        public string ViewFields
        {
            get { return viewFields; }
        }
        string query;

        public string Query
        {
            get { return query; }
        }

       

        public QueryManager()
        {
            fields = new List<QueryFieldBase>();
            queryFields = new List<QueryFieldBase>();
          
        }

        /// <summary>
        /// Add a fields to hte list of fields for later comilation
        /// </summary>
        /// <param name="queryField"></param>
        public void AddField(QueryFieldBase queryField)
        {
            
            fields.Add(queryField);
            if (queryField.GetType() == typeof(ContainsQueryField) || queryField.GetType() == typeof(EqualsQueryField))
                queryFields.Add(queryField);
        }


     
     
        public void Compile()
        {
            CompileViewFields();

            CompileQueryFields();
           
            
        }

        private string AddAndTags(string firstOperand, string secondOperand)
        {
            return (AndStartTag + firstOperand + secondOperand + AndEndTag);
        }
        private void CompileQueryFields()
        {
            

            string temp = AddAndTags(queryFields[0].GetQueryXml(),queryFields[1].GetQueryXml());
            

          if( queryFields.Count >2)
          {  for(int j=2;j<queryFields.Count ;j++)
                {

                   temp = AddAndTags(temp,queryFields[j].GetQueryXml() );


                }
          }


           
                

            query =  (WhereStartTag + temp + WhereEndTag);
        }

        private void CompileViewFields()
        {
            StringBuilder sbViewFields = new StringBuilder();
            foreach (QueryFieldBase field in fields)
            {
                if (sbViewFields.ToString().IndexOf(field.GetViewFieldXml()) == -1)
                    sbViewFields.Append(field.GetViewFieldXml());

            }
            viewFields = sbViewFields.ToString();
        }
      


    }
}
