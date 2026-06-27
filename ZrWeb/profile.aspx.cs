using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Web;

public partial class profile : System.Web.UI.Page
{
    public string PageTitle = "个人资料 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,个人资料,会员中心,公司信息";
    public string PageDescription = "查看和管理您的会员资料信息，完善公司和联系人信息。";
    
    public string CompanyName = "";
    public string ContactName = "";
    public string ContactPhone = "";
    public string OriginalContactPhone = "";
    public string ContactQQ = "";
    public string ContactEmail = "";
    public bool IsCertified = false;
    public bool CompanyModified = false;
    public string MainBrand1 = "";
    public string MainBrand2 = "";
    public string MainBrand3 = "";
    public string MainBrand4 = "";
    public string MainBrand5 = "";
    public string Model1 = "";
    public string Model2 = "";
    public string Model3 = "";
    public string Model4 = "";
    public string Model5 = "";
    public string CompanyDescription = "";
    public string BusinessLicense = "";
    public string IDCard = "";
    public string Province = "";
    public string City = "";
    public string District = "";
    public string Street = "";
    public string Address = "";
    public string UserRoleName = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        // 自动扩大数据库字段长度（只执行一次）
        if (Session["FieldsMigrated"] == null)
        {
            MigrateDatabaseFields();
            Session["FieldsMigrated"] = true;
        }
        
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

