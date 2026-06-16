using System;
using System.Web;
using System.Collections.Generic;

public static class UserHelper
{
    private const string CookieName = "ZrWebUser";

    public static int GetUserId()
    {
        int userId = 0;
        
        if (HttpContext.Current.Session["UserID"] != null)
        {
            int.TryParse(HttpContext.Current.Session["UserID"].ToString(), out userId);
        }
        
        if (userId == 0)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
            {
                int.TryParse(cookie["UserID"], out userId);
                if (userId > 0)
                {
                    RestoreFromRedis(userId);
                    HttpContext.Current.Session["UserID"] = userId;
                }
            }
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
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
            {
                int userId = 0;
                int.TryParse(cookie["UserID"], out userId);
                if (userId > 0)
                {
                    RestoreFromRedis(userId);
                }
            }
            
            if (HttpContext.Current.Session["UserName"] != null)
            {
                userName = HttpContext.Current.Session["UserName"].ToString();
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
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
            {
                int userId = 0;
                int.TryParse(cookie["UserID"], out userId);
                if (userId > 0)
                {
                    RestoreFromRedis(userId);
                }
            }
            
            if (HttpContext.Current.Session["LinkMan"] != null)
            {
                linkMan = HttpContext.Current.Session["LinkMan"].ToString();
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
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
            {
                int userId = 0;
                int.TryParse(cookie["UserID"], out userId);
                if (userId > 0)
                {
                    RestoreFromRedis(userId);
                }
            }
            
            if (HttpContext.Current.Session["MobilePhone"] != null)
            {
                mobilePhone = HttpContext.Current.Session["MobilePhone"].ToString();
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
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
            {
                int userId = 0;
                int.TryParse(cookie["UserID"], out userId);
                if (userId > 0)
                {
                    RestoreFromRedis(userId);
                }
            }
            
            if (HttpContext.Current.Session["RoseID"] != null)
            {
                int.TryParse(HttpContext.Current.Session["RoseID"].ToString(), out roseId);
            }
        }
        
        return roseId;
    }

    public static int GetShopId()
    {
        int shopId = 0;
        
        if (HttpContext.Current.Session["ShopId"] != null)
        {
            int.TryParse(HttpContext.Current.Session["ShopId"].ToString(), out shopId);
        }
        
        if (shopId == 0)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
            {
                int userId = 0;
                int.TryParse(cookie["UserID"], out userId);
                if (userId > 0)
                {
                    RestoreFromRedis(userId);
                }
            }
            
            if (HttpContext.Current.Session["ShopId"] != null)
            {
                int.TryParse(HttpContext.Current.Session["ShopId"].ToString(), out shopId);
            }
        }
        
        return shopId;
    }

    public static string GetShopName()
    {
        string shopName = "";
        
        if (HttpContext.Current.Session["ShopName"] != null)
        {
            shopName = HttpContext.Current.Session["ShopName"].ToString();
        }
        
        if (string.IsNullOrEmpty(shopName))
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
            {
                int userId = 0;
                int.TryParse(cookie["UserID"], out userId);
                if (userId > 0)
                {
                    RestoreFromRedis(userId);
                }
            }
            
            if (HttpContext.Current.Session["ShopName"] != null)
            {
                shopName = HttpContext.Current.Session["ShopName"].ToString();
            }
        }
        
        return shopName;
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
            if (cookie != null && !string.IsNullOrEmpty(cookie["UserID"]))
            {
                int userId = 0;
                int.TryParse(cookie["UserID"], out userId);
                if (userId > 0)
                {
                    RestoreFromRedis(userId);
                }
            }
            
            if (HttpContext.Current.Session["UserGuid"] != null)
            {
                userGuid = HttpContext.Current.Session["UserGuid"].ToString();
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
        int userId = 0;
        if (HttpContext.Current.Session["UserID"] != null)
        {
            int.TryParse(HttpContext.Current.Session["UserID"].ToString(), out userId);
        }
        
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
        
        if (userId > 0)
        {
            RedisHelper.ClearUserCache(userId);
        }
    }

    private static void RestoreFromRedis(int userId)
    {
        try
        {
            var userData = RedisHelper.GetUserInfo(userId);
            if (userData != null)
            {
                HttpContext.Current.Session["UserID"] = userId;
                
                if (userData.ContainsKey("UserName"))
                    HttpContext.Current.Session["UserName"] = userData["UserName"];
                if (userData.ContainsKey("LinkMan"))
                    HttpContext.Current.Session["LinkMan"] = userData["LinkMan"];
                if (userData.ContainsKey("MobilePhone"))
                    HttpContext.Current.Session["MobilePhone"] = userData["MobilePhone"];
                if (userData.ContainsKey("RoseID"))
                    HttpContext.Current.Session["RoseID"] = userData["RoseID"];
                if (userData.ContainsKey("UserGuid"))
                    HttpContext.Current.Session["UserGuid"] = userData["UserGuid"];
                if (userData.ContainsKey("ShopId"))
                {
                    int shopId = 0;
                    int.TryParse(userData["ShopId"], out shopId);
                    HttpContext.Current.Session["ShopId"] = shopId;
                }
                if (userData.ContainsKey("ShopName"))
                    HttpContext.Current.Session["ShopName"] = userData["ShopName"];
                if (userData.ContainsKey("ShopCompany"))
                    HttpContext.Current.Session["ShopCompany"] = userData["ShopCompany"];
            }
        }
        catch
        {
        }
    }
}