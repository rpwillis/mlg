using System;
using Microsoft.LearningComponents;
using Microsoft.SharePointLearningKit;
using Microsoft.SharePointLearningKit.Schema;
using Microsoft.LearningComponents.Storage;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Data;

namespace MLG2007.Helper.SharePointLearningKit
{
    /// <summary>A collection of <see cref="Appointments"/> based on SLK assignments.</summary>
    public class SLKEvents : Appointments
    {
        #region Private Variables
        private string userName;
        private DateTime startDate;
        private DateTime endDate;
        private bool hasError;
        private string errorDesc;
        private AssignmentMode assignmentMode;
        private string classesUrl;
        #endregion

        #region Public Properties
        /// <summary>The user's username.</summary>
        public string Username
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>The start of the period to get assignments for.</summary>
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        /// <summary>The end of the period to get assignments for.</summary>
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        /// <summary>Whether there was an error retrieving assignments.</summary>
        public bool HasError
        {
            get { return hasError; }
            set { hasError = value; }
        }

        /// <summary>The description of any error.</summary>
        public string ErrorDescription
        {
            get { return errorDesc; }
            set { errorDesc = value; }
        }

        /// <summary>The mode to retrieve assignments as.</summary>
        public AssignmentMode Mode
        {
            get { return assignmentMode; }
            set { assignmentMode = value; }
        }

        /// <summary>The url of the classes site.</summary>
        public string ClassesUrl
        {
            get { return classesUrl; }
            set { classesUrl = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Class Constructor. It initializes an SLKStore Object
        /// </summary>
        public SLKEvents()
        {
        }

        /// <summary>Retrieves the data.</summary>
        public void GetData()
        {
            //Getting the SLKstore object
            SlkStore _slkStore = GetSLKStore();
            //
            switch (assignmentMode)
            {
                case AssignmentMode.Instructor:
                    ProcessInstructorAssignments(_slkStore);
                    break;
                case AssignmentMode.Learner:
                    ProcessLearnerAssignments(_slkStore);
                    break;
                case AssignmentMode.All:
                    ProcessAllAssignments(_slkStore);
                    break;
            }
        }

        /// <summary>
        /// This is an internal helper function. This function will bring back an SLKStore Object.
        /// </summary>
        /// <returns>SLKStore Object stamped with the currently logged user</returns>
        private SlkStore GetSLKStore()
        {
            try
            {
                //Getting User Object
                using (SPSite siteForUser = new SPSite(classesUrl))
                {
                    using (SPWeb webForUser = siteForUser.RootWeb)
                    {
                        //TODO: Does not handle invalid users gracefully.
                        SPUserToken token = webForUser.AllUsers[Username].UserToken;

                        using (SPSite site = new SPSite(classesUrl ,token))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                return SlkStore.GetStore(web);
                            }
                        }
                    }

                }
            }
            catch (Exception exception)
            {
                hasError = true;
                errorDesc = exception.Message;
                return null;
            }

        }

