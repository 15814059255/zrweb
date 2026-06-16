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
        IsLoggedIn = UserHelper.GetUserId() > 0;
        CurrentUserName = UserHelper.GetUserName().Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");

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