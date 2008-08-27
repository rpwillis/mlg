using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MLG2007.Helper.Exchange;

public partial class ShowExchange : System.Web.UI.Page
{
    protected string description;
    protected string subject;
    protected DateTime starDate;
    protected DateTime endDate;
    protected string id;
    protected string exchangeURL;
    protected string calendarURL;
    protected string exchangeDomain;

    protected void Page_Load(object sender, EventArgs e)
    {
        GetQueryStringParameters();
        ExchEvents exchEvents = new ExchEvents();
        exchEvents.Integrated = false;
        exchEvents.ExchangeDomain = exchangeDomain;
        exchEvents.UTCStartDate = DateTime.Now; //Dummy Values
        exchEvents.UTCEndDate = DateTime.Now;
        exchEvents.ExchangeUser = this.Context.User.Identity.Name;
        if (exchEvents.ExchangeUser.IndexOf("\\") != 0)
            exchEvents.ExchangeUser = exchEvents.ExchangeUser.Substring(exchEvents.ExchangeUser.LastIndexOf("\\") + 1);
        exchEvents.ExchangePassword = this.Context.Request.ServerVariables["AUTH_PASSWORD"].ToString();
        exchEvents.ExchangeURL = exchangeURL;
        string x = this.Page.Server.HtmlEncode(calendarURL);
        exchEvents.GetDataByID(id, x);

        if (exchEvents.Tables[0].Rows.Count > 0)
        {
            Appointments.AppointmentRow dr = (Appointments.AppointmentRow)exchEvents.Appointment.Rows[0];
            subject = dr.Title;
            description = dr.Description;
            starDate = dr.BeginDate.ToLocalTime();
            if (dr.AllDayEvent)
                endDate = dr.EndDate.ToLocalTime().AddDays(-1);
            else
                endDate = dr.EndDate.ToLocalTime();
        }

    }
    //mosaleh@microsoft.com
    void GetQueryStringParameters()
    {
        if (Request.QueryString["CUID"] == null)
        {
            Response.Write("Error retrieving CUID");
            Response.End();
        }
        else
            id = Request.QueryString["CUID"].ToString();

        if (Request.QueryString["SourceURL"] == null)
        {
            Response.Write("Error retrieving SourceURL");
            Response.End();
        }
        else
            calendarURL = Request.QueryString["SourceURL"].ToString();

        if (Request.QueryString["ExchanegURL"] == null)
        {
            Response.Write("Error retrieving ExchanegURL");
            Response.End();
        }
        else
            exchangeURL = Request.QueryString["ExchanegURL"].ToString();

        if (Request.QueryString["Domain"] == null)
        {
            Response.Write("Error retrieving ExchanegURL");
            Response.End();
        }
        else
            exchangeDomain = Request.QueryString["Domain"].ToString();
    }
}
