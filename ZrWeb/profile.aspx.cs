using System;
using System.Data;
using System.Threading;

public partial class profile : System.Web.UI.Page
{
    public string PageTitle = "个人资料 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,个人资料,会员中心,公司信息";
    public string PageDescription = "查看和管理您的会员资料信息，完善公司和联系人信息。";
    
    public string CompanyName = "";
    public string ContactName = "";
    public string ContactPhone = "";
    public string MainBrands = "";
    public string BusinessCapability = "";
    public string CompanyDescription = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            if (Request["action"] == "save_profile")
            {
                SaveProfile();
                return;
            }
            else if (Request["action"] == "change_password")
            {
                ChangePassword();
                return;
            }
        }
        
        LoadUserProfile();
    }

    private void LoadUserProfile()
    {
        try
        {
            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                CompanyName = "请登录查看";
                ContactName = "请登录查看";
                ContactPhone = "请登录查看";
                CompanyDescription = "请登录查看";
                return;
            }

            DataTable userDt = DbHelper.ExecuteQuery("SELECT LinkMan, MobilePhone FROM userinfo WHERE UserID = @userId AND SysStatus = 0",
                DbHelper.CreateParameter("@userId", userId));
            
            if (userDt != null && userDt.Rows.Count > 0)
            {
                DataRow userRow = userDt.Rows[0];
                ContactName = GetStringValue(userRow["LinkMan"]);
                ContactPhone = GetStringValue(userRow["MobilePhone"]);
                
                if (!string.IsNullOrEmpty(ContactPhone) && ContactPhone.Length >= 11)
                {
                    ContactPhone = ContactPhone.Substring(0, 3) + "****" + ContactPhone.Substring(7);
                }
            }

            DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopName, shopCompany, shopAddress FROM shops WHERE userId = @userId AND dataFlag = 1",
                DbHelper.CreateParameter("@userId", userId));
            
            if (shopDt != null && shopDt.Rows.Count > 0)
            {
                DataRow shopRow = shopDt.Rows[0];
                CompanyName = GetStringValue(shopRow["shopName"]);
                if (string.IsNullOrEmpty(CompanyName))
                {
                    CompanyName = GetStringValue(shopRow["shopCompany"]);
                }
                
                if (string.IsNullOrEmpty(CompanyName))
                {
                    CompanyName = "请完善店铺信息";
                }
                
                CompanyDescription = GetStringValue(shopRow["shopAddress"]);
                if (string.IsNullOrEmpty(CompanyDescription))
                {
                    CompanyDescription = "暂无公司简介";
                }
            }
            else
            {
                CompanyName = "请完善店铺信息";
                CompanyDescription = "暂无公司简介";
            }

            if (string.IsNullOrEmpty(ContactName))
            {
                ContactName = "请完善联系人信息";
            }
            
            if (string.IsNullOrEmpty(ContactPhone))
            {
                ContactPhone = "请完善联系电话";
            }
        }
        catch (Exception ex)
        {
            CompanyName = "加载失败";
            ContactName = "加载失败";
            ContactPhone = "加载失败";
            CompanyDescription = "加载失败";
        }
    }

    private string GetStringValue(object value)
    {
        return value != null && value != DBNull.Value ? value.ToString().Trim() : "";
    }

    private void SaveProfile()
    {
        try
        {
            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write("{\"success\":false,\"message\":\"请先登录\"}");
                Response.End();
                return;
            }

            string companyName = Request["companyName"] ?? "";
            string mainBrands = Request["mainBrands"] ?? "";
            string businessCapability = Request["businessCapability"] ?? "";
            string companyDescription = Request["companyDescription"] ?? "";
            string contactName = Request["contactName"] ?? "";
            string contactPhone = Request["contactPhone"] ?? "";

            bool success = false;
            int affectedRows = 0;

            if (!string.IsNullOrEmpty(contactName) || !string.IsNullOrEmpty(contactPhone))
            {
                string updateUserSql = "UPDATE userinfo SET LinkMan = @linkMan, MobilePhone = @mobilePhone WHERE UserID = @userId";
                affectedRows = DbHelper.ExecuteNonQuery(updateUserSql,
                    DbHelper.CreateParameter("@linkMan", contactName),
                    DbHelper.CreateParameter("@mobilePhone", contactPhone),
                    DbHelper.CreateParameter("@userId", userId));
                success = success || affectedRows > 0;
            }

            if (!string.IsNullOrEmpty(companyName) || !string.IsNullOrEmpty(mainBrands) || 
                !string.IsNullOrEmpty(businessCapability) || !string.IsNullOrEmpty(companyDescription))
            {
                DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopId FROM shops WHERE userId = @userId AND dataFlag = 1",
                    DbHelper.CreateParameter("@userId", userId));
                
                if (shopDt != null && shopDt.Rows.Count > 0)
                {
                    int shopId = Convert.ToInt32(shopDt.Rows[0]["shopId"]);
                    string updateShopSql = "UPDATE shops SET shopName = @shopName, shopCompany = @shopCompany, shopAddress = @shopAddress WHERE shopId = @shopId";
                    affectedRows = DbHelper.ExecuteNonQuery(updateShopSql,
                        DbHelper.CreateParameter("@shopName", companyName),
                        DbHelper.CreateParameter("@shopCompany", companyName),
                        DbHelper.CreateParameter("@shopAddress", companyDescription),
                        DbHelper.CreateParameter("@shopId", shopId));
                    success = success || affectedRows > 0;
                }
            }

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write(success ? "{\"success\":true,\"message\":\"保存成功\"}" : "{\"success\":false,\"message\":\"保存失败\"}");
            Response.End();
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write("{\"success\":false,\"message\":\"保存异常: " + ex.Message.Replace("\"", "\\\"") + "\"}");
            Response.End();
        }
    }

    private void ChangePassword()
    {
        try
        {
            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                WriteJsonResponse(false, "请先登录");
                return;
            }

            string oldPassword = Request["oldPassword"] ?? "";
            string newPassword = Request["newPassword"] ?? "";
            string confirmPassword = Request["confirmPassword"] ?? "";

            if (string.IsNullOrEmpty(oldPassword))
            {
                WriteJsonResponse(false, "请输入原密码");
                return;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                WriteJsonResponse(false, "请输入新密码");
                return;
            }

            if (newPassword.Length < 6)
            {
                WriteJsonResponse(false, "新密码长度不能少于6位");
                return;
            }

            if (newPassword != confirmPassword)
            {
                WriteJsonResponse(false, "两次输入的密码不一致");
                return;
            }

            DataTable userDt = DbHelper.ExecuteQuery("SELECT Password FROM userinfo WHERE UserID = @userId AND SysStatus = 0",
                DbHelper.CreateParameter("@userId", userId));
            
            if (userDt == null || userDt.Rows.Count == 0)
            {
                WriteJsonResponse(false, "用户不存在");
                return;
            }

            string currentPassword = GetStringValue(userDt.Rows[0]["Password"]);
            
            if (oldPassword != currentPassword)
            {
                WriteJsonResponse(false, "原密码不正确");
                return;
            }

            string updateSql = "UPDATE userinfo SET Password = @newPassword WHERE UserID = @userId";
            int affectedRows = DbHelper.ExecuteNonQuery(updateSql,
                DbHelper.CreateParameter("@newPassword", newPassword),
                DbHelper.CreateParameter("@userId", userId));

            if (affectedRows > 0)
            {
                WriteJsonResponse(true, "密码修改成功");
            }
            else
            {
                WriteJsonResponse(false, "密码修改失败");
            }
        }
        catch (ThreadAbortException)
        {
            throw;
        }
        catch (Exception ex)
        {
            try
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write("{\"success\":false,\"message\":\"修改异常: " + ex.Message.Replace("\"", "\\\"") + "\"}");
                Response.Flush();
                Response.End();
            }
            catch (ThreadAbortException)
            {
            }
        }
    }

    private void WriteJsonResponse(bool success, string message)
    {
        try
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";
            Response.Write("{\"success\":" + (success ? "true" : "false") + ",\"message\":\"" + message.Replace("\"", "\\\"") + "\"}");
            Response.Flush();
            Response.End();
        }
        catch (ThreadAbortException)
        {
        }
    }
}