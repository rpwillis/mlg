using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI;
using System.Xml.Serialization;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages.Communication;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint;


namespace MLG2007.WebParts.MySites
{
    /// <summary>
    /// This web part iterates through all sites below a given root site, and shows a 
    /// list of sites that the current user has access to. 
    /// </summary>
    [ToolboxData("<{0}:My Sites runat=server></{0}:Teams>"), XmlRoot(Namespace = "MLG2007.WebParts.MySites")]
    [Guid("6d19703c-f590-46b0-99ae-5755103e43ab")]
    public class MySites : Microsoft.SharePoint.WebPartPages.WebPart
    {

        #region Variables
        SPRoleDefinition definition;
        string connectedUser;
        const string defaultUnitName = "site";
        string startsite;
        string unitName = defaultUnitName;
        string permission;
        int pageSize = 30;
        DataTable dataTable;
        GridView grid;
        bool includeDescription;
        //List<string> debug = new List<string>();

        #endregion Variables

        #region Resources
        // Get resource manager
        private ResourceManager rm = new ResourceManager("MLG2007.WebParts.MySites.Strings", Assembly.GetExecutingAssembly());

        // Must have this override for the ResourcesAttributes to work
        
        ///<summary>See <see cref="Microsoft.SharePoint.WebPartPages.WebPart.LoadResource"/>.</summary>
        public override string LoadResource(string strId)
        {
            return rm.GetString(strId, CultureInfo.CurrentCulture);
        }
        #endregion

        #region Properties
        ///<summary>The site to start from.</summary>
        [Browsable(true),
            ResourcesAttribute("StartSiteName", "CatSiteSettings", "StartSiteDesc"),
            WebPartStorage(Storage.Shared)]
        public string StartSite
        {
            get { return startsite; }
            set { startsite = value; }
        }

        ///<summary>The paging size.</summary>
        [Browsable(true),
           ResourcesAttribute("PageSizeName", "CatSiteSettings", "PageSizeDesc"),
           DefaultValue(30),
           WebPartStorage(Storage.Shared)]
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        ///<summary>The name of the site type.</summary>
        [Browsable(true),
            ResourcesAttribute("UnitName", "CatSiteSettings", "UnitDesc"),
            DefaultValue(defaultUnitName),
            WebPartStorage(Storage.Shared)]
        public string UnitName
        {
            get { return unitName; }
            set { unitName = value; }
        }

        ///<summary></summary>
        [Browsable(true),
            ResourcesAttribute("PermissionProperty", "CatSiteSettings", "PermissionDescription"),
            WebPartStorage(Storage.Shared)]
        public string Permission
        {
            get { return permission; }
            set { permission = value; }
        }

        ///<summary></summary>
        [Browsable(true),
            ResourcesAttribute("IncludeDescriptionProperty", "CatSiteSettings", "IncludeDescriptionDescription"),
            DefaultValue(false),
            WebPartStorage(Storage.Shared)]
        public bool IncludeDescription
        {
            get { return includeDescription; }
            set { includeDescription = value; }
        }

        ///<summary>The connected user.</summary>
        string ConnectedUser
        {
            get
            {
                if (string.IsNullOrEmpty(connectedUser))
                {
                    connectedUser = (string)ViewState["mlgMySitesUser"];
                }
                return connectedUser;
            }

            set
            {
                //debug.Add("Set ConnectedUser " + value);
                connectedUser = value;
                ViewState["mlgMySitesUser"] = connectedUser;
            }
        }
        #endregion

#region Protected Methods
        /// <summary>
        /// overrides the CreateChildControls of webpart
        /// </summary>
        protected override void CreateChildControls()
        {
            //debug.Add("CreateChildControls");
            base.CreateChildControls();

            //if the url is not given
            if (string.IsNullOrEmpty(StartSite))
            {
                SPWeb web = SPControl.GetContextWeb(Context);
                StartSite = web.Url;
            }

            if (PageSize < 1)
            {
                Controls.Add(new LiteralControl("The number of items per page is not valid, Please enter a number greater than 0."));
            }
            else
            {
                BuildDataTable();
                BuildForm();
                FindAccessibleSubSites();
            }
        }

