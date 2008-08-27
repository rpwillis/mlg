using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using System.Data;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Data.SqlClient;
//using Microsoft.SharePoint.Portal.UserProfiles;
using Microsoft.SharePoint.Administration;
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint.WebControls;
//using localhost;
using MLG2007.Helper.ListSearch;

namespace MLG2007.Helper.SharePoint
{
	/// <summary>
	/// This component is used by the MyPlanner web part to retrieve data from SharePoint.
	/// 
	/// This component queries the list of event URL's set in the My Planner Web Part and 
	/// will optionally query the users' personal Quick Links list for any event's which 
	/// have been added there. It consolidates all these events in a typed dataset and 
	/// returns this to the Calendar component for rendering.
	/// </summary>
	public class SPSEvents : Appointments
	{
		private HttpContext cont;
		private DateTime startDate; 
		private DateTime endDate;
		private string quicklinksgroup;
		private Boolean hasError; 
		private string errorDesc;
		private string userID;
		private string quickLinkWSURL="";
		private string listTitles="";
		private string lists;
        private string dnsName;
		public string EventLists
		{
			get
			{
				return lists;
			}

			set
			{
				lists = value;
			}
		}
		public string ListTitles
		{
			get
			{
				return listTitles;
			}

			set
			{
				listTitles = value;
			}
		}		
		public string ErrorDesc
		{
			get
			{
				return errorDesc;
			}

			set
			{
				errorDesc = value;
			}
		}		
		public bool HasError
		{
			get
			{
				return hasError;
			}

			set
			{
				hasError = value;
			}
		}		
		public string LinkGroup
		{
			get
			{
				return quicklinksgroup;
			}

			set
			{
				quicklinksgroup = value;
			}
		}		
		public DateTime StartDate
		{
			get
			{
				return startDate;
			}

			set
			{
				startDate = value;
			}
		}		
		public DateTime EndDate
		{
			get
			{
				return endDate;
			}

			set
			{
				endDate = value;
			}
		}        
		public HttpContext Cont
		{
			get
			{
				return cont;
			}

			set
			{
				cont = value;
			}
		}
		public string UserID
		{
			get
			{
				return userID;
			}

			set
			{
				userID = value;
			}
		}

