using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.Helper.UserPreferences
{
    /// <summary>The user preferences.</summary>
    public class UserPreferences
    {
        private bool showAssignments=true;
        private bool showPersonalCalendar=true;
        private string wssCalendars;
        private string exchangeCalendars;
        private string userSID;

        /// <summary>The user's SID.</summary>
        public string UserSID
        {
            get { return userSID; }
            set { userSID = value; }
        }

        /// <summary>Whether to show assignments or not.</summary>
        public bool ShowAssignments
        {
            get { return showAssignments; }
            set { showAssignments = value; }
        }

        /// <summary>Whether to show the personal calendar.</summary>
        public bool ShowPersonalCalendar
        {
            get { return showPersonalCalendar; }
            set { showPersonalCalendar = value; }
        }

        /// <summary>The SharePoint calendars to show.</summary>
        public string WssCalendars
        {
            get { return wssCalendars; }
            set { wssCalendars = value; }
        }

        /// <summary>The Exchange calendars to show.</summary>
        public string ExchangeCalendars
        {
            get { return exchangeCalendars; }
            set { exchangeCalendars = value; }
        }
    }
}
