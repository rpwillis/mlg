/*
 * date:31-5-2007
 * By: Wael Mounier
 * added support to show selected child on web part on a seperate label as an alternative for the Web Part title
 * /
/*
 * date:8-3-2007
 * By Marwan Tarek
 * added support to show selected child on web part title
 * /
/*
 * date:8-3-2007
 * By Marwan Tarek
 * solved issue 137323
 * /
/*
 * date: 1-3-2007
 * By Marwan Tarek
 * Description: sorted students alphabitcly
 * validate the student url is a valid site in the portal
 * Reviewed by: Mohamed Yehia
 */

/*
 * Date: 28-2-2007
 * By Marwan Tarek
 * Description: set most of the properties to default values according to MLG 2007 acrchticture
 * to decrease the number of configurations required
 * /
/*
 * Date: 27/2/2007
 * By: Marwan Tarek
 * retrive the children images from image library
 * implmented paging
 * implmented web part connection (cell provider) to provide selected child name
 * /
/*
 * Date: 14-01-2007
 * Checked In by:   Mohamed Yehia
 * Description  :   Migrated to new Sharepoint APIs.
 *              :   Promoted IPPhone to a web part property in AD Settings category
 *              :   Added a property for AD Entry point in AD Settings category
 *              :   Added  picture library property that stores the studnent images in sharepoint settings category
 *              :   Added ShowErrors property to display error messages.  This aids in debugging the web part.  This property is in the Misc category.
 *              :   Displays error message when Student object not found
 * Reviewed by: 
 */
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.Xml.Serialization;
using Microsoft.SharePoint.WebPartPages.Communication;
using System.Drawing;

namespace MLG2007.WebParts.MyChildren
{
    /// <summary>
    /// This web part displays the photos of the children of the signed on user. It is 
    /// used on the Parent Portal. For each child it displays links to the child's Planner 
    /// and Assignments.
    /// 
    /// The parent/child relationship is stored in the Active Directory. On the parents 
    /// account each child's domain\username is entered in the unused IP Phone attribute. 
    /// This attribute was chosen as it is not likely to be used and so the Active 
    /// Directory did not need to have the schema extended to accommodate this 
    /// information.
    /// 
    /// The photos are retrieved from the specified Picture Library %username%.jpg
    /// Where %username% is the child's sign-on name (e.g. student01).
    /// </summary>
    [ToolboxData("<{0}:MyChildren runat=server></{0}:MyChildren>"),
   XmlRoot(Namespace = "MyChildren")]
    [Guid("d208b435-7bf9-41b1-8ced-ee8a1565550f")]
    public class MyChildren : Microsoft.SharePoint.WebPartPages.WebPart, ICellProvider
    {

        #region "Constants"

        /// <summary>The name of the user name column in the children <see cref="DataTable"/>.</summary>
        protected const string UserNameColumnName = "username";
        /// <summary>The name of the user name column in the children <see cref="DataTable"/>.</summary>
        protected const string DisplayNameColumnName = "displayName";
        /// <summary>The name of the user name column in the children <see cref="DataTable"/>.</summary>
        protected const string ImageUrlColumnName = "imageUrl";
        /// <summary>The name of the user name column in the children <see cref="DataTable"/>.</summary>
        protected const string IsSelectedColumnName = "isSelected";
        /// <summary>The key to the generic error message resource.</summary>
        protected const string GenericErrMsg = "GenericErr";

        #endregion

        #region "Variables"

        //Carries any error messages that might occur
        private Label error = new Label();
        
        //The repeater item that is used to render the child items
        private Repeater childRepeater;

        //private memeber for teh AD child attribute
        private string _adChildAttribute = "otheripphone";
        //private member for holding picture library that holds student images
        private string _pictureLibraryUrl = "../../Students Picture Library";
        //private member for holding picture library title that holds student images
        private string _pictureLibraryTitle = "Students Picture Library";
        private string _defaultPictureURL = "../../Students Picture Library/DefaultChild.jpg";
        private UInt16 _pageSize = 5;
        // Get resource manager
        private ResourceManager _resourceManager = new ResourceManager("MLG2007.WebParts.MyChildren.Strings", Assembly.GetExecutingAssembly());
        //The ldap path that holds the parents and thier children
        private string _adEntryPoint = String.Empty;
        //private member for holding students site url to check the children member of this site
        private string _studentsSiteURL = "../../students";
        private bool _showErrors = false;

