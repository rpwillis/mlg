/*Date : 07-03-2007
 * Check in by  :   Mohamed Yehia
 * Description  :   Refactored the code to remove unnecessary properties
 *              :   Removed MB and KB resources as they are formated in the Tempalte class
 *                 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.SharePoint;

namespace MLG2007.WebParts.MetaSearch.Helper
{
    /// <summary>
    /// Settings that are required by SearchResultItemTemplate
    /// </summary>
    internal  class SearchResultItemSettings
    {
       
        string listID;

        public string ListID
        {
            get { return "{"+ listID + "}"; }
        }
        Hashtable keywords;

        public Hashtable Keywords
        {
            get { return keywords; }
        }

        string actionsLinkText;

        public string ActionsLinkText
        {
            get { return actionsLinkText; }
        }
       

        SPList documentLibrary;

        public SPList DocumentLibrary
        {
            get { return documentLibrary; }
        }
        SPContentType contentType;

        public SPContentType SelectedContentType
        {
            get { return contentType; }
        }



        public SearchResultItemSettings( Hashtable searchKeywords,string eLearningActionsText,SPList docLibrary ,SPContentType selectedContentType)
        {

            listID = docLibrary.ID.ToString();
            keywords = searchKeywords;
            actionsLinkText = eLearningActionsText;
            documentLibrary = docLibrary;
            contentType = selectedContentType;
            

 
        }


    }
}
