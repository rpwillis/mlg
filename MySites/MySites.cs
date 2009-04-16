/*
 * Date: 08-03-2007
 * Issue solved:137208
 * Checked In by: Peter Riad
 * Description: chekc that the web URL returned equal to the requested URL
 */
/*
 * Date: 08-03-2007
 * Checked In by: Peter Riad
 * Description: fix the issue related to the display nameof the child that appears on the parent children classes page
 * we use SiteUsers insteade of Users of the web object to retrive the the dispaly name of the child
 */
/*
 * Date: 04-03-2007
 * Checked In by: Peter Riad
 * Description: Fix some UI issues related to the bullets that appear beside the sites names'
 */
/*
 * Date: 01-03-2007
 * Checked In by: Peter Riad
 * Description: The webpart is completed and the minnor issues related to UI is fixed.
 */
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.WebControls;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Data;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.WebPartPages.Communication;


namespace MLG2007.WebParts.MySites
{
    [Guid("6d19703c-f590-46b0-99ae-5755103e43ab")]
    /// <summary>
    /// This web part iterates through all sites below a given root site, and shows a 
    /// list of sites that the current user has access to. 
    /// </summary>
    //DefaultProperty("StartSite"),
    [ToolboxData("<{0}:My Sites runat=server></{0}:Teams>"),
       XmlRoot(Namespace = "MLG2007.WebParts.MySites")]
    public class MySites : Microsoft.SharePoint.WebPartPages.WebPart, ICellConsumer
    {
        #region ICellConsumerInterface

        public event CellConsumerInitEventHandler CellConsumerInit;

        //Used to keep track of whether or not the connection will be running client-side
        private bool _runAtClient = false;

        //Keep a count of ICell connections
        private int _cellConnectedCount = 0;

        //Cell information
        private string _cellName;
        private string _cellDisplayName;

        public override void EnsureInterfaces()
        {
            //Registers an interface for the Web Part.
            RegisterInterface("MyCellConsumerInterface_WPQ_",   //InterfaceName
               InterfaceTypes.ICellConsumer,               //InterfaceType
               WebPart.UnlimitedConnections,               //MaxConnections
               ConnectionRunAt.ServerAndClient,            //RunAtOptions
               this,                                 //InterfaceObject
               "CellConsumerInterface_WPQ_",               //InterfaceClientReference
               "Consume child username",                        //MenuLabel
               "Consumes the username of a child.");               //Description
        }

        public override ConnectionRunAt CanRunAt()
        {
            //This Web Part can run on both the client and the server
            return ConnectionRunAt.ServerAndClient;
        }

        public override void PartCommunicationConnect(string interfaceName,
         WebPart connectedPart,
         string connectedInterfaceName,
         ConnectionRunAt runAt)
        {
            //Check to see if this is a client-side part
            if (runAt == ConnectionRunAt.Client)
            {
                //This is a client-side part
                _runAtClient = true;
                return;
            }

            //Must be a server-side part so need to create the Web Part's controls
            EnsureChildControls();

            //Check if this is my particular cell interface
            if (interfaceName == "MyCellConsumerInterface_WPQ_")
            {
                //Keep a count of the connections
                _cellConnectedCount++;
            }
        }

        public override void PartCommunicationInit()
        {
            //If the connection wasn't actually formed then don't want to send Init event
            if (_cellConnectedCount > 0)
            {
                //If there is a listener, send init event
                if (CellConsumerInit != null)
                {
                    //Need to create the args for the CellConsumerInit event
                    CellConsumerInitEventArgs cellConsumerInitArgs = new CellConsumerInitEventArgs();

                    //Set the FieldNames
                    cellConsumerInitArgs.FieldName = _cellName;

                    //Fire the CellConsumerInit event.
                    //This basically tells the Provider Web Part what type of
                    //cell the Consuemr is expecting in the CellReady event.
                    CellConsumerInit(this, cellConsumerInitArgs);
                }
            }
        }

