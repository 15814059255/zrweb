using System;
using System.Web.UI;
using System.Configuration;

public partial class UserControls_head : UserControl
{
    protected string SiteName = ConfigurationManager.AppSettings["SiteName"] ?? "阻容网";
    protected string SiteDomain = ConfigurationManager.AppSettings["SiteDomain"] ?? "ZR.net.cn";
    protected string ICP = ConfigurationManager.AppSettings["ICP"] ?? "粤ICP备2026073346";

    protected string CurrentPage = "";
    protected bool IsLoggedIn = false;
    protected string CurrentUserName = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        // 检查登录状态
        IsLoggedIn = Session["UserID"] != null;
        if (Session["UserName"] != null)
        {
            CurrentUserName = Session["UserName"].ToString();
        }
        else
        {
            CurrentUserName = "";
        }

        // 获取当前页面名称用于导航高亮
        string path = Request.Path.ToLower();
        if (path.Contains("index.aspx") || path == "/" || path.EndsWith("/"))
            CurrentPage = "index";
        else if (path.Contains("merchant"))
            CurrentPage = "merchant";
        else if (path.Contains("buyer"))
            CurrentPage = "buyer";
        else if (path.Contains("profile"))
            CurrentPage = "profile";
        else if (path.Contains("login"))
            CurrentPage = "login";
    }

    protected string GetNavClass(string pageName)
    {
        return CurrentPage == pageName ? "active" : "";
    }
}