using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public partial class admin_diagnose : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TestDatabaseConnection();
        CheckAdminTable();
        CheckUserData();
        TestPasswordHash();
    }

    private void TestDatabaseConnection()
    {
        try
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ZrWebConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                litDbTest.Text = "<div class='result success'>✓ 数据库连接成功</div>";
            }
        }
        catch (Exception ex)
        {
            litDbTest.Text = "<div class='result error'>✗ 数据库连接失败: " + ex.Message + "</div>";
        }
    }

    private void CheckAdminTable()
    {
        try
        {
            string sql = @"SELECT COUNT(*) FROM sys.tables WHERE name = 'admin_users'";
            object result = DbHelper.ExecuteScalar(sql);
            int count = Convert.ToInt32(result);
            
            if (count > 0)
            {
                litTableCheck.Text = "<div class='result success'>✓ admin_users 表存在</div>";
            }
            else
            {
                litTableCheck.Text = "<div class='result error'>✗ admin_users 表不存在，请先执行 SQL 脚本创建表</div>";
                litTableCheck.Text += "<p>请执行以下 SQL 创建管理员表：</p>";
                litTableCheck.Text += "<pre>" + GetCreateTableSQL() + "</pre>";
            }
        }
        catch (Exception ex)
        {
            litTableCheck.Text = "<div class='result error'>✗ 检查表时出错: " + ex.Message + "</div>";
        }
    }

    private void CheckUserData()
    {
        try
        {
            string sql = "SELECT AdminID, AdminName, Password, RealName, Status FROM admin_users WHERE AdminName = 'superadmin'";
            DataTable dt = DbHelper.ExecuteQuery(sql);
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                litUserData.Text = "<div class='result success'>✓ 管理员账号存在</div>";
                litUserData.Text += "<pre>";
                litUserData.Text += "AdminID: " + row["AdminID"] + "\n";
                litUserData.Text += "AdminName: " + row["AdminName"] + "\n";
                litUserData.Text += "Password (MD5): " + row["Password"] + "\n";
                litUserData.Text += "RealName: " + row["RealName"] + "\n";
                litUserData.Text += "Status: " + row["Status"] + " (1=正常, 0=禁用)";
                litUserData.Text += "</pre>";
                
                // 验证密码是否为正确值
                string correctHash = "18d27d4cadb243620ae331138788b3fe";
                if (row["Password"].ToString() != correctHash)
                {
                    litUserData.Text += "<div class='result error'>✗ 密码哈希值不正确！应该是: " + correctHash + "</div>";
                    litUserData.Text += "<p>请执行以下 SQL 修复密码：</p>";
                    litUserData.Text += "<pre>UPDATE admin_users SET Password = '18d27d4cadb243620ae331138788b3fe' WHERE AdminName = 'superadmin';</pre>";
                }
            }
            else
            {
                litUserData.Text = "<div class='result error'>✗ 管理员账号不存在</div>";
                litUserData.Text += "<p>请执行以下 SQL 插入管理员：</p>";
                litUserData.Text += "<pre>INSERT INTO admin_users (AdminName, Password, RealName, Status, CreateTime) VALUES ('superadmin', '18d27d4cadb243620ae331138788b3fe', '超级管理员', 1, GETDATE());</pre>";
            }
        }
        catch (Exception ex)
        {
            litUserData.Text = "<div class='result error'>✗ 查询用户数据时出错: " + ex.Message + "</div>";
        }
    }

    private void TestPasswordHash()
    {
        string testPassword = "ZrAdmin@2026";
        string expectedHash = "18d27d4cadb243620ae331138788b3fe";
        string actualHash = GetMD5Hash(testPassword);
        
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class='result info'>密码验证测试</div>");
        sb.Append("<pre>");
        sb.Append("测试密码: " + testPassword + "\n");
        sb.Append("期望的MD5: " + expectedHash + "\n");
        sb.Append("计算的MD5: " + actualHash + "\n");
        
        if (actualHash == expectedHash)
        {
            sb.Append("\n✓ 密码哈希值匹配！");
        }
        else
        {
            sb.Append("\n✗ 密码哈希值不匹配！");
        }
        sb.Append("</pre>");
        
        litPasswordTest.Text = sb.ToString();
    }

    private string GetMD5Hash(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    private string GetCreateTableSQL()
    {
        return @"CREATE TABLE [dbo].[admin_users] (
    [AdminID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AdminName] nvarchar(50) NOT NULL,
    [Password] nvarchar(50) NOT NULL,
    [RealName] nvarchar(50) NULL,
    [Status] int DEFAULT 1,
    [CreateTime] datetime DEFAULT GETDATE(),
    [LastLoginTime] datetime NULL,
    [LastLoginIP] nvarchar(50) NULL
);

INSERT INTO admin_users (AdminName, Password, RealName, Status, CreateTime)
VALUES ('superadmin', '18d27d4cadb243620ae331138788b3fe', '超级管理员', 1, GETDATE());";
    }
}
