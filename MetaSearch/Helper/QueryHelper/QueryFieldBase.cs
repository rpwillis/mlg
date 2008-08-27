using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.WebParts.MetaSearch.Helper.QueryHelper
{
    /// <summary>
    /// Represents a base class for query fields that are used in buliding CAML queries
    /// </summary>
    internal abstract class QueryFieldBase
    {
        protected  const string FieldRefTemplateXml = "<FieldRef Name=\"{0}\" />";
        protected const string ValueTemplateXml = "<Value Type=\"Text\">{0}</Value>";

        private Hashtable xmlCharacters = new Hashtable();
         string fieldName;

        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }
        string queryValue;

        public string QueryValue
        {
            get { return queryValue; }
            set { queryValue = value; }
        }
        bool showInView;

        public bool ShowInView
        {
            get { return showInView; }
            set { showInView = value; }
        }

        /// <summary>
        /// Query base constricutor
        /// </summary>
        /// <param name="fieldInternalName">SPFileds internal name</param>
        /// <param name="searchValue">Search keyword</param>
        /// <param name="showInViewFields">Whether this field is available in the ViewFields property</param>
        public QueryFieldBase (string fieldInternalName, string searchValue, bool showInViewFields)
        {
            fieldName = fieldInternalName;
            queryValue = EncodeXmlCharacters( searchValue);
            showInView = showInViewFields;

            InitializeXmlCharacters();
        }
        /// <summary>
        /// Returns CAML XML to be put in ViewFields property of SPQuery object
        /// </summary>
        /// <returns></returns>
        public  string GetViewFieldXml()
        {
            if (showInView)
                return string.Format(FieldRefTemplateXml, FieldName);
            else
                return string.Empty;
        }
        /// <summary>
        /// Returns the XML of the field to be put in the Query property of SPQuery object
        /// </summary>
        /// <returns></returns>
        public virtual string GetQueryXml(){return string.Empty;}

        /// <summary>
        /// Encodes any invalid XML charactres in the  given string
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        private string EncodeXmlCharacters(string rawValue)
        {
            StringBuilder sb = new StringBuilder(rawValue);
            foreach (DictionaryEntry entry in xmlCharacters)
            {
                sb.Replace(entry.Key.ToString(),entry.Value.ToString());
            }
            return sb.ToString();
        }
        /// <summary>
        /// Initilizes a hastable the contains the invalid XML characters
        /// </summary>
        private void InitializeXmlCharacters()
        {
            xmlCharacters.Add("&", "&amp;");
            xmlCharacters.Add("<", "&lt;");
            xmlCharacters.Add(">", "&gt;");
            xmlCharacters.Add("\"", "&quot;");
            xmlCharacters.Add("\'", "&apos;");
        }
    }
}
