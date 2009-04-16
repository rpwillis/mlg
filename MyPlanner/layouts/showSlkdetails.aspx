<%@ Page Language="C#" AutoEventWireup="true" CodeFile="showSlkdetails.aspx.cs" Inherits="showSlkdetails" %>

<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>
        <%=GetLocalResourceObject("Assignment").ToString() + ":" + assignmentTitle%>
    </title>
    <link href="/_layouts/1033/Styles/core.css" type="text/css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="/_layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.LCID%>/LgUtilities/styles/CustomStyles.css" />

    <script language="javascript" src="/_layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.LCID%>/LgUtilities/scripts/scripts.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <!-- Banner -->
        <table class="ms-main" height="100%" cellspacing="0" cellpadding="0" width="100%"
            border="0">
            <tbody>
                <tr>
                    <td class="calendar_titleareaframe" colspan="3">
                        <div>
                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                <tr>
                                    <td>
                                        <table style="padding-left: 2px; padding-top: 0px" cellspacing="0" cellpadding="0"
                                            border="0">
                                            <tr>
                                                <td nowrap align="center" width="108" height="46">
                                                    <img id="onetidtpweb1" alt="Icon" src="/_layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.LCID%>/LGUtilities/images/blank.gif">
                                                </td>
                                                <td>
                                                    <img height="1" alt="" src="/_layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.LCID%>/LGUtilities/images/blank.gif"
                                                        width="22"></td>
                                                <td nowrap width="100%">
                                                    <table cellspacing="0" cellpadding="0">
                                                        <tr>
                                                            <td class="ms-pagetitle" id="onetidPageTitle">
                                                                <%=GetLocalResourceObject("Assignment").ToString() + ":" + assignmentTitle%>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                            <tr>
                                                <td colspan="5" height="2">
                                                    <img height="1" alt="" src="/_layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.LCID%>/LgUtilities/images/blank.gif"
                                                        width="1"></td>
                                            </tr>
                                            <tr>
                                                <td class="ms-titlearealine" colspan="5" height="1">
                                                    <img height="1" alt="" src="/_layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.LCID%>/LgUtilities/images/blank.gif"
                                                        width="1"></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="ms-nav" valign="top" height="100%">
                        <table class="ms-navframe" height="100%" cellspacing="0" cellpadding="0" width="126"
                            border="0">
                            <tr>
                                <td valign="top" width="100%">
                                    &nbsp;</td>
                                <td class="ms-verticaldots" valign="top">
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </td>
                    <!-- Contents -->
                    <td>
                        <img height="1" alt="" src="/_layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.LCID%>/LgUtilities/images/blank.gif"
                            width="7"></td>
                    <td width="100%" height="100%">
                        <placeholder id="MSO_ContentDiv">
								<table cellSpacing="0" cellPadding="2">
									<tr>
										<td><IMG height="1" alt="" src="/_layouts/<%=System.Threading.Thread.CurrentThread.CurrentUICulture.LCID%>/LgUtilities/images/blank.gif" width="1"></td>
									</tr>
								</table>
								<table class="ms-bodyareaframe" cellSpacing="0" cellPadding="4" width="100%" border="0">
									<tr vAlign="top">
										<td width="100%">
											
    
        <asp:Table ID="Table1" runat="server" CssClass = "ms-formtable" CellPadding=0 CellSpacing=0 Width = "100%">
        <asp:TableRow><asp:TableCell runat=server CssClass="ms-formlabel" > <%= GetLocalResourceObject ("Title").ToString()+":" %></asp:TableCell><asp:TableCell ID="TableCell1" CssClass="ms-formbody"  runat=server><%=assignmentTitle%></asp:TableCell></asp:TableRow>
        <asp:TableRow><asp:TableCell ID="TableCell2" runat=server CssClass="ms-formlabel" ><%= GetLocalResourceObject("Description").ToString() + ":"%></asp:TableCell><asp:TableCell CssClass="ms-formbody"  ID="TableCell3" runat=server><%=assignmentDescription%></asp:TableCell></asp:TableRow>        
        <asp:TableRow><asp:TableCell ID="TableCell6" runat=server  CssClass="ms-formlabel" ><%= GetLocalResourceObject("StartDate").ToString() + ":"%></asp:TableCell><asp:TableCell CssClass="ms-formbody" ID="TableCell7" runat=server><%=assignmentStartDate.ToString()%></asp:TableCell></asp:TableRow>
                <asp:TableRow><asp:TableCell ID="TableCell4" runat=server   CssClass="ms-formlabel"><%= GetLocalResourceObject ("DueDate").ToString()+":" %></asp:TableCell><asp:TableCell CssClass="ms-formbody" ID="TableCell5" runat=server><%=assignmentDueDate.ToString()%></asp:TableCell></asp:TableRow>                              
        <asp:TableRow><asp:TableCell ID="TableCell8" runat=server  CssClass="ms-formlabel" ><%= GetLocalResourceObject("Status").ToString() + ":"%></asp:TableCell><asp:TableCell CssClass="ms-formbody" ID="TableCell9" runat=server><%=assignmentStatus%></asp:TableCell></asp:TableRow>
        <asp:TableRow><asp:TableCell ID="TableCell10" runat=server   CssClass="ms-formlabel"><%= GetLocalResourceObject("Instructor").ToString() + ":"%></asp:TableCell><asp:TableCell CssClass="ms-formbody" ID="TableCell11" runat=server><%=createdBy%></asp:TableCell></asp:TableRow>
        <asp:TableRow><asp:TableCell ID="TableCell12" runat=server  CssClass="ms-formlabel" ><%= GetLocalResourceObject("Class").ToString() + ":"%></asp:TableCell><asp:TableCell CssClass="ms-formbody" ID="TableCell13" runat=server><%=className%></asp:TableCell></asp:TableRow>
        </asp:Table>
         <TABLE cellSpacing=0 cellPadding=0 width="100%">
                                <TBODY>
                                <TR>
                                <TD class=ms-formline><IMG height=1 alt="" 
                                src="Form Templates - Calendar_files/blank.gif" 
                                width=1></TD></TR></TBODY></TABLE>
<input type="button" onclick ="Javascript:window.close();" value="Close" />
											<table cellSpacing="0" cellPadding="0" width="100%" border="0">
												<tr>
													<td>&nbsp;</td>
												</tr>
												<tr>
													<td>
														<table cellSpacing="0" cellPadding="0" width="100%" border="0" class=ms-formtoolbar>
															<tr>
																<td  height="1"><IMG height="1" alt="" src="/_layouts/images/blank.gif" width="1"></td>
															</tr>
															<tr>
																<td height="1"><IMG height="1" alt="" src="/_layouts/images/blank.gif" width="1"></td>
															</tr>
															
															
														</table>
													</td>
												</tr>
											</table>
											<!-- FooterBanner closes the TD, TR, TABLE, BODY, And HTML regions opened above -->
											&nbsp;
										</td>
									</tr>
								</table>
							</placeholder>
                    </td>
                </tr>
            </tbody>
        </table>
    </form>   
</body>
</html>
