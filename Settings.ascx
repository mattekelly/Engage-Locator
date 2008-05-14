﻿<%@ Control Language="C#" AutoEventWireup="True" Inherits="Engage.Dnn.Locator.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/labelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="sectionhead" Src="~/controls/sectionheadcontrol.ascx" %>


<%-- <asp:UpdatePanel ID="upDisplaySettings" runat="server" > 
    <ContentTemplate>
    --%>
        <dnn:sectionhead ID="dshMapProvider" runat="Server" text="Locator Settings" CSSClass="Head" 
                section="tblMapProvider" resourcekey="dshMapProvider" IsExpanded="True" IncludeRule="true" />
            <table id="tblMapProvider" runat="server" cellspacing="0" cellpadding="0" style="padding-bottom: 20px;" border="0" summary="Module Mode">
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:Label ID="lblMapProviders" ResourceKey="lblMapProviders" runat="server" />
                    </td>
                    <td width="350">
    	                <asp:RadioButtonList ID="rblProviderType" CssClass="Normal" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblProviderType_SelectedIndexChanged" RepeatDirection="horizontal" Width="200px">
    	                </asp:RadioButtonList>
                        <div><asp:Label ID="lblApiMapProvider" CssClass="Normal" runat="server" resourceKey="lblApiMapProvider" /></div>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        &nbsp;
                    </td>
                    <td width="350">
                        <asp:CustomValidator ID="cvProviderType" runat="server" CssClass="Normal" ErrorMessage="Please select a map provider." OnServerValidate="cvProviderType_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:Label ID="lblAPIKey" ResourceKey="lblAPIKey" runat="server" />
                    </td>
                    <td width="350">                  
                        <asp:TextBox CssClass="Normal" ID="txtApiKey" runat="server" Columns="55" />
		                <div><asp:Label ID="lblApiInstructions" CssClass="Normal" runat="server" resourceKey="lblApiInstructions" /></div>
                        <asp:CustomValidator ID="CustomValidator1" runat="server" CssClass="Normal" ErrorMessage="Invalid API Key" OnServerValidate="apiKey_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:Label ID="lblLocatorCountry" runat="server" Visible="true" resourcekey="lblLocatorCountry" CssClass="Normal" />
                    </td>
                    <td width="350" class="Normal">   
                        <div><asp:DropDownList ID="ddlLocatorCountry" runat="server" CssClass="Normal" /></div>
                        <asp:CustomValidator ID="cvLocatorCountry" runat="server" ControlToValidate="ddlLocatorCountry" ErrorMessage="Please select a default country." OnServerValidate="cvLocatorCountry_OnServerValidate" />
                    </td>
                </tr>
            </table>

        <dnn:sectionhead ID="dshDisplaySetting" runat="Server" text="Display Settings" CSSClass="Head" 
            section="tblDisplaySettings" resourcekey="dshDisplaySetting" IsExpanded="true" IncludeRule="true" />   
            <table id="tblDisplaySettings" runat="server" cellspacing="0" cellpadding="0" style="padding-bottom: 20px;" border="0" summary="Module Mode">
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:label id="lblSearchTitle" resourcekey="lblSearchTitle" runat="server"  Text="Search Instructions:"  />
                    </td>
                    <td width="350">
                        <asp:TextBox CssClass="Normal" ID="txtSearchTitle" runat="server" Columns="55" />
                        <div><asp:Label CssClass="Normal" ID="lblSearchInst" runat="server" resourceKey="lblSearchInst" /></div><br />
                    </td>
                </tr>
                <tr>
                    <td class="SubHead">
                        <dnn:Label id="lblDefaultDisplay" runat="server" resourcekey="lblDefaultDisplay" />
                    </td>
                    <td width="350">
                        <asp:RadioButton ID="rbSearch" runat="server" CssClass="Normal" GroupName="rbDefaultDisplay" Text="Search" resourcekey="rbSearch" />
                        <asp:RadioButton ID="rbDisplayAll" runat="server" CssClass="Normal" GroupName="rbDefaultDisplay" Text="Show Locations" resourcekey="rbDisplayAll" />
                        <asp:RadioButton ID="rbShowMap" runat="server" CssClass="Normal" GroupName="rbDefaultDisplay" Text="Show Locations & Map" resourcekey="rbShowMap" />
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:Label ID="lblShowLocationDetails" runat="server" resourcekey="lblShowLocationDetails" />
                    </td>
                    <td width="350">
                                <asp:RadioButton ID="rbSamePage" runat="server" GroupName="rbLocationDetails" Text="Same Page" resourcekey="rbSamePage" CssClass="Normal" AutoPostBack="true" OnCheckedChanged="rbLoctionDetails_CheckChanged" />
                                <asp:RadioButton ID="rbDetailsPage" runat="server" GroupName="rbLocationDetails" Text="Details Page" resourcekey="rbDetailsPage" CssClass="Normal" AutoPostBack="true" OnCheckedChanged="rbLoctionDetails_CheckChanged" />
                                <asp:RadioButton ID="rbNoDetails" runat="server" GroupName="rbLocationDetails" Text="None" resourcekey="rbNoDetails" CssClass="Normal" AutoPostBack="true" OnCheckedChanged="rbLoctionDetails_CheckChanged" />
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td class="SubHead">
                        <dnn:Label ID="lblLocationComments" runat="server" Text="Allow Location Comments" resourcekey="lblLocationComments" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cbLocationComments" runat="server" Enabled="false" />
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
                                <tr>
                    <td class="SubHead">
                        <dnn:Label ID="lblModerateComments" runat="server" Text="Moderate Comments" resourcekey="lblModerateComments" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cbModerateComments" runat="server" Enabled="false" />
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:Label ID="lblMapType" runat="server" ResourceKey="lblMapType" />
                    </td>
                    <td width="350">
                        <asp:DropDownList ID="ddlMapType" runat="server" CssClass="Normal" >
                            <asp:ListItem Value="Normal" Text="Normal" Selected="true" />
                            <asp:ListItem Value="Satellite" Text="Satellite" />
                            <asp:ListItem Value="Hybrid" Text="Hybrid" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:Label ID="lblLocationType" resourcekey="lblLocationType" runat="server" Text="Location Type" />
                    </td>                    
                    <td width="350">
                        <div id="dvLocationType" runat="server" visible="true">
                            <asp:ListBox ID="lbLocationType" runat="server" SelectionMode="Multiple" Width="150px" CssClass="Normal" ></asp:ListBox>
                            <asp:LinkButton ID="lbEditLocationType" runat="server" OnClick="lbEditLocationType_Click" CssClass="CommandButton" resourcekey="lbEditLocationType">Edit</asp:LinkButton>
                            <asp:LinkButton ID="lbAddLocationType" runat="server" OnClick="lbAddLocationType_Click" CssClass="CommandButton" resourcekey="lbAddLocationType">Add</asp:LinkButton>
                            <asp:LinkButton ID="lbDeleteLocationType" runat="server" OnClick="lbDeleteLocationType_Click" CssClass="CommandButton" resourcekey="lblDeleteLocatinType">Delete</asp:LinkButton>
                        </div>
                        <div>
                            <asp:Label ID="lblLocationTypeNotInUse" runat="server" Visible="false" CssClass="Normal" resourcekey="lblLocationTypeNotInUse"></asp:Label>
                        </div>
                        <div id="dvLocationTypeEdit" runat="server" visible="false">
                            <asp:TextBox ID="txtEditLocationType" runat="server" Width="150px" CssClass="Normal"></asp:TextBox>
                            <asp:LinkButton ID="lbUpdateLocationType" runat="server" OnClick="lbUpdateLocationType_Click" CssClass="CommandButton">Update</asp:LinkButton>
                            <asp:LinkButton ID="lbCancelLocationType" runat="server" OnClick="lbCancelLocationType_Click" CssClass="CommandButton">Cancel</asp:LinkButton>
                        </div>
		                <div><asp:Label ID="lblLocationTypeInst" CssClass="Normal" runat="server" resourceKey="lblLocationTypeInst" /></div>
                    </td>                                               
                </tr>

            </table>
        <dnn:sectionhead ID="dshSearchSettings" runat="Server" text="Search Settings" CSSClass="Head" 
            section="tblSearchSettings" resourcekey="tblSearchSettings" IsExpanded="true" IncludeRule="true" />   
            <%-- <asp:UpdatePanel ID="upMiniSearchSettings" runat="server"> --%>
                <ContentTemplate>
                    <table id="tblSearchSettings" runat="server" cellspacing="0" cellpadding="0" style="padding-bottom: 20px;" border="0" summary="Module Mode">
                        <tr>
                            <td class="SubHead" width="150" valign="top">
                                <dnn:Label ID="lblLocatorModules" ResourceKey="lblLocatorModules" runat="server" />
                            </td>
                            <td width="350">
                                <asp:GridView ID="gvTabModules" runat="server" GridLines="vertical" AllowPaging="false" 
                                    AutoGenerateColumns="false" EnableViewState="true" Width="450px" >
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-Width="10px" ControlStyle-Width="10px" >
                                            <ItemTemplate>                                 
                                                <asp:RadioButton id="rbLocatorModule" runat="server" AutoPostBack="true" GroupName="rbLocatorModules" OnCheckedChanged="rbLocatorModules_CheckChanged" CssClass="Normal" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Page Title"  ControlStyle-Width="60px">
                                            <ItemTemplate>
                                                <asp:Label id="lblPageTitle" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Title", "{0:d}") %>' />
                                            </ItemTemplate>
                                            <ItemStyle CssClass="Normal" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="TabId" HeaderStyle-Width="50px" ControlStyle-Width="50px" >
                                            <ItemTemplate>
                                                <asp:Label id="lblTabId" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TabId", "{0:d}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle CssClass="Normal" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Module Title" ControlStyle-Width="150px" >
                                            <ItemTemplate>
                                                <asp:Label id="lblModuleTitle" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ModuleTitle", "{0:d}") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle CssClass="Normal" Width="200px" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                                    <AlternatingRowStyle BackColor="#eeeeee" />
                                    <RowStyle BackColor="#f8f8f8" ForeColor="Black" />
                                </asp:GridView>
                                <asp:CustomValidator ID="cvLocatorModules" runat="server" CssClass="Normal" ErrorMessage="Please select a results display module." OnServerValidate="cvLocatorModules_ServerValidate"></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="SubHead">
                                <dnn:Label ID="lblSearchOptions" runat="server" Text="Search Options" ResourceKey="lblSearchOptions" />
                            </td>
                            <td>
                                <table class="Normal">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkAddress" runat="server" Text="Address" resourcekey="chkAddress" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkCityRegion" runat="server" Text="City & Region" resourcekey="chkCityState" />
                                            <asp:CheckBox ID="chkPostalCode" runat="server" Text="Postal Code" resourcekey="chkPostalCode" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkCountry" runat="server" Text="Country" resourcekey="chkCountry" /></p>
                                        </td>
                                    </tr>
                                </table>
                                <asp:CustomValidator ID="cvSearchOptions" runat="server" CssClass="Normal" ErrorMessage="You must select at least one search options." OnServerValidate="cvSearchOptions_ServerValidate" />
                            </td>            
                        </tr>
                        <tr>
                            <td class="SubHead" width="150" valign="top">
                                <dnn:label id="lblOptional" resourcekey="lblOptional" runat="server" Text="Optional Settings" />
                            </td>
                            <td width="350">
                                <div id="dvAddress">
                                    <asp:RadioButtonList ID="rblRestrictions" runat="server" cssclass="Normal" RepeatDirection="horizontal" >
                                        <asp:ListItem Text="Country" Value="Country" />
                                        <asp:ListItem Text="Radius"  Value="Radius" />
                                        <asp:ListItem Text="None" Value="None" />
                                    </asp:RadioButtonList>
                                </div>
                                <div><asp:Label ID="lblOptionalInst" CssClass="Normal" runat="server" resourceKey="lblOptionalInst" /></div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            <%-- </asp:UpdatePanel> --%>
        <dnn:sectionhead ID="dshSubmissionSettings" runat="Server" text="Location Submission Settings" CSSClass="Head" 
            section="tblSubmissionSettings" resourcekey="dshSubmissionSettings" IsExpanded="true" IncludeRule="true" />   
            <table id="tblSubmissionSettings" runat="server" cellspacing="0" cellpadding="0" style="padding-bottom: 20px;" border="0" summary="Module Mode">
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:Label ID="lblAllowSubmissions" runat="server" ResourceKey="lblAllowSubmissions" />
                    </td>
                    <td width="350">
                        <asp:CheckBox ID="cbAllowSubmissions" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" width="150" valign="top">
                        <dnn:Label ID="lblSubmissionModeration" runat="server" ResourceKey="lblSubmissionModeration" />
                    </td>
                    <td width="350">
                        <asp:CheckBox ID="cbSubmissinModeration" runat="server" />
                    </td>
                </tr>
            </table>
            <%-- 
        </ContentTemplate>
</asp:UpdatePanel> --%>