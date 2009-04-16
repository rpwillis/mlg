using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace MLG2007.LGUtilities
{

    /// <summary>
    ///	This web page is used to show a SharePoint Event in a separate window, without 
    ///	the usual SharePoint navigation buttons. This is used by the MyPlanner web part. 
    ///	If the standard SharePoint event page was used in a pop-up window, the user would 
    ///	be able to click buttons within this window and navigate off to other places in 
    ///	the portal. This is not desirable in a pop-up window, as it confuses the 
    ///	navigation structure. Therefore the decision was made to write this page to 
    ///	show a read-only view of an event.
    ///	
    /// This page is hosted at C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\TEMPLATE\LAYOUTS\1033
    /// The DLL is hosted at   C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\TEMPLATE\LAYOUTS\BIN
    /// 
    /// Inputs: NewsURL	e.g.	"http://portal.demo.edu/sites/AmstelPS/Lists/School%20News/"
    ///			NewsID	e.g.	1
    ///			
    ///	The field names come from: C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\TEMPLATE\1033\STUDENT\LISTS\EVENTS
    /// </summary>

    public partial class ShowEventForm : System.Web.UI.Page
    {

         Literal ltlLBTitle = new Literal();
         Literal ltlBegin = new Literal();
         Literal ltlEnd = new Literal();
         Literal ltlDescription = new Literal();
         Literal ltlLocation = new Literal();
         Literal ltlCreatedAt = new Literal();
         Literal ltlModifiedAt = new Literal();
         Literal ltlListName = new Literal();
         Literal ltlTitle = new Literal();
         Literal ltlItemTitle = new Literal();
         Literal ltlItemBegin = new Literal();
         Literal ltlItemEnd = new Literal();
         Literal ltlItemDescription = new Literal();
         Literal ltlItemLocation = new Literal();
         Literal ltlItemCreated = new Literal();
         Literal ltlItemLastModified = new Literal();

         ///<summary>A literal value.</summary>
         protected Literal LtlLBTitle
         {
             get { return ltlLBTitle  ;}
             set { ltlLBTitle = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlBegin
         {
             get { return ltlBegin  ;}
             set { ltlBegin = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlEnd
         {
             get { return ltlEnd  ;}
             set { ltlEnd = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlDescription
         {
             get { return ltlDescription  ;}
             set { ltlDescription = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlLocation
         {
             get { return ltlLocation  ;}
             set { ltlLocation = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlCreatedAt
         {
             get { return ltlCreatedAt  ;}
             set { ltlCreatedAt = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlModifiedAt
         {
             get { return ltlModifiedAt  ;}
             set { ltlModifiedAt = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlListName
         {
             get { return ltlListName  ;}
             set { ltlListName = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlTitle
         {
             get { return ltlTitle  ;}
             set { ltlTitle = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlItemTitle
         {
             get { return ltlItemTitle  ;}
             set { ltlItemTitle = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlItemBegin
         {
             get { return ltlItemBegin  ;}
             set { ltlItemBegin = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlItemEnd
         {
             get { return ltlItemEnd  ;}
             set { ltlItemEnd = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlItemDescription
         {
             get { return ltlItemDescription  ;}
             set { ltlItemDescription = value ;}
         }

         ///<summary>A literal value.</summary>
         protected Literal LtlItemLocation
         {
             get { return ltlItemLocation  ;}
             set { ltlItemLocation = value ;}
         }

         protected Literal LtlItemCreated
         {
             get { return ltlItemCreated  ;}
             set { ltlItemCreated = value ;}
         }

         protected Literal LtlItemLastModified
         {
             get { return ltlItemLastModified  ;}
             set { ltlItemLastModified = value ;}
         }

        // Get resource manager
        //private ResourceManager rm = new ResourceManager("LearningPortalUtilities.Strings", Assembly.GetExecutingAssembly());

        protected void Page_Load(object sender, System.EventArgs e)
        {

            //            ltlPageTitle.Text = GetLocalResourceObject("X").ToString();
            ltlLBTitle.Text = GetLocalResourceObject ("Title").ToString() ;
            ltlBegin.Text = GetLocalResourceObject("Begin").ToString();// Resources.Strings.Begin;
            ltlEnd.Text =GetLocalResourceObject ("End").ToString();// Resources.Strings.End;
            ltlDescription.Text =GetLocalResourceObject ("Description").ToString();// Resources.Strings.Description;
            ltlLocation.Text = GetLocalResourceObject ("Location").ToString();//Resources.Strings.Location;
            ltlCreatedAt.Text = GetLocalResourceObject ("CreatedAt").ToString(); //Resources.Strings.CreatedAt;
            ltlModifiedAt.Text = GetLocalResourceObject ("LastModifiedAt").ToString();//Resources.Strings.LastModifiedAt;
            //ltlBy1.Text = GetLocalResourceObject ("By").ToString();//Resources.Strings.By;
            //ltlBy2.Text = GetLocalResourceObject ("By").ToString();//Resources.Strings.By;



            HtmlTableRow rw;
            HtmlTableCell cell;

            string NewsURL;
            int NewsID;

            NewsURL = Request.QueryString.GetValues("URL")[0];

            // Add the terminating slash if not present
            if (!NewsURL.EndsWith("/"))
                NewsURL += "/";

            NewsID = int.Parse(Request.QueryString.GetValues("ID")[0]);

            // Open the site, and the default web
            SPSite mysite = new SPSite(NewsURL);
            SPWeb myWeb = mysite.OpenWeb();

            // Get the view of this list, and from that the reference to the list itself
            SPView view = myWeb.GetViewFromUrl(NewsURL + "allitems.aspx");
            SPList list = myWeb.Lists[view.ParentList.Title];
            ltlListName.Text = list.Title;

            // Get the Item from the list
            SPListItem myItem = list.GetItemById(NewsID);
            ltlTitle.Text = myItem["ows_Title"].ToString();
            ltlItemTitle.Text = myItem["ows_Title"].ToString();

            // Begin (mandatory)
            ltlItemBegin.Text = ((DateTime)myItem["ows_EventDate"]).ToString();

            // End (not mandatory)
            if (myItem["ows_EndDate"] != null)
                ltlItemEnd.Text = ((DateTime)myItem["ows_EndDate"]).ToString();
            else
                ltlItemEnd.Text = "--";

            // Description (not mandatory)
            if (myItem["ows_Description"] != null)
                ltlItemDescription.Text = myItem["ows_Description"].ToString();
            else
                ltlItemDescription.Text = "--";

            // Location (not mandatory)
            if (myItem["ows_Location"] != null)
                ltlItemLocation.Text = myItem["ows_Location"].ToString();
            else
                ltlItemLocation.Text = "--";

            // Created
            ltlItemCreated.Text = ((DateTime)myItem["ows_Created"]).ToString();
            //ltlItemCreatedBy.Text = myItem["ows_Author"].ToString().Split('#')[1];

            // Modified
            ltlItemLastModified.Text = ((DateTime)myItem["ows_Modified"]).ToString();
            //ltlItemLastModifiedBy.Text = myItem["ows_Editor"].ToString().Split('#')[1];
            try
            {
                //Issue #9 - Sometimes the instant messenger icon doesn't appear
                //myWeb.SiteUsers.GetByID(UserID) is used to get the user object with id = UserID
                int AuthorID = int.Parse(myItem["ows_Author"].ToString().Split('#')[0].Replace(";", ""));
                int EditorID = int.Parse(myItem["ows_Editor"].ToString().Split('#')[0].Replace(";", ""));

                //if (myWeb.SiteUsers.GetByID(AuthorID).Email.Length > 0)
                //    imn0.Attributes.Add("onload", "IMNRC('" + myWeb.SiteUsers.GetByID(AuthorID).Email + "')");
                //if (myWeb.SiteUsers.GetByID(EditorID).Email.Length > 0)
                //    imn1.Attributes.Add("onload", "IMNRC('" + myWeb.SiteUsers.GetByID(EditorID).Email + "')");
            }
            catch
            {
            }
            //Clean up
            myWeb.Close();
            mysite.Close();
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
