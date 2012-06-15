using System;
using System.Collections;
using System.Configuration;

using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Web.Hosting;
using System.Diagnostics;

using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using DotNetNuke;
using DotNetNuke.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Scheduling;

using System.Reflection;
using System.Security;
using System.Security.Permissions;

public partial class DesktopModules_Brafton_View2 : DotNetNuke.Entities.Modules.PortalModuleBase
{
    //Connection properties
    public SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());
    public SqlCommand cmd = new SqlCommand();

    //Application path variable
    public string appPath;

    //DNN Variables
    int intPortalID;
    int PageTabId;
    string checkDomain;

    //Local Variables
    public int checkBlogModule;
    public string checkFriendURLS;
    public int checkBlogCreated;
    public int checkNewsAPI;
    public string checkPath;
    public int checkBlogID;
	public string checkImagePath;

    public string strip(string alias)
    {
        // invalid chars, make into spaces
        alias = Regex.Replace(alias, @"[^a-zA-Z0-9\s-]", "");
        // convert multiple spaces/hyphens into one space       
        alias = Regex.Replace(alias, @"[\s-]+", " ").Trim();
        // hyphens
        alias = Regex.Replace(alias, @"\s", "-");

        return alias;
    }

    public void checks()
    {


        cmd.CommandText = "IF OBJECT_ID('Blog_Settings') IS NOT NULL(SELECT Value FROM Blog_Settings WHERE [Key] ='ShowSeoFriendlyUrl') else select 'False'";
        checkFriendURLS = ((string)cmd.ExecuteScalar());

        cmd.CommandText = "IF OBJECT_ID('Blog_Blogs') IS NOT NULL(SELECT COUNT(*) FROM Blog_Blogs) Else Select 0";
        checkBlogCreated = (int)cmd.ExecuteScalar();

        cmd.CommandText = "IF (SELECT Title FROM Brafton WHERE content='1') IS NOT NULL BEGIN SELECT 1 END ELSE SELECT 0";
        checkNewsAPI = (int)cmd.ExecuteScalar();

        checkPath = HostingEnvironment.ApplicationPhysicalPath + "Bin\\BraftonSchedule.dll";
		
		checkImagePath = HostingEnvironment.ApplicationPhysicalPath + "Bin\\ImportImages.dll";

        cmd.CommandText = "SELECT count(*) as total_record  FROM DesktopModules WHERE FriendlyName = 'blog'";
        checkBlogModule = ((int)cmd.ExecuteScalar());

        cmd.CommandText = "SELECT count(*) as total_record FROM Brafton WHERE ID >= 1";
        checkBlogID = ((int)cmd.ExecuteScalar());

    }

    /*Shared hosting environments do not allow for files to be moved or deleted. This is here because a try catch statement does not work with Security Exceptions unless it is in a different method.
    void testPermissions()
    {
        //Path used to move the BraftonSchedule.dll
        string movePath = HostingEnvironment.ApplicationPhysicalPath + "DesktopModules\\Brafton\\BraftonSchedule.dll";

        if (File.Exists(checkPath))
        {

            if (File.Exists(movePath))
            {

                FileVersionInfo originalDLL = FileVersionInfo.GetVersionInfo(checkPath);
                FileVersionInfo newDLL = FileVersionInfo.GetVersionInfo(movePath);

                switch (originalDLL.FileVersion.CompareTo(newDLL.FileVersion))
                {
                    case 0:
                        //If they are the same do nothing                       
                        break;
                    case 1:
                        //If the installed version is older do nothing                      
                        break;
                    case -1:
                        //If the  installed version is newer replace the old BraftonSchedule.dll in the DNN bin folder
                        File.Delete(checkPath);
                        File.Move(movePath, checkPath);
                        break;
                }
            }

            boolBin.Text = "True";
            boolBin.CssClass = "boolTrue";

        }

        else
        {
            try
            {
                File.Move(movePath, checkPath);
                boolBin.Text = "True";
                boolBin.CssClass = "boolTrue";
            }
            catch
            {
                boolBin.Text = "False";
                boolBin.CssClass = "boolFalse";
            }
        }
    }
	*/


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //Get current directory for style sheets and images
            appPath = HttpRuntime.AppDomainAppVirtualPath == "/" ? appPath = "" : appPath = HttpRuntime.AppDomainAppVirtualPath;
           
            connection.Open();
            cmd.Connection = connection;

            checks();

            //Showing your current portal ID
            intPortalID = PortalSettings.PortalId;
            currentPortalID.Text = intPortalID.ToString();

            //Showing your current TabID
            PageTabId = PortalSettings.ActiveTab.TabID;
            currentTabID.Text = PortalSettings.ActiveTab.TabID.ToString();

            //Get your current domain to insert into database for error checking
            checkDomain = HttpContext.Current.Request.Url.Host;
			
			//Sets the current PortalID in the Brafton Table
            cmd.CommandText = "IF Exists (SELECT * FROM Brafton WHERE content='1') UPDATE Brafton SET Category = " + intPortalID + " WHERE Content = '1' else INSERT INTO Brafton (Content, Category) VALUES (1, '" + intPortalID + "')";
            cmd.ExecuteNonQuery();
			
			//Sets the current TabID in the Brafton Table
            cmd.CommandText = "IF Exists (SELECT * FROM Brafton WHERE content='1') UPDATE Brafton SET Date = " + PageTabId + " WHERE Content = '1' else INSERT INTO Brafton (Content, Date) VALUES (1, '" + PageTabId + "')";
            cmd.ExecuteNonQuery();

			//Sets the current domain in the Brafton Table
            cmd.CommandText = "IF Exists (SELECT * FROM Brafton WHERE content='1') UPDATE Brafton SET Extract = 'http://" + checkDomain + "' WHERE Content = '1' else INSERT INTO Brafton (Content, Extract) VALUES (1, 'http://" + checkDomain + "')";
            cmd.ExecuteNonQuery();

            if (checkFriendURLS == "True")
            {
                boolFriendURL.Text = "True";
                boolFriendURL.CssClass = "boolTrue";
            }
            else
            {
                boolFriendURL.Text = "False";
                boolFriendURL.CssClass = "boolFalse";
            }

            if (checkBlogModule == 1)
            {
                boolBlogModule.Text = "True";
                boolBlogModule.CssClass = "boolTrue";
                cmd.CommandText = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Blog_Entries' AND column_name='BraftonID') BEGIN ALTER TABLE Blog_Entries ADD BraftonID nvarchar(255) END";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Blog_Entries' AND column_name='TestNull') BEGIN ALTER TABLE Blog_Entries ADD TestNull AS (CASE WHEN BraftonID IS NULL THEN -1*EntryID ELSE BraftonID END) END";
                cmd.ExecuteNonQuery();
            }
            else
            {
                boolBlogModule.Text = "False";
                boolBlogModule.CssClass = "boolFalse";
            }

  
                if (File.Exists(checkPath))
                {
                    boolBin.Text = "True";
                    boolBin.CssClass = "boolTrue";
                }
                else
                {
                    boolBin.Text = "<br />Due to insufficient permissions the BraftonSchedule.dll could not be moved programatically. Please make sure that the BraftonSchedule.dll has been moved from <span style='font-style:italic; font-weight:bold;'>" + appPath + "/DesktopModules/Brafton</span> to <span style='font-style:italic; font-weight:bold;'>" + appPath + "/Bin</span>";
                    boolBin.CssClass = "permissions";
                }
				
				 if (File.Exists(checkImagePath))
                {
                    boolBinImage.Text = "True";
                    boolBinImage.CssClass = "boolTrue";
                }
                else
                {
                    boolBinImage.Text = "<br />Due to insufficient permissions the ImportImages.dll could not be moved programatically. Please make sure that the ImportImages.dll has been moved from <span style='font-style:italic; font-weight:bold;'>" + appPath + "/DesktopModules/Brafton</span> to <span style='font-style:italic; font-weight:bold;'>" + appPath + "/Bin</span>";
                    boolBinImage.CssClass = "permissions";
                }
            
            if (checkBlogCreated > 0)
            {
                boolBlogCreated.Text = "True";
                boolBlogCreated.CssClass = "boolTrue";

                //VERY IMPORTANT************* SETS THE INDEXES FOR ALL THREE TABLES THAT ARE BEING IMPORTED INTO
                //WITHOUT THIS THERE WILL BE DUPLICATES

                try
                {
                    //Sets the Index for Blog_Entries
                    StringBuilder createIndex = new StringBuilder();
                    createIndex.Append("CREATE UNIQUE NONCLUSTERED INDEX [braftonIndex] ON [dbo].[Blog_Entries] ");
                    createIndex.Append("([TestNull] ASC) ");
                    createIndex.Append("WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, ");
                    createIndex.Append("IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ");
                    createIndex.Append("ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
                    cmd.CommandText = createIndex.ToString();
                    cmd.ExecuteNonQuery();
                }
                catch { }

                try
                {
                    //Sets the Index for Blog_Categories
                    StringBuilder createIndexCat = new StringBuilder();
                    createIndexCat.Append("CREATE UNIQUE NONCLUSTERED INDEX [blogCategory] ON [dbo].[Blog_Categories] ");
                    createIndexCat.Append("([Category] ASC) ");
                    createIndexCat.Append("WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, ");
                    createIndexCat.Append("IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ");
                    createIndexCat.Append("ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
                    cmd.CommandText = createIndexCat.ToString();
                    cmd.ExecuteNonQuery();
                }
                catch { }

                try
                {
                    //Sets the Index for Blog_Entry_Categories
                    StringBuilder createIndexEntCat = new StringBuilder();
                    createIndexEntCat.Append("CREATE UNIQUE NONCLUSTERED INDEX [blogEntCategory] ON [dbo].[Blog_Entry_Categories] ");
                    createIndexEntCat.Append("([CatID] ASC, [EntryID] ASC) ");
                    createIndexEntCat.Append("WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, ");
                    createIndexEntCat.Append("IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ");
                    createIndexEntCat.Append("ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
                    cmd.CommandText = createIndexEntCat.ToString();
                    cmd.ExecuteNonQuery();
                }
                catch { }

                cmd.CommandText = "Select Title From Blog_Blogs where PortalID = '" + intPortalID + "'";
                SqlDataReader blogOptions = cmd.ExecuteReader();

                if (!IsPostBack && blogOptions.HasRows)
                {
                    while (blogOptions.Read())
                    {
                        blogIdDrpDwn.Items.Add(new ListItem(blogOptions.GetString(0)));
                    }
                }

                blogOptions.Close();
                setAPIPH.Visible = true;
            }
            else
            {
                boolBlogCreated.Text = "False";
                boolBlogCreated.CssClass = "boolFalse";
            }

            if (checkNewsAPI == 0)
            {
                boolCheckAPI.Text = "False";
                boolCheckAPI.CssClass = "boolFalse";
                nextStep.Visible = false;
            }
            else
            {
                cmd.CommandText = "SELECT Title FROM Brafton WHERE content='1'";
                string newsAPI = (string)cmd.ExecuteScalar();
                boolCheckAPI.Text = "<span class='boolTrue'>True</span>";
                apiURLLabel.Text = newsAPI;
                nextStep.Visible = true;
            }

            if (checkBlogID == 0)
            {
                boolCheckBlogID.Text = "<span class='boolFalse'>False</span>";
            }
            else
            {
                boolCheckBlogID.Text = "<span class='boolTrue'>True</span>";
                cmd.CommandText = "Select Title From Blog_Blogs Where BlogID = " + getBlogID();
                string blogTitle = (string)cmd.ExecuteScalar();
                currentBlogID.Text = blogTitle;
            }

            if (checkFriendURLS == "True" && checkBlogID > 0 && checkBlogCreated > 0 && checkNewsAPI > 0 && checkBlogCreated > 0)
            {
                Import.Visible = true;
            }

            connection.Close();
            cmd.Dispose();
        }
        catch (System.NullReferenceException ex)
        {
            labelError.Text = ex.ToString();
            connection.Close();
            cmd.Dispose();
        }
        catch (Exception ex)
        {
            labelError2.Text = "Generic exception: " + ex.ToString();
            connection.Close();
            cmd.Dispose();
        }

    }

    ///////////////////GET AND SET BLOG ID//////////////////////////
    protected void setBlogID_Click(object sender, EventArgs e)
    {
        connection.Open();
        int findBlogID;
        string test = blogIdDrpDwn.Text;
        cmd.CommandText = "IF OBJECT_ID('Blog_Blogs') IS NOT NULL(Select BlogID FROM Blog_Blogs Where title = '" + blogIdDrpDwn.Text + "') Else select 0";
        findBlogID = (int)cmd.ExecuteScalar();

        if (findBlogID != 0)
        {
            cmd.CommandText = "UPDATE Brafton SET ID = " + findBlogID + " WHERE Content = '1'";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "Select Title From Blog_Blogs Where BlogID = " + getBlogID();
            string blogTitle = (string)cmd.ExecuteScalar();
            currentBlogID.Text = blogTitle;

            //Set Check Blog ID Label = "TRUE"
            boolCheckBlogID.Text = "<span class='boolTrue'>True</span>";

            //Make import button visible
            Import.Visible = true;
        }
        connection.Close();
    }

    int getBlogID()
    {
        cmd.CommandText = "Select ID from Brafton Where Content = '1'";
        int blogID = (int)cmd.ExecuteScalar();
        return blogID;
    }

    ///////////////////GET AND SET API KEY//////////////////////////
    protected void setAPI_Click(object sender, EventArgs e)
    {
        string newsURL = apiURL.Text;
        connection.Open();
        cmd.Connection = connection;

        try
        {

            cmd.CommandText = "IF OBJECT_ID('Brafton') IS NOT NULL(SELECT count(*) as total_record FROM Brafton WHERE content='1') ELSE SELECT 0";
            int apiAvail = (int)cmd.ExecuteScalar();

            if (apiAvail == 1 && apiURL.Text != "")
            {
                cmd.CommandText = "UPDATE Brafton SET Title = '" + newsURL + "' WHERE Content = '1'";
                cmd.ExecuteNonQuery();
                apiURLLabel.Text = newsURL;
                boolCheckAPI.Text = "<span class='boolTrue'>True</span>";
            }
            else if (apiAvail == 0 && apiURL.Text != "")
            {
                cmd.CommandText = "INSERT INTO Brafton (Content, Title) VALUES (1, '" + newsURL + "' )";
                cmd.ExecuteNonQuery();
                apiURLLabel.Text = newsURL;
                boolCheckAPI.Text = "<span class='boolTrue'>True</span>";
            }

            nextStep.Visible = true;
        }
        catch (System.NullReferenceException ex)
        {
            labelError.Text = ex.ToString();
        }
        catch (Exception ex)
        {
            labelError2.Text = "Generic exception: " + ex.ToString();
        }
        connection.Close();

    }

    //////////////GET NEWS API URL///////////////
    public string getNewsURL()
    {
        cmd.CommandText = "SELECT Title FROM Brafton WHERE content='1'";
        string feedURL = cmd.ExecuteScalar().ToString();
        return feedURL;
    }

    protected void Import_Click(object sender, EventArgs e)
    {
        Assembly _Assemblies = Assembly.LoadFrom(checkPath);
        Type _Type = _Assemblies.GetType("Brafton.DotNetNuke.BraftonSchedule");
        MethodInfo _MethodInfo = _Type.GetMethod("updateScript");

        // The first parameter to pass into the Invoke Method coming up.
        Object _InvokeParam1 = Activator.CreateInstance(_Type);
        _MethodInfo.Invoke(_InvokeParam1, null);

        Response.Redirect(Request.RawUrl);
    }

}