        ///<summary>See <see cref="Control.OnPreRender"/>.</summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            /*
            foreach (string message in //debug)
            {
                Controls.Add(new LiteralControl("<br />" + message));
            }
            */
        }
#endregion



#region ConnectionConsumer
        ///<summary>The connection to My Children.</summary>
        [ConnectionConsumer("Web Part Consumer")]
        public void GetConnectedProviderInterface(IWebPartField connectProvider)
        {
            //debug.Add("GetConnectedProviderInterface");
            connectProvider.GetFieldValue(new FieldCallback(ReceiveField));
        }

        void ReceiveField(object field)
        {
            //debug.Add("ReceiveField");
            if (field != null)
            {
                string original = ConnectedUser;
                //debug.Add("Original " + original);
                ConnectedUser = field.ToString();
                //debug.Add("Retrieved " + field.ToString());
                // If connection changes after a postback then need to retrieve the sites again
                // If a postback then CreateChildControls will have been called already and the dataTable will exist
                if (original != ConnectedUser && dataTable != null)
                {
                    //debug.Add("Retrieve the sites");
                    grid.PageIndex = 0;
                    FindAccessibleSubSites();
                }
            }
        }
#endregion ConnectionConsumer

#region Private Methods
        ///<summary>Finds the role definition to use when filtering the webs by permission.</summary>
        void FindPermissionToUse()
        {
            if (!string.IsNullOrEmpty(permission))
            {
                try
                {
                    definition = SPControl.GetContextWeb(Context).RoleDefinitions[permission];
                    if (definition == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, Strings.PermissionNotExist, permission));
                    }
                }
                catch (SPException)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, Strings.PermissionNotExist, permission));
                }
            }
        }

        private void FindSitesForConnectedUser()
        {
            //debug.Add("FindSitesForConnectedUser " + ConnectedUser);
            bool currentSecurity = SPSecurity.CatchAccessDeniedException;
            SPSecurity.CatchAccessDeniedException = false;
            string actualUser = Context.User.Identity.Name;

            try
            {
                using (SPSite site = new SPSite(StartSite))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        foreach (SPWeb child in web.Webs)
                        {
                            try
                            {
                                using (child)
                                {
                                    //debug.Add("Checking " + child.Url + " for " + ConnectedUser);
                                    if (child.DoesUserHavePermissions(ConnectedUser, (SPBasePermissions)SPPermissionGroup.Reader))
                                    {
                                        //debug.Add("Adding " + child.Url);
                                        bool actualUserHasAccess = child.DoesUserHavePermissions(actualUser, (SPBasePermissions)SPPermissionGroup.Reader);
                                        AddSite(child, actualUserHasAccess);
                                    }
                                }
                            }
                            catch (UnauthorizedAccessException)
                            {
                            }
                        }
                    }
                }
            }
            finally
            {
                SPSecurity.CatchAccessDeniedException = currentSecurity;
            }
        }
        

        /// <summary>Finds all sites for the user.</summary>
        void FindSitesForUser()
        {
            //debug.Add("FindSitesForUser");
            using (SPSite site = new SPSite(StartSite))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    foreach (SPWeb child in web.GetSubwebsForCurrentUser())
                    {
                        using (child)
                        {
                            //debug.Add("Adding " + child.Url);
                            AddSite(child, true);
                        }
                    }
                }
            }
        }

        void AddSite(SPWeb web, bool hasAccess)
        {
            bool addSite = false;

            if (string.IsNullOrEmpty(permission))
            {
                addSite = true;
            }
            else
            {
                foreach (SPRoleDefinition role in web.AllRolesForCurrentUser)
                {
                    if (role.Name == permission)
                    {
                        addSite = true;
                        break;
                    }
                }
            }

            if (addSite)
            {
                try 
                {
                    string url = web.Url;
                    if (hasAccess == false)
                    {
                        url = String.Empty;
                    }
                    string description = web.Description;
                    if (string.IsNullOrEmpty(description) && includeDescription)
                    {
                        description = Strings.NoSiteDescription;
                    }

                    dataTable.Rows.Add(new object[] { web.Title, url, description});
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        /// <summary>
        /// called from the CreatChildControls, to get the sub sites list
        /// </summary>
        private void FindAccessibleSubSites()
        {
            //debug.Add("FindAccessibleSubSites");
            try
            {
                dataTable.Rows.Clear();
                FindPermissionToUse();
                if (string.IsNullOrEmpty(ConnectedUser))
                {
                    FindSitesForUser();
                }
                else
                {
                    SPSecurity.CodeToRunElevated elevatedCode = new SPSecurity.CodeToRunElevated(FindSitesForConnectedUser);
                    SPSecurity.RunWithElevatedPrivileges(elevatedCode);
                }
                grid.DataBind();
            }
            catch (Exception e)
            {
                //debug.Add(e.ToString());
                Controls.Add(new LiteralControl("Error occurred: " + e.Message));
                Controls.Add(new LiteralControl("<br />" + e.StackTrace));
                return;
            }

        }


        void BuildDataTable()
        {
            dataTable = new DataTable();
            dataTable.Columns.Add("Title");
            dataTable.Columns.Add("URL");
            dataTable.Columns.Add("Description");
        }

        /// <summary> build the objects to be rendered </summary>
        private void BuildForm()
        {
            /*
            if (dataTable.Rows.Count < 1)
            {
                if (string.IsNullOrEmpty(UnitName))
                {
                    unitName = "sites";
                }
                Controls.Add(new LiteralControl(string.Format("You do not have access to any of the {0}.", UnitName)));
                return;
            }
            */

            DataView sortedView = new DataView(dataTable);
            sortedView.Sort = "Title";

            grid = new GridView();
            grid.ID = "grid";
            grid.DataSource = sortedView;
            grid.AutoGenerateColumns = false;
            grid.AllowSorting = false;
            grid.AllowPaging = true;
            grid.PageSize = PageSize;
            grid.GridLines = GridLines.None;
            grid.ShowHeader = false;
            grid.CssClass = "ms-summarycustombody";
            grid.RowCreated += new GridViewRowEventHandler(GridViewRowCreated);
            grid.PageIndexChanging += new GridViewPageEventHandler(GridPageIndexChanging);


            TemplateField descriptionField = new TemplateField();
            descriptionField.ItemTemplate = new MySitesTemplate(includeDescription);
            grid.Columns.Add(descriptionField);

            Controls.Add(grid);

        }

        void GridPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            grid.DataBind();
        }

        void GridViewRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "ms-vb";
            }
        }

