using System;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Reflection;
using System.Resources;
using MLGCalendar = MLG2007.Helper.CalendarStore;

namespace MLG2007.Helper.Calendar
{
    /// <summary>
    /// The Calendar class is a helper class that is used by the MyPlanner web part 
    /// and the PlannerData classes to consolidate and display appointments from a 
    /// variety of sources. It defines a typed dataset (Appointments) that is used 
    /// by the PlannerData data sources to return their appointments. It also creates 
    /// an HTML Table containing the controls required for the calendar (e.g. next 
    /// month, previous month), and generates URLs for all of the appointments.  
    /// 
    /// The Appointments dataset is as follows:
    /// URL (string) - This is the link that the planner will go to when the item is clicked
    /// Title (string) - This is the text that will be displayed for the Appointment
    /// ID (string) - Not currently used
    /// Begin Date (dateTime) - The date in the planner that the appointment will show up in
    /// End Date (dateTime) - (Optional) An additional date if required for any other purpose 
    /// Subject (string) - (Optional) An additional string if required for any other purpose
    /// Source (string) - A string that shows where this appointment originated from. The item 
    ///                   will be rendered in the Planner with a style named the same as this.
    /// </summary>

    [Serializable]
    public class Calendar : System.Web.UI.HtmlControls.HtmlTable
    {
        #region Private variables
        private DateTime startDate;
        private DateTime endDate;
        private Appointments eventList;
        private string dayClass = "";
        private string headClass = "";
        private string showmoreClass = "";
        private string dayHeadingClass = "";
        private string dayOMClass = "";
        private string appointmentCssClass = "";
        private string currentDayCssClass = "";
        private string cssClass;
        private string userid = "";
        private string exurl = "";
        private string domain = "";
        private string spsurl = "";
        private string spstitles = "";
        private string siteCollectionURL = "";
        private int slkMode;
        private bool showErrors = false;
        private bool enableOverrideDomain = false;
        private System.Data.DataView dvEvents;
        private int numberEventsToDisplayPerDay = 5;
        private HtmlTableRow headRow = new HtmlTableRow();
        private HtmlTableRow toolbarRow = new HtmlTableRow();
        private System.Web.UI.WebControls.Label monthName = new System.Web.UI.WebControls.Label();
        public System.Web.UI.WebControls.LinkButton prevMonth = new System.Web.UI.WebControls.LinkButton();
        public System.Web.UI.WebControls.LinkButton nextMonth = new System.Web.UI.WebControls.LinkButton();
        public System.Web.UI.WebControls.LinkButton reloadButton = new System.Web.UI.WebControls.LinkButton();
        private MLG2007.Helper.CalendarStore.CalendarCollection calendarCollecion;
        private bool showSlk;
        private bool showExchangeCalendar;
        private string sharepointCalendarIdsList;
        private string exchangeCalendarIdsList;
        private string dnsName;
        private ResourceManager rm = new ResourceManager("Planner.Strings", Assembly.GetExecutingAssembly());
        public Calendar(bool m_showSlk, bool m_showExchange, string sharepointidlist, string exchangeidlist, MLGCalendar.CalendarCollection m_cals)
        {
            ShowSLK = m_showSlk;
            ShowPersonalExchnage = m_showExchange;
            SharePointCalendarIdsList = sharepointidlist;
            ExchangeCalendarIdsList = exchangeidlist;
            Calendars = m_cals;
            HtmlTableCell lCell = new HtmlTableCell("td");
            HtmlTableCell rCell = new HtmlTableCell("td");
            HtmlTableCell mCell = new HtmlTableCell("td");
            HtmlTableCell reloadCell = new HtmlTableCell("td");
            HtmlTableCell myExchangeButton;
            HtmlTableCell mySLKButton;
            HtmlTableCell calendarsMenus;
            HtmlTableCell dummyTD;
            HtmlTableRow dummyTR;
            HtmlTable toolbarTable = new HtmlTable();
            


            dummyTD = new HtmlTableCell("td");
            dummyTD.ColSpan = 7;
            dummyTR = new HtmlTableRow();

            toolbarTable.Attributes.Add("class", "ms-menutoolbar");
            toolbarTable.Width = "100%";

            //Add prev month link
            prevMonth.Text = "<";            
            prevMonth.ID = "prevMonth";
            prevMonth.CommandName = "ChangeMonth";
            nextMonth.Text = ">";
            nextMonth.ID = "nextMonth";
            nextMonth.CommandName = "ChangeMonth";
            prevMonth.CssClass = CssHeadClass;
            nextMonth.CssClass = CssHeadClass;
            reloadButton.Text = "Reload";
            reloadButton.ID = "reload";
            reloadButton.CommandName = "ChangeMonth";
            //reloadButton.Attributes.Add("onclick", "Javascript:this.document.forms['aspnetForm'].submit();");

            lCell.Attributes.Add("class", "ms-cal-navheader");
            lCell.Align = "right";
            rCell.Attributes.Add("class", "ms-cal-navheader");
            rCell.Align = "left";
            mCell.Attributes.Add("class", "ms-cal-navheader");
            //ms-cal-navheader

            //Add next month link
            lCell.Controls.Add(prevMonth);
            rCell.Controls.Add(nextMonth);
            mCell.Controls.Add(monthName);
            
            mCell.Align = "center";
            mCell.ColSpan = 5;
            headRow.Cells.Add(lCell);
            headRow.Cells.Add(mCell);
            headRow.Cells.Add(rCell);
           // headRow.bor
            myExchangeButton = GetMyExchangeButton("My Calendar");
            mySLKButton = GetMySlkButton("Assignments");
            dummyTR.Cells.Add(myExchangeButton);
            dummyTR.Cells.Add(mySLKButton);

            myExchangeButton.Attributes.Add("class", "ms-toolbar");
            mySLKButton.Attributes.Add("class", "ms-toolbar");

            if (m_cals != null)
            {
                calendarsMenus = GetToolbarCalendarMenus();
                calendarsMenus.Attributes.Add("class", "ms-toolbar");
                calendarsMenus.ColSpan = 4;
                dummyTR.Cells.Add(calendarsMenus);
            }

            reloadCell.Controls.Add(reloadButton);
            reloadCell.Width = "100%";
            reloadCell.Align = "right";
            dummyTR.Cells.Add(reloadCell);

            toolbarTable.Rows.Add(dummyTR);
            dummyTD.Controls.Add(toolbarTable);
            toolbarRow.Cells.Add(dummyTD);
            //toolbarRow.Attributes.Add("class", "ms-toolbar");
            //this.Border = 10;
            //this.Border = 3;
            this.Rows.Add(toolbarRow);
            this.Rows.Add(headRow);
        }
        #endregion

