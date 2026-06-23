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
        if (!IsPostBack)
        {
            CheckLogin();
            HandleActions();
            LoadAdList();
        }
    }

    private void CheckLogin()
    {
        if (Session["AdminID"] == null)
        {
            Response.Redirect("/admin-login.aspx");
        }
        AdminName = Session["AdminName"] != null ? Session["AdminName"].ToString() : "";
    }

    private void HandleActions()
    {
        string action = Request.QueryString["action"];
        string adId = Request.QueryString["adId"];

        if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(adId))
        {
            int AdID = Convert.ToInt32(adId);
            
            if (action == "toggleStatus")
            {
                int status = Convert.ToInt32(Request.QueryString["status"]);
                DbHelper.ExecuteNonQuery("UPDATE ads SET Status = @status WHERE AdID = @adId",
                    DbHelper.CreateParameter("@status", status),
                    DbHelper.CreateParameter("@adId", AdID));
            }
            else if (action == "delete")
            {
                DbHelper.ExecuteNonQuery("DELETE FROM ads WHERE AdID = @adId",
                    DbHelper.CreateParameter("@adId", AdID));
            }
        }
    }

    private void LoadAdList()
    {
        try
        {
            AdList = DbHelper.ExecuteQuery("SELECT AdID, AdSlot, Title, Position, LinkUrl, StartDate, EndDate, Status FROM ads ORDER BY AdID ASC");
            TotalAds = AdList != null ? AdList.Rows.Count : 0;
            if (AdList != null)
            {
                rptAds.DataSource = AdList;
                rptAds.DataBind();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadAdList error: " + ex.Message);
            // 发生错误时创建空DataTable防止绑定失败
            AdList = new DataTable();
            rptAds.DataSource = AdList;
            rptAds.DataBind();
        }
    }
}