        /// <summary>The event to show the cell is initialized.</summary>
        [Obsolete]
        public event CellProviderInitEventHandler CellProviderInit;
        /// <summary>The event to show the cell is ready.</summary>
        [Obsolete]
        public event CellReadyEventHandler CellReady;

        //Keep a count of ICell connections
        private int _cellConnectedCount = 0;
        //Cell information
        private string _cellName = "child";
        private string _cellDisplayName = string.Empty;

        //Used to keep track of whether or not the Button in the Web Part was clicked
        private bool _cellClicked = false;
        private string _passedUserName = "";

        private string _user, _site;
        private bool _validMember;

        #endregion

        #region "Properties"

        /// <summary>
        /// This property specifies the LDAP Entry point on active directory to search for parents and children information
        /// typically the ldap path of the school OU 
        /// </summary>
        /// 
        [Browsable(true),
 ResourcesAttribute("ADEntryPointName", "ADSettingsCategory", "ADEntryPointDesc"),
  WebPartStorage(Storage.Shared)]
        public string ADEntryPoint
        {
            get { return _adEntryPoint; }
            set { _adEntryPoint = value; }
        }


        /// <summary>
        /// This propery specifies the Active Directory attribute that the web part uses to 
        /// get the current logged on children.  The default is ipphone
        /// </summary>
        /// 
        [Browsable(true),
       ResourcesAttribute("ADChildAttributeName", "ADSettingsCategory", "ADChildAttributeDesc"),
       WebPartStorage(Storage.Shared)]
        public string ADChildAttribute
        {
            get { return _adChildAttribute; }
            set { _adChildAttribute = value; }
        }

        /// <summary>
        /// This is the url of the picture library that holds the student images.  The current user must have access to the list.
        /// Student images must be stored using their user names.
        /// </summary>
        /// 
        [Browsable(true),
ResourcesAttribute("PictureLibraryUrlName", "PicturesSettingsCategory", "PictureLibraryUrlDesc"),
WebPartStorage(Storage.Shared)]
        public string PictureLibraryUrl
        {
            get { return _pictureLibraryUrl; }
            set { _pictureLibraryUrl = value; }
        }

        /// <summary>The title of the picture library.</summary>
        [Browsable(true),
ResourcesAttribute("PictureLibraryTitleName", "PicturesSettingsCategory", "PictureLibraryTitleDesc"),
WebPartStorage(Storage.Shared)]
        public string PictureLibraryTitle
        {
            get { return _pictureLibraryTitle; }
            set { _pictureLibraryTitle = value; }
        }

        /// <summary>The default picture url.</summary>
        [Browsable(true),
ResourcesAttribute("DefaultPictureURL", "PicturesSettingsCategory", "DefaultPictureDesc"),
WebPartStorage(Storage.Shared)]
        public string DefaultPictureURL
        {
            get { return _defaultPictureURL; }
            set { _defaultPictureURL = value; }
        }

        /// <summary>The number of children to show on a page.</summary>
        [Browsable(true),
ResourcesAttribute("PageSizeTitle", "PicturesSettingsCategory", "PageSizeDesc"),
WebPartStorage(Storage.Shared)]
        public UInt16 PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        /// <summary>The url to the students site.</summary>
        [Browsable(true),
ResourcesAttribute("StudentSiteURLTitle", "MiscSettingsCategory", "StudentSiteURLDesc"),
WebPartStorage(Storage.Shared)]
        public string StudentsSiteURL
        {
            get { return _studentsSiteURL; }
            set { _studentsSiteURL = stripURL(value); }
        }


        /// <summary>Whether to show errors or not.</summary>
        [Browsable(true),
ResourcesAttribute("ShowErrorsName", "MiscSettingsCategory", "ShowsErrorsDesc"),
WebPartStorage(Storage.Shared)]
        public bool ShowErrors
        {
            get { return _showErrors; }
            set { _showErrors = value; }
        }

