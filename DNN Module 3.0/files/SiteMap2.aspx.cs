using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Configuration;
using System.Collections;
using System.Data.SqlClient;

public partial class DesktopModules_Brafton_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GenerateGoogleSitemap();
    }

    // structure that represents a sitemap item
    struct SitemapItem
    {
        // public fields representing the sitemap item properties
        public string Url, LastMod, ChangeFreq, Priority;

        // constructor initializes the fields
        public SitemapItem(string url, string lastMod, string changeFreq, string priority)
        {
            this.Url = url;
            this.LastMod = lastMod;
            this.ChangeFreq = changeFreq;
            this.Priority = priority;
        }
    }

    // generates the Google sitemap of the site
    private void GenerateGoogleSitemap()
    {
        // obtain the current HttpResponse object
        HttpResponse response = HttpContext.Current.Response;

        // set the content type
        response.ContentType = "text/xml";

        // use an XmlWriter to generate the Google sitemap
        XmlWriter xmlWriter = XmlWriter.Create(response.OutputStream);

        // write the start element
        xmlWriter.WriteStartElement("urlset", "http://www.google.com/schemas/sitemap/0.84");

        // obtain the list of sitemap items
        ArrayList sitemapItems = GetSitemapItems();

        // generate the sitemap items
        foreach (SitemapItem sitemapItem in sitemapItems)
        {
            // generate the <url> element and its contents
            xmlWriter.WriteStartElement("url");
            xmlWriter.WriteElementString("loc", sitemapItem.Url);
            xmlWriter.WriteElementString("lastmod", sitemapItem.LastMod);
            xmlWriter.WriteElementString("changefreq", sitemapItem.ChangeFreq);
            xmlWriter.WriteElementString("priority", sitemapItem.Priority);
            xmlWriter.WriteEndElement();
        }

        // close the document 
        xmlWriter.WriteEndElement();
        xmlWriter.Flush();
    }

    // builds the list of items that need to be included in the sitemap
    private ArrayList GetSitemapItems()
    {
        // declare list of sitemap items
        //List<SitemapItem> sitemapItems = new List<SitemapItem>();
        //List<SitemapItem> sitemapItems = new List<SitemapItem>();
        string priority = "0.5";
        string changefreq = "daily";
        DateTime lastmod;
        string permalink;

        ArrayList sitemapItems = new ArrayList();

        string queryString =
        "SELECT AddedDate, PermaLink From Blog_Entries";

        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString()))
        {
            SqlCommand command =
                new SqlCommand(queryString, connection);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            // Call Read before accessing data.
            while (reader.Read())
            {
                lastmod = reader.GetDateTime(0);
                permalink = "http://" + HttpContext.Current.Request.Url.Host + reader.GetString(1);
            // create the list of URLs to include in the sitemap
        sitemapItems.Add(new SitemapItem(
          permalink, lastmod.ToString("yyyy-MM-dd"), changefreq, priority));
            }

            // Call Close when done reading.
            reader.Close();
        }

        

        // return the list of Sitemap objects
        return sitemapItems;
    } 
}