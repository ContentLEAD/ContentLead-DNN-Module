<%@ Control Language="C#" AutoEventWireup="true" CodeFile="View2.ascx.cs" Inherits="DesktopModules_Brafton_View2" %>
<link href="<%= appPath %>/DesktopModules/Brafton/css/style.css" rel="stylesheet" type="text/css" />

<div id="braftonView">
    <h1>
        Brafton DotNetNuke Module</h1>
    <p class="IDs">
        Current Portal ID:
        <asp:Label ID="currentPortalID" runat="server" />
    </p>
    <p>
        ****The Brafton Module has to be installed on the same page as the DNN Blog Module
        in order to build the Permalinks****</p>
    <p class="IDs">
        Current Tab ID:
        <asp:Label ID="currentTabID" runat="server" />
    </p>
        <asp:UpdatePanel ID="updateAPIKey" runat="server" UpdateMode="Always" EnableViewState="true">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="setAPI" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="setBlogID" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Import" EventName="Click" />
        </Triggers>
        <ContentTemplate>
    <asp:Label CssClass="error" ID="labelError" runat="server" />
    <asp:Label CssClass="error" ID="labelError2" runat="server" />
    <asp:Label ID="labelTest" runat="server" />
    <p>
        Before articles can be imported from our feed all of these statements have to be
        <span class="boolTrue">True</span>.</p>
    <div id="checks">
        <p>
            1.) Is the DotNetNuke Blog Module Installed?
            <asp:Label ID="boolBlogModule" runat="server" /></p>
        <p>
            2.) Have you created a blog?
            <asp:Label ID="boolBlogCreated" runat="server" /></p>
        <p>
            3.) Are friendly URLs turned on?
            <asp:Label ID="boolFriendURL" runat="server" /></p>
        <p>
            4.) Have you set the Brafton API Key?
            <asp:Label ID="boolCheckAPI" runat="server" /></p>
        <p>
            5.) Have you set the specific blog that you want to import the articles into?
            <asp:Label ID="boolCheckBlogID" runat="server" /></p>
        <p>
            6.) Is the BraftonSchedule.dll in the DotNetNuke root Bin folder?
            <asp:Label ID="boolBin" runat="server" /></p>
			    <p>
            7.) Is the ImportImages.dll in the DotNetNuke root Bin folder?
            <asp:Label ID="boolBinImage" runat="server" /></p>
    </div>


            <asp:PlaceHolder ID="setAPIPH" runat="server" Visible="false">
                <h2>
                    API URL</h2>
                <p>
                    Enter your Brafton API key EXACTLY like this format: http://api.brafton.com/your
                    api key/news/</p>
                <p class="IDs">
                    Current:
                    <asp:Literal ID="apiURLLabel" runat="server" /></p>
                <p>
                    Set The API Key Here:
                    <br />
                    <asp:TextBox ID="apiURL" runat="server" Width="350px"></asp:TextBox></p>
                <asp:Button ID="setAPI" runat="server" Text="Set API" OnClick="setAPI_Click" />
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="nextStep" Visible="false">
                <h3>
                    Set the Blog that you want to import the articles to:</h3>
                <p class="IDs">
                    Current:
                    <asp:Literal ID="currentBlogID" runat="server" /></p>
                <p>
                    Blog Name:
                    <asp:DropDownList runat="server" ID="blogIdDrpDwn" ClientIDMode="Static" AutoPostBack="true" ViewStateMode="Enabled" />
                </p>
                <asp:Button ID="setBlogID" AutoPostBack="true" ClientIDMode="Static" ViewStateMode="Enabled"
                    runat="server" Text="Set Blog ID" OnClick="setBlogID_Click" />
            </asp:PlaceHolder>
            <br />
            <br />
            <asp:Button ID="Import" AutoPostBack="true" ClientIDMode="Static" ViewStateMode="Enabled"
                Visible="false" OnClick="Import_Click" runat="server" Text="Import Articles" />

           <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="false">
    <ProgressTemplate>
        <div id="updateBack">
     <img src="<%= appPath %>/DesktopModules/Brafton/images/ajax-loader.gif" alt="loading brafton articles" /> Loading ... 
</div>
    </ProgressTemplate>
</asp:UpdateProgress>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
