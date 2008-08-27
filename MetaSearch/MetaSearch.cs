/*
 * date 21-3-2007
 * Marwan Tarek
 * added support for date fileds
 * reviewed by Karim el hoissny
 * */
/*
 * Date: 18-3-2007
 * checkin by: Marwam Tarek
 * solved issue 138627 for wraping the long columns
 * solved the wraping of the content drop down label
 * */
/*
 * Date: 14-03-2007
 * Checked In by:   Mohamed Yehia
 * Description  :   Corrected the issue of results when paging is a single page
 * 
 * Date: 14-03-2007
 * Checked In by:   Mohamed Yehia
 * Description  :   File names can be searched useing Cintains operator - issue # 138080
 *              :   Javascript disables the button if the textboxes are empty.  This issue occured due because that validation controls  error message had the word "value". issue # 137993
 *              :   Corrected the calculation of the results statistics. 
/*
 * Date: 06-03-2007
 * Checked In by:   Mohamed Yehia
 * Description  :   Removed the author field from the query to resoolve issue no 136846.
 *              :   Added support for configuring document library using relative paths issue # 136878
 *              :   Added validation controls for string, integer, number, currency and string # 136932, # 137012, # 137077
 *              :   Added the search keywords into the view state to resolve issue # 137184

 * 
 * Date: 05-03-2007
 * Checked In by:   Mohamed Yehia
 * Description  :   All access to the property values are made using the Property accessors.
 */
