using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.Helper.CalendarStore
{
    /// <summary>A calendar.</summary>
    public class Calendar
    {
        private string calendarName;
        private string calendarDescription;        
        private string calendarUrl;
        private string calendarRole;
        private int calendarId;
        private bool isUserSelected;
        private CalendarType calendarType;

        /// <summary>The calendar's name.</summary>
        public string CalendarName
        {
            get { return calendarName; }
            set { calendarName = value; }
        }

        /// <summary>The calendar's description.</summary>
        public string CalendarDescription
        {
            get { return calendarDescription; }
            set { calendarDescription = value; }
        }
     
        /// <summary>The calendar's url.</summary>
        public string CalendarUrl
        {
            get { return calendarUrl; }
            set { calendarUrl = value; }
        }
        /// <summary>The calendar's id.</summary>
        public int CalendarId
        {
            get { return calendarId; }
            set { calendarId = value; }
        }
        /// <summary>The calendar's role.</summary>
        public string CalendarRole
        {
            get { return calendarRole; }
            set { calendarRole = value; }
        }

        /// <summary>Indicates if the calendar is user selected.</summary>
        public bool IsUserSelected
        {
            get { return isUserSelected; }
            set { isUserSelected = value; }
        }

        /// <summary>The calendar's type.</summary>
        public CalendarType CalendarType
        {
            get { return calendarType; }
            set { calendarType = value;}
        }

    }
}
