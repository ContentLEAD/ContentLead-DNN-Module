<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Settings.ascx.cs" Inherits="DesktopModules_Brafton_Settings" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/labelControl.ascx" %>
        
<div style="display:none;"><table cellspacing="0" cellpadding="2" border="0">
    <tr>
        <td width="150"><dnn:Label ID="ShowDescriptionLabel" runat="server" ControlName="ShowDescription" Suffix=":"></dnn:Label></td>
        <td><asp:CheckBox id="ShowDescription" runat="server" /></td>
    </tr>
</table></div>


<br />