        public override InitEventArgs GetInitEventArgs(string interfaceName)
        {
            //Check if this is my particular cell interface
            if (interfaceName == "MyCellConsumerInterface_WPQ_")
            {
                EnsureChildControls();

                //Need to create the args for the CellConsumerInit event
                CellConsumerInitEventArgs cellConsumerInitArgs = new CellConsumerInitEventArgs();

                //Set the FieldName
                cellConsumerInitArgs.FieldName = _cellName;
                cellConsumerInitArgs.FieldDisplayName = _cellDisplayName;

                //return the InitArgs
                return (cellConsumerInitArgs);
            }
            else
            {
                return (null);
            }
        }

        public void CellProviderInit(object sender, CellProviderInitEventArgs cellProviderInitArgs)
        {
        }

        public void CellReady(object sender, CellReadyEventArgs cellReadyArgs)
        {
            //Set the label text to the value of the "Cell" that was passed by the Provider
            if (cellReadyArgs.Cell != null)
            {
                if (string.IsNullOrEmpty(cellReadyArgs.Cell.ToString()))
                    return;
                if (CurrentSelectedUser != cellReadyArgs.Cell.ToString())
                    CurrentPage = 0;
                CurrentSelectedUser = cellReadyArgs.Cell.ToString();
                ChildControlsCreated = false;
            }
        }
        #endregion

        #region Variables
        private const string defaultSite = "";
        private const string defaultUnitName = "site";
        private const bool defaultexpandTop = true;

        private string startsite = defaultSite;
        private string unitName = defaultUnitName;
        private uint pageSize = 10;
        private DataSet dataSet = new DataSet();
        private LinkButton next, prev;
        private Repeater repeater;
        SPWebCollection webCollection;
        #endregion

        #region Resources
        // Get resource manager
        private ResourceManager rm = new ResourceManager("MLG2007.WebParts.MySites.Strings", Assembly.GetExecutingAssembly());

        // Must have this override for the ResourcesAttributes to work
        public override string LoadResource(string strId)
        {
            return rm.GetString(strId, CultureInfo.CurrentCulture);
        }
        #endregion

        #region Properties
        [Browsable(true),
            ResourcesAttribute("StartSiteName", "CatSiteSettings", "StartSiteDesc"),
            DefaultValue(defaultSite),
            WebPartStorage(Storage.Shared)]
        public string StartSite
        {
            get
            {
                return startsite;
            }

            set
            {
                startsite = value;
            }
        }

        [Browsable(true),
           ResourcesAttribute("PageSizeName", "CatSiteSettings", "PageSizeDesc"),
           DefaultValue(10),
           WebPartStorage(Storage.Shared)]
        public uint PageSize
        {
            get
            {
                return pageSize;
            }

            set
            {
                pageSize = value;
            }
        }

        [Browsable(true),
            ResourcesAttribute("UnitName", "CatSiteSettings", "UnitDesc"),
            DefaultValue(defaultUnitName),
            WebPartStorage(Storage.Shared)]
        public string UnitName
        {
            get
            {
                return unitName;
            }

            set
            {
                unitName = value;
            }
        }

        private int CurrentPage
        {
            get
            {
                // look for current page in ViewState
                object currPage = this.ViewState["SitesCurrentPage"];
                if (currPage == null)
                {
                    this.ViewState["SitesCurrentPage"] = 0;
                    return 0;	// default to showing the first page
                }
                else
                    return (int)currPage;
            }

            set
            {
                this.ViewState["SitesCurrentPage"] = value;
            }
        }

        private string CurrentSelectedUser
        {
            get
            {
                // look for current selected user in ViewState
                object currentSelectedUser = this.ViewState["_CurrentSelectedUser"];
                if (currentSelectedUser == null)
                {
                    this.ViewState["_CurrentSelectedUser"] = Context.User.Identity.Name;
                    return Context.User.Identity.Name;	// default to showing the first page
                }
                else
                    return (string)currentSelectedUser;
            }

            set
            {
                this.ViewState["_CurrentSelectedUser"] = value;
            }
        }

