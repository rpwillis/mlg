﻿<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=12.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c"
    meta:progid="SharePoint.WebPartPage.Document" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:EncodedLiteral runat="server" Text="<%$Resources:wss,multipages_homelink_text%>"
        EncodeMethod="HtmlEncode" />
    -
    <SharePoint:ProjectProperty Property="Title" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageImage" runat="server">
    <img src="/_layouts/images/blank.gif" width="1" height="1" alt=""></asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <label class="ms-hidden">
        <SharePoint:ProjectProperty Property="Title" runat="server" />
    </label>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderTitleBreadcrumb" runat="server" />
<asp:Content ContentPlaceHolderID="PlaceHolderTitleAreaClass" runat="server">
    <style type="text/css">
TD.ms-titleareaframe, .ms-pagetitleareaframe {
	height: 10px;
}
Div.ms-titleareaframe {
	height: 100%;
}
.ms-pagetitleareaframe table {
	background: none;
	height: 10px;
}
</style>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <meta name="CollaborationServer" content="SharePoint Team Web Site">

    <script type="text/javascript">
	var navBarHelpOverrideKey = "wssmain";
    </script>

</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderLeftActions" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageDescription" runat="server" />
<asp:Content ContentPlaceHolderID="PlaceHolderBodyAreaClass" runat="server">
    <style type="text/css">
.ms-bodyareaframe {
	padding: 0px;
}
</style>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table cellspacing="0" border="0" width="100%">
        <tr>
            <td class="ms-pagebreadcrumb">
                <asp:SiteMapPath SiteMapProvider="SPContentMapProvider" ID="ContentMap" SkipLinkText=""
                    NodeStyle-CssClass="ms-sitemapdirectional" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="ms-webpartpagedescription">
                <SharePoint:ProjectProperty Property="Description" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" cellpadding="0" cellspacing="0" style="padding: 5px 10px 10px 10px;">
                    <tr>
                        <td valign="top" width="100%">
                            <WebPartPages:WebPartZone runat="server" FrameType="TitleBarOnly" ID="Left" Title="loc:Left">
                                <ZoneTemplate>
                                </ZoneTemplate>
                            </WebPartPages:WebPartZone>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>