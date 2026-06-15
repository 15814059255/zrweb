using System;
using System.Data;

public partial class CreateTestUser : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 检查用户是否已存在
            string checkSql = "SELECT COUNT(*) FROM userinfo WHERE MobilePhone='15814059255'";
            object result = DbHelper.ExecuteScalar(checkSql);
            int count = Convert.ToInt32(result);

            if (count > 0)
            {
                Response.Write("用户已存在，正在更新审核状态...");
                // 更新审核状态为已审核
                string updateSql = "UPDATE userinfo SET IsCheck=1, SysStatus=0 WHERE MobilePhone='15814059255'";
                DbHelper.ExecuteNonQuery(updateSql);
                Response.Write("<br/>审核状态已更新");
            }
            else
            {
                Response.Write("创建测试用户...");
                // 创建测试用户
                string userGuid = Guid.NewGuid().ToString();
                string insertSql = @"
                    INSERT INTO userinfo (UserName, Password, UserGuid, SysStatus, LinkMan, MobilePhone, RoseID, IsCheck, CreateTime) 
                    VALUES (@UserName, @Password, @UserGuid, 0, @LinkMan, @MobilePhone, 1, 1, GETDATE())
                ";
                DbHelper.ExecuteNonQuery(insertSql,
                    DbHelper.CreateParameter("@UserName", "15814059255"),
                    DbHelper.CreateParameter("@Password", "123456"),
                    DbHelper.CreateParameter("@UserGuid", userGuid),
                    DbHelper.CreateParameter("@LinkMan", "测试用户"),
                    DbHelper.CreateParameter("@MobilePhone", "15814059255"));
                Response.Write("<br/>测试用户创建成功！");
                Response.Write("<br/>用户名：15814059255");
                Response.Write("<br/>密码：123456");
            }
        }
        catch (Exception ex)
        {
            Response.Write("错误：" + ex.Message);
        }
    }
}