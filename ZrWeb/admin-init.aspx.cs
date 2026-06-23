using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

public partial class admin_init : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    protected void btnInit_Click(object sender, EventArgs e)
    {
        pnlInitForm.Visible = false;
        pnlResult.Visible = true;
        
        StringBuilder result = new StringBuilder();
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ZrWebConnectionString"].ConnectionString;
        
        try
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                
                // 1. 创建表
                result.Append("<div class='result info'>正在创建管理员表...</div>");
                string createTableSQL = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'admin_users')
                    BEGIN
                        CREATE TABLE [dbo].[admin_users] (
                            [AdminID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [AdminName] nvarchar(50) NOT NULL,
                            [Password] nvarchar(50) NOT NULL,
                            [RealName] nvarchar(50) NULL,
                            [Status] int DEFAULT 1,
                            [CreateTime] datetime DEFAULT GETDATE(),
                            [LastLoginTime] datetime NULL,
                            [LastLoginIP] nvarchar(50) NULL
                        )
                        CREATE UNIQUE NONCLUSTERED INDEX [uc_AdminName] ON [dbo].[admin_users] ([AdminName] ASC)
                    END";
                
                using (SqlCommand cmd = new SqlCommand(createTableSQL, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                result.Append("<div class='result success'>✓ 管理员表创建成功（或已存在）</div>");
                
                // 2. 删除旧账号
                result.Append("<div class='result info'>正在清理旧数据...</div>");
                using (SqlCommand cmd = new SqlCommand("DELETE FROM admin_users WHERE AdminName = 'superadmin'", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                result.Append("<div class='result success'>✓ 旧数据清理完成</div>");
                
                // 3. 插入新账号 (密码: ZrAdmin@2026 的 MD5)
                result.Append("<div class='result info'>正在创建管理员账号...</div>");
                string insertSQL = @"
                    INSERT INTO admin_users (AdminName, Password, RealName, Status, CreateTime)
                    VALUES ('superadmin', '18d27d4cadb243620ae331138788b3fe', '超级管理员', 1, GETDATE())";
                
                using (SqlCommand cmd = new SqlCommand(insertSQL, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                result.Append("<div class='result success'>✓ 管理员账号创建成功</div>");
                
                // 4. 验证
                result.Append("<div class='result info'>正在验证数据...</div>");
                using (SqlCommand cmd = new SqlCommand("SELECT AdminID, AdminName, Password, RealName, Status FROM admin_users WHERE AdminName = 'superadmin'", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.Append("<div class='result success'>✓ 数据验证成功！</div>");
                            result.Append("<p>AdminID: " + reader["AdminID"] + "</p>");
                            result.Append("<p>AdminName: " + reader["AdminName"] + "</p>");
                            result.Append("<p>Password: " + reader["Password"] + "</p>");
                            result.Append("<p>RealName: " + reader["RealName"] + "</p>");
                            result.Append("<p>Status: " + reader["Status"] + " (1=正常)</p>");
                            
                            // 检查密码是否正确
                            if (reader["Password"].ToString() == "18d27d4cadb243620ae331138788b3fe")
                            {
                                result.Append("<div class='result success'>✓ 密码哈希正确</div>");
                                RegisterStartupScript("showSuccess", "");
                            }
                            else
                            {
                                result.Append("<div class='result error'>✗ 密码哈希不正确！</div>");
                            }
                        }
                        else
                        {
                            result.Append("<div class='result error'>✗ 无法读取创建的账号</div>");
                        }
                    }
                }
            }
            
            litResult.Text = result.ToString();
            
            // 显示成功提示
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "document.getElementById('divSuccess').style.display='block';", true);
        }
        catch (Exception ex)
        {
            result.Append("<div class='result error'>✗ 初始化失败: " + ex.Message + "</div>");
            litResult.Text = result.ToString();
        }
    }
}