/*
* Date: 04-03-2007
* Checked In by:   Mohamed Yehia
* Description  :   Corrected statistics label behaviour 
* Reviewed by  :   Marwan Tarek
*/
/*
 * Date: 01-03-2007
 * Checked In by:   Mohamed Yehia
 * Description  :   Developed main functionality:
 *              :   Excluded content types and fields are not populated
 *              :   Search items are similar to SharePoint styles with paging
 *              :   Integration with SLK is enabled
 * Reviewed by  :   Marwan Tarek
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

using MLG2007.WebParts.MetaSearch.Helper;
using MLG2007.WebParts.MetaSearch.Helper.QueryHelper;


namespace MLG2007.WebParts.MetaSearch
{
    /// <summary>
    
    /// </summary>
    [ToolboxData("<{0}:MetaSearch runat=server></{0}:MetaSearch>"),
        XmlRoot(Namespace = "MetaSearch")]
    [Guid("3d4e6c42-b231-4079-996d-38f637a2e690")]
    public class MetaSearch : Microsoft.SharePoint.WebPartPages.WebPart 
    {
        

        
        #region Controls IDs
        private const string SearchButtonID = "SearchButton";
        private const string FormPanelID = "FormPanel";
        private const string ContentTypeListID = "ContentTypeList";
        private const string ResultsGridID = "ResultsGrid";
        private const string EvenFieldsTableID = "EvenFieldsTable";
        private const string OddFieldsTableID = "OddFieldsTable";
        private const string FieldsTableID = "FieldsTableID";
        private const string ContentTypeSearchTableID = "ContentTypeSearchTable";
        private const string StatisticsLabelID = "StatisticsLabelID";
        private const string StatisticsCellID = "StatisticsCellID";
        private const string SearchKeywordsVSKey = "SearchKeywords";
        
        #endregion


        #region Private Members
        /// <summary>
        /// A string pr        /// efix to append while building search texboxes IDs'.  TextBocexes IDs are builts by concatenating the prefix and the field internal name.
        /// This prefix helps in finding the tex boxes to obtain the search keywords 
        /// </summary>
        private const string SearchFieldPrefix = "SearchColumn";
        /// <summary>
        /// The selected content type ID is saved in the view state using this key
        /// </summary>
        private const string ContentTypeViewStateKey = "ctypeviewstate";

        private const string MetaSearchValidationGroup = "MetaSearchValidationGroup";


        Table searchFormTable;
        //Holds a comma separated list of fields that are not populated to the user
        string excludedFields = String.Empty;
        //Holds a comma separated list of content type titles that are not populated to the user
        string excludedContentTypes = String.Empty;
        //Holds the document library url
        string sourceDocumentLibrary = String.Empty;
        //Holds any error messages.
        Label errorLabel = null;


        private ResourceManager rm = new ResourceManager("MLG2007.WebParts.MetaSearch.Strings", Assembly.GetExecutingAssembly());


        //Stores the selected content type Id
        string ctypeid = string.Empty;
        
        #endregion

        #region WebPart Properties

        /// <summary>
        /// Document library URL with custom content types to search for learning resources 
        /// </summary>
        [Browsable(true),
ResourcesAttribute("SourceDocumentLibraryTitle", "SourceDocumentLibraryCat", "SourceDocumentLibraryDesc"),
 WebPartStorage(Storage.Shared)]
        public string SourceDocumentLibrary
        {
            get { return sourceDocumentLibrary; }
            set { sourceDocumentLibrary = value; }
        }

        /// <summary>
        /// Holds a comma separated list of content types that are not populated for the user to search in.
        /// </summary>
        [Browsable(true),
ResourcesAttribute("ExcludedContentTypesTitle", "ExcludedContentTypesCat", "ExcludedContentTypesDesc"),
WebPartStorage(Storage.Shared)]
        public string ExcludedContentTypes
        {
            get { return excludedContentTypes; }
            set { excludedContentTypes = value; }
        }

        /// <summary>
        /// Holds a comma separated list of fields that are not populated for the user.
        /// </summary>
        [Browsable(true),
ResourcesAttribute("ExcludedFieldsTitle", "ExcludedFieldsCat", "ExcludedFieldsDesc"),
WebPartStorage(Storage.Shared)]
        public string ExcludedFields
        {
            get { return excludedFields; }
            set { excludedFields = value; }
        }

        bool showErrors = false;
        /// <summary>
        /// Provides error details in messages.
        /// </summary>
        [Browsable(true),
ResourcesAttribute("ShowErrorsTitle", "ShowErrorsCat", "ShowErrorsDesc"),
WebPartStorage(Storage.Shared)]
        public bool ShowErrors
        {
            get { return showErrors; }
            set { showErrors = value; }
        }


         int numberofResults = 10;
       /// <summary>
        /// The number of search result items per page to view in search results. 
       /// </summary>
        [Browsable(true),
ResourcesAttribute("NumberofResultsTitle", "NumberofResultsCat", "NumberofResultsDesc"),
WebPartStorage(Storage.Personal)]
        public int NumberofResults
        {
            get { return numberofResults; }
            set {
                if (value <= 0)
                    numberofResults = 10;
                else
                    numberofResults = value;
                }
        }

        #endregion


        #region Control Events

        void searchButton_Click(object sender, EventArgs e)
        {
            this.Page.Validate();
            if (Page.IsValid)
            {

                try
                {
                    ((Button)sender).Enabled = true;
                    DataTable results = Search();
                    if (results != null)
                        AdjustSearchStatistics(0, results);
                    else
                    {
                        Label statLabel = ((Label)this.FindControl(StatisticsLabelID));
                        statLabel.Visible = false;
                        
                    }

                }
                catch (Exception ex)
                {

                    ShowError(ex);

                }
            }
        }
        void resultsGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            
            this.Page.Validate();
            if (Page.IsValid)
            {
                try
                {
                    ((GridView)sender).PageIndex = e.NewPageIndex;
                    DataTable results = Search();
                    ((Button)this.FindControl(SearchButtonID)).Enabled = true;

                    if (results != null)
                        AdjustSearchStatistics(e.NewPageIndex , results);

                }
                catch (Exception ex)
                {

                    ShowError(ex);

                }
            }
        }

         void contentTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ViewState[SearchKeywordsVSKey] = null;
            AddContentTypeIDToViewState(GetSelectedContentTypeID());
            ChildControlsCreated = false;
            
        }
        #endregion
      


        #region Overrides

        public override string LoadResource(string id)
        {
            return rm.GetString(id, CultureInfo.CurrentCulture);
        }
        protected override void   CreateChildControls()
        {


            try
            {


                //Check if all properites are set
                if (!AllPropertiesSet())
                {
                    ShowMessage(LoadResource("PropertiesEmpty"));

                }
                else
                {
                    //Required properties are set
                    //Check if a valid the given URL is for an existing document library.
                    if (!IsValidDocumentLibrary())
                    {
                        ShowMessage(LoadResource("InvalidDocumentLibrary"));
                    }
                    else
                    {
                        SPList documentLibrary = GetDocumentLibraryFromUrl(SourceDocumentLibrary);
                        if (!documentLibrary.DoesUserHavePermissions(SPBasePermissions.ViewListItems))
                        {
                            ShowMessage(String.Format(LoadResource("NoPermissions"), documentLibrary.Title, documentLibrary.DefaultViewUrl));
                            return;
                        }
                        searchFormTable = new Table();
                        //valid document library
                        //Get Content types

                        List<SPContentType> allowedCtypes = GetAllowedContentTypes(documentLibrary);
                        if (allowedCtypes.Count == 0)
                        {//display error message
                            ShowMessage(LoadResource("NoContentTypesAttached"));
                        }
                        else
                        {
                            //Build UI
                            Controls.Add(searchFormTable);
                            //Add the search header
                            ctypeid = BuildSearchFormHeader(searchFormTable, allowedCtypes);
                            //Add the row that will holds the text boxes popualted
                            BuildFormRow(searchFormTable);
                            //Add the row holding the search button
                            BuildSearchRow(searchFormTable);
                            //Add the results grid 
                            BuildResultsRow(searchFormTable);






                            //For the first time loading
                            //If there is nothing have been selected , then use the content type Id returned from building the UI
                            object temp = this.ViewState[ContentTypeViewStateKey];
                            if (temp != null)
                                ctypeid = temp.ToString();
                            else
                                AddContentTypeIDToViewState(ctypeid);


                            //After all parent controls have been added
                            //Build the search form textboxes and labels depending on the selected content type
                            BuildSearchForm(GetContentTypeByID(ctypeid));

                            //Register the script requred after all controlshave been added
                            RegisterRequiredScripts();

                            //  Get the results table form cache
                            if (this.ViewState[SearchKeywordsVSKey] != null)
                            {
                                Hashtable keywords = (Hashtable)this.ViewState[SearchKeywordsVSKey];
                                if (keywords.Count == 0)
                                {
                                    ShowMessage(LoadResource("NoSearchWords"));
                                    
                                }
                                else 
                                    Search(keywords);


                            }

                        }



                    }
                }

            }




            catch (Exception ex)
            {

                ShowError(ex);

            }


        }
        
       

        #endregion

        /// <summary>
        /// Registers required validation scripts.  EnableSearchButton function enables the search button if there is at least one search keyword.
        /// Trim function removes trailing spaces from search textboxes
        /// 
        /// </summary>
        private void RegisterRequiredScripts()
        {
            string searchButtonClientID = this.FindControl(SearchButtonID).ClientID;
            string searchTableClientID = this.FindControl(ContentTypeSearchTableID).ClientID;
            string js = "<script type=\"text/javascript\">function Trim(textBox){if (textBox.value==' ')textBox.value ='';}function EnableSearchButton(){var searchTable = document.getElementById('" + searchTableClientID + "');  var button = document.getElementById('" + searchButtonClientID + "');button.disabled =(searchTable.innerHTML.indexOf('value=')==-1)	;}</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EnableSearchButton", js);
        }

        /// <summary>
        /// Shows error messages in case of exceptions depending on ShowErrors property value.
        /// </summary>
        /// <param name="ex">Exception that occurred</param>
        private void ShowError(Exception ex)
        {
            if (ShowErrors)
                //show details
                ShowMessage(String.Format(LoadResource("DetailedErrorMessage"), ex.Message));
            else
                ShowMessage(LoadResource("StandardErrorMessage"));
 
        }
        /// <summary>
        /// Adds the error label to the control collection.
        /// </summary>
        private void AddErrorLabel()
        {
             if (errorLabel == null)
                {
                    errorLabel = new Label();
                }
                else
                {
                    errorLabel.Text = String.Empty;
                    
                }
                errorLabel.Visible = true;
            if(!this.Controls.Contains(errorLabel))
                this.Controls.Add(errorLabel);


        }
        /// <summary>
        /// Shows messages in the error label to the user
        /// </summary>
        /// <param name="message"></param>
        private void ShowMessage(string message)
        {
            AddErrorLabel();

            errorLabel.Text =message;
        }
        /// <summary>
        /// Validate whether sourceDocumentLibrary contains a valid document library
        /// </summary>
        /// <returns>True if sourceDocumentLibrary is adocument library URL, false otherwise</returns>
        private bool IsValidDocumentLibrary()
        {
            try
            {

                return (GetDocumentLibraryFromUrl(SourceDocumentLibrary) != null);
            }
            catch (Exception)
            {

                return false;
            }
        }
    
        /// <summary>
        /// Builds the results row including a separator row , statiscs row and label and  results grid. 
        /// </summary>
        /// <param name="searchFormTable">Root search form table to add the results row in</param>
        private void BuildResultsRow(Table searchFormTable)
        {
            //Results Row includes:
            //Separator row - separating the search form from the results
            //Statistics row - contains the label for displaying results statistics
            //Results grid - grid view that rendres the search results items

            //Add separator row
            TableRow separatorRow = new TableRow();
            TableCell separatorCell = new TableCell();
            separatorCell.ColumnSpan = 3;
            separatorCell.Width = new Unit(100, UnitType.Percentage);
            separatorRow.Cells.Add(separatorCell);
            searchFormTable.Rows.Add(separatorRow);
            
            //Add the result statistics
            TableRow statisticsRow = new TableRow();
            TableCell statCell = new TableCell();
              statCell.ColumnSpan = 3;
              statCell.ID = StatisticsCellID;
              searchFormTable.Rows.Add(statisticsRow);
              statisticsRow.Cells.Add(statCell);

              Label statsLabel = new Label();
              statsLabel.Visible = false;
              statsLabel.ID = StatisticsLabelID;
              statsLabel.CssClass = Styles.Srch_Stats;
              statCell.Controls.Add(statsLabel);
            //Add the row for the search results data grid
            TableRow resultsRow = new TableRow();
            TableCell resultsCell = new TableCell();
            resultsCell.ColumnSpan = 2;
            GridView resultsGrid = new GridView();
            resultsGrid.AllowPaging = false;
            resultsGrid.PageIndexChanging += new GridViewPageEventHandler(resultsGrid_PageIndexChanging);
            resultsGrid.PageSize = NumberofResults;
            resultsGrid.ID = ResultsGridID;
            resultsGrid.EmptyDataText = LoadResource("EmptySearchResults");
            resultsGrid.Visible = true;
            resultsGrid.BorderStyle = BorderStyle.None;
            resultsGrid.CellPadding=0;
            resultsGrid.CellSpacing = 0;
            resultsGrid.BorderWidth = new Unit("0");
            resultsGrid.AutoGenerateColumns = false;
            resultsCell.Controls.Add(resultsGrid);
            resultsRow.Cells.Add(resultsCell);
            
            searchFormTable.Rows.Add(resultsRow);
        }

        
        /// <summary>
        /// Builds that form row that contains a panel holding the populated text boxes 
        /// </summary>
        /// <param name="searchFormTable">Root search form table</param>
        private void BuildFormRow(Table searchFormTable)
        {
            
            TableRow formRow = new TableRow();
            TableCell formCell = new TableCell();
            formCell.ColumnSpan = 2;
            //Panel holds the generated form for the custom content types
            Panel formPanel = new Panel();
            formPanel.ID = FormPanelID;
            formPanel.Visible = true;

            //Add controls to the table structure
            formCell.Controls.Add(formPanel);
            formRow.Cells.Add(formCell);
            searchFormTable.Rows.Add(formRow);
        }
        /// <summary>
        /// Builds the row containing the search button and sets the button's properties
        /// </summary>
        /// <param name="searchFormTable">Root table that holds the search row</param>
        private void BuildSearchRow(Table searchFormTable)
        {
            //Add the row for the search button 
            TableRow searchRow = new TableRow();
            TableCell searchCell = new TableCell();
            searchCell.ColumnSpan = 2;
            //Search Button

            Button searchButton = new Button();
            searchButton.Visible = false;
            searchButton.ID = SearchButtonID;
            searchButton.Click += new EventHandler(searchButton_Click);
            searchButton.Text = LoadResource("SearchText");
            searchButton.CssClass = "ms-ButtonHeightWidth";
            searchButton.ValidationGroup = MetaSearchValidationGroup;
            searchButton.CausesValidation = true;
            //Add controls to containers
            searchCell.Controls.Add(searchButton);
            searchRow.Cells.Add(searchCell);
            searchFormTable.Rows.Add(searchRow);

            //Add Validation Summary row
            TableRow validationRow = new TableRow();
            searchFormTable.Rows.Add(validationRow);
            TableCell validationCell = new TableCell();
            validationRow.Cells.Add(validationCell);
            validationCell.ColumnSpan = 2; 

            ValidationSummary validationSummary = new ValidationSummary();
            validationSummary.ValidationGroup = MetaSearchValidationGroup;
            validationSummary.EnableClientScript = true;
            validationCell.Controls.Add(validationSummary);
        }
       
      
        /// <summary>
        /// Caculates the search results statistics
        /// </summary>
        /// <param name="newPageIndex">The page index of the results grid</param>
        /// <param name="totalResults">Total number of search results found</param>
        /// <returns></returns>
        private string CalculateResults(int newPageIndex, int totalResults)
        {
            int startResultsNumber,endResultsNumber = 0;

            if (totalResults > NumberofResults)
            //mutiple pages
            {
                startResultsNumber = (newPageIndex * NumberofResults) + 1;
                if (NumberofResults == 1)
                    endResultsNumber = startResultsNumber;
                else
                {
                    int temp = (startResultsNumber + (NumberofResults - 1));
                    endResultsNumber = Math.Min(temp, totalResults);
                }

            }
            else
            //single page
            {
                startResultsNumber = 1;
                endResultsNumber = totalResults;

            }
             
            return string.Format("{0} {1} - {2} {3} {4}", LoadResource("Results"), startResultsNumber.ToString(), endResultsNumber.ToString(), LoadResource("Of"), totalResults.ToString());
        }
       
        /// <summary>
        /// Obtains a document library object from the given URL
        /// </summary>
        /// <param name="documentLibraryUrl">The doucment library URL</param>
        /// <returns>SPList object if the URL is correct, otherwise null</returns>
        private SPList  GetDocumentLibraryFromUrl(string documentLibraryUrl)
        {
            Uri webUrl  = new Uri(new Uri(Context.Request.Url.ToString()),documentLibraryUrl);
            SPWeb oWeb = new SPSite(webUrl.OriginalString).OpenWeb();
            
            SPList documentLibrary = oWeb.GetList(webUrl.OriginalString);

             if (documentLibrary.BaseType == SPBaseType.DocumentLibrary)
                 return documentLibrary;
             else
                 return null;
        }
   

      

       
        /// <summary>
        /// Retrieves the search criteria, builds the search query and binds the results griv with the search results
        /// </summary>
        /// <returns>DataTable  of the search results</returns>
        private DataTable Search()
        {
            
            //Get TextBoxes that have values
            Hashtable keywords = GetSearchCriteria();
            this.ViewState[SearchKeywordsVSKey] = keywords;
            if (keywords.Count == 0)
            {
                ShowMessage(LoadResource("NoSearchWords"));
             
                return null;
            }
            else
            {
                this.ViewState[SearchKeywordsVSKey] = keywords;
                DataTable resultsTable = Search(keywords);

                

                
                //Add the results table to the cache
                //this used to make sure that the grid renders content incase the post back is not from the web part it self
                //PartCacheInvalidate();
                //PartCacheWrite(Storage.Personal, ResultsCacheKey, resultsTable, TimeSpan.MaxValue);
                return resultsTable;




            }

            
        }

        private DataTable Search(Hashtable keywords)
        {
            SPList documentLibrary = GetDocumentLibraryFromUrl(SourceDocumentLibrary);
            DataTable resultsTable = null;
            if (keywords.Count > 0)
            {
                //Build Search Query
                SPQuery searchQuery = BuildSearchQuery(keywords, GetSelectedContentTypeFromViewState());
                //Get Search Data
                resultsTable = documentLibrary.GetItems(searchQuery).GetDataTable();
            }
            //Bind Data Grid to the results.
            GridView resultsGrid = (GridView)this.FindControl(ResultsGridID);
            resultsGrid.DataSource = null;
            TemplateField resultsField = new TemplateField();

            SearchResultItemSettings settings = new SearchResultItemSettings(keywords, LoadResource("ELearningActions"), documentLibrary, GetSelectedContentTypeFromViewState());
            resultsField.ItemTemplate = new SearchResultItemTemplate(settings);
            resultsField.ShowHeader = false;
            resultsGrid.Columns.Clear();
            resultsGrid.Columns.Add(resultsField);
            resultsGrid.Visible = true;
            resultsGrid.AllowPaging = true;
            resultsGrid.PagerSettings.Visible = true;
            resultsGrid.DataSource = resultsTable;
            resultsGrid.DataBind();
            return resultsTable;
        }
/// <summary>
/// Adjusts the search statistics label based on the pageindex and search results
/// </summary>
/// <param name="pageIndex">Page index of the results</param>
/// <param name="resultsTable">Search results table</param>
        private void AdjustSearchStatistics(int pageIndex, DataTable resultsTable)
        {
            GridView resultsGrid = (GridView)this.FindControl(ResultsGridID);
            resultsGrid.PageIndex = 0;
            Label statLabel = ((Label)this.FindControl(StatisticsLabelID));
            if (resultsTable.Rows.Count > 0)
            {
                statLabel.Text = CalculateResults(pageIndex,  resultsTable.Rows.Count);
                statLabel.Visible = true;
                statLabel.CssClass = Styles.Srch_Stats;
                resultsGrid.Visible = true;
                TableCell statisticscell = (TableCell)this.FindControl(StatisticsCellID);
                statisticscell.CssClass = Styles.Srch_Stats;
            }

            //else
            //{
            //    TableCell statisticscell = (TableCell)this.FindControl(StatisticsCellID);
            //    statisticscell.CssClass = Styles.MS_descriptiontext;
            //    statLabel.Visible = true;
            //    statLabel.Text = LoadResource("EmptySearchResults");
            //    resultsGrid.Visible = false;
            //}
        }

        /// <summary>
        /// Populates a list of content types that the user is allowed to search in.
        /// Excluded content types are removed from the list
        /// </summary>
        /// <param name="documentLibrary">Source Document Library</param>
        /// <returns>A list of content type objects that are allowed</returns>
        private List<SPContentType> GetAllowedContentTypes(SPList documentLibrary)
        {
            
            List<string> excluded = new List<string>();
            List<SPContentType> allowed = new List<SPContentType>();
            if (!String.IsNullOrEmpty(ExcludedContentTypes))
            {   

                excluded.AddRange(ExcludedContentTypes.Split(','));
            }
            //Add content types  that are not hidden and at least have one allowed field
            foreach (SPContentType ctype in documentLibrary.ContentTypes)
            { if (!ctype.Hidden)
                if (!excluded.Contains(ctype.Name))
                    if (GetAllowedFields(ctype).Count >0)
                        allowed.Add(ctype);
            }
            return allowed;
            
        }
        /// <summary>
        /// Retrieves a list of fields in a goven content type trimming the list in Excluded fields
        /// </summary>
        /// <param name="contentType">Content Type object to populates fields from</param>
        /// <returns>A list of SPField objects that are allowed</returns>
        private List<SPField> GetAllowedFields(SPContentType contentType)
        {
            List<string> excluded = new List<string>();
            List<SPField> allowed = new List<SPField>();
            if (!String.IsNullOrEmpty(ExcludedFields))
            {
                excluded.AddRange(ExcludedFields.Split(','));

            }
            foreach (SPField field in contentType.Fields )
            {
                if (!excluded.Contains(field.Title) && !field.Hidden)//&& (field.Type == SPFieldType.Text || field.Type == SPFieldType.Note)
                    allowed.Add(field);
            }
            return allowed;

        }
        
        /// <summary>
        /// Builds the CAML search query based on the search criteria and content type 
        /// </summary>
        /// <param name="keywords">Hashtable containing the search field names and the search keywords</param>
        /// <param name="selectedContentType">Selected content type to search</param>
        /// <returns>SPQuery object with query and view fields properties set with the correct CAML</returns>
        private  SPQuery BuildSearchQuery(Hashtable keywords,SPContentType selectedContentType)
        {
            SPQuery searchQuery = new SPQuery();
            
            
            
            QueryManager qryHelper = new QueryManager();
            // Add static fields
            //These felds are required for search resuls as they appear in sharePoint search results 
            qryHelper.AddField(new ViewOnlyField(Fields.IDField));
            qryHelper.AddField(new ViewOnlyField(Fields.TitleField));
            qryHelper.AddField(new ViewOnlyField(Fields.FileLeafRefField));
            qryHelper.AddField(new ViewOnlyField(Fields.CreatedField));
            qryHelper.AddField(new ViewOnlyField(Fields.Created_By));
            qryHelper.AddField(new ViewOnlyField(Fields.FileSizeDisplayField));
            qryHelper.AddField(new EqualsQueryField(Fields.ContentTypeField,selectedContentType.Name,false));
        

          
            //Add fields searched by the user
            System.Collections.IDictionaryEnumerator keywordsIterator = (System.Collections.IDictionaryEnumerator)keywords.Keys.GetEnumerator();

            while (keywordsIterator.MoveNext())
            {
                //For field types that are text and notes use Contains operator
                //For other fields types use Eqaul operator
                SPField field = selectedContentType.Fields.GetFieldByInternalName(keywordsIterator.Current.ToString());
                if (field.Type == SPFieldType.Text || field.Type == SPFieldType.Note || (field.Type == SPFieldType.File && field.InternalName == Fields.FileLeafRefField))
                    qryHelper.AddField(new ContainsQueryField(keywordsIterator.Current.ToString(), keywords[keywordsIterator.Current.ToString()].ToString(), true));
                else if (field.Type == SPFieldType.DateTime)
                {
                    DateTime tempdate = DateTime.Parse(keywords[keywordsIterator.Current.ToString()].ToString().Trim());
                    string dateSearchValue = Microsoft.SharePoint.Utilities.SPUtility.CreateISO8601DateTimeFromSystemDateTime(tempdate);
                    qryHelper.AddField(new EqualsQueryField(keywordsIterator.Current.ToString(), dateSearchValue, true));
                }
                else
                    qryHelper.AddField(new EqualsQueryField(keywordsIterator.Current.ToString(), keywords[keywordsIterator.Current.ToString()].ToString().Trim(), true));
            }

            qryHelper.Compile();

             searchQuery.ViewFields = qryHelper.ViewFields;
             searchQuery.Query = qryHelper.Query;

            return searchQuery;
        }
        /// <summary>
        /// Retrieves a hsatable of the search fields and seearch keywords
        /// </summary>
        /// <returns></returns>
        private Hashtable GetSearchCriteria()
        {
            Hashtable keywords = new Hashtable();

            //Find  the fields table  
            Table fieldsTable = (Table) this.FindControl(FieldsTableID);
            if (fieldsTable != null)
                GetSearchKeywordValues(keywords, fieldsTable);
            //the content type is spread among two tables
            else 
            {
                Table oddFields = (Table)this.FindControl(OddFieldsTableID);
                GetSearchKeywordValues(keywords, oddFields);

                Table evenFields = (Table)this.FindControl(EvenFieldsTableID);
                GetSearchKeywordValues(keywords, evenFields);

            }

           

            return keywords;
        }
        /// <summary>
        /// Searches in the control collection of the given table for textboxes with Id's that have the SearchPerfix
        /// in order to obtain their values 
        /// </summary>
        /// <param name="keywords">Hashtable that holds the keywords</param>
        /// <param name="ctypeSearchTable">Table that holds the textboxes</param>
        private static void GetSearchKeywordValues(Hashtable keywords, Table ctypeSearchTable)
        {
            foreach (TableRow searchRow in ctypeSearchTable.Rows)
                foreach (TableCell searchCell in searchRow.Cells)
                    foreach (Control searchFormControl in searchCell.Controls)
                    {
                        if (searchFormControl.ID != null)
                        {
                            //find text boxes that has values
                            if (searchFormControl.ID.StartsWith(SearchFieldPrefix))
                            {
                                TextBox keywordTextBox = (TextBox)searchFormControl;
                                if (!String.IsNullOrEmpty(keywordTextBox.Text.Trim()))
                                {
                                    string searchColumnInternalName = keywordTextBox.ID.Remove(0, SearchFieldPrefix.Length);
                                    keywords.Add(searchColumnInternalName, keywordTextBox.Text.Trim());
                                }
                            }
                        }

                    }
        }
        /// <summary>
        /// Builds the search header row containing the drop down list
        /// </summary>
        /// <param name="searchFormTable"></param>
        /// <param name="allowedContentTypes"></param>
        /// <returns></returns>
        private string BuildSearchFormHeader(Table searchFormTable, List<SPContentType> allowedContentTypes)
        {
            string ctypeID = String.Empty;
            //Holds the header cells that contain the drop down and its label
            TableRow headerRow = new TableRow();
            //Holds the label of the content types
            headerRow.EnableViewState = true;
            TableCell headerCell = new TableCell();
            headerCell.Wrap = false;
            headerCell.EnableViewState = true;
            headerCell.CssClass = Styles.MS_sbscopes;
            //Holds teh content types drop down or text box in case of one content type
            TableCell contentTypesCell = new TableCell();
            contentTypesCell.CssClass = Styles.MS_sbscopes;
            contentTypesCell.EnableViewState = true;
            //Holds the text of the content types
            Label headerLabel = new Label();
            headerLabel.CssClass = Styles.MS_sbscopes;


            if (allowedContentTypes.Count == 1)
            {
                headerLabel.Text = LoadResource("SingleContentTypeHeader");

                Label contentTypeLabel = new Label();
                contentTypeLabel.CssClass = Styles.MS_sbscopes;
                contentTypeLabel.Text = allowedContentTypes[0].Name;
                contentTypesCell.Controls.Add(contentTypeLabel);
            }
            else
            { //More than one content type exists
                // Mutiple content types are displayed in a list

                headerLabel.Text = LoadResource("MultipleContentTypesHeader");
                DropDownList contentTypeList = new DropDownList();
                contentTypeList.EnableViewState = true;
                contentTypeList.ID = ContentTypeListID;
                contentTypeList.CssClass = Styles.MS_sbscopes;
                contentTypeList.AutoPostBack = true;
                foreach (SPContentType contentType in allowedContentTypes)
                {
                    contentTypeList.Items.Add(new ListItem(contentType.Name, contentType.Id.ToString()));

                }



                if (this.ViewState[ContentTypeViewStateKey] != null)
                    contentTypeList.SelectedValue = this.ViewState[ContentTypeViewStateKey].ToString();
                contentTypeList.SelectedIndexChanged += new EventHandler(contentTypeList_SelectedIndexChanged);
                contentTypesCell.Controls.Add(contentTypeList);
            }

            //Add controls to the table structure
            headerCell.Controls.Add(headerLabel);
            headerRow.Cells.Add(headerCell);
            headerRow.Cells.Add(contentTypesCell);
            searchFormTable.Rows.Add(headerRow);

            return allowedContentTypes[0].Id.ToString();
        }

        /// <summary>
        /// Retrives the selected content type's ID 
        /// </summary>
        /// <returns>String of Content Type ID</returns>
        private string GetSelectedContentTypeID()
        
        {
            DropDownList contentTypeList = (DropDownList)this.FindControl(ContentTypeListID);
            return contentTypeList.SelectedValue.ToString();

        }
        /// <summary>
        /// Retrives the content type from the ID saved in view state
        /// </summary>
        /// <returns>Selected SPContentType object</returns>
        private SPContentType GetSelectedContentTypeFromViewState()
        {
            object contentTypeID = this.ViewState[ContentTypeViewStateKey];
            if (contentTypeID != null)
                return GetContentTypeByID(contentTypeID.ToString());
            else
                return null;
        }
        
        /// <summary>
        /// Adds the selcted content type ID to view state
        /// </summary>
        /// <param name="contentTypeID"></param>
        private void AddContentTypeIDToViewState(string contentTypeID)

        {
            this.ViewState[ContentTypeViewStateKey] = contentTypeID;
          
        }
        /// <summary>
        /// Builds the search form for the selected content type
        /// </summary>
        /// <param name="selectedContentType"></param>
        /// <returns></returns>
        private Table BuildSearchForm(SPContentType selectedContentType)
        {
            
            //Clear the panel controls
            Panel formPanel = (Panel)this.FindControl(FormPanelID);
            formPanel.Controls.Clear();
            Table searchTable =  BuildSearchFormForContentType(selectedContentType,formPanel);
            formPanel.Visible = true;
            //Show the Search button
            Button searchButton = (Button)this.FindControl(SearchButtonID);
            searchButton.Visible = true;
            searchButton.Enabled = false;
            return searchTable;
        }

      /// <summary>
      /// Adds the text boxes search fields depending on the selected content type
      /// </summary>
      /// <param name="contentType"></param>
      /// <param name="parent"></param>
      /// <returns></returns>
        private Table BuildSearchFormForContentType(SPContentType contentType, Control parent)
        {
            Table ctypeTable = new Table();
            parent.Controls.Add(ctypeTable);
            ctypeTable.ID = ContentTypeSearchTableID;


            
            List<SPField> allowedFields = GetAllowedFields(contentType);

            if (allowedFields.Count <= 5)
            { //Add the fields in one column
                TableRow singleRow = new TableRow();
                ctypeTable.Rows.Add(singleRow);
                TableCell singleCell = new TableCell();
                singleRow.Cells.Add(singleCell);
                                
                Table fieldsTable = new Table();
                fieldsTable.ID = FieldsTableID;
                singleCell.Controls.Add(fieldsTable);

                for (int i = 0; i < allowedFields.Count; i++)
                {

                    AddFieldToSearchForm( allowedFields[i], fieldsTable);
                   
                }
                

            }
            else
            {//Spread the fields among two columns
                //This row will hold two table which the content type fields are distributed upon 
                TableRow fieldsRow = new TableRow();
                ctypeTable.Rows.Add(fieldsRow);
                //Cells that holds the odd fields table
                TableCell oddTableCell = new TableCell();
                fieldsRow.Cells.Add(oddTableCell);
                //
                Table oddFieldsTable = new Table();
                oddFieldsTable.ID = OddFieldsTableID;
                oddTableCell.Controls.Add(oddFieldsTable);
                //Add separator cell

                TableCell separatorCell = new TableCell();
                fieldsRow.Cells.Add(separatorCell);
                Table separatorTable = new Table();
                separatorCell.Controls.Add(separatorTable);


               //Even Table settings
                 TableCell evenTableCell = new TableCell();
                 fieldsRow.Cells.Add(evenTableCell);

                Table evenFieldsTable = new Table();
                evenFieldsTable.ID = EvenFieldsTableID;
                evenTableCell.Controls.Add(evenFieldsTable);

                              

                for (int i = 0; i < allowedFields.Count; i++)
                {

                    if (i % 2 == 0) //odd row
                    {
                        AddFieldToSearchForm( allowedFields[i], oddFieldsTable);
                        AddSeparatorRow(separatorTable);
                    }
                    else // even row
                    {
                        AddFieldToSearchForm( allowedFields[i], evenFieldsTable);
                    }

                    
                        
                    


                }
                
            }

            
           
            
            


            return ctypeTable;

        }

        private  void AddSeparatorRow(Table separatorTable)
        {
            TableRow separatorRow = new TableRow();
            separatorTable.Rows.Add(separatorRow);
            separatorRow.Cells.Add(new TableCell());
        }

        private  void AddFieldToSearchForm( SPField field, Table fieldsTable)
        {
            TableRow fieldRow = new TableRow();
            fieldsTable.Rows.Add(fieldRow);
            TableCell fieldLabelCell = new TableCell();            
            //fieldLabelCell.CssClass =  Styles.MS_sbscopes+ " " +   Styles.MS_sbcell;
            fieldLabelCell.CssClass = Styles.MS_sbscopes;            
            TableCell fieldValueCell = new TableCell();

            fieldRow.Cells.Add(fieldLabelCell);
            fieldRow.Cells.Add(fieldValueCell);

            fieldValueCell.CssClass = Styles.MS_sbcell;            
            Label fieldLabel = new Label();
            fieldLabel.CssClass = Styles.MS_sbplain;
            fieldLabel.Text = field.Title;
            //Set up text box properties
            TextBox fieldValueTextBox = new TextBox();
            fieldValueTextBox.ID = SearchFieldPrefix + field.InternalName;
            fieldValueTextBox.CssClass = Styles.MS_sbplain;
            fieldValueTextBox.Attributes.Add("onkeyup", "Trim(this);EnableSearchButton();");
            fieldValueTextBox.Attributes.Add("onkeypress", "Trim(this);EnableSearchButton();");
            fieldLabelCell.Controls.Add(fieldLabel);
            fieldValueCell.Controls.Add(fieldValueTextBox);
            
            //Add Type validator
            AddTypeValidator(field, fieldValueCell, fieldValueTextBox);
        }

        private  void AddTypeValidator(SPField field, TableCell fieldValueCell, TextBox fieldValueTextBox)
        {
            ValidationDataType dataType = ValidationDataType.String;
            bool isSupportedFieldType = false;
            string dataTypeString = String.Empty;
            switch (field.Type)
            {

                case SPFieldType.Currency:
                    dataType = ValidationDataType.Currency;
                    isSupportedFieldType = true;
                    dataTypeString = LoadResource("Currency");
                    break;
                case SPFieldType.DateTime:
                    dataType = ValidationDataType.Date;
                    isSupportedFieldType = true;
                    dataTypeString = LoadResource("Date");
                    break;
                case SPFieldType.Number:
                    dataType = ValidationDataType.Double;
                    dataTypeString = LoadResource("Number");
                    isSupportedFieldType = true;
                    break;
                case SPFieldType.Integer:
                    isSupportedFieldType = true;
                    dataType = ValidationDataType.Integer;
                    dataTypeString = LoadResource("Integer");
                    break;
                case SPFieldType.Text:
                case SPFieldType.Note:
                    dataType = ValidationDataType.String;
                    dataTypeString = LoadResource("Text");
                    isSupportedFieldType = true;
                    break;
            }
            if (isSupportedFieldType)
            {
                CompareValidator comparer = new CompareValidator();
                comparer.ControlToValidate = fieldValueTextBox.ID;
                comparer.Operator = ValidationCompareOperator.DataTypeCheck;
                comparer.Type = dataType;
                comparer.Display = ValidatorDisplay.None;
                comparer.ValidationGroup = MetaSearchValidationGroup;
                comparer.EnableClientScript = true;
                comparer.SetFocusOnError = true;
                comparer.ErrorMessage = String.Format(LoadResource("TypeValidationError"), field.Title, dataTypeString);
                fieldValueCell.Controls.Add(comparer);
            }

        }
        private SPContentType GetContentTypeByID(string contentTypeID)
        {
            SPList documentLibrary = GetDocumentLibraryFromUrl(SourceDocumentLibrary);
            foreach (SPContentType contentType in documentLibrary.ContentTypes)
            {
                if (String.Equals(contentType.Id.ToString(), contentTypeID))
                    return contentType;
            }
            return null;
        }
        

        private bool AllPropertiesSet()
        {
            return !String.IsNullOrEmpty(SourceDocumentLibrary);
        }

      
        
        
    }
}
