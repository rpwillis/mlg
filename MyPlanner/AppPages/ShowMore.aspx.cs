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
//using Planner;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.DirectoryServices;
using MLG2007.Helper.SharePointLearningKit;
using MLG2007.Helper.Exchange;
using MLG2007.Helper.SharePoint;
using MLG2007.Helper.ListSearch;




namespace MLG2007.LGUtilities
{
    /// <summary>
    /// Summary description for ShowMore.
    /// </summary>
    public partial class ShowMore : System.Web.UI.Page
    {
        private const int defaultTimeOutms = 10000;
        string userID = "";
        string csurl = "";
        string csclient = "";
        string exurl = "";
        string domain = "";
        string spsurl = "";
        string spstitles = "";
        string date = "";
        string quicklinksgroup = "";
        string quicklinkwsurl = "";
        string CssClass = "";
        string CssShowMoreClass = "";
        string CssAppointment = "";
        bool showErrors = false;
        bool EnableOverrideDomain = false;
        bool showSlk;
        bool showPersonalCalendar;
        string sharepointCalendarsIDList;
        string exchangeCalendarsIDList;
        string siteCollectionURL;
        int assignmentMode;
        string calendarNames = "";
        string calendarUrls = "";
        string exchangeCalednarsUrls = "";
        string currentUser = "";
        string dnsName = "";
        HtmlGenericControl pageTitle = new HtmlGenericControl();
        // Get resource manager
        //private ResourceManager rm = new ResourceManager("LearningPortalUtilities.Strings", Assembly.GetExecutingAssembly());

