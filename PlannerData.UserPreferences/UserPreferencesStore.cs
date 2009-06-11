using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace MLG2007.Helper.UserPreferences
{
    /// <summary>Represents a user preference store.</summary>
    public class UserPreferencesStore
    {
        private string userPreferencesStoreUrl;

        /// <summary>The url of the store.</summary>
        public string UserPreferencesStoreUrl
        {
            get { return userPreferencesStoreUrl; }
            set { userPreferencesStoreUrl = value; }
        }

        /// <summary>Get the preferences for a user.</summary>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A <see cref="UserPreferences"/> object.</returns>
        public UserPreferences GetUserPreferences(string userName)
        {
            UserPreferences preferences = null;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
                {
                    SPUser userObject = GetUserObject(userName);
                    if (userObject != null)
                    {
                        SPListItem preferencesItem = GetPreferencesItem(userObject.Sid.ToString());
                        preferences = new UserPreferences();
                        if (preferencesItem == null)
                        {
                            preferences = SaveUserPreferences(preferences, userName, true);
                        }
                        else
                        {
                            preferences.ShowAssignments = bool.Parse(preferencesItem["Show_Assignments"].ToString());
                            preferences.ShowPersonalCalendar = bool.Parse(preferencesItem["Show_Personal_Calendar"].ToString());
                            preferences.WssCalendars = (preferencesItem["WSS_Calendars"] != null) ? preferencesItem["WSS_Calendars"].ToString() : "";
                            preferences.ExchangeCalendars = (preferencesItem["Exchange_Calendars"] != null) ? preferencesItem["Exchange_Calendars"].ToString() : "";
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

        /// <summary>Saves the preferences of a user.</summary>
        /// <param name="preferencesObject">The preferences to save.</param>
        /// <param name="userName">The user name of the user.</param>
        /// <param name="IsFirstTime">Whether this is the first time or not.</param>
        /// <returns></returns>
        public UserPreferences SaveUserPreferences(UserPreferences preferencesObject, string userName, bool IsFirstTime)
        {

            try
            {
                SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
                {
                    SPUser user = GetUserObject(userName);
                    string userSID = user.Sid;
                    string listName;
                    string siteUrl = ParseSiteUrl(userPreferencesStoreUrl, out listName);
                    using (SPSite siteCollection = new SPSite(siteUrl))
                    {
                        using (SPWeb web = siteCollection.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            SPList list = web.Lists[listName];

                            SPListItem preferencesItem = null;
                            if (IsFirstTime == false)
                            {
                                SPQuery query = new SPQuery();
                                query.Query += "<Where><Eq><FieldRef Name=\"User_SID\" /><Value Type=\"Text\">" + userSID + "</Value></Eq></Where>";
                                SPListItemCollection preferencesListItems = list.GetItems(query);
                                if (preferencesListItems.Count > 0)
                                {
                                    preferencesItem = preferencesListItems[0];
                                }
                            }

                            if (preferencesItem == null)
                            {
                                // Does not already exist so add it to the list.
                                preferencesItem = list.Items.Add();
                                preferencesItem["User_SID"] = userSID;
                            }

                            preferencesItem["Show_Assignments"] = preferencesObject.ShowAssignments;
                            preferencesItem["Show_Personal_Calendar"] = preferencesObject.ShowPersonalCalendar;
                            preferencesItem["WSS_Calendars"] = preferencesObject.WssCalendars;
                            preferencesItem["Exchange_Calendars"] = preferencesObject.ExchangeCalendars;
                            preferencesItem.Update();
                        }
                    }
                }));
            }
            catch (Exception)
            {
                return null;
            }
            return preferencesObject;
        }

        private SPListItem GetPreferencesItem(string userSID)
        {
            SPListItem item = null;

            SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
           {
               SPQuery query = new SPQuery();
               string listName;
               string siteUrl = ParseSiteUrl(userPreferencesStoreUrl, out listName);
               using (SPSite siteCollection = new SPSite(siteUrl))
               {
                   using (SPWeb web = siteCollection.OpenWeb())
                   {
                       try
                       {
                           SPList list = web.Lists[listName];
                           query.Query += "<Where><Eq><FieldRef Name=\"User_SID\" /><Value Type=\"Text\">" + userSID + "</Value></Eq></Where>";
                           SPListItemCollection preferencesListItems = list.GetItems(query);
                           if (preferencesListItems.Count > 0)
                           {
                               item = preferencesListItems[0];
                           }
                       }
                       catch (ArgumentException)
                       {
                           //List does not exist
                           item = null;
                       }
                   }
               }
           }));

            return item;
        }

        private SPUser GetUserObject(string userName)
        {
            try
            {
                // TODO - Handle invalid username gracefully
                return SPContext.Current.Site.RootWeb.AllUsers[userName];
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>Parses the site url</summary>
        /// <param name="listUrl">The url of the list.</param>
        /// <param name="listName">The name of the list. An output parameter.</param>
        /// <returns>The parsed url.</returns>
        public string ParseSiteUrl(string listUrl, out string listName)
        {
            string m_listUrl, m_SiteUrl, m_listName;
            int m_LastIndex, m_urlength;
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

        /// <summary>Determines if the user preferences list exists.</summary>
        /// <param name="userPerferencesStoreUrl">The url of the list.</param>
        /// <returns>True if the list exists.</returns>
        public bool IsUserPerefencesExist(string userPerferencesStoreUrl)
        {
            bool storeExists = false;

            try
            {
                if (userPerferencesStoreUrl.Length > 0)
                {
                    SPSecurity.RunWithElevatedPrivileges(
                            new SPSecurity.CodeToRunElevated(
                                delegate
                                {
                                    string listName;
                                    string url = ParseSiteUrl(userPerferencesStoreUrl, out listName);

                                    using (SPSite userPerferencesStoreSiteCollection = new SPSite(url))
                                    {
                                        using (SPWeb userPerferencesStoreWeb = userPerferencesStoreSiteCollection.OpenWeb())
                                        {
                                            try
                                            {
                                                storeExists = (userPerferencesStoreWeb.Lists[listName.Replace("%20", " ")] != null);
                                            }
                                            catch (ArgumentException)
                                            {
                                                storeExists = false;
                                            }
                                        }
                                    }
                                }
                              )
                            );
                }
                return storeExists;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
