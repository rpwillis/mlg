/*
 * Date 19-3-2007
 * By Marwan Tarek
 * hided the e-learning actions button if the SLK feature is deactivated on school site
 * */
/*
 * Date             :   06-03-2007
 * Checked in by    :   Mohamed Yehia
 * Description      :   Removed Author field from Summary and added the created by field to resolve issue no 136846
 *                  :   Used SharePoint size formatting and removed KB and MB resources
 *                  :   Item title and url in th summary section link to the display properties page isse # 137153
 * 
 * 
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace MLG2007.WebParts.MetaSearch.Helper
{
    /// <summary>
    /// The template in which search results items are rendered.
    /// </summary>
    internal class SearchResultItemTemplate : ITemplate

    {
       
        private const string DocIconID = "DocIconID";
        private const string TitleLinkID = "TitleLinkID";
        private const string ActionsLinkID = "ActionsLinkID";
        private const string DetailsCellID = "DetailsCellID";
        private const string TextlabelID = "TextlabelID";
        private const string FileLinkID = "FileLinkID";
        private const string SummaryControlID = "SummaryControlID";
        private const string SummaryControlHTML = "<span class=\"srch-URL\"> <a href='{0}' title='{0}'>{0}</a>{1}</span>";
        

        private const string ELearningActionsUrl = "{0}/_layouts/SharePointLearningKit/Actions.aspx?ListId={1}&ItemId={2}";

        private const string SLKFeatureID = "00057002-c978-11da-ba52-00042350e42e";
        
        SearchResultItemSettings settings;
            
        public SearchResultItemTemplate (SearchResultItemSettings   itemSettings)

        {
            settings = itemSettings;
    }
        
        #region ITemplate Members

        public void InstantiateIn(Control container)
        {
            //This table holds all other UI elements for each search result
            //The table is structure with 3 rows:
            //* Header row : holds the document icon, title link and E-learning actions
            //* Details row: holds the details of the fields selected in search
            //* Summary row: holds the summary infomration about the item such as the full url, size, author and creation date
            
            Table rootTable = new Table();
            rootTable.CellPadding = 0;
            rootTable.CellSpacing = 0;

            TableRow row = new TableRow();
            rootTable.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.ColumnSpan=3;
           
            
            Table headerTable = new Table();
            headerTable.CellPadding = 0;
            headerTable.CellSpacing = 0;
            headerTable.Width= new Unit(100,UnitType.Percentage);

            cell.Controls.Add(headerTable);
            //Create header row
            TableRow headerRow = new TableRow();
            //create header row cells  and controls
            
            
            //Doc Icon
            TableCell docIconCell = new TableCell();
            //Holds the document file type icon url
            Image docIcon = new Image();
            docIcon.ID = DocIconID;
            docIcon.CssClass = Styles.Srch_Icon;
            
            docIconCell.Controls.Add(docIcon);
            headerRow.Cells.Add(docIconCell);
            //Title link
            TableCell titleCell = new TableCell();
            titleCell.Width = new Unit(100, UnitType.Percentage);
            HyperLink titleLink = new HyperLink();
            titleLink.ID = TitleLinkID;
            titleLink.CssClass = Styles.Srch_Title;
            titleCell.Controls.Add(titleLink);
            headerRow.Cells.Add(titleCell);
            //E- Learning actions link
            //check if SLK feature is activated to show e-learning action link
            SPFeature SLKFeature = settings.DocumentLibrary.ParentWeb.Features[new Guid(SLKFeatureID)];
            if (SLKFeature != null)
            {
                TableCell actionsCell = new TableCell();
                HyperLink actionsLink = new HyperLink();
                actionsLink.ID = ActionsLinkID;
                actionsLink.CssClass = Styles.Srch_Icon;
                actionsCell.Controls.Add(actionsLink);
                headerRow.Cells.Add(actionsCell);
            }
            headerTable.Rows.Add(headerRow);

            //Details Row
            TableRow detailsRow = new TableRow();
            rootTable.Rows.Add(detailsRow);
            //This cell holds the table containg the dynamic list of searched fields
            TableCell detailsCell = new TableCell();
            detailsCell.ID = DetailsCellID;
            detailsCell.ColumnSpan = 3;
            detailsCell.CssClass = Styles.Srch_Description;
            detailsRow.Cells.Add(detailsCell);
            //Summary row
            TableRow summaryRow = new TableRow();
            rootTable.Rows.Add(summaryRow);
           
            
            //File Link
            TableCell fileLinkCell = new TableCell();
            fileLinkCell.CssClass = Styles.Srch_Metadata;
            summaryRow.Cells.Add(fileLinkCell);
            fileLinkCell.ColumnSpan = 3;
            LiteralControl summaryControl = new LiteralControl();
            summaryControl.ID = SummaryControlID;
            fileLinkCell.Controls.Add(summaryControl);
            


            rootTable.DataBinding += new EventHandler(rootTable_DataBinding);

            container.Controls.Add(rootTable);
        }

        void rootTable_DataBinding(object sender, EventArgs e)
        {
            Table rootTable = (Table)sender;
            GridViewRow row = (GridViewRow)rootTable.NamingContainer;
            int itemID = int.Parse(DataBinder.Eval(row.DataItem, Fields.IDField).ToString());
            SPFile itemFile = settings.DocumentLibrary.GetItemById(itemID).File;
            
            string fileurl =  new Uri( new Uri(settings.DocumentLibrary.ParentWeb.Url), String.Format("{0}?ID={1}", settings.DocumentLibrary.Forms[PAGETYPE.PAGE_DISPLAYFORM].ServerRelativeUrl, itemID)).OriginalString;
            
            
            //Bind image controls
            Image docIconImage = (Image)rootTable.FindControl(DocIconID);
            string iconUrl = itemFile.IconUrl;
            docIconImage.ImageUrl = "/_layouts/images/" + iconUrl.TrimStart('/');
            
            //Bind the title link
            HyperLink titleLink = (HyperLink) rootTable.FindControl(TitleLinkID);
            titleLink.Text = (String.IsNullOrEmpty(itemFile.Title)) ? itemFile.Name : itemFile.Title;
            titleLink.NavigateUrl = fileurl;

            //check if SLK feature is activated to show e-learning action link
            SPFeature SLKFeature= settings.DocumentLibrary.ParentWeb.Features[new Guid(SLKFeatureID)];
            if (SLKFeature != null)
            {
                //Bind Actions link
                HyperLink actionsImage = (HyperLink)rootTable.FindControl(ActionsLinkID);
                actionsImage.NavigateUrl = String.Format(ELearningActionsUrl, settings.DocumentLibrary.ParentWebUrl.TrimEnd('/'), settings.ListID, itemID);
                actionsImage.ImageUrl = "/_layouts/SharePointLearningKit/Images/ActionsIcon.gif";
                actionsImage.ToolTip = settings.ActionsLinkText;
            }
            
            //Bind the details row
            if(settings.Keywords.Count>0)
            {
                TableCell detailsCell = (TableCell) rootTable.FindControl(DetailsCellID);
                IDictionaryEnumerator keywordIterator = settings.Keywords.GetEnumerator();
                while (keywordIterator.MoveNext())
                {
                    Label keywordLabel = new Label();
                    string keyword = keywordIterator.Key.ToString();
                    string fieldTitle = settings.SelectedContentType.Fields.GetFieldByInternalName(keyword).Title;
                    keywordLabel.Text = String.Format("{0}:{1}", fieldTitle, DataBinder.Eval(row.DataItem, keyword).ToString());

                    detailsCell.Controls.Add(keywordLabel);
                    detailsCell.Controls.Add(new LiteralControl("<BR>"));
                }
              
            
            }
          

            string tempAuthor = String.Empty;
            string tempSize = String.Empty;
            string tempdate = string.Empty;

            
              if (DataBinder.Eval(row.DataItem, Fields.Created_By).GetType() != typeof(DBNull))
                {
                    tempAuthor = " - " + DataBinder.Eval(row.DataItem, Fields.Created_By).ToString();
                }
            

            if (DataBinder.Eval(row.DataItem, Fields.FileSizeDisplayField).GetType() != typeof(DBNull))
            {
                //format size
               long sizeInBytes =  long.Parse(DataBinder.Eval(row.DataItem, Fields.FileSizeDisplayField).ToString());
               tempSize = " - " + SPUtility.FormatSize(sizeInBytes);
            }

            if (DataBinder.Eval(row.DataItem, Fields.CreatedField).GetType() != typeof(DBNull))
            {
                
                DateTime creationDate =  DateTime.Parse(DataBinder.Eval(row.DataItem, Fields.CreatedField).ToString());

                tempdate = " - " + SPUtility.FormatDate(settings.DocumentLibrary.ParentWeb, creationDate, SPDateFormat.DateOnly);

            }

          
            LiteralControl summaryText = (LiteralControl)rootTable.FindControl(SummaryControlID);

            summaryText.Text = String.Format(SummaryControlHTML, fileurl, (tempSize + tempAuthor + tempdate));



        }

        

        #endregion
    }
}
