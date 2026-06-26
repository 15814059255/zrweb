using System;
using System.Data;
using System.Data.SqlClient;

public partial class admin_ads : System.Web.UI.Page
{
    public string AdminName = "";
    public DataTable AdList = null;
    public int TotalAds = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckLogin();
        HandleActions();
        LoadAdList();
    }

    private void CheckLogin()
    {
        if (Session["AdminID"] == null)
        {
            Response.Redirect("/admin-login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        AdminName = Session["AdminName"] != null ? Session["AdminName"].ToString() : "";
    }

    private void HandleActions()
    {
        string action = Request.Form["action"];
        if (string.IsNullOrEmpty(action))
        {
            action = Request.QueryString["action"];
        }

        if (action == "addAd")
        {
            string adSlot = Request.Form["adSlot"];
            string title = Request.Form["title"];
            string position = Request.Form["position"];
            string linkUrl = Request.Form["linkUrl"];
            string startDate = Request.Form["startDate"];
            string endDate = Request.Form["endDate"];
            int status = Convert.ToInt32(Request.Form["status"]);

            try
            {
                DbHelper.ExecuteNonQuery("INSERT INTO ads (keyWord, adName, adPositionId, adURL, adStartDate, adEndDate, dataFlag, createTime) VALUES (@adSlot, @title, @position, @linkUrl, @startDate, @endDate, @status, GETDATE())",
                    DbHelper.CreateParameter("@adSlot", adSlot),
                    DbHelper.CreateParameter("@title", title),
                    DbHelper.CreateParameter("@position", position),
                    DbHelper.CreateParameter("@linkUrl", linkUrl),
                    DbHelper.CreateParameter("@startDate", startDate),
                    DbHelper.CreateParameter("@endDate", endDate),
                    DbHelper.CreateParameter("@status", status));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddAd error: " + ex.Message);
            }
        }

        string adId = Request.QueryString["adId"];

        if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(adId))
        {
            int AdID = Convert.ToInt32(adId);
            
            if (action == "toggleStatus")
            {
                int status = Convert.ToInt32(Request.QueryString["status"]);
                DbHelper.ExecuteNonQuery("UPDATE ads SET dataFlag = @status WHERE adId = @adId",
                    DbHelper.CreateParameter("@status", status),
                    DbHelper.CreateParameter("@adId", AdID));
            }
            else if (action == "delete")
            {
                DbHelper.ExecuteNonQuery("DELETE FROM ads WHERE adId = @adId",
                    DbHelper.CreateParameter("@adId", AdID));
            }
        }
    }

    private void EnsureAdTableExists()
    {
        try
        {
            string checkSql = "SELECT COUNT(*) FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ads]') AND type IN ('U')";
            int count = Convert.ToInt32(DbHelper.ExecuteScalar(checkSql));
            if (count == 0)
            {
                string createSql = @"CREATE TABLE [dbo].[ads] (
                    [AdID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [AdSlot] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL,
                    [Title] nvarchar(100) COLLATE Chinese_PRC_CI_AS NULL,
                    [Position] nvarchar(20) COLLATE Chinese_PRC_CI_AS NULL,
                    [LinkUrl] nvarchar(255) COLLATE Chinese_PRC_CI_AS NULL,
                    [StartDate] date NULL,
                    [EndDate] date NULL,
                    [Status] int NULL DEFAULT 1,
                    [CreateTime] datetime NULL DEFAULT GETDATE()
                )";
                DbHelper.ExecuteNonQuery(createSql);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("EnsureAdTableExists error: " + ex.Message);
        }
    }

    private void LoadAdList()
    {
        EnsureAdTableExists();
        try
        {
            AdList = DbHelper.ExecuteQuery("SELECT adId as AdID, keyWord as AdSlot, adName as Title, adPositionId as Position, adURL as LinkUrl, adStartDate as StartDate, adEndDate as EndDate, dataFlag as Status FROM ads ORDER BY adId ASC");
            TotalAds = AdList != null ? AdList.Rows.Count : 0;
            
            if (AdList != null && AdList.Rows.Count > 0)
            {
                rptAds.DataSource = AdList;
                rptAds.DataBind();
            }
            else
            {
                AdList = new DataTable();
                rptAds.DataSource = AdList;
                rptAds.DataBind();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadAdList error: " + ex.Message);
            AdList = new DataTable();
            rptAds.DataSource = AdList;
            rptAds.DataBind();
        }
    }
}