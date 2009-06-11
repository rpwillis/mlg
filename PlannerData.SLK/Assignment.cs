using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.Helper.SharePointLearningKit
{
    /// <summary>Represents an SLK assignment.</summary>
    public class Assignment
    {
        private string title;
        private string description;
        private DateTime dueDate;
        private DateTime createdAt;
        private string createdBy;
        private string schoolClass;
        private string status;
        private Microsoft.SharePointLearningKit.SlkUserCollection instructors;
        private string score;

        /// <summary>The assignment's score.</summary>
        public string Score
        {
            get { return score; }
            set { score = value; }
        }

        /// <summary>The collection of instructors for the assignment.</summary>
        public Microsoft.SharePointLearningKit.SlkUserCollection Instructors
        {
            get { return instructors; }
            set { instructors = value; }
        }

        /// <summary>The assignment's title.</summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>The assignment's description.</summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>The assignment's due date.</summary>
        public DateTime DueDate
        {
            get { return dueDate; }
            set { dueDate = value; }
        }

        /// <summary>The assignment's created date.</summary>
        public DateTime CreateAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
        /// <summary>Who the assignment was created by.</summary>
        public string CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; ; }
        }
        /// <summary>The assignment's class.</summary>
        public string SchoolClass
        {
            get { return schoolClass; }
            set { schoolClass = value; }
        }

        /// <summary>The assignment's status.</summary>
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
