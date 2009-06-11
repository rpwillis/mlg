/*
 * Checked in by: Peter Riad
 * Date: 27-12-2006
 * Description: update the web refrence to the lists.asmx web service, and generate the web id form the list URL
 * Reviewed by: Mohammed Yehia
*/
using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Microsoft.SharePoint;

namespace MLG2007.Helper.ListSearch
{
    /// <summary>
    /// Uses the built in WSS Lists Web Service to do a 
    /// custom search based on metadata supplied
    /// 
    /// This helper class is used to query a given SharePoint list via the 
    /// SharePoint lists web service. It takes the WHERE constraints, and the 
    /// fields that we want to select and returns the result as a dataset. It 
    /// is used to query a list which may have custom metadata to be queried 
    /// and/or returned such as the Professional Development and Learning Resources 
    /// lists or standard SharePoint Lists such as Events and News.
    /// </summary>
    public class ListData : DataSet
    {
        private ArrayList conditions;
        private ArrayList values;
        private ArrayList selectFields;
        private ArrayList selectFieldTypes;
        private bool recurseSubFolders = false;
        private bool hasError = false;
        private string errorMessage = "";
        private DataTable dt;
        private string listName = "";
        private string viewName = "";
        private int rowLimit = 150;
        private string url = "";
        private string username = "";
        private string password = "";
        private string domain = "";
        private bool debug = false;
        private string xml = "";
        private string nextpage = "";
        private string prevpage = "";
        private string pageinfo = "";
        private string orderby = "";
        private string dnsName = "";

        // Get resource manager
        private ResourceManager rm = new ResourceManager("ListSearch.Strings", Assembly.GetExecutingAssembly());

        /// <summary>Initializes a new instance of <see cref="ListData"/>.</summary>
        public ListData()
        {
            dt = new DataTable("Data");
            this.Tables.Add(dt);
            conditions = new ArrayList();
            values = new ArrayList();
            selectFields = new ArrayList();
            selectFieldTypes = new ArrayList();
        }

        /// <summary>The info for the next page.</summary>
        public string NextPageInfo
        {
            get { return nextpage; }
        }

        /// <summary>The info for the previous page.</summary>
        public string PrevPageInfo
        {
            get { return prevpage; }
        }

        /// <summary>The page info.</summary>
        public string PageInfo
        {
            get { return pageinfo; }
            set { pageinfo = value; }
        }

        /// <summary>Whether to output debug messages.</summary>
        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        /// <summary>Not used.</summary>
        public string DebugXML
        {
            get { return xml; }
            set { xml = value; }
        }

        /// <summary>The order by statement.</summary>
        public string OrderBy
        {
            get { return orderby; }
            set { orderby = value; }
        }

        /// <summary>The field types to select.</summary>
        public ArrayList SelectFieldTypes
        {
            get { return selectFieldTypes; }
            set { selectFieldTypes = value; }
        }

        /// <summary>The conditions to use.</summary>
        public ArrayList Conditions
        {
            get { return conditions; }
            set { conditions = value; }
        }

        /// <summary>The values.</summary>
        public ArrayList Values
        {
            get { return values; }
            set { values = value; }
        }

        /// <summary>The select fields.</summary>
        public ArrayList SelectFields
        {
            get { return selectFields; }
            set { selectFields = value; }
        }

        /// <summary>Whether to recurse sub folders or not.</summary>
        public bool RecurseSubFolders
        {
            get { return recurseSubFolders; }
            set { recurseSubFolders = value; }
        }

        /// <summary>Whether an error occurred.</summary>
        public bool HasError
        {
            get { return hasError; }
            set { hasError = value; }
        }

        /// <summary>Any error message.</summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
        /// <summary>The url to use.</summary>
        public string URL
        {
            get { return url; }
            set { url = value; }
        }

        /// <summary>The name of the list.</summary>
        public string ListName
        {
            get { return listName; }
            set { listName = value; }
        }

        /// <summary>The maximum number of rows to return.</summary>
        public int RowLimit
        {
            get { return rowLimit; }
            set { rowLimit = value; }
        }

        /// <summary>The user to connect as.</summary>
        public string UserName
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>The password to connect with.</summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>The domain the user is in.</summary>
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        /// <summary>The root of the url to use.</summary>
        public string DNSName
        {
            get { return dnsName; }
            set { dnsName = value; }
        }

        private string GetWebServiceUrl(string serverName, string url)
        {
            System.Uri uri = new System.Uri(url);
            string returnedUrl = string.Empty;
            if (uri.Scheme != "https" && !uri.IsDefaultPort)
                returnedUrl = uri.Scheme + "://" + serverName + ":" + uri.Port + "/_vti_bin/lists.asmx";
            else
                returnedUrl = uri.Scheme + "://" + serverName + "/_vti_bin/lists.asmx";
#if DEBUG   
         LogToFile ( returnedUrl );
#endif
            return returnedUrl;
        }

