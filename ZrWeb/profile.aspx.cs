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
    public string MainBrand1 = "";
    public string MainBrand2 = "";
    public string MainBrand3 = "";
    public string MainBrand4 = "";
    public string MainBrand5 = "";
    public string CompanyDescription = "";
    public string Province = "";
    public string City = "";
    public string District = "";
    public string Street = "";
    public string Address = "";

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

            // 先查询基本信息（不包含可能不存在的新字段）
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

            // 尝试加载地址信息（如果字段存在）
            try
            {
                string addrCheckSql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('userinfo', 'U') AND name = 'Province'";
                object checkResult = DbHelper.ExecuteScalar(addrCheckSql);
                if (checkResult != null && Convert.ToInt32(checkResult) > 0)
                {
                    string addrSql = "SELECT Province, City, District, Street, Address FROM userinfo WHERE UserID = @userId";
                    DataTable addrDt = DbHelper.ExecuteQuery(addrSql,
                        DbHelper.CreateParameter("@userId", userId));
                    
                    if (addrDt != null && addrDt.Rows.Count > 0)
                    {
                        DataRow addrRow = addrDt.Rows[0];
                        Province = GetStringValue(addrRow["Province"]);
                        City = GetStringValue(addrRow["City"]);
                        District = GetStringValue(addrRow["District"]);
                        Street = GetStringValue(addrRow["Street"]);
                        Address = GetStringValue(addrRow["Address"]);
                        
                        // 调试日志（可在日志文件中查看）
                        System.Diagnostics.Debug.WriteLine("加载地址: Province=" + Province + ", City=" + City + ", District=" + District + ", Street=" + Street + ", Address=" + Address);
                    }
                }
            }
            catch
            {
                // 地址字段可能不存在，忽略错误
            }

            DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopName, shopCompany, shopAddress FROM shops WHERE userId = @userId AND dataFlag = 1",
                DbHelper.CreateParameter("@userId", userId));
            
            if (shopDt != null && shopDt.Rows.Count > 0)
            {
                DataRow shopRow = shopDt.Rows[0];
                // shopName 用于存储主营品牌（多个品牌用|分隔）
                string mainBrandsStr = GetStringValue(shopRow["shopName"]);
                string[] brands = mainBrandsStr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                MainBrand1 = brands.Length > 0 ? brands[0] : "";
                MainBrand2 = brands.Length > 1 ? brands[1] : "";
                MainBrand3 = brands.Length > 2 ? brands[2] : "";
                MainBrand4 = brands.Length > 3 ? brands[3] : "";
                MainBrand5 = brands.Length > 4 ? brands[4] : "";
                
                // shopCompany 用于存储公司名称
                CompanyName = GetStringValue(shopRow["shopCompany"]);
                
                if (string.IsNullOrEmpty(CompanyName))
                {
                    CompanyName = "请完善公司信息";
                }
                
                // shopAddress 用于存储公司简介
                CompanyDescription = GetStringValue(shopRow["shopAddress"]);
                if (string.IsNullOrEmpty(CompanyDescription))
                {
                    CompanyDescription = "暂无公司简介";
                }
            }
            else
            {
                CompanyName = "请完善公司信息";
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
    
    // JavaScript 字符串转义
    protected string jsEncode(string str)
    {
        if (string.IsNullOrEmpty(str)) return "";
        return str
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
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

            string contactName = Request["contactName"] ?? "";
            string contactPhone = Request["contactPhone"] ?? "";
            string province = Request["province"] ?? "";
            string city = Request["city"] ?? "";
            string district = Request["district"] ?? "";
            string street = Request["street"] ?? "";
            string addressDetail = Request["address"] ?? "";

            // 1. 保存联系人信息（必须成功）
            string updateUserSql = "UPDATE userinfo SET LinkMan = @linkMan, MobilePhone = @mobilePhone WHERE UserID = @userId";
            int userRows = DbHelper.ExecuteNonQuery(updateUserSql,
                DbHelper.CreateParameter("@linkMan", contactName),
                DbHelper.CreateParameter("@mobilePhone", contactPhone),
                DbHelper.CreateParameter("@userId", userId));
            
            bool success = userRows > 0;
            string message = success ? "保存成功" : "保存失败";

            // 2. 保存地址信息（必须保存）
            string addrMessage = "";
            try
            {
                string addrCheckSql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('userinfo', 'U') AND name = 'Province'";
                object checkResult = DbHelper.ExecuteScalar(addrCheckSql);
                
                if (checkResult == null || Convert.ToInt32(checkResult) == 0)
                {
                    // 字段不存在，需要先添加
                    addrMessage = "地址字段不存在，请联系管理员添加数据库字段";
                }
                else
                {
                    string updateAddrSql = "UPDATE userinfo SET Province = @province, City = @city, District = @district, Street = @street, Address = @address WHERE UserID = @userId";
                    int addrRows = DbHelper.ExecuteNonQuery(updateAddrSql,
                        DbHelper.CreateParameter("@province", province),
                        DbHelper.CreateParameter("@city", city),
                        DbHelper.CreateParameter("@district", district),
                        DbHelper.CreateParameter("@street", street),
                        DbHelper.CreateParameter("@address", addressDetail),
                        DbHelper.CreateParameter("@userId", userId));
                    
                    if (addrRows > 0)
                    {
                        addrMessage = "地址保存成功";
                    }
                    else
                    {
                        addrMessage = "地址保存失败";
                    }
                }
            }
            catch (Exception addrEx)
            {
                addrMessage = "地址保存异常：" + addrEx.Message;
            }
            
            // 如果联系人保存成功但地址有问题，显示提示
            if (success && !string.IsNullOrEmpty(addrMessage) && !addrMessage.Contains("成功"))
            {
                message = "联系人保存成功，但" + addrMessage;
            }

            // 3. 尝试保存公司信息到 shops 表（可选）
            try
            {
                string companyName = Request["companyName"] ?? "";
                string mainBrand1 = Request["mainBrand1"] ?? "";
                string mainBrand2 = Request["mainBrand2"] ?? "";
                string mainBrand3 = Request["mainBrand3"] ?? "";
                string mainBrand4 = Request["mainBrand4"] ?? "";
                string mainBrand5 = Request["mainBrand5"] ?? "";
                string companyDescription = Request["companyDescription"] ?? "";
                
                // 合并5个品牌为用|分隔的字符串
                string mainBrands = "";
                if (!string.IsNullOrEmpty(mainBrand1)) mainBrands = mainBrand1;
                if (!string.IsNullOrEmpty(mainBrand2)) mainBrands += "|" + mainBrand2;
                if (!string.IsNullOrEmpty(mainBrand3)) mainBrands += "|" + mainBrand3;
                if (!string.IsNullOrEmpty(mainBrand4)) mainBrands += "|" + mainBrand4;
                if (!string.IsNullOrEmpty(mainBrand5)) mainBrands += "|" + mainBrand5;
                
                if (!string.IsNullOrEmpty(companyName) || !string.IsNullOrEmpty(mainBrands))
                {
                    DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopId FROM shops WHERE userId = @userId AND dataFlag = 1",
                        DbHelper.CreateParameter("@userId", userId));
                    
                    if (shopDt != null && shopDt.Rows.Count > 0)
                    {
                        int shopId = Convert.ToInt32(shopDt.Rows[0]["shopId"]);
                        string updateShopSql = "UPDATE shops SET shopName = @shopName, shopCompany = @shopCompany, shopAddress = @shopAddress WHERE shopId = @shopId";
                        DbHelper.ExecuteNonQuery(updateShopSql,
                            DbHelper.CreateParameter("@shopName", mainBrands),
                            DbHelper.CreateParameter("@shopCompany", companyName),
                            DbHelper.CreateParameter("@shopAddress", companyDescription),
                            DbHelper.CreateParameter("@shopId", shopId));
                    }
                }
            }
            catch
            {
                // shops 表保存失败不影响主保存
            }

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write("{\"success\":" + (success ? "true" : "false") + ",\"message\":\"" + message + "\"}");
            Response.End();
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            try
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write("{\"success\":false,\"message\":\"异常: " + EscapeJsonString(ex.Message) + "\"}");
                Response.Flush();
            }
            catch { }
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
        }
        catch (Exception ex)
        {
            try
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Write("{\"success\":false,\"message\":\"修改异常: " + EscapeJsonString(ex.Message) + "\"}");
                Response.Flush();
            }
            catch { }
        }
    }

    private string EscapeJsonString(string str)
    {
        if (string.IsNullOrEmpty(str)) return "";
        return str
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\b", "\\b")
            .Replace("\f", "\\f");
    }

    private void WriteJsonResponse(bool success, string message)
    {
        try
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";
            Response.Write("{\"success\":" + (success ? "true" : "false") + ",\"message\":\"" + EscapeJsonString(message) + "\"}");
            Response.Flush();
            Response.End();
        }
        catch (ThreadAbortException)
        {
        }
    }
}