        private int CurrentPage
        {
            get
            {
                object obj = this.ViewState["CurrentPage"];
                if (obj == null)
                    return 0;
                else
                    return (int)obj;
            }
            set { this.ViewState["CurrentPage"] = value; }
        }

        #endregion

        #region "Public and Proteced Methods"
        /// <summary>Get the student image given the student name.</summary>
        /// <param name="studentName">The name of the student.</param>
        /// <returns>The url to the image.</returns>
        protected string GetStudentImage(string studentName)
        {
            try
            {
                System.Uri picLibURL = new Uri(new Uri(Context.Request.Url.ToString()), _pictureLibraryUrl);
                using (SPSite site = new SPSite(picLibURL.OriginalString.ToString()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList pictureLibrary = web.Lists[PictureLibraryTitle];
                        SPQuery q = new SPQuery();
                        q.Query = "<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>" + studentName + "</Value></Eq></Where>";
                        SPListItemCollection studentImages = pictureLibrary.GetItems(q);
                        if (studentImages != null && studentImages.Count > 0)
                        {
                            return web.Url + "/" + studentImages[0].Url.ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(string.Format(LoadResource("LoadPicErr"), studentName), string.Format(LoadResource("LoadPicErr"), studentName) + " " + LoadResource(GenericErrMsg) + " " + ex.Message);
                return null;
            }

        }

        /// <summary>Shows an error message.</summary>
        /// <param name="message">The message to show.</param>
        /// <param name="exceptionModeMessage">The message to show if ShowErrors is true.</param>
        protected void ShowMessage(string message, string exceptionModeMessage)
        {
            if (_showErrors)
            {
                ShowMessage(exceptionModeMessage);
            }
            else
            {
                ShowMessage(message);
            }
        }

        /// <summary>Shows an error message.</summary>
        /// <param name="message">The message to show.</param>
        protected void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(error.Text))
            {
                error.Text = message;
            }
            else
            {
                error.Text = error.Text + "<br/>" + message;
            }
        }

        /// <summary>Gets the children of the logged in user.</summary>
        /// <param name="result">The DataTable to fill with the results.</param>
        protected virtual void GetChildrenOfUser(DataTable result)
        {
            GetPropertyFromAD(result);
        }


        /// <summary>Loads a resource.</summary>
        public override string LoadResource(string strId)
        {

            return _resourceManager.GetString(strId, CultureInfo.CurrentCulture);
        }

        /// <summary>See <see cref="Control.CreateChildControls"/>.</summary>
        protected override void CreateChildControls()
        {
            try
            {
                //Check that all properites are set.
                if (!AllPropertiesSet())
                {
                    //Display error message indicating that not all peroperties are set.
                    ShowMessage(LoadResource("PropNotSet"));
                }
                else
                {
                    if (CheckStudentSiteExists())
                    {
                        GetChildRows();
                    }
                    else
                    {
                        throw new Exception(LoadResource("InvalidStudentURL"));
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == LoadResource("InvalidStudentURL"))
                    ShowMessage(LoadResource(GenericErrMsg), LoadResource(GenericErrMsg) + " " + LoadResource("InvalidStudentURL"));
                else
                    ShowMessage(LoadResource(GenericErrMsg), LoadResource(GenericErrMsg) + " " + ex.Message);

            }
            finally
            {
                Controls.Add(error);
            }

        }

        bool CheckStudentSiteExists()
        {
            
            Uri url = new Uri(new Uri(Context.Request.Url.ToString()), _studentsSiteURL);
            using (SPSite site = new SPSite(url.OriginalString))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    Uri webUrl = new Uri(web.Url);
                    return (String.Compare(url.AbsolutePath, webUrl.AbsolutePath, true) == 0);
                }
            }
        }