        private void LogToFile ( string entry )
        {

            SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
                                {
                                    System.IO.StreamWriter streamWriter = System.IO.File.CreateText (@"C:\Debugging" + DateTime.Now.ToLongDateString());
                                    streamWriter.WriteLine(entry);
                                    streamWriter.Close();
                                }));

        }

        /// <summary>Retrieves the data.</summary>
        public void GetData()
        {
            edu.demo.portal.Lists listService = new edu.demo.portal.Lists();

            if (UserName.Length > 0)
                listService.Credentials = new System.Net.NetworkCredential(UserName, Password);
            else
                listService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            listService.Url = GetWebServiceUrl(this.DNSName, this.URL);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement query = xmlDoc.CreateElement("Query");
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlElement queryOptions = xmlDoc.CreateElement("QueryOptions");
            int i = 0;

            if (SelectFields.Count == 0)
            {
                HasError = true;
                ErrorMessage = rm.GetString("ErrSelectFieldEmpty", CultureInfo.CurrentCulture);
                //				ErrorMessage="SelectFields is empty.";
                return;
            }
            //Add columns to datatable
            //Construct viewfields
            try
            {
                foreach (string field in SelectFields)
                {
                    dt.Columns.Add(field, System.Type.GetType(SelectFieldTypes[i].ToString()));
                    viewFields.InnerXml += "<FieldRef Name=\"" + field + "\" />";
                    i++;
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = rm.GetString("ErrProcessingSelectFields", CultureInfo.CurrentCulture) + ": " + ex.Message;
                //				ErrorMessage="Error processing SelectFields: " + ex.Message;
                return;
            }
            //RecurseSubFolders
            if (RecurseSubFolders)
                queryOptions.InnerXml = "<ViewAttributes Scope=\"Recursive\" />";

            try
            {
                //Paging
                prevpage = PageInfo;
                queryOptions.InnerXml += "<Paging ListItemCollectionPositionNext=\"" + System.Web.HttpUtility.HtmlEncode(PageInfo) + "\" />";
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = rm.GetString("ErrPagingError", CultureInfo.CurrentCulture) + ": " + ex.Message;
                //				ErrorMessage="Paging error: " + ex.Message;
                return;
            }

            try
            {
                //Build where clause
                query.InnerXml = BuildQuery();
                //Add order by
                if (OrderBy.Length > 0)
                    query.InnerXml += OrderBy;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = rm.GetString("ErrWhereClauseError") + ": " + ex.Message;
                //				ErrorMessage="Error building where clause: " + ex.Message;
                return;
            }

            //Run the query
            try
            {
                XmlNode nodeListItems = listService.GetListItems(ListName, viewName, query, viewFields, RowLimit.ToString(), queryOptions, WebId());
                if (debug)
                    DebugXML = nodeListItems.OuterXml;
                // Loop through each node in the XML response and display each item.
                System.Data.DataRow dr;
                foreach (XmlNode listItem in nodeListItems)
                    if (listItem.HasChildNodes)
                    {
                        if (listItem.Attributes["ListItemCollectionPositionNext"] != null)
                            nextpage = listItem.Attributes["ListItemCollectionPositionNext"].Value;
                        foreach (XmlNode nd in listItem.ChildNodes)
                            if (nd.Attributes != null)
                            {
                                dr = dt.NewRow();
                                for (int a = 0; a < SelectFields.Count; a++)
                                {
                                    if (nd.Attributes["ows_" + SelectFields[a].ToString()] != null)
                                        dr[a] = nd.Attributes["ows_" + SelectFields[a].ToString()].Value;
                                }
                                dt.Rows.Add(dr);
                            }
                    }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
#if DEBUG
                LogToFile ( "Error Message:"+ex.Message+"   Stack Trace"+ex.StackTrace );
#endif

            }
        }

        /// <summary>Builds the query.</summary>
        /// <returns>The query.</returns>
        virtual protected string BuildQuery()
        {
            string query = "";
            ArrayList terms = new ArrayList();
            for (int i = 0; i < Values.Count; i++)
                if (Values[i].ToString().Length > 0)
                    terms.Add(Conditions[i].ToString().Replace("{0}", Values[i].ToString()));
            AndTerms(ref terms, ref query);
            if (query.Length > 0)
                query = "<Where>" + query + "</Where>";
            return query;
        }

        private void AndTerms(ref ArrayList terms, ref string query)
        {
            if (terms.Count == 0)
                return;
            if (query.Length == 0)
                query = terms[terms.Count - 1].ToString();
            else
                query = "<And>" + terms[terms.Count - 1] + query + "</And>";
            terms.RemoveAt(terms.Count - 1);
            AndTerms(ref terms, ref query);
            return;
        }

        /// <summary>Retrieves the ID of the site.</summary>
        /// <returns>The ID of the site.</returns>
        string WebId()
        {
            using (Microsoft.SharePoint.SPSite site = new Microsoft.SharePoint.SPSite(URL))
            {
                using (Microsoft.SharePoint.SPWeb web = site.OpenWeb())
                {
                    return web.ID.ToString();
                }
            }
        }

    }
}
