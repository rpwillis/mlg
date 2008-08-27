

<%@ Page language="C#" MasterPageFile="~masterurl/default.master"    Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=12.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderPageTitle" runat="server"><SharePoint:ListProperty ID="ListProperty1" Property="Title" runat="server"/></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePoint:ListProperty ID="ListProperty2" Property="Title" runat="server"/>
</asp:Content>
<asp:content ID="Content3" contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
	<SharePoint:RssLink ID="RssLink1" runat="server" />
</asp:content>
<asp:Content ID="Content4" ContentPlaceHolderId="PlaceHolderSearchArea" runat="server">
	<SharePoint:DelegateControl ID="DelegateControl1" runat="server"
		ControlId="SmallSearchInputBox"/>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderId="PlaceHolderPageImage" runat="server"><SharePoint:ViewIcon Width="145" Height="54" runat="server" /></asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderId="PlaceHolderLeftActions" runat="server">
<SharePoint:ModifySettingsLink ID="ModifySettingsLink1" runat="server" />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderId ="PlaceHolderBodyLeftBorder" runat="server">
 <div height=100% class="ms-pagemargin"><IMG SRC="/_layouts/images/blank.gif" width=6 height=1 alt=""></div>
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderId="PlaceHolderMain" runat="server">
		<WebPartPages:WebPartZone runat="server" FrameType="None" ID="Main" Title="loc:Main" />
</asp:Content>
<asp:Content ID="Content9" ContentPlaceHolderId="PlaceHolderBodyAreaClass" runat="server">
<style type="text/css">
.ms-bodyareaframe {
	padding: 0px;
}
</style>
</asp:Content>
<asp:Content ID="Content10" ContentPlaceHolderId="PlaceHolderPageDescription" runat="server">
<SharePoint:ListProperty ID="ListProperty3" CssClass="ms-listdescription" Property="Description" runat="server"/>
</asp:Content>
<asp:Content ID="Content11" ContentPlaceHolderId="PlaceHolderCalendarNavigator" runat="server">
	  <SharePoint:SPCalendarNavigator id="CalendarNavigatorId" runat="server"/>
</asp:Content>