        void GetChildRows()
        {
            try
            {
                if (ViewState["ChildrensTable"] == null)
                {
                    //Get Children from the AD relationship
                    DataTable childrenDataTable = BuildTable();
                    GetChildrenOfUser(childrenDataTable);
                    if (childrenDataTable == null)
                    {
                        return;
                    }
                    if (childrenDataTable.Rows.Count == 0)
                    {
                        ShowMessage(LoadResource("NoChild"));
                        return;
                    }
                    //sort datatable by displabe name
                    DataView childrenDataView = childrenDataTable.DefaultView;
                    childrenDataView.Sort = DisplayNameColumnName;

                    _passedUserName = (string)childrenDataView[0][UserNameColumnName];
                    childrenDataView[0][IsSelectedColumnName] = true;
                    this.ViewState["_passedUserName"] = _passedUserName;
                    _cellClicked = true;
                    ViewState["ChildrensTable"] = childrenDataView.Table;

                }

                DataView sortingDataView = ((DataTable)ViewState["ChildrensTable"]).DefaultView;
                sortingDataView.Sort = DisplayNameColumnName;
                MainDraw(sortingDataView.Table);
            }
            catch (Exception exc)
            {
                ShowMessage(LoadResource(GenericErrMsg), LoadResource(GenericErrMsg) + " " + exc.Message);
            }
        }

        #endregion

        #region Private Methods
        DataTable BuildTable()
        {
            DataTable result = new DataTable("Children");
            result.Columns.Add(UserNameColumnName, typeof(System.String));
            result.Columns.Add(DisplayNameColumnName, typeof(System.String));
            result.Columns.Add(ImageUrlColumnName, typeof(System.String));
            result.Columns.Add(IsSelectedColumnName, typeof(System.Boolean));
            return result;
        }


        private bool AllPropertiesSet()
        {
            return (_adChildAttribute != String.Empty && _adEntryPoint != String.Empty && _pictureLibraryUrl != String.Empty && _pictureLibraryTitle != String.Empty
                && _studentsSiteURL != String.Empty);
        }

        private void GetPropertyFromAD(DataTable result)
        {
            string userName = this.Context.User.Identity.Name;
            using (HostingEnvironment.Impersonate())
            {
                DirectoryEntry dirEntry = new DirectoryEntry(_adEntryPoint);
                string studentName = "";

                // Trim the domain name
                if (userName.IndexOf("\\") != 0)
                    userName = userName.Substring(userName.LastIndexOf("\\") + 1);

                // We are storing the Children of a Parent in the  property of Active Directory specified
                System.DirectoryServices.DirectorySearcher mySearcher = new System.DirectoryServices.DirectorySearcher(dirEntry);
                mySearcher.PropertiesToLoad.Add(_adChildAttribute);
                //Set the filter for the current user
                mySearcher.Filter = "(&(objectCategory=user)(samaccountname=" + userName + "))";

                ResultPropertyValueCollection myResultPropColl = null;

                try
                {
                    myResultPropColl = mySearcher.FindOne().Properties[_adChildAttribute];
                }
                catch (Exception ex)
                {
                    ShowMessage(LoadResource(GenericErrMsg), String.Format(LoadResource("ErrorRetrievingChildren"), ex.ToString()));
                    return;
                }

                //if null an error occured
                if (myResultPropColl == null)
                {
                    ShowMessage(LoadResource(GenericErrMsg));
                    return;
                }
                //if count =0 then no childern
                if (myResultPropColl.Count == 0)
                {
                    ShowMessage(LoadResource("NoChild"));
                }

                // Loop through each found child, and return their Display Name , Image Url and First Name
                foreach (object myCollection in myResultPropColl)
                {
                    string orgStudentName = myCollection.ToString();

                    if (!IsMember(orgStudentName, _studentsSiteURL))
                        continue;

                    DataRow studentRow = result.NewRow();
                    studentRow[IsSelectedColumnName] = false;

                    studentRow[UserNameColumnName] = orgStudentName;

                    // Trim the domain name
                    if (orgStudentName.IndexOf("\\") != 0)
                        studentName = orgStudentName.Substring(orgStudentName.LastIndexOf("\\") + 1);

                    try
                    {
                        //read display name property
                        mySearcher.PropertiesToLoad.Add(DisplayNameColumnName);
                        mySearcher.Filter = "(&(objectCategory=user)(samaccountname=" + studentName + "))";

                        System.DirectoryServices.SearchResult SrchRes;
                        SrchRes = mySearcher.FindOne();

                        if (SrchRes != null)
                        {
                            studentRow[DisplayNameColumnName] = SrchRes.Properties[DisplayNameColumnName][0].ToString();
                            string pictureURL = GetStudentImage(studentName);
                            if (pictureURL == null)
                            {
                                System.Uri defaultpic = new Uri(new Uri(Context.Request.Url.ToString()), DefaultPictureURL);
                                pictureURL = defaultpic.OriginalString.ToString();
                            }
                            studentRow[ImageUrlColumnName] = pictureURL;

                            result.Rows.Add(studentRow);
                        }
                        else // No user object found for the attached child
                        {
                            ShowMessage(String.Format(LoadResource("NoChildFound"), studentName));
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(LoadResource(GenericErrMsg), String.Format(LoadResource("ErrorRetrievingChildInfo"), studentName, ex.Message));
                    }
                }

                mySearcher.Dispose();
            }
        }

        private bool IsMember(string userName, string siteURL)
        {
            _user = userName; _site = siteURL;
            SPSecurity.CodeToRunElevated checkSiteCode = new SPSecurity.CodeToRunElevated(CheckSite);
            SPSecurity.RunWithElevatedPrivileges(checkSiteCode);
            return _validMember;
        }

        private void CheckSite()
        {
            try
            {
                System.Uri url = new Uri(new Uri(Context.Request.Url.ToString()), _site);
                using (SPSite site = new SPSite(url.OriginalString))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        _validMember = web.DoesUserHavePermissions(_user, SPBasePermissions.Open);
                    }
                }
            }
            catch (SPException ex)
            {
                if (ex.Message == "The user does not exist or is not unique.")
                    ShowMessage(LoadResource(GenericErrMsg), LoadResource(GenericErrMsg) + " " + LoadResource("ChildDontExist"));
                else
                    throw;
            }
        }

