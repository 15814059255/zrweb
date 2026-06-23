using System;
using System.Data;

public partial class profile_debug : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request["action"] ?? "";
        
        switch (action)
        {
            case "check_fields":
                CheckFields();
                break;
            case "load_address":
                LoadAddress();
                break;
            case "test_save":
                TestSave();
                break;
        }
    }
    
    private void CheckFields()
    {
        Response.ContentType = "application/json";
        try
        {
            string sql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('userinfo', 'U') AND name IN ('Province', 'City', 'District', 'Street', 'Address')";
            object result = DbHelper.ExecuteScalar(sql);
            bool hasFields = result != null && Convert.ToInt32(result) >= 5;
            
            object addrData = null;
            if (hasFields)
            {
                int userId = UserHelper.GetUserId();
                if (userId > 0)
                {
                    DataTable dt = DbHelper.ExecuteQuery(
                        "SELECT Province, City, District, Street, Address FROM userinfo WHERE UserID = @userId",
                        DbHelper.CreateParameter("@userId", userId));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        addrData = new {
                            Province = row["Province"]?.ToString() ?? "",
                            City = row["City"]?.ToString() ?? "",
                            District = row["District"]?.ToString() ?? "",
                            Street = row["Street"]?.ToString() ?? "",
                            Address = row["Address"]?.ToString() ?? ""
                        };
                    }
                }
            }
            
            string json = hasFields 
                ? "{\"hasFields\":true,\"currentAddress\":" + (addrData != null ? ToJson(addrData) : "null") + "}"
                : "{\"hasFields\":false}";
            
            Response.Write(json);
        }
        catch (Exception ex)
        {
            Response.Write("{\"hasFields\":false,\"error\":\"" + ex.Message.Replace("\"", "'") + "\"}");
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
                Response.Write("{\"error\":\"请先登录\"}");
                return;
            }
            
            string sql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('userinfo', 'U') AND name = 'Province'";
            object result = DbHelper.ExecuteScalar(sql);
            
            if (result == null || Convert.ToInt32(result) == 0)
            {
                Response.Write("{\"error\":\"字段不存在\"}");
                return;
            }
            
            DataTable dt = DbHelper.ExecuteQuery(
                "SELECT Province, City, District, Street, Address FROM userinfo WHERE UserID = @userId",
                DbHelper.CreateParameter("@userId", userId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                string json = "{\"Province\":\"" + (row["Province"]?.ToString() ?? "") + "\",";
                json += "\"City\":\"" + (row["City"]?.ToString() ?? "") + "\",";
                json += "\"District\":\"" + (row["District"]?.ToString() ?? "") + "\",";
                json += "\"Street\":\"" + (row["Street"]?.ToString() ?? "") + "\",";
                json += "\"Address\":\"" + (row["Address"]?.ToString() ?? "") + "\"}";
                Response.Write(json);
            }
            else
            {
                Response.Write("{\"error\":\"用户不存在或未找到数据\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"error\":\"" + ex.Message.Replace("\"", "'") + "\"}");
        }
    }
    
    private void TestSave()
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
            string sql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('userinfo', 'U') AND name = 'Province'";
            object result = DbHelper.ExecuteScalar(sql);
            
            if (result == null || Convert.ToInt32(result) == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"数据库字段不存在，请先执行ALTER TABLE添加字段\"}");
                return;
            }
            
            // 执行保存
            string updateSql = "UPDATE userinfo SET Province = @province, City = @city, District = @district, Street = @street, Address = @address WHERE UserID = @userId";
            int rows = DbHelper.ExecuteNonQuery(updateSql,
                DbHelper.CreateParameter("@province", province),
                DbHelper.CreateParameter("@city", city),
                DbHelper.CreateParameter("@district", district),
                DbHelper.CreateParameter("@street", street),
                DbHelper.CreateParameter("@address", address),
                DbHelper.CreateParameter("@userId", userId));
            
            if (rows > 0)
            {
                Response.Write("{\"success\":true,\"message\":\"保存成功\",\"data\":{\"province\":\"" + province + "\",\"city\":\"" + city + "\",\"district\":\"" + district + "\",\"street\":\"" + street + "\",\"address\":\"" + address + "\"}}");
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
    
    private string ToJson(object obj)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    }
}
