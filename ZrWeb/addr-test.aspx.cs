using System;

public partial class addr_test : System.Web.UI.Page
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
            
            // 检查字段
            string checkSql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('userinfo', 'U') AND name = 'Province'";
            object checkResult = DbHelper.ExecuteScalar(checkSql);
            
            if (checkResult == null || Convert.ToInt32(checkResult) == 0)
            {
                // 字段不存在，先创建
                string[] addFields = new string[] {
                    "ALTER TABLE userinfo ADD Province nvarchar(50)",
                    "ALTER TABLE userinfo ADD City nvarchar(50)",
                    "ALTER TABLE userinfo ADD District nvarchar(50)",
                    "ALTER TABLE userinfo ADD Street nvarchar(50)",
                    "ALTER TABLE userinfo ADD Address nvarchar(255)"
                };
                
                foreach (string sql in addFields)
                {
                    try { DbHelper.ExecuteNonQuery(sql); } catch { }
                }
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
            
            string result = rows > 0 
                ? "{\"success\":true,\"message\":\"地址保存成功\",\"debug\":{\"province\":\"" + province + "\",\"city\":\"" + city + "\",\"district\":\"" + district + "\",\"street\":\"" + street + "\",\"address\":\"" + address + "\"}}"
                : "{\"success\":false,\"message\":\"保存失败，可能用户不存在\"}";
            
            Response.Write(result);
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"异常: " + ex.Message.Replace("\"", "'") + "\"}");
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
            
            string sql = "SELECT Province, City, District, Street, Address FROM userinfo WHERE UserID=@userId";
            System.Data.DataTable dt = DbHelper.ExecuteQuery(sql,
                DbHelper.CreateParameter("@userId", userId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                string json = "{\"success\":true,\"province\":\"" + (row["Province"] ?? "") + "\",\"city\":\"" + (row["City"] ?? "") + "\",\"district\":\"" + (row["District"] ?? "") + "\",\"street\":\"" + (row["Street"] ?? "") + "\",\"address\":\"" + (row["Address"] ?? "") + "\"}";
                Response.Write(json);
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"用户不存在\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"异常: " + ex.Message.Replace("\"", "'") + "\"}");
        }
    }
}
