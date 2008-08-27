using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.Helper.UserPreferences
{
    public class UserPreferences
    {
        private bool showAssignments=true;
        private bool showPersonalCalendar=true;
        private string wssCalendars;
        private string exchangeCalendars;
        private string userSID;

        public string UserSID
        {
            get { return userSID; }
            set { userSID = value; }
        }

        public bool ShowAssignments
        {
            get { return showAssignments; }
            set { showAssignments = value; }
        }

        public bool ShowPersonalCalendar
        {
            get { return showPersonalCalendar; }
            set { showPersonalCalendar = value; }
        }

        public string WssCalendars
        {
            get { return wssCalendars; }
            set { wssCalendars = value; }
        }

        public string ExchangeCalendars
        {
            get { return exchangeCalendars; }
            set { exchangeCalendars = value; }
        }
    }
}