        private string CurrentSelectedUserDisplayName
        {
            get
            {
                // look for current selected user in ViewState
                object currentSelectedUserDisplayName = this.ViewState["_CurrentUserDispalyName"];
                if (currentSelectedUserDisplayName == null)
                {
                    return null;	// default to showing the first page
                }
                else
                    return (string)currentSelectedUserDisplayName;
            }

            set
            {
                this.ViewState["_CurrentUserDispalyName"] = value;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// overrides the CreateChildControls of webpart
        /// </summary>
        protected override void CreateChildControls()
        {
            GetAccessibleSubSites();
        }
        #endregion

        #region Private Methodes

        /// <summary>
        /// this function is called from the CreateChildControls, where it run the code with elevated permissions
        /// </summary>
        private void GetSitesForUserElevated()
        {
            SPSecurity.CodeToRunElevated checkSiteCode = new SPSecurity.CodeToRunElevated(GetSitesOfUser);
            SPSecurity.RunWithElevatedPrivileges(checkSiteCode);
        }

        /// <summary>
        /// crop the end of the URL
        /// </summary>
        /// <param name="enteredURL"></param>
        /// <returns></returns>
        private string FixURL(string enteredURL)
        {
            int idx = enteredURL.ToLower().IndexOf("/pages");
            if (idx != -1)
            {
                return enteredURL.Substring(0, idx).TrimEnd(new char []{'/'});
            }
            else
                return enteredURL.TrimEnd(new char[] { '/' });
        }
        /// <summary>
        /// the function to be called from the  GetSitesForUserElevated
        /// </summary>
        private void GetSitesOfUser()
        {
            try
            {
                SPSite site = new SPSite(StartSite);
                site.CatchAccessDeniedException = false;
                SPWeb web = site.OpenWeb();
                //if the required URL is not equal to the returned web, throw exception
                if(FixURL(StartSite).ToLower()!=web.Url.ToLower().TrimEnd(new char []{'/'}))
                {
                    throw new Exception();
                }
                webCollection = web.Webs;

                //build the data set
                BuildDataSetStructure();

                CurrentSelectedUserDisplayName = null;
                //loop over the sub webs of the required site, and get the collection of webs that the user got permission on.
                for (int i = 0; i < webCollection.Count; i++)
                {
                    // if a connection is not avilable, set the childusername to the login username
                    if (string.IsNullOrEmpty(CurrentSelectedUser))
                    {
                        CurrentSelectedUser = Context.User.Identity.Name;
                    }
                    if (webCollection[i].DoesUserHavePermissions(CurrentSelectedUser, (SPBasePermissions)SPPermissionGroup.Reader))
                    {
                        try
                        {
                            //save the display name of the child
                            CurrentSelectedUserDisplayName = webCollection[i].SiteUsers[CurrentSelectedUser].Name;
                        }
                        catch
                        {//the user is not in the collection, ignore it
                        }
                        //in case of parent access to child sites, check that the parent got accss permissions on the child sites list
                        if (webCollection[i].DoesUserHavePermissions(Context.User.Identity.Name, (SPBasePermissions)SPPermissionGroup.Reader))
                        {
                            //if login user got permission on the site, render as link, else as text
                            dataSet.Tables["Links"].Rows.Add(new object[] { webCollection[i].Title, webCollection[i].Url, "1" });
                        }
                        else
                        {
                            dataSet.Tables["Links"].Rows.Add(new object[] { webCollection[i].Title, webCollection[i].Url, "2" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// build the dataset the holds the links information
        /// </summary>
        private void BuildDataSetStructure()
        {
            //build the dataset
            dataSet = new DataSet();
            dataSet.Tables.Add("Links");
            dataSet.Tables["Links"].Columns.Add("Title");
            dataSet.Tables["Links"].Columns.Add("URL");
            dataSet.Tables["Links"].Columns.Add("IsLink");
        }

        /// <summary>
        /// called from the CreatChildControls, to get the sub sites list
        /// </summary>
        private void GetAccessibleSubSites()
        {
            //if the url is not given
            if (string.IsNullOrEmpty(StartSite))
            {
                Controls.Add(new LiteralControl(LoadResource("ErrorNotConfigured")));
                return;
            }
            if (PageSize < 1)
            {
                Controls.Add(new LiteralControl(LoadResource("PageSizeError")));
                return;
            }
            try
            {
                GetSitesForUserElevated();
            }
            catch (Exception ex)
            {
                Controls.Add(new LiteralControl(LoadResource("ErrorURL")));
                return;
            }

            BuildForm();

        }

        /// <summary>
        /// build the objects to be rendered
        /// </summary>
        private void BuildForm()
        {
            if (dataSet.Tables["Links"].Rows.Count < 1)
            {
                if (string.IsNullOrEmpty(UnitName))
                {
                    unitName = LoadResource("Sites");
                }
                //in case of the parent site, render another message to ask him to select a child.
                if (CurrentSelectedUser != Context.User.Identity.Name)
                {
                    Controls.Add(new LiteralControl(string.Format(LoadResource("NoSitesForLoginUserSelectAnother"), UnitName)));
                }
                else
                {
                    Controls.Add(new LiteralControl(string.Format(LoadResource("NoSiteAvilable"), UnitName)));
                }
                return;
            }
            try
            {
                PagedDataSource pDataSource = BuildFormControls();

                // Disable Prev or Next LinkButtons if necessary
                prev.Visible = !pDataSource.IsFirstPage;
                next.Visible = !pDataSource.IsLastPage;

                RenderForm(pDataSource);


            }
            catch (Exception ex)
            {
                Controls.Add(new LiteralControl(LoadResource("ErrorURL")));
            }
        }

        /// <summary>
        /// build the controls like buttons and the repeater control
        /// </summary>
        /// <returns></returns>
        private PagedDataSource BuildFormControls()
        {
            PagedDataSource pDataSource = new PagedDataSource();
            DataView sortedView = new DataView(dataSet.Tables["Links"]);
            sortedView.Sort = "Title";

            pDataSource.DataSource = sortedView;
            pDataSource.AllowPaging = true;
            pDataSource.PageSize = (int)PageSize;

            //if the user changes the page size, 
            //and the selected page number was greater than the no. of pages in the grid,
            //reset the current page index to 0
            if (CurrentPage > pDataSource.PageCount)
                CurrentPage = 0;
            pDataSource.CurrentPageIndex = CurrentPage;

            repeater = new Repeater();
            repeater.ItemTemplate = new MyTemplate(ListItemType.Item);
            repeater.DataSource = pDataSource;
            repeater.DataBind();

            next = new LinkButton();
            next.Text = LoadResource("NextText");
            next.Click += new EventHandler(next_Click);

            prev = new LinkButton();
            prev.Text = LoadResource("PrevText");
            prev.Click += new EventHandler(prev_Click);

            return pDataSource;
        }

        /// <summary>
        /// render the controls in the desired shape
        /// </summary>
        /// <param name="pDataSource"></param>
        private void RenderForm(PagedDataSource pDataSource)
        {
            HtmlTable t1 = new HtmlTable();
            t1.Attributes["class"] = "ms-summarycustombody";
            t1.Style.Add(HtmlTextWriterStyle.MarginLeft, "5");
            t1.CellPadding = 0;
            t1.CellSpacing = 0;
            t1.Border = 0;
            t1.Width = "100%";
            //if we are showing sites for a user rather than the login user show a message with the display name
            if ((CurrentSelectedUser != Context.User.Identity.Name) && CurrentSelectedUserDisplayName != null)
            {
                HtmlTable headerTable = new HtmlTable();
                headerTable.Attributes["class"] = "ms-summarycustombody";
                headerTable.CellPadding = 0;
                headerTable.CellSpacing = 0;
                headerTable.Border = 0;
                headerTable.Width = "100%";
                HtmlTableRow messageRow = new HtmlTableRow();
                HtmlTableCell messageCell = new HtmlTableCell();
                if (string.IsNullOrEmpty(UnitName))
                {
                    unitName = LoadResource("Sites");
                }
                messageCell.InnerText = string.Format(LoadResource("VeiwSitesForUser"), UnitName, CurrentSelectedUserDisplayName);
                messageRow.Cells.Add(messageCell);
                headerTable.Rows.Add(messageRow);
                Controls.Add(headerTable);

            }
            HtmlTableRow r1 = new HtmlTableRow();
            HtmlTableRow r2 = new HtmlTableRow();
            HtmlTableCell r1c1 = new HtmlTableCell();


            HtmlTableCell r2c1 = new HtmlTableCell();
            r2c1.Align = "left";
            r2c1.ColSpan = 2;

            r1c1.Controls.Add(repeater);

            if (prev.Visible)
                r2c1.Controls.Add(prev);
            if (next.Visible)
                r2c1.Controls.Add(next);

            r1.Cells.Add(r1c1);
            r2.Cells.Add(r2c1);
            t1.Rows.Add(r1);
            t1.Rows.Add(r2);

            Controls.Add(t1);

        }

        /// <summary>
        /// next button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void next_Click(object sender, EventArgs e)
        {
            // Set viewstate variable to the next page
            CurrentPage += 1;

            // Reload controls
            ChildControlsCreated = false;

        }

        /// <summary>
        /// previous button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void prev_Click(object sender, EventArgs e)
        {
            // Set viewstate variable to the prev page
            CurrentPage -= 1;

            // Reload controls
            ChildControlsCreated = false;

        }
        #endregion
    }
    public class MyTemplate : System.Web.UI.ITemplate
    {
        ListItemType templateType;
        public MyTemplate(ListItemType type)
        {
            templateType = type;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Height = "18px";
            HtmlTableCell cell1 = new HtmlTableCell("td");
            cell1.Attributes["class"] = "ms-vb";
            cell1.Width = "10px";
            cell1.VAlign = "top";



            //add the image to the td
            Image image = new Image();
            image.AlternateText = "";
            image.ImageUrl = "/_layouts/images/square.gif";
            cell1.Controls.Add(image);
            cell1.Style.Add(HtmlTextWriterStyle.PaddingTop, "5px");

            HtmlTableCell cell2 = new HtmlTableCell("td");
            cell2.Attributes["class"] = "ms-vb";

            PlaceHolder ph = new PlaceHolder();
            HyperLink link = new HyperLink();
            link.ID = "link";

            if (templateType == ListItemType.Item)
            {
                cell2.Controls.Add(link);
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                ph.Controls.Add(row);
                ph.DataBinding += new EventHandler(ph_DataBinding);
            }
            container.Controls.Add(ph);
        }

        void ph_DataBinding(object sender, EventArgs e)
        {
            PlaceHolder ph = (PlaceHolder)sender;
            RepeaterItem ri = (RepeaterItem)ph.NamingContainer;
            string linkTitle = (string)DataBinder.Eval(ri.DataItem, "Title");
            string linkURL = (String)DataBinder.Eval(ri.DataItem, "URL");
            string Islink = (String)DataBinder.Eval(ri.DataItem, "IsLink");
            ((HyperLink)ph.FindControl("link")).Text = linkTitle;
            ((HyperLink)ph.FindControl("link")).NavigateUrl = linkURL;
            //if the link is to be disabled, disable it
            if (Islink == "2")
            {
                ((HyperLink)ph.FindControl("link")).Enabled = false;
            }
            else
            {
                ((HyperLink)ph.FindControl("link")).Enabled = true;
            }
        }

    }
}
