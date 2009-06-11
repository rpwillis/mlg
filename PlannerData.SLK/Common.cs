using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.Helper.SharePointLearningKit
{
    /// <summary>The mode to retrieve the assignment.</summary>
    [ Serializable ]
    public enum AssignmentMode
    {
        /// <summary>For an instructor.</summary>
        Instructor,
        /// <summary>For a learner.</summary>
        Learner,
        /// <summary>For all.</summary>
        All 
    }
}