		public string QuickLinkWSURL
		{
			get
			{
				return quickLinkWSURL;
			}

			set
			{
				quickLinkWSURL = value;
			}
		}
        public string DNSName
        {
            get
            {
                return dnsName;
            }
            set
            {
                dnsName = value;
            }
        }
		public void GetData()
		{
			try
            {
                #region Quick Links Code
                // Get all of this user's quick links into a data set
				//process quick links only if quick links WS is supplied
                //if(QuickLinkWSURL.Length>0)
                //{
                //    QuickLinksExt qlWS = new QuickLinksExt();
                //    if(this.cont.User.Identity.AuthenticationType.ToUpper() == "BASIC")
                //        qlWS.Credentials = new System.Net.NetworkCredential(this.cont.User.Identity.Name, this.cont.Request.ServerVariables["AUTH_PASSWORD"]);
                //    else
                //        qlWS.Credentials = System.Net.CredentialCache.DefaultCredentials;

                //    qlWS.Url = QuickLinkWSURL;
                //    DataSet ds = qlWS.GetQuickLinks();

                //    // Iterate through each item in Quick Links
                //    if(ds!=null)
                //        foreach (DataRow dr in ds.Tables[0].Rows)
                //        {
                //            // We're only interested in Announcements
                //            if (dr["ContentClass"].ToString() == "STS_List_Events")
                //            {
                //                if (quicklinksgroup == "")		// Show items from all groups
                //                    GenerateEventList(dr["PageURL"].ToString(), dr["Title"].ToString());
                //                else							// Only show items from a specific group
                //                {
                //                    if (dr["Group"].ToString() == quicklinksgroup)
                //                        GenerateEventList(dr["PageURL"].ToString(), dr["Title"].ToString().ToLower());
                //                }
                //            }
                //        }
                //}
                #endregion
                // Go through the other supplied lists, and find the announcements in each one
				if (lists.Length != 0)
				{
					string[] listarr = lists.Split(',');
					string[] titlearr = ListTitles.Split(',');
					if(listarr.Length != titlearr.Length)
						throw new Exception("The number of list url's does not match number of list titles.");
					
					for(int i=0;i<listarr.Length;i++)
						GenerateEventList(listarr[i], titlearr[i]);
				}
			}

			catch (Exception exc)
			{
				ErrorDesc = exc.Message;
				HasError = true;
			}
		}
		private void GenerateEventList(string URL, string ListTitle)
		{
			string WSURL="";
			string ListURLPart="";

			URL = URL.ToLower();

			// Some entries will already have "allitems.aspx", and some won't. Remove it to give us a consistent URL to work with
			URL = URL.Replace("allitems.aspx", "");
            URL = URL.Replace("calendar.aspx", "");    

			// Trim the trailing slash if there
			if (URL.EndsWith("/"))
				URL = URL.TrimEnd('/');
			
			string[] ListURLBits = URL.Split('/');  

			//If the list title is not specified then try and determine form URL
			ListURLPart =  ListURLBits[ListURLBits.Length-1].ToString();
			if(ListTitle=="")
				ListTitle = ListURLPart;

			WSURL = URL.Replace("lists/" + ListURLPart,"_vti_bin/lists.asmx"); 

			//Use the list search class to query for list items via web services
			ListSearch.ListPlannerData listdata = new ListPlannerData();
			if(this.cont.User.Identity.AuthenticationType.ToUpper() == "BASIC")
			{
				listdata.UserName = this.cont.User.Identity.Name;
				listdata.Password = this.cont.Request.ServerVariables["AUTH_PASSWORD"];
			}
			listdata.RowLimit = 20;
			listdata.URL = WSURL.ToLower();
            listdata.ListName = ListTitle.Replace("%20", " ") ;
            listdata.DNSName = this.DNSName;
//			listdata.Conditions.Add("<Leq><FieldRef Name=\"EventDate\"/><Value Type=\"DateTime\">{0}Z</Value></Leq>");
//			listdata.Conditions.Add("<Geq><FieldRef Name=\"EventDate\"/><Value Type=\"DateTime\">{0}Z</Value></Geq>");
//			//listdata.Conditions.Add("<Geq><FieldRef Name=\"EndDate\"/><Value Type=\"DateTime\">{0}Z</Value></Geq>");
			listdata.Values.Add(EndDate.ToString("s"));
			listdata.Values.Add(StartDate.ToString("s"));
			
			listdata.SelectFields.Add("Title");
			listdata.SelectFieldTypes.Add("System.String");
			listdata.SelectFields.Add("ID");
			listdata.SelectFieldTypes.Add("System.Double");
			listdata.SelectFields.Add("EventDate");
			listdata.SelectFieldTypes.Add("System.DateTime");
			listdata.SelectFields.Add("EndDate");
			listdata.SelectFieldTypes.Add("System.DateTime");
			listdata.SelectFields.Add("RecurrenceData");
			listdata.SelectFieldTypes.Add("System.String");

			listdata.OrderBy="<OrderBy><FieldRef Name=\"Modified\" Ascending=\"FALSE\"/></OrderBy>";
			
			listdata.Debug=false;
			listdata.GetData();

			if(listdata.HasError)
			{
				this.HasError = true;
				this.ErrorDesc = listdata.ErrorMessage;	
			}
			string TargetURL;

			foreach (DataRow item in listdata.Tables[0].Rows)
			{   
				try
				{
					AppointmentRow dr = this.Appointment.NewAppointmentRow();
					
					dr["Title"] = item["Title"];
					
					TargetURL = URL.Replace("lists/" + ListURLPart,"_layouts/") + System.Threading.Thread.CurrentThread.CurrentUICulture.LCID.ToString() + "/LgUtilities/showevent.aspx?URL=" + URL + "&ID=" + item["ID"].ToString() + "&DNSName="+this.DNSName;
					dr.URL = TargetURL;

					dr.ID = item["ID"].ToString(); 
					
					if (item["EventDate"] != null)
					{
						dr.BeginDate = ((DateTime)item["EventDate"]).ToUniversalTime(); 
						//dr.BeginDate = dr.BeginDate.ToUniversalTime();
					}

					try
					{
						if (item["EndDate"] != null)
						{
							//dr["EndDate"] = item["EndDate"];
							//						dr.EndDate = dr.EndDate.ToUniversalTime();
							dr.EndDate = ((DateTime)item["EndDate"]).ToUniversalTime(); 
						}
					}
					catch
					{
						dr["EndDate"] = item["EndDate"];
					}

					/////////////////////////////////////////////////////////////////////////////////////
					string recurrence_data = "";

					if (item["RecurrenceData"] != null)
					{
						recurrence_data = item["RecurrenceData"].ToString();
						recurrence_data = System.Web.HttpUtility.HtmlDecode(recurrence_data);
						if(recurrence_data != "")
							dr.Recurrent = true;
					}
					/////////////////////////////////////////////////////////////////////////////////////
					
					dr.Source = "SPSEvents";

					this.Appointment.AddAppointmentRow(dr);
				}
				catch (System.Data.ConstraintException)
				{
					// Ignore this exception. Just means a row was added twice.
				}
			}
			listdata.Dispose();
		}
      
        
	}
}
