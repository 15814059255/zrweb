using System;
using System.Web;

/// <summary>
/// 需要登录的页面基类
/// </summary>
public class BasePage : System.Web.UI.Page
{
    /// <summary>
    /// 当前登录用户ID
    /// </summary>
    public int CurrentUserID
    {
        get
        {
            if (Session["UserID"] != null)
            {
                return Convert.ToInt32(Session["UserID"]);
            }
            return 0;
        }
    }

    /// <summary>
    /// 当前登录用户名
    /// </summary>
    public string CurrentUserName
    {
        get
        {
            if (Session["UserName"] != null)
            {
                return Session["UserName"].ToString();
            }
            return "";
        }
    }

    /// <summary>
    /// 当前登录用户联系人
    /// </summary>
    public string CurrentLinkMan
    {
        get
        {
            if (Session["LinkMan"] != null)
            {
                return Session["LinkMan"].ToString();
            }
            return "";
        }
    }

    /// <summary>
    /// 当前登录用户手机
    /// </summary>
    public string CurrentMobilePhone
    {
        get
        {
            if (Session["MobilePhone"] != null)
            {
                return Session["MobilePhone"].ToString();
            }
            return "";
        }
    }

    /// <summary>
    /// 当前登录用户角色
    /// </summary>
    public int CurrentRoseID
    {
        get
        {
            if (Session["RoseID"] != null)
            {
                return Convert.ToInt32(Session["RoseID"]);
            }
            return 0;
        }
    }

    /// <summary>
    /// 是否已登录
    /// </summary>
    public bool IsLoggedIn
    {
        get
        {
            return Session["UserID"] != null;
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        // 检查登录状态
        if (!IsLoggedIn)
        {
            Response.Redirect("/login.aspx?returnUrl=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery));
        }
    }
}