        private void MainDraw(DataTable users)
        {
            //main table declartaion
            HtmlTable htmlTable = new HtmlTable();

            //Repeater Row that contains the child templete Items 
            HtmlTableRow rowInfo = new HtmlTableRow();
            HtmlTableCell cellInfo = new HtmlTableCell();
            cellInfo.ColSpan = 3;

            //PageDataSource
            PagedDataSource pageDataSource = new PagedDataSource();
            pageDataSource.DataSource = users.DefaultView;
            pageDataSource.AllowPaging = true;
            pageDataSource.PageSize = PageSize;
            pageDataSource.CurrentPageIndex = CurrentPage;

            //Children Templete repeater
            childRepeater = new Repeater();
            childRepeater.ItemTemplate = new ChildTemplate(ListItemType.Item, new ImageClickEventHandler(childImage_Clicked));
            childRepeater.DataSource = pageDataSource;
            childRepeater.DataBind();

            //add repeater to the Table
            cellInfo.Controls.Add(childRepeater);
            rowInfo.Cells.Add(cellInfo);
            htmlTable.Rows.Add(rowInfo);

            //The Navigation Row and cells
            HtmlTableRow rowNavigation = new HtmlTableRow();
            HtmlTableCell cellPrv = new HtmlTableCell();
            cellPrv.Width = "10px";
            HtmlTableCell cellNext = new HtmlTableCell();
            cellNext.Width = "10px";
            HtmlTableCell cellPadding = new HtmlTableCell();
            cellPadding.Width = "100%";

            //Previous & Next Link Buttons
            LinkButton btnprv = new LinkButton();
            btnprv.Text = LoadResource("Prvtext");
            btnprv.Click += new EventHandler(btnprv_Click);

            LinkButton btnnext = new LinkButton();
            btnnext.Text = LoadResource("Nexttext");
            btnnext.Click += new EventHandler(btnnext_Click);

            //Adding the navigation buttons to the HTML table
            if (!pageDataSource.IsFirstPage)
            {
                cellPrv.Controls.Add(btnprv);
            }
            rowNavigation.Cells.Add(cellPrv);

            if (!pageDataSource.IsLastPage)
            {
                cellNext.Controls.Add(btnnext);
            }
            rowNavigation.Cells.Add(cellNext);

            if (!pageDataSource.IsFirstPage || !pageDataSource.IsLastPage)
            {
                rowNavigation.Cells.Add(cellPadding);
                htmlTable.Rows.Add(rowNavigation);
            }

            Controls.Add(htmlTable);
        }

