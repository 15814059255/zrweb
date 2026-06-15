using System;
using System.Data;

public partial class LoginDebug : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string mobile = "15814059255";
        string password = "123456";
        
        Response.Write("<h3>登录调试信息</h3>");
        Response.Write("<hr/>");
        
        // 1. 测试MD5加密
        string md5Password = GetMD5Hash(password);
        Response.Write("<p><strong>输入密码:</strong> " + password + "</p>");
        Response.Write("<p><strong>MD5加密结果:</strong> " + md5Password + "</p>");
        Response.Write("<p><strong>MD5长度:</strong> " + md5Password.Length + "</p>");
        Response.Write("<hr/>");
        
        // 2. 查询数据库中的用户信息
        Response.Write("<h4>数据库查询结果:</h4>");
        try
        {
            string sql = "SELECT * FROM userinfo WHERE MobilePhone=@MobilePhone";
            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@MobilePhone", mobile));
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Response.Write("<table border='1' cellpadding='5'>");
                Response.Write("<tr><th>字段名</th><th>值</th></tr>");
                foreach (DataColumn col in dt.Columns)
                {
                    string value = row[col.ColumnName] != DBNull.Value ? row[col.ColumnName].ToString() : "NULL";
                    Response.Write("<tr><td>" + col.ColumnName + "</td><td>" + value + "</td></tr>");
                }
                Response.Write("</table>");
                
                string dbPassword = row["Password"] != DBNull.Value ? row["Password"].ToString() : "";
                Response.Write("<p><strong>数据库密码长度:</strong> " + dbPassword.Length + "</p>");
                Response.Write("<p><strong>密码比对结果:</strong> " + (md5Password.Equals(dbPassword, StringComparison.OrdinalIgnoreCase) ? "匹配 ✓" : "不匹配 ✗") + "</p>");
                
                int sysStatus = row["SysStatus"] != DBNull.Value ? Convert.ToInt32(row["SysStatus"]) : -1;
                int isCheck = row["IsCheck"] != DBNull.Value ? Convert.ToInt32(row["IsCheck"]) : -1;
                Response.Write("<p><strong>SysStatus:</strong> " + sysStatus + " (0=正常, 1=删除)</p>");
                Response.Write("<p><strong>IsCheck:</strong> " + isCheck + " (0=未审核, 1=已审核)</p>");
            }
            else
            {
                Response.Write("<p style='color:red'>未找到用户: " + mobile + "</p>");
                Response.Write("<p>正在创建测试用户...</p>");
                
                // 创建测试用户
                CreateTestUser(mobile, password);
            }
        }
        catch (Exception ex)
        {
            Response.Write("<p style='color:red'>数据库连接错误: " + ex.Message + "</p>");
            Response.Write("<p style='color:red'>堆栈: " + ex.StackTrace + "</p>");
        }
    }
    
    private string GetMD5Hash(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
    
    private void CreateTestUser(string mobile, string password)
    {
        try
        {
            string md5Password = GetMD5Hash(password);
            string userGuid = Guid.NewGuid().ToString();
            
            string insertSql = @"
                INSERT INTO userinfo (UserName, Password, UserGuid, SysStatus, LinkMan, MobilePhone, RoseID, IsCheck, CreateTime) 
                VALUES (@UserName, @Password, @UserGuid, 0, @LinkMan, @MobilePhone, 1, 1, GETDATE())
            ";
            
            DbHelper.ExecuteNonQuery(insertSql,
                DbHelper.CreateParameter("@UserName", mobile),
                DbHelper.CreateParameter("@Password", md5Password),
                DbHelper.CreateParameter("@UserGuid", userGuid),
                DbHelper.CreateParameter("@LinkMan", "测试用户"),
                DbHelper.CreateParameter("@MobilePhone", mobile));
            
            Response.Write("<p style='color:green'>测试用户创建成功！</p>");
            Response.Write("<p>用户名: " + mobile + "</p>");
            Response.Write("<p>密码: " + password + "</p>");
            Response.Write("<p>MD5密码: " + md5Password + "</p>");
        }
        catch (Exception ex)
        {
            Response.Write("<p style='color:red'>创建用户失败: " + ex.Message + "</p>");
        }
    }
}