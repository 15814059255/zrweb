using System;
using System.Data;

public partial class addr_simple : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            SaveAddress();
        }
        else if (Request["action"] == "load")
        {
            LoadAddress();
        }
        else
        {
            LoadAddress();
        }
    }
    
    private void LoadAddress()
    {
        Response.ContentType = "application/json";
        
        try
        {
            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"请先登录\"}");
                return;
            }
            
            string sql = "SELECT Province, City, District, Street, Address FROM userinfo WHERE UserID = @userId";
            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@userId", userId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                string p = row["Province"] == null ? "" : row["Province"].ToString();
                string c = row["City"] == null ? "" : row["City"].ToString();
                string d = row["District"] == null ? "" : row["District"].ToString();
                string s = row["Street"] == null ? "" : row["Street"].ToString();
                string a = row["Address"] == null ? "" : row["Address"].ToString();
                
                Response.Write("{\"success\":true,\"province\":\"" + p + "\",\"city\":\"" + c + "\",\"district\":\"" + d + "\",\"street\":\"" + s + "\",\"address\":\"" + a + "\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"用户不存在\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"加载异常: " + ex.Message.Replace("\"", "'") + "\"}");
        }
    }
    
    private void SaveAddress()
    {
        Response.ContentType = "application/json";
        
        try
        {
            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"请先登录\"}");
                return;
            }
            
            string province = Request["province"] ?? "";
            string city = Request["city"] ?? "";
            string district = Request["district"] ?? "";
            string street = Request["street"] ?? "";
            string address = Request["address"] ?? "";
            
            // 检查字段是否存在
            string checkSql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('userinfo', 'U') AND name = 'Province'";
            object checkResult = DbHelper.ExecuteScalar(checkSql);
            
            if (checkResult == null || Convert.ToInt32(checkResult) == 0)
            {
                // 字段不存在，先创建
                try { DbHelper.ExecuteNonQuery("ALTER TABLE userinfo ADD Province nvarchar(50) NULL"); } catch { }
                try { DbHelper.ExecuteNonQuery("ALTER TABLE userinfo ADD City nvarchar(50) NULL"); } catch { }
                try { DbHelper.ExecuteNonQuery("ALTER TABLE userinfo ADD District nvarchar(50) NULL"); } catch { }
                try { DbHelper.ExecuteNonQuery("ALTER TABLE userinfo ADD Street nvarchar(50) NULL"); } catch { }
                try { DbHelper.ExecuteNonQuery("ALTER TABLE userinfo ADD Address nvarchar(255) NULL"); } catch { }
            }
            
            // 保存地址
            string saveSql = "UPDATE userinfo SET Province=@province, City=@city, District=@district, Street=@street, Address=@address WHERE UserID=@userId";
            int rows = DbHelper.ExecuteNonQuery(saveSql,
                DbHelper.CreateParameter("@province", province),
                DbHelper.CreateParameter("@city", city),
                DbHelper.CreateParameter("@district", district),
                DbHelper.CreateParameter("@street", street),
                DbHelper.CreateParameter("@address", address),
                DbHelper.CreateParameter("@userId", userId));
            
            if (rows > 0)
            {
                Response.Write("{\"success\":true,\"message\":\"地址保存成功！省:" + province + " 市:" + city + " 区:" + district + " 街道:" + street + " 地址:" + address + "\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"保存失败，可能用户不存在\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"保存异常: " + ex.Message.Replace("\"", "'") + "\"}");
        }
    }
}
