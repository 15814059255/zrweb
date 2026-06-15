using System;
using System.Web;

public partial class logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 获取当前用户ID用于清除缓存
        int userId = UserHelper.GetUserId();

        // 清除Session和Cookie
        UserHelper.ClearUserInfo();

        // 清除Redis缓存中的用户信息
        if (userId > 0)
        {
            RedisHelper.ClearUserCache(userId);
        }
        
        // 转到首页
        Response.Redirect("/index.aspx");
    }
}