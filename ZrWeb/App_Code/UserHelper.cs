using System;
using System.Web;

public static class UserHelper
{
    private const string CookieName = "ZrWebUser";

    public static int GetUserId()
    {
        int userId = 0;
        
        try
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserID"] != null)
            {
                int.TryParse(HttpContext.Current.Session["UserID"].ToString(), out userId);
            }
            
            if (userId == 0)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
                if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
                {
                    int.TryParse(cookie["UserID"], out userId);
                    if (userId > 0 && HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session["UserID"] = userId;
                        HttpContext.Current.Session["UserName"] = cookie["UserName"] ?? "";
                        HttpContext.Current.Session["LinkMan"] = cookie["LinkMan"] ?? "";
                        HttpContext.Current.Session["MobilePhone"] = cookie["MobilePhone"] ?? "";
                        HttpContext.Current.Session["RoseID"] = cookie["RoseID"] ?? "";
                        HttpContext.Current.Session["UserGuid"] = cookie["UserGuid"] ?? "";
                        
                        int shopId = 0;
                        if (int.TryParse(cookie["ShopId"], out shopId))
                        {
                            HttpContext.Current.Session["ShopId"] = shopId;
                        }
                        HttpContext.Current.Session["ShopName"] = cookie["ShopName"] ?? "";
                        HttpContext.Current.Session["ShopCompany"] = cookie["ShopCompany"] ?? "";
                    }
                }
            }
        }
        catch
        {
            userId = 0;
        }
        
        return userId;
    }

    public static string GetUserName()
    {
        string userName = "";
        
        if (HttpContext.Current.Session["UserName"] != null)
        {
            userName = HttpContext.Current.Session["UserName"].ToString();
        }
        
        if (string.IsNullOrEmpty(userName))
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserName"]))
            {
                userName = cookie["UserName"];
                HttpContext.Current.Session["UserName"] = userName;
            }
        }
        
        return userName;
    }

    public static string GetLinkMan()
    {
        string linkMan = "";
        
        if (HttpContext.Current.Session["LinkMan"] != null)
        {
            linkMan = HttpContext.Current.Session["LinkMan"].ToString();
        }
        
        if (string.IsNullOrEmpty(linkMan))
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie["LinkMan"]))
            {
                linkMan = cookie["LinkMan"];
                HttpContext.Current.Session["LinkMan"] = linkMan;
            }
        }
        
        return linkMan;
    }

    public static string GetMobilePhone()
    {
        string mobilePhone = "";
        
        if (HttpContext.Current.Session["MobilePhone"] != null)
        {
            mobilePhone = HttpContext.Current.Session["MobilePhone"].ToString();
        }
        
        if (string.IsNullOrEmpty(mobilePhone))
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie["MobilePhone"]))
            {
                mobilePhone = cookie["MobilePhone"];
                HttpContext.Current.Session["MobilePhone"] = mobilePhone;
            }
        }
        
        return mobilePhone;
    }

    public static int GetRoseId()
    {
        int roseId = 0;
        
        if (HttpContext.Current.Session["RoseID"] != null)
        {
            int.TryParse(HttpContext.Current.Session["RoseID"].ToString(), out roseId);
        }
        
        if (roseId == 0)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie["RoseID"]))
            {
                int.TryParse(cookie["RoseID"], out roseId);
                if (roseId > 0)
                {
                    HttpContext.Current.Session["RoseID"] = roseId;
                }
            }
        }
        
        return roseId;
    }

    public static string GetUserGuid()
    {
        string userGuid = "";
        
        if (HttpContext.Current.Session["UserGuid"] != null)
        {
            userGuid = HttpContext.Current.Session["UserGuid"].ToString();
        }
        
        if (string.IsNullOrEmpty(userGuid))
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserGuid"]))
            {
                userGuid = cookie["UserGuid"];
                HttpContext.Current.Session["UserGuid"] = userGuid;
            }
        }
        
        return userGuid;
    }

    public static bool IsLoggedIn()
    {
        return GetUserId() > 0;
    }

    public static void ClearUserInfo()
    {
        HttpContext.Current.Session.Remove("UserID");
        HttpContext.Current.Session.Remove("UserName");
        HttpContext.Current.Session.Remove("LinkMan");
        HttpContext.Current.Session.Remove("MobilePhone");
        HttpContext.Current.Session.Remove("RoseID");
        HttpContext.Current.Session.Remove("UserGuid");
        
        HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
        if (cookie != null)
        {
            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}