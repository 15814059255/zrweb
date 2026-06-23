using System;
using System.Data;
using System.Data.SqlClient;

public partial class admin_console : System.Web.UI.Page
{
    public string AdminName = "";
    public int MemberCount = 0;
    public int TodayNewMember = 0;
    public int SupplyCount = 0;
    public int TodayNewSupply = 0;
    public int DemandCount = 0;
    public int TodayNewDemand = 0;
    public int QuoteCount = 0;
    public int TodayNewQuote = 0;
    public DataTable RecentMembers = null;
    public DataTable RecentGoods = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CheckLogin();
            LoadDashboardData();
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

    private void LoadDashboardData()
    {
        try
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            
            MemberCount = GetCount("SELECT COUNT(*) FROM userinfo WHERE SysStatus=0");
            TodayNewMember = GetCount("SELECT COUNT(*) FROM userinfo WHERE SysStatus=0 AND CONVERT(varchar(10), CreateTime, 120) = @today", 
                DbHelper.CreateParameter("@today", today));
            
            SupplyCount = GetCount("SELECT COUNT(*) FROM goods WHERE pubType=1 AND isSale=1 AND dataFlag=1");
            TodayNewSupply = GetCount("SELECT COUNT(*) FROM goods WHERE pubType=1 AND dataFlag=1 AND CONVERT(varchar(10), createTime, 120) = @today",
                DbHelper.CreateParameter("@today", today));
            
            DemandCount = GetCount("SELECT COUNT(*) FROM goods WHERE pubType=2 AND isSale=1 AND dataFlag=1");
            TodayNewDemand = GetCount("SELECT COUNT(*) FROM goods WHERE pubType=2 AND dataFlag=1 AND CONVERT(varchar(10), createTime, 120) = @today",
                DbHelper.CreateParameter("@today", today));
            
            QuoteCount = GetCount("SELECT COUNT(*) FROM enquiryquoteprice WHERE dataFlag=1");
            TodayNewQuote = GetCount("SELECT COUNT(*) FROM enquiryquoteprice WHERE dataFlag=1 AND CONVERT(varchar(10), createTime, 120) = @today",
                DbHelper.CreateParameter("@today", today));
            
            RecentMembers = DbHelper.ExecuteQuery("SELECT TOP 10 UserID, UserName, LinkMan, MobilePhone, IsCheck, CreateTime FROM userinfo WHERE SysStatus=0 ORDER BY CreateTime DESC");
            RecentGoods = DbHelper.ExecuteQuery("SELECT TOP 10 goodsId, goodsSn, shopPrice, goodsUnit, pubType, isSale, createTime FROM goods WHERE dataFlag=1 ORDER BY createTime DESC");
            
            if (RecentMembers != null)
            {
                rptRecentMembers.DataSource = RecentMembers;
                rptRecentMembers.DataBind();
            }
            
            if (RecentGoods != null)
            {
                rptRecentGoods.DataSource = RecentGoods;
                rptRecentGoods.DataBind();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadDashboardData error: " + ex.Message);
        }
    }

    private int GetCount(string sql, params SqlParameter[] parameters)
    {
        try
        {
            object result = DbHelper.ExecuteScalar(sql, parameters);
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
        catch
        {
            return 0;
        }
    }
}