        /// <summary>
        /// This is an internal helper function. This function will bring back instructor assignments.
        /// </summary>
        /// <param name="slkStore">SLkStore object stamped with the identity of the currently logged user</param>
        private void ProcessInstructorAssignments(SlkStore slkStore)
        {
            //Declarations
            LearningStoreQuery _storeQueryObject;
            LearningStoreJob _storeJobObject;
            DataRowCollection _dataRowsCollectionObject;
            Appointments.AppointmentRow _appointmentDataRow;
            LearningStoreItemIdentifier _assignmentItemIdentifier;

            try
            {
                // Constructing the StoreQuery Object
                _storeQueryObject = slkStore.LearningStore.CreateQuery(AssignmentListForInstructors.ViewName);
                _storeQueryObject.AddColumn(AssignmentListForInstructors.AssignmentId);
                _storeQueryObject.AddColumn(AssignmentListForInstructors.AssignmentTitle);
                _storeQueryObject.AddColumn(AssignmentListForInstructors.AssignmentStartDate);
                _storeQueryObject.AddColumn(AssignmentListForInstructors.AssignmentDueDate);
                _storeQueryObject.AddColumn(AssignmentListForInstructors.AssignmentDescription);
                _storeQueryObject.AddColumn(AssignmentListForInstructors.AssignmentSPWebGuid);
                _storeQueryObject.AddSort(AssignmentListForInstructors.AssignmentDueDate, LearningStoreSortDirection.Ascending);
                _storeQueryObject.AddCondition(AssignmentListForInstructors.AssignmentDueDate, LearningStoreConditionOperator.NotEqual, null);


                //Creating the Job Object
                _storeJobObject = slkStore.LearningStore.CreateJob();
                _storeJobObject.PerformQuery(_storeQueryObject);

                //Executing the Job
                _dataRowsCollectionObject = _storeJobObject.Execute<DataTable>().Rows;

                if (_dataRowsCollectionObject != null)
                {
                    // Formatting the results
                    foreach (DataRow _rowTempObject in _dataRowsCollectionObject)
                    {
                        //Console.WriteLine(dr[AssignmentListForInstructors.AssignmentTitle].ToString());
                        _appointmentDataRow = this.Appointment.NewAppointmentRow();
                        _appointmentDataRow.Title = _rowTempObject[AssignmentListForInstructors.AssignmentTitle].ToString();
                        _assignmentItemIdentifier = (LearningStoreItemIdentifier)_rowTempObject[AssignmentListForInstructors.AssignmentId];
                        _appointmentDataRow.ID = _assignmentItemIdentifier.GetKey().ToString();
                        _appointmentDataRow.BeginDate = (DateTime)((_rowTempObject[AssignmentListForInstructors.AssignmentDueDate] != null) ? _rowTempObject[AssignmentListForInstructors.AssignmentDueDate] : DateTime.MinValue);
                        _appointmentDataRow.EndDate = (DateTime)((_rowTempObject[AssignmentListForInstructors.AssignmentDueDate] != null) ? _rowTempObject[AssignmentListForInstructors.AssignmentDueDate] : DateTime.MinValue);
                        _appointmentDataRow.Subject = _rowTempObject[AssignmentListForInstructors.AssignmentTitle].ToString();
                        _appointmentDataRow.Source = "ClassServerAssignment";
                        _appointmentDataRow.URL = "/_layouts/1033/LgUtilities/showSlkdetails.aspx?assignmentID=" + _appointmentDataRow.ID + "&UT=1" + "&classesURL=" + ClassesUrl + "&userName=" + userName.Replace(@"\", @"\\");
                        this.Appointment.AddAppointmentRow(_appointmentDataRow);
                    }
                }

            }
            catch (Exception exception)
            {
                ErrorDescription = "Instructor Assignment Retrieval" + exception.Message;
                HasError = true;
            }
        }

        /// <summary>
        /// This is an internal helper function. This function will bring back Learner assignments.
        /// </summary>
        /// <param name="slkStore">SLkStore object stamped with the identity of the currently logged user</param>
        private void ProcessLearnerAssignments(SlkStore slkStore)
        {
            //Declarations
            LearningStoreQuery _storeQueryObject;
            LearningStoreJob _storeJobObject;
            DataRowCollection _dataRowsCollectionObject;
            Appointments.AppointmentRow _appointmentDataRow;
            LearningStoreItemIdentifier _assignmentItemIdentifier;

            try
            {
                // Constructing the StoreQuery Object
                _storeQueryObject = slkStore.LearningStore.CreateQuery(LearnerAssignmentListForLearners.ViewName);
                _storeQueryObject.AddColumn(LearnerAssignmentListForLearners.LearnerAssignmentId);
                _storeQueryObject.AddColumn(LearnerAssignmentListForLearners.AssignmentTitle);
                _storeQueryObject.AddColumn(LearnerAssignmentListForLearners.AssignmentStartDate);
                _storeQueryObject.AddColumn(LearnerAssignmentListForLearners.AssignmentDueDate);
                _storeQueryObject.AddColumn(LearnerAssignmentListForLearners.AssignmentDescription);
                _storeQueryObject.AddColumn(LearnerAssignmentListForLearners.AssignmentSPWebGuid);
                _storeQueryObject.AddCondition(LearnerAssignmentListForLearners.AssignmentDueDate, LearningStoreConditionOperator.NotEqual, null);
                _storeQueryObject.AddSort(LearnerAssignmentListForLearners.AssignmentDueDate, LearningStoreSortDirection.Ascending);
                //_storeQueryObject.AddCondition(LearnerAssignmentListForLearners.LearnerName, LearningStoreConditionOperator.Equal, userName);

                //Creating the Job Object
                _storeJobObject = slkStore.LearningStore.CreateJob();
                _storeJobObject.PerformQuery(_storeQueryObject);

                //Executing the Job
                _dataRowsCollectionObject = _storeJobObject.Execute<DataTable>().Rows;

                if (_dataRowsCollectionObject != null)
                {
                    // Formatting the results
                    foreach (DataRow _rowTempObject in _dataRowsCollectionObject)
                    {
                        //Console.WriteLine(dr[AssignmentListForInstructors.AssignmentTitle].ToString());
                        _appointmentDataRow = this.Appointment.NewAppointmentRow();
                        _appointmentDataRow.Title = (_rowTempObject[LearnerAssignmentListForLearners.AssignmentTitle] != null) ? _rowTempObject[LearnerAssignmentListForLearners.AssignmentTitle].ToString() : string.Empty;
                        _assignmentItemIdentifier = (LearningStoreItemIdentifier)((_rowTempObject[LearnerAssignmentListForLearners.LearnerAssignmentId] != null) ? _rowTempObject[LearnerAssignmentListForLearners.LearnerAssignmentId] : string.Empty);
                        _appointmentDataRow.ID = _assignmentItemIdentifier.GetKey().ToString();
                        _appointmentDataRow.BeginDate = (DateTime)((_rowTempObject[LearnerAssignmentListForLearners.AssignmentDueDate] != null) ? _rowTempObject[LearnerAssignmentListForLearners.AssignmentDueDate] : DateTime.MinValue);
                        _appointmentDataRow.EndDate = (DateTime)((_rowTempObject[LearnerAssignmentListForLearners.AssignmentDueDate] != null) ? _rowTempObject[LearnerAssignmentListForLearners.AssignmentDueDate] : DateTime.MinValue);
                        _appointmentDataRow.Subject = (_rowTempObject[LearnerAssignmentListForLearners.AssignmentTitle] != null) ? _rowTempObject[LearnerAssignmentListForLearners.AssignmentTitle].ToString() : string.Empty;
                        _appointmentDataRow.Source = "ClassServerAssignment";
                        _appointmentDataRow.URL = "/_layouts/1033/LgUtilities/showSlkdetails.aspx?assignmentID=" + _appointmentDataRow.ID + "&UT=0" + "&classesURL=" + ClassesUrl + "&userName=" + userName.Replace(@"\", @"\\");
                        this.Appointment.AddAppointmentRow(_appointmentDataRow);
                    }
                }
            }
            catch (Exception exception)
            {
                HasError = true;
                ErrorDescription = "Learner Assignment Retrieval :" + exception.Message;
            }
        }

        /// <summary>Returns an assignment for a learner.</summary>
        /// <param name="assignmentId">The is of the assignment.</param>
        /// <returns>An <see cref="Assignment"/>.</returns>
        public Assignment GetAssignmentByIdForLearners(long assignmentId)
        {
            LearnerAssignmentProperties assignmentsProperties;
            LearnerAssignmentItemIdentifier assignmentIdentifier;
            SlkStore slkStore;
            Assignment assignmentObject = null;
            string className;
            try
            {
                slkStore = GetSLKStore();
                SPSecurity.RunWithElevatedPrivileges(new SPSecurity.CodeToRunElevated(delegate
                                {
                                    assignmentIdentifier = new LearnerAssignmentItemIdentifier(assignmentId);
                                    Guid learnerAssignmentGuid = slkStore.GetLearnerAssignmentGuidId(assignmentIdentifier);
                                    assignmentsProperties = slkStore.GetLearnerAssignmentProperties(learnerAssignmentGuid, SlkRole.Learner);

                                    if (assignmentsProperties != null)
                                    {
                                        assignmentObject = new Assignment();
                                        assignmentObject.Description = assignmentsProperties.Description;
                                        assignmentObject.Title = assignmentsProperties.Title;
                                        assignmentObject.DueDate = (DateTime)((assignmentsProperties.DueDate != null) ? assignmentsProperties.DueDate : DateTime.MinValue);
                                        assignmentObject.CreateAt = assignmentsProperties.StartDate;
                                        assignmentObject.CreatedBy = assignmentsProperties.CreatedByName;
                                        className = GetClassName(assignmentsProperties.SPSiteGuid, assignmentsProperties.SPWebGuid);
                                        assignmentObject.SchoolClass = (className != null) ? className : string.Empty;
                                        assignmentObject.Status = GetAssignmentStatus(assignmentsProperties.Status);
                                        //assignmentObject.Score = assignmentsProperties.sc
                                    }
                                }));
            }

            catch (Exception excption)
            {
                HasError = true;
                ErrorDescription = "Learner Assignment Retrieval :" + excption.Message;
            }

            return assignmentObject;
        }

        /// <summary>Returns an assignment for an instructor.</summary>
        /// <param name="assignmentId">The is of the assignment.</param>
        /// <returns>An <see cref="Assignment"/>.</returns>
        public Assignment GetAssignmentsByIdForInstructor(long assignmentId)
        {
            AssignmentProperties assignmentsProperties;
            AssignmentItemIdentifier assignmentIdentifier;
            SlkStore slkStore;
            Assignment assignmentObject = null;
            string className;
            try
            {
                slkStore = GetSLKStore();

                assignmentIdentifier = new AssignmentItemIdentifier(assignmentId);
                assignmentsProperties = slkStore.GetAssignmentProperties(assignmentIdentifier, SlkRole.Instructor);

                if (assignmentsProperties != null)
                {
                    assignmentObject = new Assignment();
                    assignmentObject.Description = assignmentsProperties.Description;
                    assignmentObject.Title = assignmentsProperties.Title;
                    assignmentObject.DueDate = (DateTime)((assignmentsProperties.DueDate != null) ? assignmentsProperties.DueDate : DateTime.MinValue);
                    assignmentObject.CreateAt = assignmentsProperties.StartDate;
                    assignmentObject.Instructors = assignmentsProperties.Instructors;
                    className = GetClassName(assignmentsProperties.SPSiteGuid, assignmentsProperties.SPWebGuid);
                    assignmentObject.SchoolClass = (className != null) ? className : string.Empty;

                    //assignmentObject.Status = GetAssignmentStatus(assignmentsProperties.Status);
                }
            }
            catch (Exception excption)
            {
                HasError = true;
                ErrorDescription = "Learner Assignment Retrieval :" + excption.Message;
            }

            return assignmentObject;
        }

        private static string GetClassName(Guid siteCollectionGuid, Guid webGuid)
        {
            try
            {
                using (SPSite siteCollection = new SPSite(siteCollectionGuid))
                {
                    using (SPWeb classWeb = siteCollection.OpenWeb(webGuid))
                    {
                        return classWeb.Title;
                    }
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }


        }

        private string GetAssignmentStatus(LearnerAssignmentState assignmentState)
        {
            switch (assignmentState)
            {
                case LearnerAssignmentState.Active:
                    return "Active";
                case LearnerAssignmentState.Completed:
                    return "Completed";
                case LearnerAssignmentState.Final:
                    return "Final";
                case LearnerAssignmentState.NotStarted:
                    return "Not Started";
            }
            return "Not determined";
        }
        private void ProcessAllAssignments(SlkStore slkStore)
        {
            ProcessInstructorAssignments(slkStore);
            ProcessLearnerAssignments(slkStore);
        }

        #endregion

    }
}