        private void SetSelectedChild()
        {
            foreach (DataRow row in ((DataTable)ViewState["ChildrensTable"]).Rows)
            {
                row[IsSelectedColumnName] = (row[UserNameColumnName].ToString() == this.ViewState["_passedUserName"].ToString());
            }
            childRepeater.DataBind();
        }

        private string stripURL(string URL)
        {
            int idx = URL.IndexOf("/Pages");
            if (idx != -1)
            {
                return URL.Substring(0, idx);
            }
            else
                return URL;
        }

        #endregion

        #region Event Handlers

        private void btnprv_Click(object sender, EventArgs e)
        {
            CurrentPage -= 1;
            _cellClicked = true;
            this.ChildControlsCreated = false;
        }

        private void btnnext_Click(object sender, EventArgs e)
        {
            CurrentPage += 1;
            _cellClicked = true;
            this.ChildControlsCreated = false;
        }

        private void childImage_Clicked(object obj, ImageClickEventArgs evt)
        {
            _cellClicked = true;
            ImageButton tempimg = ((ImageButton)obj);
            _passedUserName = tempimg.CommandArgument;
            this.ViewState["_passedUserName"] = _passedUserName;
            SetSelectedChild();
        }

        #endregion

        #region ICellProvider Members

        /// <summary>Part of the old style connection interface.</summary>
        [Obsolete]
        public override void EnsureInterfaces()
        {
            //Registers an interface for the Web Part
            RegisterInterface("MyCellProviderInterface_WPQ_",  //InterfaceName
               InterfaceTypes.ICellProvider,               //InterfaceType
               WebPart.UnlimitedConnections,               //MaxConnections
               ConnectionRunAt.Server,            //RunAtOptions
               this,                                 //InterfaceObject
               "CellProviderInterface_WPQ_",               //InterfaceClientReference
               "Provide child",               //MenuLabel
               "Provides child username");               //Description
        }

        /// <summary>Part of the old style connection interface.</summary>
        [Obsolete]
        public override ConnectionRunAt CanRunAt()
        {
            //This Web Part can run on both the client and the server
            return ConnectionRunAt.Server;
        }

        /// <summary>Part of the old style connection interface.</summary>
        [Obsolete]
        public override void PartCommunicationConnect(string interfaceName, WebPart connectedPart, string connectedInterfaceName, ConnectionRunAt runAt)
        {
            //Check to see if this is a client-side part
            if (runAt == ConnectionRunAt.Client)
            {
                //This is a client-side part
                return;
            }

            //Must be a server-side part so need to create the Web Part's controls
            EnsureChildControls();

            //Check if this is my particular cell interface
            if (interfaceName == "MyCellProviderInterface_WPQ_")
            {
                //Keep a count of the connections
                _cellConnectedCount++;
            }
        }

        /// <summary>Part of the old style connection interface.</summary>
        [Obsolete]
        public override void PartCommunicationInit()
        {
            //If the connection wasn't actually formed then don't want to send Init event
            if (_cellConnectedCount > 0)
            {
                //If there is a listener, send Init event
                if (CellProviderInit != null)
                {
                    //Need to create the args for the CellProviderInit event
                    CellProviderInitEventArgs cellProviderInitArgs = new CellProviderInitEventArgs();

                    //Set the FieldName
                    cellProviderInitArgs.FieldName = _cellName;
                    cellProviderInitArgs.FieldDisplayName = _cellDisplayName;

                    //Fire the CellProviderInit event.
                    CellProviderInit(this, cellProviderInitArgs);
                }
            }
        }