    private void MigrateDatabaseFields()
    {
        // 先确保 userinfo 表有必要的字段（ADD COLUMN 如果不存在）
        string[] addColumnSqls = new string[]
        {
            "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('userinfo') AND name = 'CompanyName') ALTER TABLE [dbo].[userinfo] ADD [CompanyName] nvarchar(500) NULL",
            "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('userinfo') AND name = 'Province') ALTER TABLE [dbo].[userinfo] ADD [Province] nvarchar(100) NULL",
            "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('userinfo') AND name = 'City') ALTER TABLE [dbo].[userinfo] ADD [City] nvarchar(100) NULL",
            "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('userinfo') AND name = 'District') ALTER TABLE [dbo].[userinfo] ADD [District] nvarchar(100) NULL",
            "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('userinfo') AND name = 'Street') ALTER TABLE [dbo].[userinfo] ADD [Street] nvarchar(200) NULL",
            "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('userinfo') AND name = 'Address') ALTER TABLE [dbo].[userinfo] ADD [Address] nvarchar(500) NULL",
            "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('userinfo') AND name = 'CompanyModified') ALTER TABLE [dbo].[userinfo] ADD [CompanyModified] int NULL DEFAULT 0"
        };
        
        foreach (string sql in addColumnSqls)
        {
            try
            {
                DbHelper.ExecuteNonQuery(sql);
            }
            catch { }
        }
        
        // 再扩大字段长度
        string[] alterSqls = new string[]
        {
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopName] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopCompany] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopTel] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopImg] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopQQ] nvarchar(255) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopAddress] nvarchar(1000) NULL"
        };
        
        foreach (string sql in alterSqls)
        {
            try
            {
                DbHelper.ExecuteNonQuery(sql);
            }
            catch { }
        }
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
                ContactQQ = "请登录查看";
                CompanyDescription = "请登录查看";
                return;
            }

            // 先从 shops 表读取公司名称（主要数据源）
            try
            {
                DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopId, shopName, shopCompany, shopAddress, shopTel, shopImg, shopQQ FROM [dbo].[shops] WHERE userId = @userId AND dataFlag = 1",
                    DbHelper.CreateParameter("@userId", userId));
                
                if (shopDt != null && shopDt.Rows.Count > 0)
                {
                    DataRow shopRow = shopDt.Rows[0];
                    
                    // shopCompany 是公司名称的主要存储位置
                    string shopCompanyName = GetStringValue(shopRow["shopCompany"]);
                    if (!string.IsNullOrEmpty(shopCompanyName))
                    {
                        CompanyName = shopCompanyName;
                    }
                    
                    // shopName 用于存储主营品牌
                    string mainBrandsStr = GetStringValue(shopRow["shopName"]);
                    string[] brands = mainBrandsStr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    MainBrand1 = brands.Length > 0 ? brands[0] : "";
                    MainBrand2 = brands.Length > 1 ? brands[1] : "";
                    MainBrand3 = brands.Length > 2 ? brands[2] : "";
                    MainBrand4 = brands.Length > 3 ? brands[3] : "";
                    MainBrand5 = brands.Length > 4 ? brands[4] : "";
                    
                    // shopAddress 用于存储公司简介
                    CompanyDescription = GetStringValue(shopRow["shopAddress"]);
                    
                    // shopTel 用于存储优势型号
                    string modelsStr = GetStringValue(shopRow["shopTel"]);
                    string[] models = modelsStr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    Model1 = models.Length > 0 ? models[0] : "";
                    Model2 = models.Length > 1 ? models[1] : "";
                    Model3 = models.Length > 2 ? models[2] : "";
                    Model4 = models.Length > 3 ? models[3] : "";
                    Model5 = models.Length > 4 ? models[4] : "";
                    
                    BusinessLicense = GetStringValue(shopRow["shopImg"]);
                    IDCard = GetStringValue(shopRow["shopQQ"]);
                }
            }
            catch (Exception shopEx)
            {
                System.Diagnostics.Debug.WriteLine("shops表读取异常: " + shopEx.Message);
            }

            // 从 userinfo 表读取联系人和其他信息
            try
            {
                DataTable userDt = DbHelper.ExecuteQuery("SELECT LinkMan, MobilePhone, QQ, Email, CompanyName, Province, City, District, Street, Address, IsCertified, CompanyModified FROM [dbo].[userinfo] WHERE UserID = @userId AND SysStatus = 0",
                    DbHelper.CreateParameter("@userId", userId));
                
                if (userDt != null && userDt.Rows.Count > 0)
                {
                    DataRow userRow = userDt.Rows[0];
                    ContactName = GetStringValue(userRow["LinkMan"]);
                    OriginalContactPhone = GetStringValue(userRow["MobilePhone"]);
                    ContactPhone = OriginalContactPhone;
                    ContactQQ = GetStringValue(userRow["QQ"]);
                    ContactEmail = GetStringValue(userRow["Email"]);
                    IsCertified = GetIntValue(userRow["IsCertified"], 0) == 1;
                    CompanyModified = GetIntValue(userRow["CompanyModified"], 0) == 1;
                    
                    Province = GetStringValue(userRow["Province"]);
                    City = GetStringValue(userRow["City"]);
                    District = GetStringValue(userRow["District"]);
                    Street = GetStringValue(userRow["Street"]);
                    Address = GetStringValue(userRow["Address"]);
                    
                    // 如果 shops 表没有公司名称，从 userinfo 表读取作为备选
                    string userCompanyName = GetStringValue(userRow["CompanyName"]);
                    if (string.IsNullOrEmpty(CompanyName) && !string.IsNullOrEmpty(userCompanyName))
                    {
                        CompanyName = userCompanyName;
                    }
                    
                    if (!string.IsNullOrEmpty(ContactPhone) && ContactPhone.Length >= 11)
                    {
                        ContactPhone = ContactPhone.Substring(0, 3) + "****" + ContactPhone.Substring(7);
                    }
                }
            }
            catch (Exception userEx)
            {
                System.Diagnostics.Debug.WriteLine("userinfo表读取异常: " + userEx.Message);
            }

            // 获取用户类型
            int roseId = UserHelper.GetRoseId();
            switch (roseId)
            {
                case 2:
                    UserRoleName = "采购商";
                    break;
                case 3:
                    UserRoleName = "供应商";
                    break;
                default:
                    UserRoleName = "普通用户";
                    break;
            }

            // 如果公司名称仍然为空，显示提示
            if (string.IsNullOrEmpty(CompanyName))
            {
                CompanyName = "请完善公司信息";
            }
            
            if (string.IsNullOrEmpty(CompanyDescription))
            {
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
            
            if (string.IsNullOrEmpty(ContactQQ))
            {
                ContactQQ = "请完善QQ号码";
            }
        }
        catch (Exception ex)
        {
            CompanyName = "请完善公司信息";
            ContactName = "请完善联系人信息";
            ContactPhone = "请完善联系电话";
            ContactQQ = "请完善QQ号码";
            CompanyDescription = "暂无公司简介";
            System.Diagnostics.Debug.WriteLine("LoadUserProfile异常: " + ex.Message);
        }
    }

    private string GetStringValue(object value)
    {
        return value != null && value != DBNull.Value ? value.ToString().Trim() : "";
    }
    
    private int GetIntValue(object value, int defaultValue)
    {
        if (value == null || value == DBNull.Value)
            return defaultValue;
        int result;
        return int.TryParse(value.ToString(), out result) ? result : defaultValue;
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
                Response.Flush();
                Response.End();
                return;
            }

            List<string> errors = new List<string>();

            string contactName = Request["contactName"] ?? "";
            string contactQQ = Request["contactQQ"] ?? "";
            string contactEmail = Request["contactEmail"] ?? "";
            string province = Request["province"] ?? "";
            string city = Request["city"] ?? "";
            string district = Request["district"] ?? "";
            string street = Request["street"] ?? "";
            string addressDetail = Request["address"] ?? "";

            bool success = false;

            bool isCertified = false;
            bool companyModified = false;
            try
            {
                DataTable certDt = DbHelper.ExecuteQuery("SELECT IsCertified, CompanyModified FROM [dbo].[userinfo] WHERE UserID = @userId",
                    DbHelper.CreateParameter("@userId", userId));
                if (certDt != null && certDt.Rows.Count > 0)
                {
                    isCertified = GetIntValue(certDt.Rows[0]["IsCertified"], 0) == 1;
                    companyModified = GetIntValue(certDt.Rows[0]["CompanyModified"], 0) == 1;
                }
            }
            catch { }

            string companyName = Request["companyName"] ?? "";

            // 如果 companyName 为空（可能是因为字段是 disabled 状态），从数据库读取原值
            if (string.IsNullOrEmpty(companyName))
            {
                try
                {
                    DataTable dt = DbHelper.ExecuteQuery("SELECT CompanyName FROM [dbo].[userinfo] WHERE UserID = @userId",
                        DbHelper.CreateParameter("@userId", userId));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        companyName = GetStringValue(dt.Rows[0]["CompanyName"]);
                    }
                }
                catch { }
            }

            // 保存联系人信息和地址信息（手机号不可修改）
            try
            {
                string updateUserSql = "UPDATE [dbo].[userinfo] SET LinkMan = @linkMan, QQ = @qq, Email = @email, CompanyName = @companyName, Province = @province, City = @city, District = @district, Street = @street, Address = @address WHERE UserID = @userId";
                int userRows = DbHelper.ExecuteNonQuery(updateUserSql,
                    DbHelper.CreateParameter("@linkMan", contactName),
                    DbHelper.CreateParameter("@qq", contactQQ),
                    DbHelper.CreateParameter("@email", contactEmail),
                    DbHelper.CreateParameter("@companyName", companyName),
                    DbHelper.CreateParameter("@province", province),
                    DbHelper.CreateParameter("@city", city),
                    DbHelper.CreateParameter("@district", district),
                    DbHelper.CreateParameter("@street", street),
                    DbHelper.CreateParameter("@address", addressDetail),
                    DbHelper.CreateParameter("@userId", userId));
                success = userRows > 0;
                if (!success)
                {
                    errors.Add("联系人信息未更新（可能没有匹配的用户记录）");
                }
            }
            catch (Exception ex)
            {
                errors.Add("联系人信息保存失败: " + ex.Message);
                success = false;
            }

            string message = success ? "保存成功" : "保存失败";
            
            // 保存公司信息到 shops 表
            try
            {
                string mainBrand1 = Request["mainBrand1"] ?? "";
                string mainBrand2 = Request["mainBrand2"] ?? "";
                string mainBrand3 = Request["mainBrand3"] ?? "";
                string mainBrand4 = Request["mainBrand4"] ?? "";
                string mainBrand5 = Request["mainBrand5"] ?? "";
                string model1 = Request["model1"] ?? "";
                string model2 = Request["model2"] ?? "";
                string model3 = Request["model3"] ?? "";
                string model4 = Request["model4"] ?? "";
                string model5 = Request["model5"] ?? "";
                string companyDescription = Request["companyDescription"] ?? "";

                string mainBrands = "";
                if (!string.IsNullOrEmpty(mainBrand1)) mainBrands = mainBrand1;
                if (!string.IsNullOrEmpty(mainBrand2)) mainBrands += "|" + mainBrand2;
                if (!string.IsNullOrEmpty(mainBrand3)) mainBrands += "|" + mainBrand3;
                if (!string.IsNullOrEmpty(mainBrand4)) mainBrands += "|" + mainBrand4;
                if (!string.IsNullOrEmpty(mainBrand5)) mainBrands += "|" + mainBrand5;

                string models = "";
                if (!string.IsNullOrEmpty(model1)) models = model1;
                if (!string.IsNullOrEmpty(model2)) models += "|" + model2;
                if (!string.IsNullOrEmpty(model3)) models += "|" + model3;
                if (!string.IsNullOrEmpty(model4)) models += "|" + model4;
                if (!string.IsNullOrEmpty(model5)) models += "|" + model5;

                // 处理图片上传
                string businessLicensePath = "";
                string idCardPath = "";
                if (!isCertified)
                {
                    HttpPostedFile businessFile = Request.Files["businessLicense"];
                    if (businessFile != null && businessFile.ContentLength > 0)
                    {
                        businessLicensePath = SaveCertFile(businessFile, userId);
                    }
                    HttpPostedFile idCardFile = Request.Files["idCard"];
                    if (idCardFile != null && idCardFile.ContentLength > 0)
                    {
                        idCardPath = SaveCertFile(idCardFile, userId);
                    }
                }
                
                // 更新公司修改标记（如果公司名称已填写且之前未修改过）
                if (!companyModified && !string.IsNullOrEmpty(companyName))
                {
                    DbHelper.ExecuteNonQuery("UPDATE [dbo].[userinfo] SET CompanyModified = 1 WHERE UserID = @userId",
                        DbHelper.CreateParameter("@userId", userId));
                }

                // 保存公司信息到 shops 表
                DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopId, shopImg, shopQQ, shopCompany FROM [dbo].[shops] WHERE userId = @userId AND dataFlag = 1",
                    DbHelper.CreateParameter("@userId", userId));

                if (shopDt != null && shopDt.Rows.Count > 0)
                {
                    int shopId = Convert.ToInt32(shopDt.Rows[0]["shopId"]);
                    
                    string existingLicense = GetStringValue(shopDt.Rows[0]["shopImg"]);
                    string existingIdCard = GetStringValue(shopDt.Rows[0]["shopQQ"]);
                    
                    // 如果 companyName 为空，从 shops 表读取原值
                    string existingCompany = GetStringValue(shopDt.Rows[0]["shopCompany"]);
                    if (string.IsNullOrEmpty(companyName))
                    {
                        companyName = existingCompany;
                    }
                    
                    if (!isCertified && !string.IsNullOrEmpty(businessLicensePath))
                    {
                        existingLicense = businessLicensePath;
                    }
                    if (!isCertified && !string.IsNullOrEmpty(idCardPath))
                    {
                        existingIdCard = idCardPath;
                    }

                    string updateShopSql = "UPDATE [dbo].[shops] SET shopName = @shopName, shopCompany = @shopCompany, shopAddress = @shopAddress, shopTel = @shopTel, shopImg = @shopImg, shopQQ = @shopQQ WHERE shopId = @shopId";
                    int shopRows = DbHelper.ExecuteNonQuery(updateShopSql,
                        DbHelper.CreateParameter("@shopName", mainBrands),
                        DbHelper.CreateParameter("@shopCompany", companyName),
                        DbHelper.CreateParameter("@shopAddress", companyDescription),
                        DbHelper.CreateParameter("@shopTel", models),
                        DbHelper.CreateParameter("@shopImg", string.IsNullOrEmpty(existingLicense) ? DBNull.Value : (object)existingLicense),
                        DbHelper.CreateParameter("@shopQQ", string.IsNullOrEmpty(existingIdCard) ? DBNull.Value : (object)existingIdCard),
                        DbHelper.CreateParameter("@shopId", shopId));
                    if (shopRows == 0)
                    {
                        errors.Add("公司信息未更新（可能没有匹配的记录）");
                    }
                }
                else
                {
                    string insertShopSql = "INSERT INTO [dbo].[shops] (userId, shopName, shopCompany, shopAddress, shopTel, shopImg, shopQQ, telephone, shopStatus, dataFlag) VALUES (@userId, @shopName, @shopCompany, @shopAddress, @shopTel, @shopImg, @shopQQ, @telephone, @shopStatus, @dataFlag)";
                    int shopRows = DbHelper.ExecuteNonQuery(insertShopSql,
                        DbHelper.CreateParameter("@userId", userId),
                        DbHelper.CreateParameter("@shopName", mainBrands),
                        DbHelper.CreateParameter("@shopCompany", companyName),
                        DbHelper.CreateParameter("@shopAddress", companyDescription),
                        DbHelper.CreateParameter("@shopTel", models),
                        DbHelper.CreateParameter("@shopImg", string.IsNullOrEmpty(businessLicensePath) ? DBNull.Value : (object)businessLicensePath),
                        DbHelper.CreateParameter("@shopQQ", string.IsNullOrEmpty(idCardPath) ? DBNull.Value : (object)idCardPath),
                        DbHelper.CreateParameter("@telephone", ""),
                        DbHelper.CreateParameter("@shopStatus", 0),
                        DbHelper.CreateParameter("@dataFlag", 1));
                    if (shopRows == 0)
                    {
                        errors.Add("公司信息插入失败");
                    }
                }
            }
            catch (Exception shopEx)
            {
                errors.Add("公司信息保存异常: " + shopEx.Message);
                System.Diagnostics.Debug.WriteLine("shops保存异常: " + shopEx.Message);
            }

            Response.Clear();
            Response.ContentType = "application/json";
            
            if (errors.Count > 0)
            {
                message = "保存失败: " + string.Join("; ", errors);
                success = false;
            }
            
            Response.Write("{\"success\":" + (success ? "true" : "false") + ",\"message\":\"" + message.Replace("\"", "\\\"") + "\"}");
            Response.Flush();
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
                Response.End();
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
                Response.End();
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

    private string SaveCertFile(System.Web.HttpPostedFile file, int userId)
    {
        if (file == null || file.ContentLength == 0)
        {
            return "";
        }

        string fileName = Path.GetFileName(file.FileName);
        string extension = Path.GetExtension(fileName).ToLower();
        string allowedExtensions = ".jpg,.jpeg,.png,.pdf";
        
        if (!allowedExtensions.Contains(extension))
        {
            return "";
        }

        if (file.ContentLength > 5 * 1024 * 1024)
        {
            return "";
        }

        string uploadPath = Server.MapPath("~/uploads/certs/");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        string newFileName = userId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
        string savePath = Path.Combine(uploadPath, newFileName);
        
        file.SaveAs(savePath);
        
        return "/uploads/certs/" + newFileName;
    }
}