        #region Public Properties
        public string DNSName
        {
            get
            {
                return dnsName;
            }
            set
            {
                dnsName = value;
            }
        }
        public string CssClass
        {
            get
            {
                return cssClass;
            }
            set
            {
                this.Attributes.Add("class", value);
                cssClass = value;
            }
        }
        public string CssDayClass
        {
            get
            {
                return dayClass;
            }
            set
            {
                dayClass = value;
            }
        }
        public string CssCurrDayClass
        {
            get
            {
                return currentDayCssClass;
            }
            set
            {
                currentDayCssClass = value;
            }
        }
        public string CssDayHeading
        {
            get
            {
                return dayHeadingClass;
            }
            set
            {
                dayHeadingClass = value;
            }
        }
        public string CssAppointment
        {
            get
            {
                return appointmentCssClass;
            }
            set
            {
                appointmentCssClass = value;
            }
        }
        public string CssHeadClass
        {
            get
            {
                return headClass;
            }
            set
            {
                headClass = value;
            }
        }
        public string CssShowMoreClass
        {
            get
            {
                return showmoreClass;
            }
            set
            {
                showmoreClass = value;
            }
        }
        public string CssDayOtherMonthClass
        {
            get
            {
                return dayOMClass;
            }
            set
            {
                dayOMClass = value;
            }
        }
        public string UserID
        {
            get
            {
                return userid;
            }
            set
            {
                userid = value;
            }
        }
        public string ExURL
        {
            get
            {
                return exurl;
            }
            set
            {
                exurl = value;
            }
        }
        public string Domain
        {
            get
            {
                return domain;
            }
            set
            {
                domain = value;
            }
        }
        public string SPSURL
        {
            get
            {
                return spsurl;
            }
            set
            {
                spsurl = value;
            }
        }
        public string SPSTitles
        {
            get
            {
                return spstitles;
            }
            set
            {
                spstitles = value;
            }
        }
        public string SiteCollectionURL
        {
            get
            {
                return siteCollectionURL;
            }
            set
            {
                siteCollectionURL = value;
            }
        }
        public int SLKMode
        {
            get
            {
                return slkMode;
            }
            set
            {
                slkMode = value;
            }
        }
        public bool ShowErrors
        {
            get
            {
                return showErrors;
            }
            set
            {
                showErrors = value;
            }
        }
        public bool EnableOverrideDomain
        {
            get
            {
                return enableOverrideDomain;
            }

            set
            {
                enableOverrideDomain = value;
            }
        }
        public int NumberEventsToDisplayPerDay
        {
            get
            {
                return numberEventsToDisplayPerDay;
            }
            set
            {
                numberEventsToDisplayPerDay = value;
            }
        }
        public MLG2007.Helper.CalendarStore.CalendarCollection Calendars
        {
            get
            {
                return calendarCollecion;
            }
            set
            {
                calendarCollecion = value;
            }
        }
        public System.DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
            }
        }
        public System.DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                endDate = value;
            }
        }
        public Appointments EventList
        {
            get
            {
                return eventList;
            }
            set
            {
                eventList = value;
                dvEvents = eventList.Appointment.DefaultView;
                dvEvents.Sort = "BeginDate asc";
            }
        }
        public bool ShowSLK
        {
            get { return showSlk; }
            set { showSlk = value; }
        }
        public bool ShowPersonalExchnage
        {
            get { return showExchangeCalendar; }
            set { showExchangeCalendar = value; }
        }
        public string SharePointCalendarIdsList
        {
            get
            {
                return sharepointCalendarIdsList;
            }
            set
            {
                sharepointCalendarIdsList = value;
            }

        }
        public string ExchangeCalendarIdsList
        {
            get
            {
                return exchangeCalendarIdsList;
            }
            set
            {
                exchangeCalendarIdsList = value;
            }

        }
        public override void DataBind()
        {
            cDay dayCell;
            cWeek weekRow;
            cDayHeading dayHeading;
            cWeekHeading weekHeading;
            HtmlTableRow footerRow;
            HtmlTableCell footerCell;

            prevMonth.CssClass = CssHeadClass;
            nextMonth.CssClass = CssHeadClass;
            prevMonth.CommandArgument = StartDate.AddMonths(-1).ToString();
            nextMonth.CommandArgument = StartDate.AddMonths(1).ToString();
            reloadButton.CommandArgument = StartDate.ToString();
            monthName.Text = StartDate.ToString("y").Replace(",", "");
            monthName.CssClass = "ms-calheader";
            footerRow = new HtmlTableRow();
            footerCell = new HtmlTableCell("td");
            footerCell.ColSpan = 7;
            footerCell.Attributes.Add("class", "ms-cal-nodataBtm2");
            footerRow.Cells.Add(footerCell);

            //this.Rows.Add(GetMyPlannerToolbar());

            //Create all the calendar cells here...
            if (CssHeadClass.Length > 0)
                headRow.Attributes.Add("class", CssHeadClass);

            //Draw the toolbar

            //Draw Day headings
            this.Rows.Add(GetDayHeadings());

            System.DateTime currDate;
            currDate = StartDate.AddDays((double)StartDate.DayOfWeek * -1);
            while (currDate <= EndDate)
            {
                weekRow = new cWeek();
                weekHeading = new cWeekHeading();
                for (int i = 1; i <= 7; i++)
                {
                    if (currDate.Month != StartDate.Month)
                    {
                        dayCell = new cDay(CssDayOtherMonthClass);
                        dayHeading = new cDayHeading("ms-cal-topday");
                        dayHeading.DayNumber = 0;
                        //dayCell.DayNumber = 0;
                    }
                    else
                    {
                        if (currDate == DateTime.Today)
                        {
                            dayCell = new cDay(CssCurrDayClass);
                            dayHeading = new cDayHeading("ms-cal-topday-today");
                        }
                        else
                        {
                            dayCell = new cDay(CssDayClass);
                            dayHeading = new cDayHeading("ms-cal-topday");
                        }
                        //dayCell.DayNumber = currDate.Day;
                        dayHeading.DayNumber = currDate.Day;
                        AddEventsToDay(ref dayCell, currDate);
                    }

                    weekHeading.Cells.Add(dayHeading);
                    weekRow.Cells.Add(dayCell);
                    currDate = currDate.AddDays(1);
                }
                this.Rows.Add(weekHeading);
                this.Rows.Add(weekRow);
            }
            //Add Events to cells
            //AddEventsToDays();
            this.Rows.Add(footerRow);
        }
        #endregion

        #region Toolbar Redndering procedures
        private HtmlTableCell GetMySlkButton(string slkButtonTitle)
        {
            HtmlTableCell cell = new HtmlTableCell("td");
            string flagValue;
            cell.NoWrap = false;

            if (ShowSLK)
            {
                flagValue = "0";
                cell.InnerHtml += "<input Type='hidden' Id='showslk' name='showslk' value='1'>";
            }
            else
            {
                flagValue = "1";
                cell.InnerHtml += "<input Type='hidden' Id='showslk' name='showslk' value='0'>";
            }
            //cell.InnerHtml += "<table cellpadding=0 cellspacing=0 border=0 id='qaPublish'  ><tr><td class='ms-consoleqaemptycorner'></td><td class='s-consoleqaemptycorner'></td><td class='ms-consoleqaborderh' id='qaPublish_t'></td><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqaemptycorner' style='width: 1px'></td></tr><tr><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqacorner' id='qaPublish_tl'></td><td><table cellspacing=0 border=0 cellpadding=0 width=100%><tr><td class='ms-consoleqaemptyborderh'></td></tr></table></td><td class='ms-consoleqacorner' id='qaPublish_tr'></td><td class='ms-consoleqaemptycorner' style='width: 1px'></td></tr><tr><td class='ms-consoleqaborderv' id='qaPublish_l'></td><td class='ms-consoleqaemptyborderv'></td><td><table  onmouseover=\"this.className='ms-consoleqabackhover';try{DeferCall('showQAButtonHover', this, true);}catch(e){}\" onmouseout=\"this.className='ms-consoleqaback';try{DeferCall('showQAButtonHover', this, false);}catch(e){}\" cellpadding='0' cellspacing='0' border='0'  class='ms-consoleqaback' id='qaPublish_m'><tr><td valign='middle' class='ms-consoletoolbar' nowrap><img src='/_layouts/images/cnspub16.gif' ID='qaPublish_image' alt='' border=0 width=16 height=16></td><td valign='middle' nowrap style='width: 44px'><a class='ms-consoletoolbar' ID='qaPublish_anchor' tabindex=0 href='#' OnClick=\"javascript:this.document.getElementById ( 'showslk').value=" + flagValue + ";\">" + slkButtonTitle + "</a></td></tr></table></td><td class='ms-consoleqaemptyborderv'></td><td class='ms-consoleqaborderv' id='qaPublish_r' style='width: 1px'></td></tr><tr><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqacorner' id='qaPublish_bl'></td><td class='ms-consoleqaemptyborderh'></td><td class='ms-consoleqacorner' id='qaPublish_br'></td><td class='ms-consoleqaemptycorner' style='width: 1px'></td></tr><tr><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqaborderh' id='qaPublish_b'></td><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqaemptycorner' style='width: 1px'></td></tr></table>";
            //cell.InnerHtml += string.Format("<img src=\"/_layouts/" + System.Threading.Thread.CurrentThread.CurrentUICulture.LCID + "/LgUtilities/images/blank.gif\" class=\"Btn_up\" style=\"cursor:hand;\" onmousedown=\"this.className='Btn_dn';\" onmouseup=\"this.className='Btn_up';\" onclick=\";javascript:this.document.getElementById ( 'showslk').value={0};\">", flagValue);
            //cell.InnerHtml += string.Format("<input type=button value=\"My Assignments\" id=\"showSLK_btn\" name=\"showSLK_btn\" class=\"Btn_up\" onmousedown=\"this.className='Btn_dn';\" onmouseup=\"this.className='Btn_up';\"onclick=\";javascript:this.document.getElementById ( 'showslk').value={0};\">", flagValue); 
            cell.InnerHtml += "<input id=\"showSLK_btn\" name=\"showSLK_btn\" type=button value=\"Assignments\" class=" + ((ShowSLK) ? "\"Btn_dn\"" : "\"Btn_up\"") + "onclick=\"ToggleShowSLK(" + flagValue + ", this.id);\" >";
            //cell.
            // <input type=image src="Form Templates - Calendar_files/blank.gif" class="BtnAssignments" style="cursor:hand;" onmousedown="this.className='BtnAssignments_dn';" onmouseup="this.className='BtnAssignments';" onclick=";javascript:this.document.getElementById ( 'showslk').value=0;">

            return cell;
        }
        private HtmlTableCell GetMyExchangeButton(string exchangeButtonTitle)
        {
            HtmlTableCell cell = new HtmlTableCell("td");
            string flagValue;
            cell.NoWrap = false;

            if (ShowPersonalExchnage)
            {
                flagValue = "0";
                cell.InnerHtml += "<input Type='hidden' Id='showExchange' name='showExchange' value='1'>";
            }
            else
            {
                flagValue = "1";
                cell.InnerHtml += "<input Type='hidden' Id='showExchange' name='showExchange' value='0'>";
            }

            // cell.InnerHtml += "<table cellpadding=0 cellspacing=0 border=0 id='qaPublish'  ><tr><td class='ms-consoleqaemptycorner'></td><td class='s-consoleqaemptycorner'></td><td class='ms-consoleqaborderh' id='qaPublish_t'></td><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqaemptycorner' style='width: 1px'></td></tr><tr><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqacorner' id='qaPublish_tl'></td><td><table cellspacing=0 border=0 cellpadding=0 width=100%><tr><td class='ms-consoleqaemptyborderh'></td></tr></table></td><td class='ms-consoleqacorner' id='qaPublish_tr'></td><td class='ms-consoleqaemptycorner' style='width: 1px'></td></tr><tr><td class='ms-consoleqaborderv' id='qaPublish_l'></td><td class='ms-consoleqaemptyborderv'></td><td><table  /onmouseover=\"this.className='ms-consoleqabackhover';try{DeferCall('showQAButtonHover', this, true);}catch(e){}\" onmouseout=\"this.className='ms-consoleqaback';try{DeferCall('showQAButtonHover', this, false);}catch(e){}\" cellpadding='0' cellspacing='0' border='0'  class='ms-consoleqaback' id='qaPublish_m'><tr><td valign='middle' class='ms-consoletoolbar' nowrap><img src='/_layouts/images/cnspub16.gif' ID='qaPublish_image' alt='' border=0 width=16 height=16></td><td valign='middle' nowrap style='width: 44px'><a class='ms-consoletoolbar' ID='qaPublish_anchor' tabindex=0 href='#' OnClick=\"javascript:this.document.getElementById ( 'showExchange').value=" + flagValue + ";\">" + exchangeButtonTitle + "</a></td></tr></table></td><td class='ms-consoleqaemptyborderv'></td><td class='ms-consoleqaborderv' id='qaPublish_r' style='width: 1px'></td></tr><tr><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqacorner' id='qaPublish_bl'></td><td class='ms-consoleqaemptyborderh'></td><td class='ms-consoleqacorner' id='qaPublish_br'></td><td class='ms-consoleqaemptycorner' style='width: 1px'></td></tr><tr><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqaborderh' id='qaPublish_b'></td><td class='ms-consoleqaemptycorner'></td><td class='ms-consoleqaemptycorner' style='width: 1px'></td></tr></table>";
            //cell.Attributes.Add("class", "");
            // cell.InnerHtml += string.Format("<img  src=\"/_layouts/" + System.Threading.Thread.CurrentThread.CurrentUICulture.LCID + "/LgUtilities/images/blank.gif\" class=\"Btn_up\" style=\"cursor:hand;\" onmousedown=\"this.className='Btn_dn';\" onmouseup=\"this.className='Btn_up';\" onclick=\";javascript:this.document.getElementById ( 'showExchange').value={0};\">", flagValue);
            // cell.InnerHtml += string.Format("<input type=button value=\"My Calendar\" class=\"Btn_up\" onmousedown=\"this.className='Btn_dn';\" onmouseup=\"this.className='Btn_up';\"onclick=\";javascript:this.document.getElementById ( 'showExchange').value={0};\">", flagValue);  
            cell.InnerHtml += "<input id=\"showExchange_btn\" name=\"showExchange_btn\" type=button value=\"My Calendar\" class=" + ((ShowPersonalExchnage) ? "\"Btn_dn\"" : "\"Btn_up\"") + "onclick=\"ToggleShowExchange(" + flagValue + ", this.id);\" >";



            //onmouseover=""this.className='ms-consoleqabackhover';try{DeferCall('showQAButtonHover', this, true);}catch(e){}"" onmouseout=""this.className='ms-consoleqaback';try{DeferCall('showQAButtonHover', this, false);}catch(e){}""
            return cell;
        }

        private HtmlTableCell GetToolbarCalendarMenus()
        {
            string menuDivHtml = "<div class=\"MainNavigation\"><ul>";
            menuDivHtml += GetCalednarsMenu(MLG2007.Helper.CalendarStore.CalendarType.SharePoint) + GetCalednarsMenu(MLG2007.Helper.CalendarStore.CalendarType.Exchange) + "</ul>";
            HtmlTableCell cell = new HtmlTableCell("td");
            cell.InnerHtml = menuDivHtml + GetFormsParameters();
            return cell;
        }
        private string GetCalednarsMenu(MLG2007.Helper.CalendarStore.CalendarType calendarType)
        {
            string menuHeaderTemplate = "<li onmouseover=\"this.className='hover';\" onmouseout=\"this.className=\'';\"><a  href=\"#\">" + ((calendarType == MLG2007.Helper.CalendarStore.CalendarType.SharePoint) ? "Sharepoint Calendars</a><ul>" : "Exchange Calendars</a><ul>");
            int intitialStringCount = menuHeaderTemplate.Length;
            if (Calendars != null)
            {
                foreach (MLGCalendar.Calendar tmpCalendar in Calendars)
                {
                    if (tmpCalendar.CalendarType == calendarType)
                        menuHeaderTemplate += GetMenuItem(tmpCalendar.CalendarId, tmpCalendar.CalendarName, tmpCalendar.CalendarDescription, tmpCalendar.IsUserSelected, tmpCalendar.CalendarType);
                }

                //Check if there's no Calendars
                if (intitialStringCount == menuHeaderTemplate.Length)
                    return "";
                menuHeaderTemplate += "</ul></li>";
            }
            return menuHeaderTemplate;
        }
        private string GetMenuItem(int calendarId, string calendarTitle, string calendarDescription, bool status, MLGCalendar.CalendarType calendarType)
        {
            string menuItemTemplate = (status) ? "<li><input type=\"Checkbox\" id=\"Checkbox_{0}\" alt=\"{1}\"  onclick=" + ((calendarType == MLG2007.Helper.CalendarStore.CalendarType.SharePoint) ? "\"unselectSharePointCalendar({0})\"" : "\"unselectExchangeCalendar({0})\"") + " name=\"Calendar_{0}\" checked /><a href=\"#\" alt=\"{1}\"  onclick=\"" + ((calendarType == MLG2007.Helper.CalendarStore.CalendarType.SharePoint) ? "unselectSharePointCalendar({0})\"" : "\"unselectExchangeCalendar({0})\"") + ">{2}</a></li>" : "<li><input type=\"Checkbox\" id=\"Checkbox_{0}\" alt=\"{1}\"  onclick=" + ((calendarType == MLG2007.Helper.CalendarStore.CalendarType.SharePoint) ? "\"selectSharePointCalendar({0})\"" : "\"selectExchangeCalendar({0})\"") + " name=\"Calendar_{0}\"  /><a href=\"#\" alt=\"{1}\"  onclick=" + ((calendarType == MLG2007.Helper.CalendarStore.CalendarType.SharePoint) ? "\"selectSharePointCalendar({0})\"" : "\"selectExchangeCalendar({0})\"") + ">{2}</a></li>";

            return string.Format(menuItemTemplate, calendarId, calendarDescription, calendarTitle);
        }
        private HtmlTableCell GetRefreashButton(string reloadButtonTitle)
        {
            HtmlTableCell cell = new HtmlTableCell("td");
            //string reloadButtonHtml = "<input type=\"button\" id=\"btnReload\" name=\"btnReload\" OnClick=\"Javascript:this.document.forms['aspnetForm'].submit();\" value=\"{0}\">";
            //reloadButtonHtml = string.Format(reloadButtonHtml, reloadButtonTitle);
            //cell.InnerHtml = reloadButtonHtml;

            cell.Controls.Add(reloadButton);
            return cell;
        }
        #endregion

        #region Utlilities
        private string GetFormsParameters()
        {
            string sharePointCalendar = "<input type=\"hidden\" name=\"selectedCalendars\" id=\"selectedCalendars\" value=\"" + SharePointCalendarIdsList + "\">";
            string exchangeCalendar = "<input type=\"hidden\" name=\"selectedCalendarsE\" id=\"selectedCalendarsE\" value=\"" + ExchangeCalendarIdsList + "\">";
            return sharePointCalendar + exchangeCalendar;
        }

        #endregion

        #region Calendar Rendering Procedures
        private HtmlTableRow GetDayHeadings()
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Cells.Add(GetDayCell(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Sunday)));
            row.Cells.Add(GetDayCell(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Monday)));
            row.Cells.Add(GetDayCell(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Tuesday)));
            row.Cells.Add(GetDayCell(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Wednesday)));
            row.Cells.Add(GetDayCell(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Thursday)));
            row.Cells.Add(GetDayCell(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Friday)));
            row.Cells.Add(GetDayCell(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Saturday)));
            return row;
        }
        private HtmlTableCell GetDayCell(string dayName)
        {
            HtmlTableCell cell = new HtmlTableCell("td");
            cell.InnerText = dayName;
            if (CssDayHeading.Length > 0)
                cell.Attributes.Add("class", CssDayHeading);
            return cell;
        }
        private void AddEventsToDays()
        {
            foreach (Appointments.AppointmentRow app in EventList.Appointment)
            {
                int rowNum;
                int cellNum;
                HtmlTableRow weekRow;
                HtmlTableCell dayCell;
                cellNum = (int)app.BeginDate.ToLocalTime().DayOfWeek;
                rowNum = ((app.BeginDate.ToLocalTime().Day - 1) / 7) + 2;
                weekRow = this.Rows[rowNum];
                dayCell = weekRow.Cells[cellNum];
                dayCell.Controls.Add(new Event(app.Title, app.URL, "Appointment " + app.Source, app.BeginDate.ToLocalTime().ToShortTimeString() + " - " + app.Title, false));
            }
        }
        private void AddEventsToDay(ref cDay calDay, DateTime calDate)
        {
            int intNumBlanks = NumberEventsToDisplayPerDay;
            HtmlAnchor aMore;
            //strHtmlEvents was used to store the html string that was displayed in the new window created by the javascript function ShowDay
            //the javascript function was the cause of Issue #7
            //when the variable strHtmlEvents exceeds a limited size, the function doesn't work so,
            //this javascript is not used anymore, it is replaced by a web page "ShowMore.aspx" in LgUtilities
            //string strHtmlEvents;

            int currEventIndex = 0;
            Appointments.AppointmentRow temp_evRow;

            //loop on all appointments in the current selected month,
            //compare the date of the current day (calDate) to the begin and end dates of each appointment
            //if the current date is found within the begin and end dates, add it to the day cell
            foreach (Appointments.AppointmentRow evRow in dvEvents.Table.Rows)
            {
                DateTime beginDate = evRow.BeginDate.ToLocalTime();
                DateTime endDate;
                try
                {
                    endDate = evRow.EndDate.ToLocalTime();
                }
                catch (System.Data.StrongTypingException)
                {
                    //if endDate = null, set its value to 30 minutes after BeginDate
                    endDate = evRow.BeginDate.ToLocalTime().AddMinutes(30);
                }

                //compare the date of the current day (calDate) to the begin and end dates of each appointment, and
                //if the event EndDate equals the current day, check the time of the event EndDate
                //if it is 00:00:00 (i.e. before the first sec of the day), then don't add it to the current day
                //example:
                //"Event"	StartDate = 1/3/2006 12:00:00 AM, EndDate = 2/3/2006 12:00:00 AM
                //this means that "Event" is a one day event - just on 1/3/2006
                //so, it is addded only to 1/3/2006
                if ((beginDate.Date <= calDate.ToLocalTime().Date) && (calDate.ToLocalTime().Date <= endDate.Date) && !((endDate.TimeOfDay.Hours == 0 && endDate.TimeOfDay.Minutes == 0 && endDate.TimeOfDay.Seconds == 0) && (calDate.Day == endDate.Day && calDate.Month == endDate.Month && calDate.Year == endDate.Year) && !(evRow.Source.ToString() == "ClassServerAssignment")) && !((evRow.Source.ToString() == "SPSEvents") && evRow.Recurrent && (beginDate.Date != calDate.ToLocalTime().Date)))
                {
                    currEventIndex++;
                    intNumBlanks--;
                    //If this would be the last need to see if a more button is required
                    if (intNumBlanks == 0)
                    {
                        #region more
                        //if its the last one, then add it anyway and break
                        if (currEventIndex >= dvEvents.Count)
                        {
                            if (evRow.Source.ToString() == "ClassServerAssignment")
                                calDay.Controls.Add(new Event(evRow.Title, evRow.URL, CssAppointment + " " + evRow.Source, evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + evRow.Title, true));
                            else
                                calDay.Controls.Add(new Event(evRow.Title, evRow.URL, CssAppointment + " " + evRow.Source, evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + evRow.Title, false));

                            break;
                        }

                        //						//if its the last one for this day then add it and break
                        //						temp_evRow = (Appointments.AppointmentRow) dvEvents[currEventIndex].Row;
                        //						beginDate = temp_evRow.BeginDate.ToLocalTime().Date;
                        //						try
                        //						{
                        //							endDate = temp_evRow.EndDate.ToLocalTime().Date;
                        //						}
                        //						catch
                        //						{
                        //							//if endDate = null, set its value to BeginDate
                        //							endDate = temp_evRow.BeginDate.ToLocalTime().Date;
                        //						}
                        //						if(beginDate != calDate.Day)
                        //						{
                        //							temp_evRow = (Appointments.AppointmentRow) dvEvents[currEventIndex-1].Row;
                        //							calDay.Controls.Add(new Event(evRow.Title, evRow.URL,CssAppointment + " " + evRow.Source,evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + evRow.Title));
                        //							break;
                        //						}

                        //Guess not,					
                        //need to construct html string for new window.

                        //						strHtmlEvents = "<table class=&quot;" + CssClass + "&quot;><tr class=&quot;" + CssHeadClass + "&quot;><td>" + calDate.ToLongDateString() + "</td></tr><tr><td><BR>";

                        //////////////////////////////////////////////////////////////////////////
                        System.Text.StringBuilder tmp = new System.Text.StringBuilder(dvEvents.Count);
                        //object[] tmp=new object[dvEvents.Count];
                        for (int i = 0; i < dvEvents.Count; i++)
                        {
                            temp_evRow = (Appointments.AppointmentRow)dvEvents[i].Row;
                            //							if(temp_evRow.BeginDate.ToLocalTime().Day == calDate.Day)
                            //							{
                            //								//Contstruct string of appts
                            //								strHtmlEvents += "<a href=&quot;" + temp_evRow.URL + "&quot; class=&quot;" + CssAppointment + " " + temp_evRow.Source +"&quot; title=&quot;" + temp_evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + temp_evRow.Title + "&quot;>" + temp_evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + temp_evRow.Title + "</A><BR>";
                            //							}
                            tmp.Append(temp_evRow.URL);
                            tmp.Append(",");
                        }
                        // ((this.SharePointCalendarIdsList.Length > 0) ? this.SharePointCalendarIdsList : string.Empty)
                        //////////////////////////////////////////////////////////////////////////
                        //						strHtmlEvents += "</td></tr></table>";
                        aMore = new HtmlAnchor();
                        aMore.InnerText = "more...";
                        string data = Page.Server.HtmlEncode(tmp.ToString().TrimEnd(new char[] { ',' }));
                        //aMore.HRef="javascript:ShowDay('"+ calDate.ToLongDateString() +"','" + strHtmlEvents + "'); void(0)";
                        string tmpuserid = userid;
                        tmpuserid = userid.Replace(@"\", @"\\");
                        string query_string = "?date=" + calDate.Date.ToShortDateString() + "&showslk=" + this.ShowSLK + "&showExchange=" + this.ShowPersonalExchnage + ((this.SharePointCalendarIdsList.Length > 0) ? "&selectedCalendars=" + this.SharePointCalendarIdsList : "&selectedCalendars=") + ((this.ExchangeCalendarIdsList.Length > 0) ? "&selectedCalendarsE=" + this.ExchangeCalendarIdsList : "&selectedCalendarsE=") + "&userid=" + tmpuserid + "&enableoverridedomain=" + EnableOverrideDomain + "&exurl=" + ExURL + "&domain=" + Domain + "&siteColUrl=" + siteCollectionURL + "&slkMode=" + slkMode + "&spsurl=" + SPSURL + "&spstitles=" + SPSTitles + "&DnsName="+this.DNSName+ "&CssClass=" + CssClass + "&CssShowMoreClass=" + CssShowMoreClass + "&CssAppointment=" + CssAppointment + "&showerrors=" + ShowErrors + "', 'Event', '500',  '700', 'status=yes,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes'); void(0)";
                        query_string = System.Web.HttpUtility.HtmlEncode(query_string);
                        aMore.HRef = "JavaScript:launchCenter('" + "/_layouts/1033/LGUtilities/ShowMore.aspx" + query_string;
                        aMore.Attributes.Add("class", CssAppointment);
                        aMore.Title = "All events on this day";
                        calDay.Controls.Add(aMore);

                        #endregion	// end of more

                        //all appointments added to the new window
                        break;
                    }
                    else
                    {
                        if (evRow.Source.ToString() == "ClassServerAssignment")
                            calDay.Controls.Add(new Event(evRow.Title, evRow.URL, CssAppointment + " " + evRow.Source, evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + evRow.Title, true));
                        else
                            calDay.Controls.Add(new Event(evRow.Title, evRow.URL, CssAppointment + " " + evRow.Source, evRow.BeginDate.ToLocalTime().ToShortTimeString() + " - " + evRow.Title, false));
                    }
                }
            }

            //Add blank lines
            for (int i = 0; i < intNumBlanks; i++)
            {
                calDay.Controls.Add(new System.Web.UI.LiteralControl("<BR>"));
            }
        }
        #endregion

    }

    #region Help Drawing Classes
    public class cWeek : System.Web.UI.HtmlControls.HtmlTableRow
    {
        public cWeek()
        {
        }

        public cWeek(string CssClass)
        {
            if (CssClass.Length > 0)
                this.Attributes.Add("class", CssClass);
        }
    }
    public class cWeekHeading : System.Web.UI.HtmlControls.HtmlTableRow
    {
        public cWeekHeading()
        {
        }

        public cWeekHeading(string CssClass)
        {
            if (CssClass.Length > 0)
                this.Attributes.Add("class", CssClass);
        }
    }
    public class cDay : System.Web.UI.HtmlControls.HtmlTableCell
    {
        public cDay()
        {

        }

        public cDay(string CssClass)
        {
            if (CssClass.Length > 0)
                this.Attributes.Add("class", CssClass);
        }


        // Display the number of the day
        public int DayNumber
        {
            set
            {
                if (value == 0)
                    this.Controls.Add(new System.Web.UI.LiteralControl("<BR>"));
                else
                    this.Controls.Add(new System.Web.UI.LiteralControl(value.ToString() + "<BR>"));
            }
        }
    }
    public class cDayHeading : System.Web.UI.HtmlControls.HtmlTableCell
    {
        public cDayHeading()
        {

        }

        public cDayHeading(string CssClass)
        {
            if (CssClass.Length > 0)
                this.Attributes.Add("class", CssClass);
        }


        // Display the number of the day
        public int DayNumber
        {
            set
            {
                if (value == 0)
                    this.Controls.Add(new System.Web.UI.LiteralControl("<BR>"));
                else
                    this.Controls.Add(new System.Web.UI.LiteralControl(value.ToString() + "<BR>"));
            }
        }
    }
    public class Event : System.Web.UI.HtmlControls.HtmlAnchor
    {
        public Event(string Title, string Url, string CssClass, string ToolTip, bool IsCSEvent)
        {
            this.InnerHtml = Title;
            this.HRef = "JavaScript:launchCenter('" + Url + "', 'Event', '500',  '700', 'status=yes,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes'); void(0)";
            if (CssClass.Length != 0)
                this.Attributes.Add("class", CssClass);
            this.Title = ToolTip;
        }
    }
    #endregion
}