        /// <summary>Part of the old style connection interface.</summary>
        [Obsolete]
        public override void PartCommunicationMain()
        {
            //If the connection wasn't actually formed then don't want to send Ready event
            if (_cellConnectedCount > 0)
            {
                //If there is a listener, send CellReady event
                if (CellReady != null)
                {
                    //Need to create the args for the CellProviderInit event
                    CellReadyEventArgs cellReadyArgs = new CellReadyEventArgs();

                    //If user clicked button then send the value
                    if (_cellClicked)
                    {
                        //Set the Cell to the value of the TextBox text
                        //This is the value that will be sent to the Consumer
                        if (this.ViewState["_passedUserName"] != null)
                            cellReadyArgs.Cell = this.ViewState["_passedUserName"].ToString();
                        else
                            cellReadyArgs.Cell = "";
                    }
                    else
                    {
                        //The user didn't actually click the button
                        //so just send an empty string to the Consumer
                        cellReadyArgs.Cell = "";
                    }


                    //Fire the CellReady event.
                    //The Consumer will then receive the Cell value
                    CellReady(this, cellReadyArgs);

                }
            }
        }

        /// <summary>Part of the old style connection interface.</summary>
        [Obsolete]
        public void CellConsumerInit(object sender, CellConsumerInitEventArgs cellConsumerInitEventArgs)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    class ChildTemplate : System.Web.UI.ITemplate
    {
        #region Private Variables

        private ListItemType _templateType;
        private ImageClickEventHandler ChildImage_Clicked;

        #endregion

        #region Constructors

        public ChildTemplate(ListItemType type)
        {
            _templateType = type;
        }

        public ChildTemplate(ListItemType type, ImageClickEventHandler imgClicked)
        {
            _templateType = type;
            ChildImage_Clicked = imgClicked;
        }

        #endregion

        #region Public Methods

        public void InstantiateIn(System.Web.UI.Control container)
        {
            PlaceHolder placeHolder = new PlaceHolder();

            if (_templateType == ListItemType.Item)
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell = new HtmlTableCell("td");
                cell.Width = "100%";

                ImageButton childImage = new ImageButton();
                childImage.ID = "img";
                Unit imageWidth = new Unit("60px");
                childImage.Width = imageWidth;
                Unit imageHieght = new Unit("75px");
                childImage.Height = imageHieght;
                childImage.BorderStyle = BorderStyle.Double;
                childImage.CssClass = "ms-quickLaunch";
                
                
                childImage.Click += new ImageClickEventHandler(ChildImage_Clicked);

                Label childLabel = new Label();
                childLabel.ID = "lbl";

                cell.Controls.Add(childImage);
                cell.Controls.Add(new LiteralControl("<br>"));
                cell.Controls.Add(childLabel);
                cell.Controls.Add(new LiteralControl("<br>"));
                cell.Controls.Add(new LiteralControl("<br>"));
                row.Cells.Add(cell);

                placeHolder.Controls.Add(row);
                placeHolder.DataBinding += new EventHandler(PlaceHolder_DataBinding);

            }
            container.Controls.Add(placeHolder);
        }

        #endregion

        #region Event Handlers

        private void PlaceHolder_DataBinding(object sender, EventArgs e)
        {
            PlaceHolder placeHolder = (PlaceHolder)sender;
            RepeaterItem repeater = (RepeaterItem)placeHolder.NamingContainer;

            ((ImageButton)placeHolder.FindControl("img")).ImageUrl = DataBinder.Eval(repeater.DataItem, "imageUrl").ToString();
            ((ImageButton)placeHolder.FindControl("img")).CommandArgument = DataBinder.Eval(repeater.DataItem, "username").ToString();
            ((Label)placeHolder.FindControl("lbl")).Text = DataBinder.Eval(repeater.DataItem, "displayName").ToString();
            ((Label)placeHolder.FindControl("lbl")).Font.Bold = (bool)DataBinder.Eval(repeater.DataItem, "isSelected");

            if ((bool)DataBinder.Eval(repeater.DataItem, "isSelected"))
            {
                ((ImageButton)placeHolder.FindControl("img")).BorderWidth = new Unit("4px");
            }
            else
            {
                ((ImageButton)placeHolder.FindControl("img")).BorderWidth = new Unit("0px");
            }
                
        }

        #endregion
    }
}