        ///<summary>The title of the page.</summary>
        protected HtmlGenericControl PageTitle
        {
            get { return pageTitle ;}
            set { pageTitle = value ;}
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Put user code to initialize the page here
            #region parse query string
            if (Request.QueryString["date"] == null)
            {

                //Response.Write("Error in parsing Date");
                Response.Write(GetLocalResourceObject("ErrorParsingDate").ToString());
                return;
            }
            else
                date = Request.QueryString["date"];

            if (Request.QueryString["siteColUrl"] == null)
            {
                //Response.Write("Error in parsing Date");
                Response.Write(GetLocalResourceObject("ErrorParsingDate").ToString());
                return;
            }
            else
                siteCollectionURL = Request.QueryString["siteColUrl"];

            if (Request.QueryString["slkMode"] == null)
            {
                //Response.Write("Error in parsing Date");
                Response.Write(GetLocalResourceObject("ErrorParsingDate").ToString());
                return;
            }
            else
                assignmentMode = int.Parse(Request.QueryString["slkMode"].ToString());


            if (Request.QueryString["userid"] == null)
            {
                //Response.Write("Error in parsing UserID");
                Response.Write(GetLocalResourceObject("ErrorParsingUserID").ToString());
                return;
            }
            else
                userID = Request.QueryString["userid"];

            if (Request.QueryString["enableoverridedomain"] == null)
            {
                //Response.Write("Error in parsing EnableOverrideDomain");
                Response.Write(GetLocalResourceObject("ErrorParsingEnableOverrideDomain").ToString());
                return;
            }
            else
                EnableOverrideDomain = Convert.ToBoolean(Request.QueryString["enableoverridedomain"]);

            //if (Request.QueryString["csurl"]==null)
            //{
            //    //Response.Write("Error in parsing Class Server URL");
            //    Response.Write(rm.GetString("ErrorParsingCSURL",CultureInfo.CurrentCulture));
            //    return;
            //}
            //else
            //    csurl=Request.QueryString["csurl"];

            //if (Request.QueryString["csclient"]==null)
            //{
            //    //Response.Write("Error in parsing CS teacher client URL");
            //    Response.Write(rm.GetString("ErrorParsingCSTeacherClientURL",CultureInfo.CurrentCulture));
            //    return;
            //}
            //else
            //    csclient=Request.QueryString["csclient"];

            if (Request.QueryString["exurl"] == null)
            {
                //Response.Write("Error in parsing Exchange URL");
                Response.Write(GetLocalResourceObject("ErrorParsingExURL").ToString());
                return;
            }
            else
                exurl = Request.QueryString["exurl"];

            if (Request.QueryString["domain"] == null)
            {
                //Response.Write("Error in parsing Domain name");
                Response.Write(GetLocalResourceObject("ErrorParsingDomain").ToString());
                return;
            }
            else
                domain = Request.QueryString["domain"];

            if (Request.QueryString["spsurl"] == null)
            {
                //Response.Write("Error in parsing SharePoint lists URLs");
                Response.Write(GetLocalResourceObject("ErrorParsingSPSListsURL").ToString());
                return;
            }
            else
                spsurl = Request.QueryString["spsurl"];

            if (Request.QueryString["spstitles"] == null)
            {
                //Response.Write("Error in parsing SharePoint Lists titles");
                Response.Write(GetLocalResourceObject("ErrorParsingSPSListsTitles").ToString());
                return;
            }
            else
                spstitles = Request.QueryString["spstitles"];

            //if (Request.QueryString["quicklinksgroup"]==null)
            //{
            //    //Response.Write("Error in parsing quicklinksgroup");
            //    Response.Write(rm.GetString("ErrorParsingQuickLinksGroup",CultureInfo.CurrentCulture));
            //    return;
            //}
            //else
            //    quicklinksgroup=Request.QueryString["quicklinksgroup"];

            //if (Request.QueryString["quicklinkwsurl"]==null)
            //{
            //    //Response.Write("Error in parsing quicklinkwsurl");
            //    Response.Write(rm.GetString("ErrorParsingQuickLinkWSURL",CultureInfo.CurrentCulture));
            //    return;
            //}
            //else
            //    quicklinkwsurl=Request.QueryString["quicklinkwsurl"];

            if (Request.QueryString["CssClass"] == null)
            {
                //Response.Write("Error in parsing CssClass");
                Response.Write(GetLocalResourceObject("ErrorParsingCssClassSS").ToString());
                return;
            }
            else
                CssClass = Request.QueryString["CssClass"];

            if (Request.QueryString["CssShowMoreClass"] == null)
            {
                //Response.Write("Error in parsing CssShowMoreClass");
                Response.Write(GetLocalResourceObject("ErrorParsingCssShowMoreClass").ToString());
                return;
            }
            else
                CssShowMoreClass = Request.QueryString["CssShowMoreClass"];

            if (Request.QueryString["CssAppointment"] == null)
            {
                //Response.Write("Error in parsing CssAppointment");
                Response.Write(GetLocalResourceObject("ErrorParsingCssAppointmentSS").ToString());
                return;
            }
            else
                CssAppointment = Request.QueryString["CssAppointment"];

            if (Request.QueryString["showerrors"] == null)
            {
                //Response.Write("Error in parsing CssAppointment");
                Response.Write(GetLocalResourceObject("ErrorParsingShowErrors").ToString());
                return;
            }
            else
                showErrors = Convert.ToBoolean(Request.QueryString["showerrors"]);

            #endregion

            if (Request.QueryString["showslk"] == null)
            {
                Response.Write("show slk is not passed");
            }
            else
                showSlk = bool.Parse(Request.QueryString["showslk"].ToString());

            if (Request.QueryString["showExchange"] == null)
            {
                Response.Write("showExchange is not passed");
            }
            else
                showPersonalCalendar = bool.Parse(Request.QueryString["showExchange"].ToString());

            if (Request.QueryString["selectedCalendars"] == null)
            {
                Response.Write("selectedCalendars is not passed");
            }
            else
                sharepointCalendarsIDList = Request.QueryString["selectedCalendars"].ToString();

            if (Request.QueryString["selectedCalendarsE"] == null)
            {
                Response.Write("selectedCalendarsE is not passed");
            }
            else
                exchangeCalendarsIDList = Request.QueryString["selectedCalendarsE"].ToString();

            if (Request.QueryString["DnsName"] == null)
            {

                //Response.Write("Error in parsing Date");
                Response.Write(GetLocalResourceObject("ErrorParsingDate").ToString());
                return;
            }
            else
                dnsName = Request.QueryString["DnsName"];

            PageTitle.InnerText = date;

            //StartDate = first day of the 'date' month
            DateTime StartDate = System.DateTime.Parse("1-" + DateTime.Parse(date).ToLocalTime().ToString("MMM-yyyy"));
            DateTime EndDate = StartDate.AddMonths(1);
            Appointments app = GetData(StartDate.ToUniversalTime(), EndDate.ToUniversalTime());

            string strHtmlEvents = "<table class=\"" + CssClass + "\"><tr class=\"" + CssShowMoreClass + "\"><td>" + DateTime.Parse(date).ToLongDateString() + "</td></tr><tr><td><BR>";

            for (int i = 0; i < app.Appointment.Rows.Count; i++)
            {
                Appointments.AppointmentRow evRow = (Appointments.AppointmentRow)app.Appointment.Rows[i];
                DateTime beginDate = evRow.BeginDate.ToLocalTime();
                DateTime endDate;
                try
                {
                    endDate = evRow.EndDate.ToLocalTime();
                }
                catch (StrongTypingException)
                {
                    //if endDate = null, set its value to BeginDate
                    endDate = evRow.BeginDate.ToLocalTime().AddMinutes(30);

                }

                //compare the date of the current day (date) to the begin and end dates of each appointment
                if ((beginDate.Date <= DateTime.Parse(date).Date) && (DateTime.Parse(date).Date <= endDate.Date) && !((endDate.TimeOfDay.Hours == 0 && endDate.TimeOfDay.Minutes == 0 && endDate.TimeOfDay.Seconds == 0) && (DateTime.Parse(date).Day == endDate.Day && DateTime.Parse(date).Month == endDate.Month && DateTime.Parse(date).Year == endDate.Year) && !(evRow.Source.ToString() == "ClassServerAssignment")) && !((evRow.Source.ToString() == "SPSEvents") && evRow.Recurrent && (beginDate.Date != DateTime.Parse(date).Date)))
                    strHtmlEvents += "<a href=\"JavaScript:launchCenter('" + evRow.URL + "', 'MyPlannerItem', '500',  '700', 'status=yes,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes'); void(0)\"; class=\"" + CssAppointment + " " + evRow.Source + "\" title=\"" + evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + evRow.Title + "\">" + evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + evRow.Title + "</A><BR>";
            }
            strHtmlEvents += "</td></tr>";
            strHtmlEvents += "<tr><td><input type='button' name='close' value='" + GetLocalResourceObject("Close").ToString() + "' onClick='window.close();'></td></tr></table>";
            Response.Write(strHtmlEvents);
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

        private Appointments GetData(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                currentUser = this.Context.User.Identity.Name;
                Appointments appointments = new Appointments();

                SLKEvents slkData = new SLKEvents();
                ExchEvents exdata = new ExchEvents();
                SPSEvents spsdata = new SPSEvents();

                //				string userID;
                //				
                //				if (this.Context.Request.QueryString["UserID"] != null)
                //					userID = this.Context.Request.QueryString["UserID"];
                //				else
                //					userID = this.Context.User.Identity.Name;


                if (showSlk)
                {
                    slkData.Username = userID;
                    slkData.StartDate = StartDate;
                    slkData.EndDate = EndDate;
                    slkData.ClassesUrl = siteCollectionURL;
                    slkData.Mode = (AssignmentMode)assignmentMode;
                }


                if (currentUser.IndexOf("\\") != 0)
                    currentUser = currentUser.Substring(currentUser.LastIndexOf("\\") + 1);

                // Strip domain name from the user ID


                // Exchange
                if (showPersonalCalendar)
                {
                    exdata.UTCStartDate = StartDate;
                    exdata.UTCEndDate = EndDate;
                    exdata.ExchangeUser = currentUser;

                    exdata.ExchangeURL = exurl;
                    exdata.ExchangeDomain = domain;
                    exdata.ExchangePassword = this.Context.Request.ServerVariables["AUTH_PASSWORD"].ToString();



                    if (this.Context.User.Identity.AuthenticationType.ToUpper() == "BASIC")
                    {
                        //If overriding exchange default mail alias is enabled, get the user mail from the Active Directory
                        if (EnableOverrideDomain)
                        {
                            exdata.User = GetUserMail(currentUser);
                        }
                        else
                        {
                            exdata.User = currentUser;
                        }
                    }
                    else
                    {
                        exdata.Integrated = true;
                    }
                }
                // SharePoint


                //spsdata.StartDate = StartDate;
                //spsdata.EndDate = EndDate;
                //spsdata.EventLists = spsurl;
                //spsdata.Cont = this.Context;
                //spsdata.LinkGroup = quicklinksgroup;
                //spsdata.QuickLinkWSURL = quicklinkwsurl;
                //spsdata.ListTitles = spstitles;

                // Call the GetData() methods asynchronously
                if (showPersonalCalendar)
                {
                    ExGetDataDelegate exGetDataDelegate = new ExGetDataDelegate(exdata.GetData);
                    //CSGetDataDelegate csGetDataDelegate = new CSGetDataDelegate(csdata.GetData);
                    //SPGetDataDelegate spGetDataDelegate = new SPGetDataDelegate(spsdata.GetData);
                    IAsyncResult exResult = exGetDataDelegate.BeginInvoke(null, null);
                    exResult.AsyncWaitHandle.WaitOne(defaultTimeOutms, false);
                    if (!exResult.IsCompleted)
                        AddErrorMessage(String.Format(GetLocalResourceObject("NoEX").ToString()));
                    else
                    {
                        if (exdata.HasError)
                            //AddErrorMessage(String.Format(  GetLocalResourceObject("Error").ToString()) + "(Exchange) " + exdata.ErrorDesc);
                            Response.Write("Error" + exdata.ErrorDesc);
                        else
                            appointments.Merge(exdata);

                        exdata.Dispose();
                    }
                }
                //IAsyncResult csResult = csGetDataDelegate.BeginInvoke(null, null);
                //IAsyncResult spResult = spGetDataDelegate.BeginInvoke(null, null);

                // TODO: Make this work asynchronously
                //exdata.GetData();
                if (showSlk)
                    slkData.GetData();

                if (sharepointCalendarsIDList.Length > 0)
                {
                    string[] list = sharepointCalendarsIDList.Split(',');
                    MLG2007.Helper.CalendarStore.Calendar calObj = null;
                    MLG2007.Helper.CalendarStore.CalendarStore store = new MLG2007.Helper.CalendarStore.CalendarStore();

                    foreach (string tmp in list)
                    {
                        string temp = null;
                        if (tmp != "0")
                        {
                            store.CalendarListURL = spsurl;
                            calObj = store.GetCalendarbyID(int.Parse(tmp));
                            if (calObj != null)
                            {
                                store.ParseSiteUrl(calObj.CalendarUrl, out temp);
                                if (calendarNames.Length > 0)
                                {
                                    calendarNames += ',' + temp;

                                }
                                else
                                {
                                    calendarNames += temp;
                                }

                                if (calendarUrls.Length > 0)
                                {
                                    calendarUrls += ',' + calObj.CalendarUrl;

                                }
                                else
                                {
                                    calendarUrls += calObj.CalendarUrl;
                                }
                            }

                        }

                    }

                    SPSEvents calendarsEvents = null;
                    if (calendarUrls.Length != 0)
                    {
                        calendarsEvents = new SPSEvents();
                        calendarsEvents.StartDate = StartDate;
                        calendarsEvents.EndDate = EndDate;
                        calendarsEvents.EventLists = calendarUrls;
                        calendarsEvents.Cont = this.Context;
                        calendarsEvents.ListTitles = calendarNames;
                        calendarsEvents.DNSName = this.dnsName;
                        calendarsEvents.GetData();
                        if (calendarsEvents.HasError)
                            AddErrorMessage(String.Format("Error" + "(SharePoint) " + calendarsEvents.ErrorDesc));
                        else
                            appointments.Merge(calendarsEvents);
                        calendarsEvents.Dispose();
                    }

                }
                if (exchangeCalendarsIDList.Length > 0)
                {
                    string[] list = exchangeCalendarsIDList.Split(',');
                    MLG2007.Helper.CalendarStore.Calendar calObj = null;
                    MLG2007.Helper.CalendarStore.CalendarStore store = new MLG2007.Helper.CalendarStore.CalendarStore();

                    foreach (string tmp in list)
                    {
                        string temp = null;
                        if (tmp != "0")
                        {
                            store.CalendarListURL = spsurl;
                            calObj = store.GetCalendarbyID(int.Parse(tmp));
                            if (calObj != null)
                            {
                                {
                                    if (exchangeCalednarsUrls.Length > 0)
                                    {
                                        exchangeCalednarsUrls += "," + this.Page.Server.HtmlEncode(calObj.CalendarUrl);
                                    }
                                    else
                                    {
                                        exchangeCalednarsUrls += this.Page.Server.HtmlEncode(calObj.CalendarUrl); ;
                                    }
                                }
                            }

                        }
                    }
                    ExchEvents exdataPC = new ExchEvents();
                    exdataPC.UTCStartDate = StartDate;
                    exdataPC.UTCEndDate = EndDate;
                    exdataPC.ExchangeUser = currentUser;
                    exdataPC.ExchangePassword = this.Context.Request.ServerVariables["AUTH_PASSWORD"].ToString();

                    exdataPC.ExchangeURL = exurl;
                    exdataPC.ExchangeDomain = domain;
                    if (this.Context.User.Identity.AuthenticationType.ToUpper() == "BASIC")
                    {
                        //If overriding exchange default mail alias is enabled, get the user mail from the Active Directory
                        if (EnableOverrideDomain)
                        {
                            exdataPC.User = GetUserMail(currentUser);
                        }
                        else
                            exdataPC.User = currentUser;
                    }
                    else
                    {
                        exdataPC.Integrated = true;
                    }
                    if (exchangeCalednarsUrls.Length > 0)
                    {
                        string[] tmpArray = exchangeCalednarsUrls.Split(',');
                        foreach (string tmp2 in tmpArray)
                        {
                            exdataPC.GetPublicCalendarData(tmp2);
                        }
                        appointments.Merge(exdataPC);
                    }
                }

                //spsdata.GetData();

                //Set the class server cookie
                //System.Web.HttpCookie cookieNew = new System.Web.HttpCookie("MSCSTicket", csdata.ClassServerTicket);
                //this.Context.Response.Cookies.Add(cookieNew);



                //csResult.AsyncWaitHandle.WaitOne(defaultTimeOutms, false);
                //spResult.AsyncWaitHandle.WaitOne(defaultTimeOutms, false);

                // Display errors if necessary, otherwise add the result to the appointments dataset


                //				if (!csResult.IsCompleted)
                //					AddErrorMessage(String.Format(rm.GetString("NoCS", CultureInfo.CurrentCulture)));
                //				else
                //				{
                if (slkData.HasError)
                    //AddErrorMessage(String.Format(GetLocalResourceObject("Error").ToString()) + "(Class Server) " + csdata.ErrorDesc);
                    Response.Write("Error" + slkData.ErrorDescription);
                else
                    appointments.Merge(slkData);

                slkData.Dispose();
                //				}

                //if (!spResult.IsCompleted)
                //    Controls.Add(new LiteralControl(String.Format(GetLocalResourceObject("NoSPS").ToString())));
                //else
                //{
                //    if (spsdata.HasError)
                //        //AddErrorMessage(String.Format(  GetLocalResourceObject("Error").ToString())+ "(SharePoint) " + spsdata.ErrorDesc);
                //        Response.Write("Error" + spsdata.ErrorDesc);
                //    else
                //        appointments.Merge(spsdata);

                //    spsdata.Dispose();
                //}

                return appointments;
            }
            catch (Exception ex)
            {
                //AddErrorMessage(String.Format(rm.GetString("Error", CultureInfo.CurrentCulture) + ex.Message));
                Response.Write("Error" + ex.Message);
                return null;
            }
        }

