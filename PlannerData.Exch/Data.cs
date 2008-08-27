using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace MLG2007.Helper.Exchange
{
    /// <summary>
    /// This component is used by the MyPlanner web part to retrieve event data from a 
    /// user's Exchange calendar.
    /// 
    /// This component takes the username and gets the Exchange OWA URL to the Calendar and 
    /// submits a WebDAV query to return all items within the data range as a typed dataset 
    /// to be used by the Calendar component to render.
    /// </summary>
    public class ExchEvents : Appointments
    {
        const int BUFFER_SIZE = 2048;

        private string UserName = "";					// e.g. "student01"
        private DateTime StartDate = DateTime.Now;		// e.g. "23:59 31/12/2003"
        private DateTime EndDate = DateTime.Now;			// e.g. "0:00 1/1/2003"
        private string OWAURL = "";					// e.g. "http://win2003/exchange/"
        private string DomainForService = "";			// e.g. "demo"
        private string UserNameForService = "";			// e.g. "exwebservice"
        private string PasswordForService = "";			// e.g. "Windows.2000"
        private string errorMsg;
        private bool blnError;
        private bool integrated = false;
        public bool isPublicCalendar = false;
        private string publicCalendarURL = "";

        // Get resource manager
        private ResourceManager rm = new ResourceManager("MLG2007.Helper.Exchange.Strings", Assembly.GetExecutingAssembly());

        // The RequestState class passes data across async calls.
        public class RequestState
        {
            public System.Text.StringBuilder RequestData;
            public byte[] BufferRead;
            public WebRequest Request;
            public Stream ResponseStream;

            // Create Decoder for appropriate encoding type.
            public System.Text.Decoder StreamDecode = System.Text.Encoding.UTF8.GetDecoder();

            // Constructor
            public RequestState()
            {
                BufferRead = new byte[BUFFER_SIZE];
                RequestData = new System.Text.StringBuilder(String.Empty);
                Request = null;
                ResponseStream = null;
            }
        }

        // Public Properties. Set these before calling GetData()
        public DateTime UTCStartDate
        {
            get
            {
                return StartDate;
            }
            set
            {
                StartDate = value;
            }
        }

        public DateTime UTCEndDate
        {
            get
            {
                return EndDate;
            }
            set
            {
                EndDate = value;
            }
        }

        public string ExchangeURL
        {
            get
            {
                return OWAURL;
            }
            set
            {
                OWAURL = value;
            }
        }

        public string User
        {
            get
            {
                return UserName;
            }
            set
            {
                UserName = value;
            }
        }

        public string ExchangeUser
        {
            get
            {
                return UserNameForService;
            }
            set
            {
                UserNameForService = value;
            }
        }

        public string ExchangePassword
        {
            get
            {
                return PasswordForService;
            }
            set
            {
                PasswordForService = value;
            }
        }

        public string ExchangeDomain
        {
            get
            {
                return DomainForService;
            }
            set
            {
                DomainForService = value;
            }
        }

        public bool Integrated
        {
            get
            {
                return integrated;
            }
            set
            {
                integrated = value;
            }
        }

        public string ErrorDesc
        {
            get
            {
                return errorMsg;
            }
            set
            {
                errorMsg = value;
            }
        }

        public bool HasError
        {
            get
            {
                return blnError;
            }
            set
            {
                blnError = value;
            }
        }

        private bool IsPublicCalendar
        {
            get { return isPublicCalendar; }
            set { isPublicCalendar = value; }
        }

        private string PublicCalendarUrl
        {
            get { return publicCalendarURL; }
            set { publicCalendarURL = value; }
        }
        //End Public Properties

        // Constructor
        public ExchEvents()
        {
            blnError = false;
            errorMsg = "";
        }

        // Call this method from a client to call to Exchange and get the appointment data
        public void GetData()
        {
            FindAppointment(UserName, StartDate, EndDate, "", "", "");
        }

        public void GetDataByID(string itemID, string calendarID)
        {
            FindAppointmentByID(itemID, calendarID);
        }

        public void FindAppointment(string sEmailAddressOfOwner, DateTime dBeginTimeRangeUTC, DateTime dendtimeRangeUTC, string sSubject,
            string sLocation, string sBodyText)
        {
            // This routine searches a calendar folder for matching items.
            // You must specifiy start and end times for the search in UTC normalized time

            // sEmailAddressOfOwner can be a user name (e.g. "student01") or a fully qualified URL
            // (e.g. "http://eduadex/public/calendars/School%20Calendar/")

            // First we need to build the URL to the calendar folder of the user
            // or assume that it's already given if it starts with "http://" or "https://"

            string sCalendarURL;
            string sResult, sQuery;

            if (sEmailAddressOfOwner.Length > 0)
            {
                if (!IsPublicCalendar)
                {
                    if (!OWAURL.EndsWith("/"))
                        OWAURL += "/";

                    if (sEmailAddressOfOwner.StartsWith("http://") || sEmailAddressOfOwner.StartsWith("https://"))
                        sCalendarURL = sEmailAddressOfOwner;
                    else
                        sCalendarURL = OWAURL + sEmailAddressOfOwner + "/calendar/";
                }
                else
                {
                    sCalendarURL = PublicCalendarUrl;
                }
                // Convert the parameters into a DAV SQL statement
                sQuery = BuildQuery(sCalendarURL, dBeginTimeRangeUTC, dendtimeRangeUTC, sSubject, sLocation, sBodyText);

                // Perform a query via WebDAV to Exchange.
                sResult = DavQuery(sCalendarURL, sQuery);

                // Populate the dataset with the results
                FormatData(sResult, sCalendarURL);
            }
            else
            {
                blnError = true;
                errorMsg = rm.GetString("ErrInvalidAddress", CultureInfo.CurrentCulture);
            }
        }

        public void FindAppointmentByID(string itemID, string calendarURL)
        {
            // This routine searches a calendar folder for matching items.
            // You must specifiy start and end times for the search in UTC normalized time

            // sEmailAddressOfOwner can be a user name (e.g. "student01") or a fully qualified URL
            // (e.g. "http://eduadex/public/calendars/School%20Calendar/")

            // First we need to build the URL to the calendar folder of the user
            // or assume that it's already given if it starts with "http://" or "https://"

            string sCalendarURL;
            string sResult, sQuery;

            if (calendarURL.Length > 0)
            {
                sCalendarURL = calendarURL;

                // Convert the parameters into a DAV SQL statement
                sQuery = BuildQueryByID(sCalendarURL, itemID);

                // Perform a query via WebDAV to Exchange.
                sResult = DavQuery(sCalendarURL, sQuery);

                // Populate the dataset with the results
                FormatData(sResult, sCalendarURL);
            }
            else
            {
                blnError = true;
                errorMsg = rm.GetString("ErrInvalidAddress", CultureInfo.CurrentCulture);
            }
        }

        private string BuildQuery(string sItemURL, DateTime dStartTimeUTC, DateTime dendtimeUTC, string sSubject, string sLocation, string sBodyText)
        {
            // This routine constructs the SQL select statement 
            // used in the DavQuery function. This format is specific to 
            // Exchange and optimized for the Appointment folder. 
            // dStartTimeUTC and dendtimeUTC specifiy a range in which to search
            // and should already be normalized (converted to UTC)

            string sSQL = "SELECT \"DAV:href\" AS url, \"urn:schemas:httpmail:subject\" AS subject,";
            sSQL += " \"urn:schemas:calendar:dtstart\" AS dtstart, \"urn:schemas:httpmail:textdescription\" as Description, \"urn:schemas:calendar:uid\" as CID,";
            sSQL += " \"urn:schemas:calendar:dtend\" AS dtend, ";
            sSQL += " \"urn:schemas:calendar:instancetype\" AS InstanceType,";
            sSQL += " \"urn:schemas:calendar:alldayevent\" AS IsAllDayEvent";
            sSQL += " FROM Scope('SHALLOW TRAVERSAL OF \"" + sItemURL + "\"')";
            sSQL += " WHERE (Not \"urn:schemas:calendar:instancetype\" = 1)";
            sSQL += " AND (\"DAV:contentclass\" = 'urn:content-classes:appointment' )";
            //sSQL += " WHERE (\"DAV:contentclass\" = 'urn:content-classes:appointment' )";
            sSQL += " AND (\"urn:schemas:calendar:dtend\" &gt; '" + dStartTimeUTC.ToString("yyyy/MM/dd HH:mm:ss") + "')";
            sSQL += " AND (\"urn:schemas:calendar:dtstart\" &lt; '" + dendtimeUTC.ToString("yyyy/MM/dd HH:mm:ss") + "')";

            if (sSubject.Trim().Length > 0)
                sSQL += " AND  ( \"urn:schemas:httpmail:subject\" LIKE //%" + sSubject.Trim() + "%' )";

            if (sLocation.Trim().Length > 0)
                sSQL += " AND  ( \"urn:schemas:calendar:location\" LIKE //%" + sLocation.Trim() + "%' )";

            if (sBodyText.Trim().Length > 0)
                sSQL += " AND  ( \"urn:schemas:httpmail:textdescription\" LIKE //%" + sBodyText.Trim() + "%' )";

            sSQL += " ORDER BY \"urn:schemas:calendar:dtstart\" ASC";

            return sSQL;
        }

        private string BuildQueryByID(string sItemURL, string itemID)
        {
            string sSQL = "SELECT \"DAV:href\" AS url, \"urn:schemas:httpmail:subject\" AS subject,";
            sSQL += " \"urn:schemas:calendar:dtstart\" AS dtstart, \"urn:schemas:httpmail:textdescription\" as Description, \"urn:schemas:calendar:uid\" as CID,";
            sSQL += " \"urn:schemas:calendar:dtend\" AS dtend,";
            sSQL += " \"urn:schemas:calendar:instancetype\" AS InstanceType,";
            sSQL += " \"urn:schemas:calendar:alldayevent\" AS IsAllDayEvent";
            sSQL += " FROM Scope('SHALLOW TRAVERSAL OF \"" + sItemURL + "\"')";
            sSQL += " WHERE (Not \"urn:schemas:calendar:instancetype\" = 1)";
            sSQL += " AND (\"DAV:contentclass\" = 'urn:content-classes:appointment' )";
            //sSQL += " WHERE (\"DAV:contentclass\" = 'urn:content-classes:appointment' )";
            //sSQL += " AND (\"urn:schemas:calendar:dtend\" &gt; '" + dStartTimeUTC.ToString("yyyy/MM/dd HH:mm:ss") + "')";
            //sSQL += " AND (\"urn:schemas:calendar:dtstart\" &lt; '" + dendtimeUTC.ToString("yyyy/MM/dd HH:mm:ss") + "')";
            sSQL += " AND (\"urn:schemas:calendar:uid\"= '" + itemID + "')";
            

            //if (sSubject.Trim().Length > 0)
            //    sSQL += " AND  ( \"urn:schemas:httpmail:subject\" LIKE //%" + sSubject.Trim() + "%' )";

            //if (sLocation.Trim().Length > 0)
            //    sSQL += " AND  ( \"urn:schemas:calendar:location\" LIKE //%" + sLocation.Trim() + "%' )";

            //if (sBodyText.Trim().Length > 0)
            //    sSQL += " AND  ( \"urn:schemas:httpmail:textdescription\" LIKE //%" + sBodyText.Trim() + "%' )";

            //sSQL += " ORDER BY \"urn:schemas:calendar:dtstart\" ASC";

            return sSQL;
        }

        private string DavQuery(string sItemURL, string sQuery)
        {
            // This routine executes a WebDAV search. 
            // An OWA search would have returned a HTML table, much more difficult to parse

            // Build the Search XML 
            string sXml;
            sXml = "<?xml version='1.0'?>";
            sXml += "<d:searchrequest xmlns:d='DAV:'>";
            sXml += "<d:sql>";
            sXml += sQuery;
            sXml += "</d:sql>";
            sXml += "</d:searchrequest>";

            // Convert it to a byte array
            byte[] bPostData = System.Text.Encoding.UTF8.GetBytes(sXml);

            // Initiate the request.
            return RequestHTTP(sItemURL, "SEARCH", "text/XML", bPostData);
        }

        private string RequestHTTP(string sItemURL, string sMethod, string sContentType, Byte[] bPostDataUTF8)
        {
            // This routine wraps up the HTTP requests into one routine
            HttpWebRequest oRequest;
            Stream oStream;
            string sResponse;

            oRequest = (HttpWebRequest)WebRequest.Create(sItemURL);

            // Set the credentials for this service
            oRequest.PreAuthenticate = true;
            if (Integrated)
                oRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
            else
                oRequest.Credentials = new NetworkCredential(UserNameForService, PasswordForService, DomainForService);

            // Tell Exchange we are an IE browser
            oRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT)";
            oRequest.KeepAlive = false;
            oRequest.Method = sMethod;
            oRequest.ContentType = sContentType;

            try
            {
                // If we have something to send, then send it
                if (sMethod.ToUpper() != "GET" && bPostDataUTF8.Length > 0)
                {
                    oRequest.ContentLength = bPostDataUTF8.Length;
                    oStream = oRequest.GetRequestStream();

                    // Send the XML to the calendar URL  
                    try
                    {
                        oStream.Write(bPostDataUTF8, 0, bPostDataUTF8.Length);
                    }
                    catch
                    {
                        oStream.Flush();
                    }

                    // Close the stream
                    oStream.Close();
                }

                // Get the response back from Exchange
                WebResponse oResponse = oRequest.GetResponse();
                Stream oResponseStream = oResponse.GetResponseStream();

                StreamReader oReader = new StreamReader(oResponseStream, System.Text.Encoding.UTF8);
                sResponse = oReader.ReadToEnd();

                // Close down the various items.
                oResponse.Close();
                oResponseStream.Close();
                oReader.Close();
                return sResponse;
            }
            catch (Exception exc)
            {
                blnError = true;
                errorMsg = rm.GetString("ErrExchError", CultureInfo.CurrentCulture) + ": " + exc.Message;
                return "";
            }
        }

        private void FormatData(string sXML, string sourceURL)
        {
            // This function takes the XML that comes back from Exchange
            // and loads it into an XML document to extract the required information
            // and put it into the Appointment dataset that this class implements

            if (sXML.Length > 0)
            {
                // Store the information in the Appointment dataset
                Appointments.AppointmentRow dr;

                System.Xml.XmlDocument oXml = new System.Xml.XmlDocument();
                System.Xml.XmlNode oNode;

                // Load the data into an XML document
                oXml.LoadXml(sXML);

                // Add the appropriate namespaces
                XmlNamespaceManager oNSManager = new XmlNamespaceManager(oXml.NameTable);
                oNSManager.AddNamespace("a", "DAV:");

                System.Xml.XmlNodeList oResponses = oXml.SelectNodes("//a:prop", oNSManager);

                // Walk the response collection 
                if (oResponses.Count > 0)
                {
                    for (int iLoop = 0; iLoop < oResponses.Count; iLoop++)
                    {
                        dr = this.Appointment.NewAppointmentRow();
                        try
                        {
                            oNode = oResponses.Item(iLoop);
                            //							DateTime sDate=DateTime.Parse(oNode.ChildNodes.Item(2).InnerText).ToUniversalTime();
                            //							DateTime eDate=DateTime.Parse(oNode.ChildNodes.Item(3).InnerText).ToUniversalTime();
                            //							DateTime tmpDate=DateTime.Parse(oNode.ChildNodes.Item(2).InnerText).AddDays(2).ToUniversalTime();
                            //							do
                            //							{
                            //								dr = this.Appointment.NewAppointmentRow();
                            //dr.URL = oNode.ChildNodes.Item(0).InnerText + "?cmd=open";                            
                            dr.Title = oNode.ChildNodes.Item(1).InnerText;
                            //Begin and End dates are set to the full date time format - time is used in some conditions in Calendar
                            dr.BeginDate = DateTime.Parse(oNode.ChildNodes.Item(2).InnerText).ToUniversalTime();
                            dr.EndDate = DateTime.Parse(oNode.ChildNodes.Item(5).InnerText).ToUniversalTime();
                            //							dr.BeginDate= DateTime.Parse(oNode.ChildNodes.Item(2).InnerText).Date.ToUniversalTime();
                            //							dr.EndDate = DateTime.Parse(oNode.ChildNodes.Item(3).InnerText).Date.ToUniversalTime();
                            dr.Description = oNode.ChildNodes.Item(3).InnerText;
                            dr.ID = oNode.ChildNodes.Item(4).InnerText;
                            dr.URL = "/_layouts/1033/LgUtilities/ShowExchange.aspx?CUID=" + dr.ID + "&SourceURL=" + sourceURL + "&Domain=" + ExchangeDomain + "&ExchanegURL=" + ExchangeURL;
                            dr.Subject = dr.Title;
                            dr.Source = "ExchangeAppointment";
                            dr.AllDayEvent = (oNode.ChildNodes.Item(7).InnerText=="0")? false:true;

                            this.Appointment.AddAppointmentRow(dr);
                            //								tmpDate=tmpDate.AddDays(1);
                            //							}
                            //							while(tmpDate.Date.CompareTo(eDate.Date)<=0);
                            ///////////////////////////////////////////////////////////////
                            //this portion of code was used to add a single row for each day in the appointment
                            //i.e., if an appointment duration is 5 days, there will be 5 rows for this appointment; one row for each day
                            //after commenting this code, each appointment will have only one row regardless what its duration is
                            //example
                            //Title: appointment, BeginDate = 1/3/2006 12:00 AM, EndDate = 21/3/2006 12:00 AM
                            //old code: this app has 21 rows
                            //after commenting: this app has 1 row

                            #region old code
                            //							//if the start  date not equal to end date then add row for each day
                            //							DateTime sDate=DateTime.Parse(oNode.ChildNodes.Item(2).InnerText).Date.ToUniversalTime();
                            //							DateTime eDate=DateTime.Parse(oNode.ChildNodes.Item(3).InnerText).Date.ToUniversalTime();
                            //							if(sDate.Date.CompareTo(StartDate)<0)
                            //							{
                            //								sDate=StartDate.AddDays(1);
                            //							}
                            //							if(sDate.Date.CompareTo(eDate.Date)<0)
                            //							{
                            //								DateTime tmpDate=DateTime.Parse(oNode.ChildNodes.Item(2).InnerText).AddDays(2).Date.ToUniversalTime();
                            //								while(tmpDate.Date.CompareTo(eDate.Date)<=0)
                            //								{
                            //									AppointmentRow tmprw=Appointment.NewAppointmentRow();
                            //									tmprw.URL = oNode.ChildNodes.Item(0).InnerText + "?cmd=open";
                            //									tmprw.Title = oNode.ChildNodes.Item(1).InnerText;
                            //									tmprw.ID = "";
                            //									tmprw.BeginDate= DateTime.Parse(tmpDate.ToString()).Date.ToUniversalTime();
                            //									tmprw.EndDate = DateTime.Parse(oNode.ChildNodes.Item(3).InnerText).Date.ToUniversalTime();
                            //									tmprw.Subject = dr.Title;
                            //									tmprw.Source = "ExchangeAppointment";
                            //									this.Appointment.AddAppointmentRow(tmprw);
                            //									tmpDate=tmpDate.Date.AddDays(1);
                            //								}
                            //							}
                            #endregion
                            ///////////////////////////////////////////////////////////////
                        }
                        catch (Exception exc)
                        {
                            blnError = true;
                            errorMsg = rm.GetString("ErrExchError", CultureInfo.CurrentCulture) + ": " + exc.Message;
                        }
                    }
                }
            }
        }

        public void GetPublicCalendarData(string calendarUrl)
        {
            this.IsPublicCalendar = true;
            this.PublicCalendarUrl = calendarUrl;
            this.GetData();

        }
    }
}
