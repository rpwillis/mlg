
/*
 * Date : 07-03-2007
 * Check in by  :   Mohamed Yehia
 * Description  :   Added illegal character validation for the file name.  (Issue # 137061)
 *              :   Display user friendly message if the file is not a valid zip file (Issue # 137028)
 *              :   All access to the property values are made using the Property accessors.
 *              :   Added security check to make sure that the user have access to the document library
 *              :   Added check for maximum file allowed
 *              :   Removed file check for Title field
 *              :   Added support for relative paths
 *              :   Non Admin users are able to upload learning resources - Issue # 136766
 *   
 *
/*
* Date: 04-03-2007
* Checked In by:   Mohamed Yehia
* Description  :   Add the follwing functionality:
*              :   Checking for existing files in the document library and displaying an appropriate message ot the user accordingly. Issue # 137011
*              :   Uploading files and populateing the metadata from the XML file are now running in different threads  
*              :   Xml file paths property is now comma separated and the web part locates one of the files specified in the list 
*              :   If the Title column is not added, the default is the file name 
*              :   Added javascript to enable Upload button is enabled when there is a change in the file upload object
* Reviewed by  :   Marwan Tarek
*/
/*
 * Date: 01-03-2007
 * Checked In by:   Mohamed Yehia
 * Description  :   Developed main functionality:
 *              :   Files are uploaded to the document library
 *              :   Xml properties are parsed according to the field 
 *              :   Document Library item are up
 * Reviewed by: :   Marwan Tarek
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Security.Permissions;
using System.Threading;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.LearningComponents;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;



namespace MLG2007.WebParts.LRUpload
{
    [ToolboxData("<{0}:LRUpload runat=server></{0}:LRUpload>"),
       XmlRoot(Namespace = "LRUpload")]
    [Guid("284f3af4-b728-4464-8896-0da1c198ec79")]
    public class LRUpload : Microsoft.SharePoint.WebPartPages.WebPart 
    {

        #region Control IDs
        private const string UploadFileID = "UploadFileID";
        private const string CheckOverrideID = "CheckOverrideID";
        private const string ContentTypeListID = "ContentTypeList"; 
        #endregion
        #region Private Members
        /// <summary>
        /// The selected content type ID is saved in the view state using this key
        /// </summary>
    
        private const string ContentTypeViewStateKey = "ctypeviewstate";



        //Holds a comma separated list of fields that are not populated to the user
        string excludedFields = String.Empty;
        //Holds a comma separated list of content type titles that are not populated to the user
        string excludedContentTypes = String.Empty;
        //Holds the document library url
        string sourceDocumentLibrary = String.Empty;
        //Holds any error messages.
        Label errorLabel = null;
        private ResourceManager rm = new ResourceManager("MLG2007.WebParts.LRUpload.Strings", Assembly.GetExecutingAssembly()); 
        #endregion

        #region WebPart Properties


        string xmlFilePath = string.Empty;
       /// <summary>
       /// THe xml fil path relative to the root of the zip package
       /// </summary>
        [Browsable(true),
ResourcesAttribute("XmlFilePathTitle", "XmlFilePathCat", "XmlFilePathDesc"),
WebPartStorage(Storage.Shared)]
        public string XmlFilePath
        {
            get { return xmlFilePath; }
            set { xmlFilePath = value; }
        }


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


        

        #endregion



        #region Overrides
        protected override void CreateChildControls()
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
                    //valid document library
                    //Check for user permissions
                    SPList documentLibrary = GetDocumentLibraryFromUrl(SourceDocumentLibrary);

                    if (!documentLibrary.DoesUserHavePermissions(SPBasePermissions.AddListItems | SPBasePermissions.EditListItems))
                    {
                        ShowMessage(String.Format(LoadResource("NoPermissions"),documentLibrary.Title, documentLibrary.DefaultViewUrl));
                        return;
                    }
                    
                    //Get Content types
                    
                    List<SPContentType> allowedCtypes = GetAllowedContentTypes(documentLibrary);
                    if (allowedCtypes.Count == 0)
                    {//display error message
                        ShowMessage(LoadResource("NoContentTypesAttached"));
                    }
                    else
                    {
                        Table formTable = new Table();
                        Controls.Add(formTable);
                        //Build UI
                        BuildFormHeader(formTable, allowedCtypes);

                        BuildUploadRow(formTable);

                        BuildOverrideRow(formTable);

                       string subtmitButtonClientID= BuildSubmitRow(formTable);

                        string javascript = "function EnableUploadButton(fileUploadValue){var button = document.getElementById('"+ subtmitButtonClientID+"');if (fileUploadValue==null || fileUploadValue==\"\"){button.disabled=true;}else{button.disabled=false;}}";
                        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EnableUploadButton", javascript, true);




                    }

                }
            }
        }


        public override string LoadResource(string resourceId)
        {
            return rm.GetString(resourceId, CultureInfo.CurrentCulture);
        } 
        #endregion

       
        void submitButton_Click(object sender, EventArgs e)
        {
            byte[] contents =null;
            string fullDirPath=string.Empty;
            string fullFilePath =string.Empty;
            bool overWrite = false;
            string xmlMetadataFilePath = String.Empty;
            string destinationUrl = String.Empty;
            Hashtable metadata = null;
            bool shouldReturn = false;
            SPListItem fileItem = null;
            

            System.Security.Principal.WindowsIdentity currentIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();

            try
            {

                if (IsValidFile())
                {
                    FileUpload uploadControl = (FileUpload)this.FindControl(UploadFileID);
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                       {

                           //DownloadFile
                           string parentPath = Path.Combine(Path.GetTempPath(), "LRUpload");
//                           string parentPath = Path.Combine(Environment.CurrentDirectory, "LRUpload");
                           //The directory that will hold the zip file and the extracted files
                           //The directory is named by the filename
                           fullDirPath = Path.Combine(parentPath, Path.GetFileNameWithoutExtension(uploadControl.FileName));
                           //the full zip file path
                           fullFilePath = Path.Combine(fullDirPath, Path.GetFileName(uploadControl.FileName));


                           //Get requried permissions for the temp directory
                           FileIOPermission perm = new FileIOPermission(FileIOPermissionAccess.AllAccess, parentPath);
                           perm.Assert();

                           //Create the directory and save the file
                           Directory.CreateDirectory(fullDirPath);
                           uploadControl.PostedFile.SaveAs(fullFilePath);



                           //Unzip
                           try
                           {
                               Compression.Unzip(new FileInfo(fullFilePath), new DirectoryInfo(fullDirPath));
                           }
                           catch (CompressionException)
                           {

                               ShowMessage(LoadResource("InvalidFile"));
                               shouldReturn = true;
                               return;
                           }
                           //Look for the file 

                           try
                           {
                               xmlMetadataFilePath = GetXmlMetadataFile(fullDirPath);
                           }

                           catch (FileNotFoundException)
                           {
                               Directory.Delete(fullDirPath, true);
                               ShowMessage(LoadResource("InvalidPackage"));
                               shouldReturn = true;
                               return;
                           }

                          
                           destinationUrl = Path.GetFileName(fullFilePath);
                           //destinationUrl = SourceDocumentLibrary.TrimEnd('/') + "/" + Path.GetFileName(fullFilePath);

                           overWrite = ((CheckBox)this.FindControl(CheckOverrideID)).Checked;
                           //Check if existing files exist
                           if (!overWrite)
                           {
                               if (FileExistsInDocumentLibrary(destinationUrl))
                               {
                                   Directory.Delete(fullDirPath, true);
                                   ShowMessage(LoadResource("FileAlreadyExists"));
                                   shouldReturn = true;
                                   return;
                               }

                           }
                           //Read the file contents
                           contents = ReadFileContents(fullFilePath);
                          //Populate file metadata
                           metadata =  PopulateMetadata(GetContentTypeByID(GetSelectedContentType()), xmlMetadataFilePath);
                         
                       });

                    if (!shouldReturn)
                    {
                        //Re-impersonate the current user
                        currentIdentity.Impersonate();
                        fileItem = UploadFile(contents, destinationUrl, overWrite).Item;
                        UpdateDocLibItem(fileItem, metadata);
                    }
                    else
                        return;

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                  {
                      Directory.Delete(fullDirPath, true);
                      
                  });

                    ShowMessage(LoadResource("FileUploaded"));
                }


            }
            catch (Exception ex)
            {

                SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                if (Directory.Exists(fullDirPath))
                    Directory.Delete(fullDirPath, true);
            });
                ShowError(ex);

            }

        }
        #region Private Methods

        private string BuildSubmitRow(Table formTable)
        {
            //Build Submit Row

            TableRow submitRow = new TableRow();
            formTable.Rows.Add(submitRow);
            TableCell submitCell = new TableCell();
            submitCell.ColumnSpan = 1;
            submitRow.Cells.Add(submitCell);
            Button submitButton = new Button();
            submitButton.Text = LoadResource("Upload");
            submitButton.Enabled = false;
            submitButton.Click += new EventHandler(submitButton_Click);
            submitButton.CssClass = Styles.MS_ButtonHeightWidth;
            submitCell.Controls.Add(submitButton);
            return submitButton.ClientID;
        }

        private void BuildOverrideRow(Table formTable)
        {
            //Build Override Row
            TableRow overrideRow = new TableRow();
            formTable.Rows.Add(overrideRow);
            TableCell overrideCell = new TableCell();
            overrideCell.ColumnSpan = 2;
            overrideCell.CssClass = Styles.MS_sbscopes;
            overrideRow.Cells.Add(overrideCell);
            CheckBox chkOverride = new CheckBox();
            chkOverride.Text = LoadResource("OverrideExistingFiles");
            chkOverride.ID = CheckOverrideID;
            overrideCell.Controls.Add(chkOverride);
        }

        private void BuildUploadRow(Table formTable)
        {
            //Build Upload Row
            TableRow uploadRow = new TableRow();
            formTable.Rows.Add(uploadRow);
            TableCell uploadCell = new TableCell();
            uploadCell.ColumnSpan = 2;
            uploadCell.Width = new Unit(100, UnitType.Percentage);
            uploadRow.Cells.Add(uploadCell);
            FileUpload uploadFile = new FileUpload();
            uploadFile.ID = UploadFileID;
            uploadFile.CssClass = Styles.MS_fileinput;
            uploadFile.Width = new Unit(100, UnitType.Percentage);
            uploadFile.Attributes.Add("onchange", "EnableUploadButton(this.value)");
            uploadCell.Controls.Add(uploadFile);
        }


        /// <summary>
        /// Checks of the file size being uploaded is more than the size allowed in the web application
        /// </summary>
        /// <param name="bytes">Bytes length of the file being uploaded</param>
        /// <returns>True if the bytes are larger than the mximum file size limit, false otherwise</returns>
        private bool FileMaximumLimitReached(int bytes)
        {
            int maxMegaBytes = GetDocumentLibraryFromUrl(SourceDocumentLibrary).ParentWeb.Site.WebApplication.MaximumFileSize;
            int fileSize = (bytes / 1024) / 1024;
            return (fileSize > maxMegaBytes);

        }

        private bool IsValidFile()
        {
            FileUpload uploadControl = (FileUpload)this.FindControl(UploadFileID);
            if (uploadControl.PostedFile == null || uploadControl.PostedFile.ContentLength <= 0)
            {
                ShowMessage(LoadResource("InvalidFile"));
                return false;
            }
            if (SPUrlUtility.IndexOfIllegalCharInUrlLeafName(Path.GetFileName(uploadControl.FileName)) > -1)
            {
                ShowMessage(LoadResource("InvalidCharsInFile"));
                return false;
            }
            if (FileMaximumLimitReached(uploadControl.PostedFile.ContentLength))
            {
                ShowMessage(LoadResource("MaximumFileSizeReached"));
                return false;
            }
            return true;
        }

     
        /// <summary>
        /// Checks whether the specified files is available in the doucment library
        /// </summary>
        /// <param name="destinationUrl">The url of the files in the doucment library</param>
        /// <returns>True if the file exists, false otherwise</returns>
        /// 
        private bool FileExistsInDocumentLibrary(string destinationUrl)
        {
            try
            {
                SPList docLib = GetDocumentLibraryFromUrl(SourceDocumentLibrary);

                SPFile file = docLib.RootFolder.Files[destinationUrl];
                if (file != null)
                    return file.Exists;
                else
                    return false;

            }
            catch (ArgumentException)
            {
                return false;
            }

        }

        /// <summary>
        /// Finds the xml metadata file of the zip file in the extreaction ndirectory.  The file name could be one of the 
        /// supplied list of xml files.  Throws a FileNotFoundException if the file is not found. 
        /// </summary>
        /// <param name="fileParentDirectory">The extraction directory to look for the file in</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns>The full path of the xml metadata file</returns>
        private string GetXmlMetadataFile(string fileParentDirectory)
        {
            if (!String.IsNullOrEmpty(XmlFilePath))
            {

                string[] filePaths = XmlFilePath.Split(',');
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string fileFullPath = Path.Combine(fileParentDirectory, filePaths[i].Trim());
                    if (File.Exists(fileFullPath))
                        return fileFullPath;

                }
                throw new FileNotFoundException();


            }
            throw new FileNotFoundException();
        }

        /// <summary>
        /// Returns the selected content type ID from the drop down list control (in case of mutiple content types)or from the view state object (in case of a single content type) 
        /// </summary>
        /// <returns>Selected content type ID</returns>
        private string GetSelectedContentType()
        {
            if (this.ViewState[ContentTypeViewStateKey] != null)
                return this.ViewState[ContentTypeViewStateKey].ToString();
            else
                return ((DropDownList)this.FindControl(ContentTypeListID)).SelectedValue;
        }

        /// <summary>
        /// Updates the added SPList item with the metadata carrying the item information
        /// </summary>
        /// <param name="fileItem">List item assosiated with the file item uploaded</param>
        /// <param name="metadata">Table of metadata associated with the file uploaded</param>
        private void UpdateDocLibItem(SPListItem fileItem, Hashtable metadata)
        {

            foreach (DictionaryEntry property in metadata)
            {
                try
                {
                    fileItem[property.Key.ToString()] = property.Value;
                    //The item is updated with every value to bypass any exceptions that might occur due to bad values
                    fileItem.Update();
                }
                catch (Exception)
                {
                    //Do nothing as any error might occur due to wrong formatting of the values 

                }
                
            }
            

        }

        /// <summary>
        /// Retrevies a hashtable of the metdata according to the XPath queries in the description fileds of the content types columns
        /// </summary>
        /// <param name="ctype">Content type selected</param>
        /// <param name="fullXmlFilePath">Full path of the XML metadata file</param>
        /// <returns>Hashtable of the internal fields and the mapped values in the xml metadata file</returns>
        private Hashtable PopulateMetadata(SPContentType ctype, string fullXmlFilePath)
        {
            Hashtable properties = new Hashtable();

            XmlDocument xdocument = new XmlDocument();
            xdocument.Load(fullXmlFilePath);
            //Create a namespaces manger for loading all the xml namespaces available in the xml document
            XmlNamespaceManager manager = FillNamespaceManager(xdocument);

            XPathNavigator nav = xdocument.CreateNavigator();

            foreach (SPField field in ctype.Fields)
            {

                try
                {
                    properties.Add(field.Title, ReadMappedPropertyValueFromXml(nav, field.Description, manager));
                }
                catch (Exception)
                {
                    //Remove the field item from the hashtable
                    if (properties.ContainsKey(field.Title))
                        properties.Remove(field.Title);
                }


            }
            
                

            //Remove the system properties that might get populated
            properties.Remove(Fields.SelectFileName);
            properties.Remove(Fields.Modified);
            properties.Remove(Fields.Modified_By);
            properties.Remove(Fields.Created_By);
            properties.Remove(Fields.Created);
            properties.Remove(Fields.FileLeafRef);
            properties[Fields.ContentType] = ctype.Name;
            
            


            return properties;
        }

        /// <summary>
        /// Retrieves the cotent type object eith the specified ID
        /// </summary>
        /// <param name="contentTypeID">The ID  of the content type</param>
        /// <returns>content type object if found, null otherwise</returns>
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
        /// <summary>
        /// Uploads the file in the specified path to the destination url in the root folder of the doucment library
        /// </summary>
        /// <param name="sourceFilePath">Full source file path</param>
        /// <param name="destinationUrl">Full destination url of the file</param>
        /// <param name="overWrite">True to override the file it exists, false otherwise</param>
        /// <returns>SPFile object uploaded</returns>
        private SPFile UploadFile(byte[] fileContents, string destinationUrl, bool overWrite)
        {

            SPList docLib = GetDocumentLibraryFromUrl(SourceDocumentLibrary, this.Page.User.Identity.Name);
           // SPUser user = docLib.ParentWeb.SiteUsers[this.Page.User.Identity.Name];
            //SPFile fileUploaded = docLib.RootFolder.Files.Add(destinationUrl, fileContents, user, user, DateTime.Now, DateTime.Now); 
           SPFile fileUploaded = docLib.RootFolder.Files.Add(destinationUrl, fileContents, overWrite);
            return fileUploaded;
        }

        /// <summary>
        /// Reads the specified file into a byte array.
        /// </summary>
        /// <param name="sourceFilePath">The full opath of the file to read</param>
        /// <returns>Byte array of the file contents</returns>
        private static byte[] ReadFileContents(string sourceFilePath)
        {
            FileStream fs = new FileStream(sourceFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            byte[] contents = new byte[fs.Length];
            fs.Read(contents, 0, (int)fs.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            return contents;
        }

        /// <summary>
        /// Builds the upload header row containing the drop down list
        /// </summary>
        /// <param name="searchFormTable"></param>
        /// <param name="allowedContentTypes"></param>
        /// <returns></returns>
        private void BuildFormHeader(Table formHeader, List<SPContentType> allowedContentTypes)
        {


            //Holds the header cells that contain the drop down and its label
            TableRow headerRow = new TableRow();
            //Holds the label of the content types
            TableCell headerCell = new TableCell();
            headerCell.CssClass = Styles.MS_sbscopes;
            //Holds teh content types drop down or text box in case of one content type
            TableCell contentTypesCell = new TableCell();
            contentTypesCell.CssClass = Styles.MS_sbscopes;
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
                this.ViewState[ContentTypeViewStateKey] = allowedContentTypes[0].Id.ToString();
            }
            else
            { //More than one content type exists
                // Mutiple content types are displayed in a list

                this.ViewState[ContentTypeViewStateKey] = null;
                headerLabel.Text = LoadResource("MultipleContentTypesHeader");
                DropDownList contentTypeList = new DropDownList();
                contentTypeList.ID = ContentTypeListID;
                contentTypeList.CssClass = Styles.MS_sbscopes;
                contentTypeList.AutoPostBack = false;
                foreach (SPContentType contentType in allowedContentTypes)
                {
                    contentTypeList.Items.Add(new ListItem(contentType.Name, contentType.Id.ToString()));

                }




                contentTypesCell.Controls.Add(contentTypeList);
            }

            //Add controls to the table structure
            headerCell.Controls.Add(headerLabel);
            headerRow.Cells.Add(headerCell);
            headerRow.Cells.Add(contentTypesCell);
            formHeader.Rows.Add(headerRow);

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
            if (!this.Controls.Contains(errorLabel))
                this.Controls.Add(errorLabel);


        }
        /// <summary>
        /// Shows messages in the error label to the user
        /// </summary>
        /// <param name="message"></param>
        private void ShowMessage(string message)
        {
            AddErrorLabel();

            errorLabel.Text = message;
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
        /// Obtains a document library object from the given URL
        /// </summary>
        /// <param name="documentLibraryUrl">The doucment library URL</param>
        /// <returns>SPList object if the URL is correct, otherwise null</returns>
        private SPList GetDocumentLibraryFromUrl(string documentLibraryUrl)
        {
            return GetDocumentLibraryFromUrl(documentLibraryUrl, null);
        }

        private SPList GetDocumentLibraryFromUrl(string documentLibraryUrl,string username )
        {
            SPWeb oWeb=null;

            Uri webUrl = new Uri(new Uri(Context.Request.Url.ToString()), documentLibraryUrl);
            oWeb = new SPSite(webUrl.OriginalString).OpenWeb();
            if (!String.IsNullOrEmpty(username))
            {
                SPUserToken userToken = oWeb.SiteUsers[username].UserToken;
                oWeb.Dispose();
                SPSite site = new SPSite(webUrl.OriginalString, userToken);
                oWeb = site.OpenWeb();
            }
            SPList documentLibrary = oWeb.GetList(webUrl.OriginalString);
            if (documentLibrary.BaseType == SPBaseType.DocumentLibrary)
                return documentLibrary;
            else
                return null;
 
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
            {
                if (!ctype.Hidden && (!excluded.Contains(ctype.Name)))
                    allowed.Add(ctype);
            }
            return allowed;

        }
        /// <summary>
        /// Verifies that all required properties are set. 
        /// </summary>
        /// <returns>True if all required properties are set, false otherwise</returns>
        private bool AllPropertiesSet()
        {
            return !String.IsNullOrEmpty(SourceDocumentLibrary) && !String.IsNullOrEmpty(XmlFilePath);
        }


        private string ReadMappedPropertyValueFromXml(XPathNavigator nav, string expression, XmlNamespaceManager manager)
        {
            try
            {
                XPathExpression xpathExpression = nav.Compile(expression);
                if (manager !=null)
                    xpathExpression.SetContext(manager);
                
                nav.MoveToRoot();
                XPathNodeIterator nodeIterator = nav.Select(xpathExpression);
                System.Text.StringBuilder sb = new StringBuilder();
                while (nodeIterator.MoveNext())
                {
                    sb.Append(nodeIterator.Current.Value);
                    sb.Append(",");


                }
                return sb.ToString().TrimEnd(',');
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns an XmlNamespaceManager with the namespaces found in the xml document
        /// </summary>
        /// <param name="xdocument">Xml document loaded with the xml file</param>
        /// <returns>XmlNamespaceManager object if there are namespaces, null otherwise</returns>
        private XmlNamespaceManager FillNamespaceManager(XmlDocument xdocument)
        {
            XmlNamespaceManager manager = null;
            try
            {


                System.Collections.ArrayList namespaces = new System.Collections.ArrayList();
                //Find the namespaces in the xml document
                FillNamespace(xdocument.DocumentElement, namespaces);
                if (namespaces.Count > 0)
                {
                    //Add the namespaces iwth the correct format to the namespace table
                    manager = new XmlNamespaceManager(xdocument.NameTable);
                    for (int i = 0; i < namespaces.Count; i++)
                    {
                        string[,] textArray = (string[,])namespaces[i];
                        manager.AddNamespace(textArray[0, 0], textArray[0, 1]);
                    }
                }
            }
            catch
            {
                throw;
            }
            return manager;
        }

        /// <summary>
        /// Finds the namespaces in the xml document and fills the given list container
        /// </summary>
        /// <param name="node">the xml node to start parsing, typically the DocumentElement </param>
        /// <param name="namespaces">Array list that stores the namespaces found</param>
        private void FillNamespace(XmlNode node, System.Collections.ArrayList namespaces)
        {
            try
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (!attribute.Name.StartsWith("xmlns"))
                        {
                            continue;
                        }
                        string[,] textArray = new string[1, 2];
                        if (attribute.Name.Equals("xmlns"))
                        {
                            textArray[0, 0] = "def";
                            textArray[0, 1] = attribute.Value;
                            namespaces.Add(textArray);
                            continue;
                        }
                        textArray[0, 0] = attribute.Name.Remove(0, 6);
                        textArray[0, 1] = attribute.Value;
                        namespaces.Add(textArray);
                    }
                }
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    FillNamespace(node2, namespaces);
                }
            }
            catch
            {
                throw;
            }
        }



        #endregion

    }
}
