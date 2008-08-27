/*
 * Date             :   06-03-2007
 * Checked in by    :   Mohamed Yehia
 * Description      :   Added Created By fields and deleted the author field.
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MLG2007.WebParts.MetaSearch.Helper
{
    /// <summary>
    /// List of default Sharepoint field's internal names used in search queries.
    /// </summary>
    internal static class Fields
    {
        internal const string TitleField = "Title";
        /// <summary>
        /// The file name of the document file
        /// </summary>
        internal const string FileLeafRefField = "FileLeafRef";
        /// <summary>
        /// List item ID
        /// </summary>
        internal const string IDField = "ID";
        
        /// <summary>
        /// Document creation date.
        /// </summary>
        internal const string CreatedField = "Created";
        /// <summary>
        /// File size in bytes
        /// </summary>
        internal const string FileSizeDisplayField = "FileSizeDisplay";
        /// <summary>
        /// Content type title in which the document belongs 
        /// </summary>
        internal const string ContentTypeField = "ContentType";

        internal const string Created_By = "Author";
        
        
    }

    /// <summary>
    /// List of CSS classes in core.css that are used in the web part  
    /// </summary>
    internal static class Styles
    { 
        internal const string Srch_Stats ="srch-stats";
        internal const string MS_descriptiontext = "ms-descriptiontext";
        internal const string MS_sbscopes = "ms-sbscopes";
        internal const string MS_sbcell = "ms-sbcell";
        internal const string MS_sbplain = "ms-sbplain";
        internal const string Srch_Icon = "srch-Icon";
        internal const string Srch_Title = "srch-Title";
        internal const string Srch_Description = "srch-Description";
        internal const string Srch_Metadata  = "srch-Metadata"; 
        
        
        
        
        
        
        
        
    }
}