#endregion
    }

#region MySitesTemplate
    class MySitesTemplate : ITemplate
    {
        const string descriptionFormat = "<br/><span style=\"padding-left:10px\">{1}</span>";
        const string hasAccessFormat = "<a href=\"{2}\">{0}</a><br/>";
        const string noAccessFormat = "{0}";
        bool showDescription;

        public MySitesTemplate(bool showDescription)
        {
            this.showDescription = showDescription;
        }

        public void InstantiateIn(Control container)
        {
            Literal literal = new Literal();
            literal.DataBinding += new EventHandler(this.BindData);
            container.Controls.Add(literal);
        }

        public void BindData(object sender, EventArgs e)
        {
            Literal literal = (Literal)sender;
            GridViewRow container = (GridViewRow)literal.NamingContainer;
            DataRowView row = (DataRowView)container.DataItem;
            string url = row["URL"].ToString();
            string format = CreateFormat(url);
            literal.Text = string.Format(format, row["Title"].ToString(), row["Description"].ToString(), url);
        }

        string CreateFormat(string url)
        {
            string format = hasAccessFormat;
            if (string.IsNullOrEmpty(url))
            {
                format = noAccessFormat;
            }

            if (showDescription)
            {
                format += descriptionFormat;
            }
            return format;
        }
    }
#endregion MySitesTemplate
}