        private string GetUserMail(string userID)
        {
            SearchResultCollection resCol = null;
            DirectorySearcher mySearcher = null;

            try
            {
                //strip domain name from the user id
                if (currentUser.IndexOf("\\") != 0)
                    currentUser = currentUser.Substring(currentUser.LastIndexOf("\\") + 1);

                mySearcher = new DirectorySearcher();
                string PropertyName = "mail";
                mySearcher.PropertiesToLoad.Add(PropertyName);
                mySearcher.Filter = "(&(objectCategory=user)(samaccountname=" + currentUser + "))";
                resCol = mySearcher.FindAll();

                if (resCol.Count != 1)
                {
                    Page.Response.Write("User Not Found");
                    return "";
                }

                if (resCol[0].Properties[PropertyName] == null)
                {
                    Page.Response.Write("Property mail not found");
                    return "";
                }

                if ((resCol[0].Properties[PropertyName].Count == 1))
                    return resCol[0].Properties[PropertyName][0].ToString();
                else
                {
                    Page.Response.Write("User has more than 1 mail");
                    return "";
                }
            }

            finally
            {
                resCol.Dispose();
                mySearcher.Dispose();
            }
            //dirEntry.Close();
        }

        private void AddErrorMessage(string Message)
        {
            if (showErrors)
                Controls.Add(new LiteralControl("<BR>" + Message + "<BR>"));
        }

        private delegate void ExGetDataDelegate();
        //private delegate void CSGetDataDelegate();
        private delegate void SPGetDataDelegate();
    }
}
