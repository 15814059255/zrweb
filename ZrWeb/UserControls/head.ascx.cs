using System;
using System.Web;
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
        
        // 如果Session中没有用户ID，尝试从Cookie恢复
        if (!IsLoggedIn)
        {
            HttpCookie userCookie = Request.Cookies["ZrWebUser"];
            if (userCookie != null && !string.IsNullOrEmpty(userCookie["UserID"]))
            {
                int userId = 0;
                if (int.TryParse(userCookie["UserID"], out userId) && userId > 0)
                {
                    // 恢复Session
                    Session["UserID"] = userId;
                    Session["UserName"] = userCookie["UserName"] ?? "";
                    Session["LinkMan"] = userCookie["LinkMan"] ?? "";
                    Session["MobilePhone"] = userCookie["MobilePhone"] ?? "";
                    Session["RoseID"] = userCookie["RoseID"] ?? "";
                    Session["UserGuid"] = userCookie["UserGuid"] ?? "";
                    
                    int shopId = 0;
                    int.TryParse(userCookie["ShopId"], out shopId);
                    Session["ShopId"] = shopId;
                    Session["ShopName"] = userCookie["ShopName"] ?? "";
                    Session["ShopCompany"] = userCookie["ShopCompany"] ?? "";
                    
                    IsLoggedIn = true;
                }
            }
        }
        
        // 如果Session中没有ShopId但有UserID，尝试从Cookie恢复ShopId
        if (IsLoggedIn && Session["ShopId"] == null)
        {
            HttpCookie userCookie = Request.Cookies["ZrWebUser"];
            if (userCookie != null)
            {
                int shopId = 0;
                if (int.TryParse(userCookie["ShopId"], out shopId))
                {
                    Session["ShopId"] = shopId;
                    Session["ShopName"] = userCookie["ShopName"] ?? "";
                    Session["ShopCompany"] = userCookie["ShopCompany"] ?? "";
                }
            }
        }
        
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