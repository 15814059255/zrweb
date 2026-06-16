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
            return UserHelper.GetUserId();
        }
    }

    /// <summary>
    /// 当前登录用户名
    /// </summary>
    public string CurrentUserName
    {
        get
        {
            return UserHelper.GetUserName();
        }
    }

    /// <summary>
    /// 当前登录用户联系人
    /// </summary>
    public string CurrentLinkMan
    {
        get
        {
            return UserHelper.GetLinkMan();
        }
    }

    /// <summary>
    /// 当前登录用户手机
    /// </summary>
    public string CurrentMobilePhone
    {
        get
        {
            return UserHelper.GetMobilePhone();
        }
    }

    /// <summary>
    /// 当前登录用户角色
    /// </summary>
    public int CurrentRoseID
    {
        get
        {
            return UserHelper.GetRoseId();
        }
    }

    /// <summary>
    /// 是否已登录
    /// </summary>
    public bool IsLoggedIn
    {
        get
        {
            return UserHelper.GetUserId() > 0;
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