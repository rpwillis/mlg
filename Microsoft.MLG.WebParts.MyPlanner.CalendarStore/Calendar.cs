using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.Helper.CalendarStore
{
    public class Calendar
    {
        private string calendarName;
        private string calendarDescription;        
        private string calendarUrl;
        private string calendarRole;
        private int calendarId;
        private bool isUserSelected;
        private CalendarType calendarType;

        public string CalendarName
        {
            get { return calendarName; }
            set { calendarName = value; }
        }
        public string CalendarDescription
        {
            get { return calendarDescription; }
            set { calendarDescription = value; }
        }
     
        public string CalendarUrl
        {
            get { return calendarUrl; }
            set { calendarUrl = value; }
        }
        public int CalendarId
        {
            get { return calendarId; }
            set { calendarId = value; }
        }
        public string CalendarRole
        {
            get { return calendarRole; }
            set { calendarRole = value; }
        }

        public bool IsUserSelected
        {
            get { return isUserSelected; }
            set { isUserSelected = value; }
        }

        public CalendarType CalendarType
        {
            get { return calendarType; }
            set { calendarType = value;}
        }

    }
}
