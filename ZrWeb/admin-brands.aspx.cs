using System;

public partial class admin_brands : System.Web.UI.Page
{
    public string AdminName = "管理员";
    
    protected void Page_Load(object sender, EventArgs e)
    {
        // 检查管理员登录状态
        int adminId = 0;
        if (Session["AdminID"] != null)
        {
            int.TryParse(Session["AdminID"].ToString(), out adminId);
            AdminName = Session["AdminName"] != null ? Session["AdminName"].ToString() : "管理员";
        }
        
        if (adminId == 0)
        {
            Response.Redirect("/admin-login.aspx");
            Response.End();
        }
    }
}
