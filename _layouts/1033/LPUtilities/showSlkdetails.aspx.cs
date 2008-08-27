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
using MLG2007.Helper.SharePointLearningKit;

//using Planner;

public partial class showSlkdetails : System.Web.UI.Page
{
    private string userName;
    private string classesUrl;
    private string assignmentID;
    protected string assignmentTitle;
    protected string assignmentDescription;
    protected DateTime assignmentDueDate;
    protected DateTime assignmentStartDate;
    protected string createdBy;
    protected string className;
    protected string assignmentStatus;
    protected string userType;
    private MLG2007.Helper.SharePointLearningKit.SLKEvents slkAssignments = null;
    private Assignment assignmentObject = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        //Getting QueryString paramaters
        GetQueryStringParameters();
        try
        {
            slkAssignments = new SLKEvents();
            slkAssignments.ClassesUrl = classesUrl;
            slkAssignments.Username = userName;
            if (userType == "0")
                assignmentObject = slkAssignments.GetAssignmentByIdForLearners(long.Parse(assignmentID));
            else
                assignmentObject = slkAssignments.GetAssignmentsByIdForInstructor(long.Parse(assignmentID));

            if (assignmentObject != null)
            {                                
                assignmentTitle = (assignmentObject.Title != null)? assignmentObject.Title: "";
                assignmentDescription = (assignmentObject.Description!=null)?assignmentObject.Description:"";
                assignmentDueDate = ( assignmentObject.DueDate != null)? assignmentObject.DueDate.ToLocalTime():DateTime.Now;
                assignmentStartDate = (assignmentObject.CreateAt !=null)?assignmentObject.CreateAt.ToLocalTime(): DateTime.Now;

                if (userType != "0")
                {
                    string instuctors = string.Empty;
                    TableCell8.Visible = false;
                    TableCell9.Visible = false;
                    foreach (Microsoft.SharePointLearningKit.SlkUser user in assignmentObject.Instructors)
                        instuctors += user.Name + "<br>";
                    createdBy = (assignmentObject.Instructors != null) ? instuctors : "";
                }
                else
                {
                    assignmentStatus = assignmentObject.Status;
                    TableCell10.Visible = false;
                    TableCell11.Visible = false;
                    createdBy = (assignmentObject.CreatedBy != null) ? assignmentObject.CreatedBy : "";
                }
                className = assignmentObject.SchoolClass;                
                
            }
        }
        catch (Exception exception)
        {
            Response.Write("error in getting assignment data");
        }       
    }

    void GetQueryStringParameters()
    {
        if (Request.QueryString["userName"] == null)
        {
            Response.Write("Error retrieving UserName");
            Response.End();
        }
        else
            userName = Request.QueryString["userName"].ToString();

        if (Request.QueryString["classesURL"] == null)
        {
            Response.Write("Error retrieving ClassesURL");
            Response.End();
        }
        else
            classesUrl = Request.QueryString["classesURL"].ToString();

        if (Request.QueryString["assignmentID"] == null)
        {
            Response.Write("Error retrieving assignmentID");
            Response.End();
        }
        else
            assignmentID = Request.QueryString["assignmentID"].ToString();

        if (Request.QueryString["UT"] == null)
        {
            Response.Write("Error retrieving user type");
            Response.End();
        }
        else
            userType = Request.QueryString["UT"].ToString();
    }
}
