using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Web;

namespace MLG2007.Helper.CalendarStore
{
    public class CalendarStore
    {
        #region private fields
        private string m_calendarsListURL;
        private string m_calendarName;
        private Calendars m_calendarsDS;
        #endregion

        #region Public Properties
        public string CalendarListURL
        {
            get
            {
                return m_calendarsListURL.Trim();
            }
            set
            {
                m_calendarsListURL = value.Trim();
            }
        }
        public string CalendarListName
        {
            get
            {
                return m_calendarName;
            }
            set
            {
                m_calendarName = value;
            }
        }

        #endregion

        public Calendar GetCalendarbyID(int calendarID)
        {
            SPSite m_calendarListSiteCollection;
            SPWeb m_calendarListWeb;
            SPList m_calendarList;
            SPListItem m_calendarListItem;
            string m_siteUrl, m_ListName;
            Calendar m_calendar = null;

            try
            {
                m_siteUrl = ParseSiteUrl(m_calendarsListURL, out m_ListName);

                m_calendarListSiteCollection = new SPSite(m_siteUrl);
                m_calendarListWeb = m_calendarListSiteCollection.OpenWeb();
                m_calendarList = m_calendarListWeb.Lists[m_ListName.Replace("%20"," ")];

                m_calendarListItem = m_calendarList.Items.GetItemById(calendarID);
                if (m_calendarListItem != null)
                {
                    m_calendar = new Calendar();
                    m_calendar.CalendarDescription = (m_calendarListItem["Description"] != null) ? m_calendarListItem["Description"].ToString() : string.Empty;
                    m_calendar.CalendarName = (m_calendarListItem["Title"] != null) ? m_calendarListItem["Title"].ToString() : string.Empty;
                    m_calendar.CalendarUrl = (m_calendarListItem["URL"] != null) ? m_calendarListItem["URL"].ToString() : string.Empty;
                    m_calendar.CalendarId = int.Parse(m_calendarListItem["ID"].ToString());
                    m_calendar.CalendarRole = (m_calendarListItem["Role"] != null) ? m_calendarListItem["URL"].ToString() : string.Empty;
                    m_calendar.CalendarType = (m_calendarListItem["Source"].ToString() != "SharePoint") ? CalendarType.Exchange : CalendarType.SharePoint;
                }
                return m_calendar;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public CalendarCollection GetCalendarByRole(string roleName)
        {
            SPSite m_calendarListSiteCollection;
            SPWeb m_calendarListWeb;
            SPList m_calendarList;
            SPListItem m_calendarListItem;
            string m_siteUrl, m_ListName;
            Calendar m_calendar = null;
            CalendarCollection m_calendarCollection = null;
            SPQuery m_query;
            SPListItemCollection m_calendarListItems;

            m_siteUrl = ParseSiteUrl(m_calendarsListURL, out m_ListName);

            m_calendarListSiteCollection = new SPSite(m_siteUrl);
            m_calendarListWeb = m_calendarListSiteCollection.OpenWeb();
            m_calendarList =  m_calendarListWeb.Lists[HttpContext.Current.Server.HtmlDecode ( m_ListName)];
            m_query = new SPQuery();
            //            query.Query = "<Where><Gt><FieldRef Name='Stock'/><Value Type='Number'>100</Value></Gt></Where>"; 

            m_query.Query = "<Where><Contains><FieldRef Name='Role'/><Value Type='Text'>"+ roleName +"</Value></Contains></Where>";
            m_calendarListItems = m_calendarList.GetItems(m_query);

            if (m_calendarListItems.Count >0)
            {
                m_calendarCollection = new CalendarCollection ();
                foreach (SPListItem item in m_calendarListItems)
                {
                    m_calendar = new Calendar();
                    m_calendar.CalendarDescription = (item["Description"] != null) ? item["Description"].ToString() : string.Empty;
                    m_calendar.CalendarName = (item["Title"] != null) ? item["Title"].ToString() : string.Empty;
                    m_calendar.CalendarUrl = (item["URL"] != null) ? item["URL"].ToString() : string.Empty;
                    m_calendar.CalendarId = int.Parse(item["ID"].ToString());
                    m_calendar.CalendarRole = (item["Role"] != null) ? item["Role"].ToString() : string.Empty;
                    m_calendar.IsUserSelected = false;
                    m_calendar.CalendarType = (item["Source"].ToString () != "SharePoint") ? CalendarType.Exchange : CalendarType.SharePoint;
                    m_calendarCollection.Add(m_calendar);
                }                
            }
            return m_calendarCollection;
            //return null;
        }      

        public Calendars GetAllCalendars()
        {
            return null;
        }

        public CalendarCollection GetUserCalendars(string calendarsIdList)
        {
            SPSite m_calendarListSiteCollection;
            SPWeb m_calendarListWeb;
            SPList m_calendarList;
            SPListItem m_calendarListItem;
            string m_siteUrl, m_ListName;
            Calendar m_calendar = null;
            CalendarCollection m_calendarCollection = null;
            SPQuery m_query;
            SPListItemCollection m_calendarListItems;
            string[] Idslist;

            if (calendarsIdList.Length > 0)
            {
                m_siteUrl = ParseSiteUrl(m_calendarsListURL, out m_ListName);

                m_calendarListSiteCollection = new SPSite(m_siteUrl);
                m_calendarListWeb = m_calendarListSiteCollection.OpenWeb();
                m_calendarList = m_calendarListWeb.Lists[m_ListName.Replace("%20"," ")];
                m_query = new SPQuery();

                Idslist = calendarsIdList.Split(',');
                m_query.Query = "<Where><Or>";
                foreach (string tmp in Idslist)
                {
                    m_query.Query += "<Eq><FieldRef Name='ID'/><Value>" + tmp;
                    m_query.Query += "</value></Eq>";
                }
                m_query.Query += "</Or></Where>";

                m_calendarListItems = m_calendarList.GetItems(m_query);

                if (m_calendarListItems.Count > 0)
                {
                    m_calendarCollection = new CalendarCollection();
                    foreach (SPListItem item in m_calendarListItems)
                    {
                        m_calendar = new Calendar();
                        m_calendar.CalendarDescription = (item["Description"] != null) ? item["Description"].ToString() : string.Empty;
                        m_calendar.CalendarName = (item["Title"] != null) ? item["Title"].ToString() : string.Empty;
                        m_calendar.CalendarUrl = (item["URL"] != null) ? item["URL"].ToString() : string.Empty;
                        m_calendar.CalendarId = int.Parse(item["ID"].ToString());
                        m_calendar.CalendarRole = (item["Role"] != null) ? item["Role"].ToString() : string.Empty;
                        m_calendarCollection.Add(m_calendar);
                    }
                }
            }
            return m_calendarCollection;
        }

        public string ParseSiteUrl(string listUrl, out string listName)
        {
            string m_listUrl, m_SiteUrl, m_listName;
            int m_LastIndex, m_listCharLength, m_urlength;
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

        public bool IsRoleExist(string role)
        {
            SPSite m_calendarListSiteCollection;
            SPWeb m_calendarListWeb;
            SPList m_calendarList;
            SPFieldMultiChoice  m_roleField;
            int index;
            string m_siteUrl, m_ListName;
            try
            {
                if (m_calendarsListURL.Length > 0)
                {

                    m_siteUrl = ParseSiteUrl(m_calendarsListURL, out m_ListName);

                    m_calendarListSiteCollection = new SPSite(m_siteUrl);
                    m_calendarListWeb = m_calendarListSiteCollection.OpenWeb();
                    m_calendarList = m_calendarListWeb.Lists[m_ListName.Replace ("%20"," ")];
                    m_roleField = (SPFieldMultiChoice)m_calendarList.Fields["Role"];
                    index = m_roleField.Choices.IndexOf(role);
                    if (index != -1)
                        return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public bool IsCalendarStoreExist(string calendarStoreUrl)
        {
            SPSite m_calendarListSiteCollection;
            SPWeb m_calendarListWeb;
            SPList m_calendarList;
            SPFieldMultiChoice m_roleField;
            int index;
            string m_siteUrl, m_ListName;
            try
            {
                if (m_calendarsListURL.Length > 0)
                {

                    m_siteUrl = ParseSiteUrl(m_calendarsListURL, out m_ListName);
                    m_calendarListSiteCollection = new SPSite(m_siteUrl);
                    m_calendarListWeb = m_calendarListSiteCollection.OpenWeb();
                    m_calendarList = m_calendarListWeb.Lists[m_ListName.Replace("%20", " ")];                    
                    if (m_calendarList != null)
                        return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
    }

}
