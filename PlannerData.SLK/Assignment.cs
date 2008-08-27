using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.Helper.SharePointLearningKit
{
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

        public string Score
        {
            get
            {
                return score;
            }

            set
            {
                score = value;
            }
        }

        public Microsoft.SharePointLearningKit.SlkUserCollection Instructors
        {
            get
            {
                return instructors;
            }
            set 
            {
                instructors = value;
            }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public DateTime DueDate
        {
            get { return dueDate; }
            set { dueDate = value; }
        }

        public DateTime CreateAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
        public string CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; ; }
        }
        public string SchoolClass
        {
            get { return schoolClass; }
            set { schoolClass = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
