using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace MLG2007.Helper.UserPreferences
{
    public class UserPreferencesStore
    {
        private string userPreferencesStoreUrl;

        public string UserPreferencesStoreUrl
        {
            get
            {
                return userPreferencesStoreUrl;
            }
            set
            {
                userPreferencesStoreUrl = value;
            }
        }

        public UserPreferences GetUserPreferences(string userName)
        {
            UserPreferences preferences = null;
            SPListItem preferencesItem = null;
            SPUser userObject = null;
            string userSID = string.Empty;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
                {
                    userObject = GetUserObject(userName);
                    if (userObject != null)
                    {
                        userSID = userObject.Sid.ToString();
                        preferencesItem = GetPreferencesItem(userSID);
                        preferences = new UserPreferences();
                        if (preferencesItem != null)
                        {
                            preferences.ShowAssignments = bool.Parse(preferencesItem["Show_Assignments"].ToString());
                            preferences.ShowPersonalCalendar = bool.Parse(preferencesItem["Show_Personal_Calendar"].ToString());
                            preferences.WssCalendars = (preferencesItem["WSS_Calendars"] != null) ? preferencesItem["WSS_Calendars"].ToString() : "";
                            preferences.ExchangeCalendars = (preferencesItem["Exchange_Calendars"] != null) ? preferencesItem["Exchange_Calendars"].ToString() : "";
                        }
                        else
                        {
                            preferences = SaveUserPreferences(preferences, userName, true);
                        }

                    }
                }));
                return preferences;
            }

            catch
            {
                return preferences;
            }
        }

        public UserPreferences SaveUserPreferences(UserPreferences preferencesObject, string userName, bool IsFirstTime)
        {

            string listName, siteUrl;
            SPSite siteCollection = null;
            SPWeb web;
            SPList list;
            SPQuery query;
            SPListItemCollection preferencesListItems;
            UserPreferences preferences = null;
            string userSID = string.Empty;
            SPUser userObject = null;
            SPListItem preferencesItem;
            try
            {


                SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
                {
                    userObject = GetUserObject(userName);
                    userSID = userObject.Sid;
                    siteUrl = ParseSiteUrl(userPreferencesStoreUrl, out listName);
                    siteCollection = new SPSite(siteUrl);
                    web = siteCollection.OpenWeb();
                    web.AllowUnsafeUpdates = true;
                    list = web.Lists[listName];
                    if (!IsFirstTime)
                    {
                        query = new SPQuery();
                        query.Query += "<Where><Eq><FieldRef Name=\"User_SID\" /><Value Type=\"Text\">" + userSID + "</Value></Eq></Where>";
                        preferencesListItems = list.GetItems(query);
                        if (preferencesListItems.Count > 0)
                        {

                            preferencesItem = preferencesListItems[0];
                            preferencesItem["Show_Assignments"] = preferencesObject.ShowAssignments;
                            preferencesItem["Show_Personal_Calendar"] = preferencesObject.ShowPersonalCalendar;
                            preferencesItem["WSS_Calendars"] = preferencesObject.WssCalendars;
                            preferencesItem["Exchange_Calendars"] = preferencesObject.ExchangeCalendars;
                            preferencesItem.Update();
                        }
                        else
                        {
                            preferencesItem = list.Items.Add();
                            preferencesItem["Show_Assignments"] = preferencesObject.ShowAssignments;
                            preferencesItem["Show_Personal_Calendar"] = preferencesObject.ShowPersonalCalendar;
                            preferencesItem["WSS_Calendars"] = preferencesObject.WssCalendars;
                            preferencesItem["Exchange_Calendars"] = preferencesObject.ExchangeCalendars;
                            preferencesItem["User_SID"] = userSID;
                            preferencesItem.Update();
                        }
                    }
                    else
                    {
                        preferencesItem = list.Items.Add();
                        preferencesItem["Show_Assignments"] = preferencesObject.ShowAssignments;
                        preferencesItem["Show_Personal_Calendar"] = preferencesObject.ShowPersonalCalendar;
                        preferencesItem["WSS_Calendars"] = preferencesObject.WssCalendars;
                        preferencesItem["Exchange_Calendars"] = preferencesObject.ExchangeCalendars;
                        preferencesItem["User_SID"] = userSID;
                        preferencesItem.Update();
                    }
                    web.Update();
                }));
            }
            catch (Exception exception)
            {
                //siteCollection.Dispose();
                return null;
            }
            return preferencesObject;
        }

        private SPListItem GetPreferencesItem(string userSID)
        {
            string listName, siteUrl;
            SPSite siteCollection = null;
            SPWeb web;
            SPList list;
            SPQuery query;
            SPListItemCollection preferencesListItems = null;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
               {
                   query = new SPQuery();
                   siteUrl = ParseSiteUrl(userPreferencesStoreUrl, out listName);
                   siteCollection = new SPSite(siteUrl);
                   web = siteCollection.OpenWeb();
                   list = web.Lists[listName];
                   query.Query += "<Where><Eq><FieldRef Name=\"User_SID\" /><Value Type=\"Text\">" + userSID + "</Value></Eq></Where>";
                   preferencesListItems = list.GetItems(query);
               }));
                if (preferencesListItems.Count > 0)
                    return preferencesListItems[0];
                else
                    return null;
            }
            catch (Exception exception)
            {
                siteCollection.Dispose();
                return null;
            }
        }

        private SPUser GetUserObject(string userName)
        {
            SPWeb webObject;
            SPUser userobject;
            try
            {
                webObject = SPContext.Current.Site.RootWeb;
                userobject = webObject.AllUsers[userName];
                //userobject = webObject.Users[userName];

                if (userobject != null)
                    return userobject;
                else
                    return null;
            }
            catch (Exception exception)
            {
                return null;
            }

        }

        public string ParseSiteUrl(string listUrl, out string listName)
        {
            string m_listUrl, m_SiteUrl, m_listName;
            int m_LastIndex, m_listCharLength, m_urlength;
            System.Uri url = new Uri(new Uri(System.Web.HttpContext.Current.Request.Url.ToString()), listUrl);

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

            listName = m_listName.Replace("%20", " ");
            return m_SiteUrl;
        }

        public bool IsUserPerefencesExist(string userPerferencesStoreUrl)
        {
            SPSite userPerferencesStoreSiteCollection = null;
            SPWeb userPerferencesStoreWeb;
            SPList userPerferencesStore = null;

            string m_siteUrl, m_ListName;
            try
            {
                if (userPerferencesStoreUrl.Length > 0)
                {
                    SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
              {
                  m_siteUrl = ParseSiteUrl(userPerferencesStoreUrl, out m_ListName);

                  userPerferencesStoreSiteCollection = new SPSite(m_siteUrl);
                  userPerferencesStoreWeb = userPerferencesStoreSiteCollection.OpenWeb();
                  userPerferencesStore = userPerferencesStoreWeb.Lists[m_ListName.Replace("%20", " ")];
              }));
                    if (userPerferencesStore != null)
                        return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                return false;
            }
            finally
            {
                //userPerferencesStoreSiteCollection.Dispose();
            }
        }
    }
}
