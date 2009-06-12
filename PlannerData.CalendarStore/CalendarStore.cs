using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Web;

namespace MLG2007.Helper.CalendarStore
{
    /// <summary>A calendar store.</summary>
    public class CalendarStore
    {
        #region private fields
        private string m_calendarsListURL;
        private string m_calendarName;
        #endregion

        #region Public Properties
        /// <summary>The url of the calendar list.</summary>
        public string CalendarListURL
        {
            get { return m_calendarsListURL.Trim(); }
            set { m_calendarsListURL = value.Trim(); }
        }

        /// <summary>The name of the calendar list.</summary>
        public string CalendarListName
        {
            get { return m_calendarName; }
            set { m_calendarName = value; }
        }

        #endregion

        /// <summary>Retrieve a Calendar by id.</summary>
        /// <param name="calendarID">The id of the calendar.</param>
        /// <returns>A <see cref="Calendar"/>.</returns>
        public Calendar GetCalendarbyID(int calendarID)
        {
            try
            {
                string listName;
                string url = ParseSiteUrl(m_calendarsListURL, out listName);

                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb m_calendarListWeb = site.OpenWeb())
                    {
                        SPList list = m_calendarListWeb.Lists[listName.Replace("%20"," ")];

                        SPListItem calendarItem = list.Items.GetItemById(calendarID);
                        if (calendarItem != null)
                        {
                            return CalendarFromListItem(calendarItem, true);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>Retrieves a collection of calendars by role.</summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>A collection of calendars.</returns>
        public CalendarCollection GetCalendarByRole(string roleName)
        {
            CalendarCollection collection = new CalendarCollection();

            string listName;
            string url = ParseSiteUrl(m_calendarsListURL, out listName);

            using (SPSite site = new SPSite(url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list =  web.Lists[HttpContext.Current.Server.HtmlDecode ( listName)];
                    SPQuery query = new SPQuery();
                    query.Query = "<Where><Contains><FieldRef Name='Role'/><Value Type='Text'>"+ roleName +"</Value></Contains></Where>";
                    SPListItemCollection items = list.GetItems(query);

                    if (items.Count >0)
                    {
                        foreach (SPListItem item in items)
                        {
                            Calendar calendar = CalendarFromListItem(item, true);
                            calendar.IsUserSelected = false;
                            collection.Add(calendar);
                        }                
                    }
                }
            }
            return collection;
        }      

        /// <summary>Returns null.</summary>
        [Obsolete]
        public Calendars GetAllCalendars()
        {
            return null;
        }

        /// <summary>Gets a collection of user calendars.</summary>
        /// <param name="calendarsIdList">A list of calendars.</param>
        /// <returns>A <see cref="CalendarCollection"/>.</returns>
        public CalendarCollection GetUserCalendars(string calendarsIdList)
        {
            CalendarCollection collection = new CalendarCollection();

            if (calendarsIdList.Length > 0)
            {
                string listName;
                string url = ParseSiteUrl(m_calendarsListURL, out listName);

                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.Lists[listName.Replace("%20"," ")];
                        string[] idslist = calendarsIdList.Split(',');

                        SPQuery query = new SPQuery();
                        query.Query = "<Where><Or>";
                        foreach (string tmp in idslist)
                        {
                            query.Query += "<Eq><FieldRef Name='ID'/><Value>" + tmp;
                            query.Query += "</value></Eq>";
                        }
                        query.Query += "</Or></Where>";

                        SPListItemCollection items = list.GetItems(query);

                        if (items.Count > 0)
                        {
                            foreach (SPListItem item in items)
                            {
                                collection.Add(CalendarFromListItem(item, false));
                            }
                        }
                    }
                }
            }
            return collection;
        }

        /// <summary>Parses the site url.</summary>
        public string ParseSiteUrl(string listUrl, out string listName)
        {
            string m_listUrl, m_SiteUrl, m_listName;
            int m_LastIndex, m_urlength;
            System.Uri url = new Uri(new Uri(System.Web.HttpContext.Current.Request.Url.ToString()), listUrl);
           // System.Uri url = new Uri(new Uri(SPContext.Current.Web.Url.ToString()), listUrl); 
            //url.
            
            m_listUrl = url.OriginalString;

            m_listUrl = m_listUrl.ToLower();
            m_listUrl = m_listUrl.Replace("allitems.aspx", "");
            m_listUrl = m_listUrl.Replace("calendar.aspx", "");


            if (m_listUrl.EndsWith("/"))
                m_listUrl = m_listUrl.TrimEnd('/');

            m_LastIndex = m_listUrl.LastIndexOf('/');
            m_urlength = m_listUrl.Length;

            m_listName = m_listUrl.Substring(m_LastIndex + 1, m_urlength - 1 - m_LastIndex);
            m_SiteUrl = m_listUrl.Substring(0, m_LastIndex);

            listName = m_listName;          
            return m_SiteUrl;
        }

        /// <summary>Checks to see if the role exists.</summary>
        public bool IsRoleExist(string role)
        {
            try
            {
                if (m_calendarsListURL.Length > 0)
                {
                    string listName;
                    string url = ParseSiteUrl(m_calendarsListURL, out listName);

                    using (SPSite site = new SPSite(url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists[listName.Replace ("%20"," ")];
                            SPFieldMultiChoice roleField = (SPFieldMultiChoice)list.Fields["Role"];
                            int index = roleField.Choices.IndexOf(role);
                            return (index != -1);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Check to see if the calendar store exists.</summary>
        /// <param name="calendarStoreUrl">The url of the store.</param>
        /// <returns>True if the store exists.</returns>
        public bool IsCalendarStoreExist(string calendarStoreUrl)
        {
            try
            {
                if (calendarStoreUrl.Length > 0)
                {
                    string listName;
                    string url = ParseSiteUrl(calendarStoreUrl, out listName);
                    using (SPSite site = new SPSite(url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists[listName.Replace("%20", " ")];                    
                            return (list != null);
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

#region private methods
        /// <summary>Creates a Calendar from a list item.</summary>
        Calendar CalendarFromListItem(SPListItem item, bool includeSource)
        {
            Calendar calendar = new Calendar();
            calendar.CalendarDescription = (item["Description"] != null) ? item["Description"].ToString() : string.Empty;
            calendar.CalendarName = (item["Title"] != null) ? item["Title"].ToString() : string.Empty;
            calendar.CalendarUrl = (item["URL"] != null) ? item["URL"].ToString() : string.Empty;
            calendar.CalendarId = int.Parse(item["ID"].ToString());
            calendar.CalendarRole = (item["Role"] != null) ? item["Role"].ToString() : string.Empty;
            if (includeSource)
            {
                calendar.CalendarType = (item["Source"].ToString() != "SharePoint") ? CalendarType.Exchange : CalendarType.SharePoint;
            }
            return calendar;
        }
#endregion private methods
    }
}
