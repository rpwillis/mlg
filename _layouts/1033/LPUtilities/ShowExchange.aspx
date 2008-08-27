<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShowExchange.aspx.cs" Inherits="ShowExchange" %>

<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>
        <%=GetLocalResourceObject("Event").ToString() + ":" + subject%>
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
                                                                <%=GetLocalResourceObject ("Event").ToString()+":"+subject%></td>
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
											
    
       <asp:Table ID="Table2" runat="server" ssClass = "ms-formtable" CellPadding=0 CellSpacing=0 Width = "100%">
                <asp:TableRow>
                    <asp:TableCell ID="TableCell15" runat="server" CssClass="ms-formlabel"><%= GetLocalResourceObject ("Subject").ToString()+":" %></asp:TableCell><asp:TableCell
                        ID="TableCell16" runat="server" CssClass="ms-formbody" ><%=subject%></asp:TableCell></asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ID="TableCell17" runat="server" CssClass="ms-formlabel"><%= GetLocalResourceObject ("StartDate").ToString()+":" %></asp:TableCell><asp:TableCell
                        ID="TableCell18" runat="server"  CssClass="ms-formbody" ><%=starDate.ToString ()%></asp:TableCell></asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ID="TableCell19" runat="server" CssClass="ms-formlabel"><%= GetLocalResourceObject ("EndDate").ToString()+":" %></asp:TableCell><asp:TableCell
                        ID="TableCell20" runat="server" CssClass="ms-formbody" ><%=endDate.ToString()%></asp:TableCell></asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ID="TableCell21" runat="server" CssClass="ms-formlabel"><%= GetLocalResourceObject ("Description").ToString()+":" %></asp:TableCell><asp:TableCell
                        ID="TableCell22" runat="server" CssClass="ms-formbody" ><%=description.ToString()%></asp:TableCell></asp:TableRow>                        
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
                        &nbsp;&nbsp; s&nbsp;
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        </table>                        
            </tbody>
        </table>                       
    </form>
</body>
</html>
