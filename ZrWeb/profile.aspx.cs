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
                ContactQQ = "请登录查看";
                CompanyDescription = "请登录查看";
                return;
            }

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
                
                // 先从 userinfo 表读取公司名称作为默认值
                string userCompanyName = GetStringValue(userRow["CompanyName"]);
                if (!string.IsNullOrEmpty(userCompanyName))
                {
                    CompanyName = userCompanyName;
                }
                
                Province = GetStringValue(userRow["Province"]);
                City = GetStringValue(userRow["City"]);
                District = GetStringValue(userRow["District"]);
                Street = GetStringValue(userRow["Street"]);
                Address = GetStringValue(userRow["Address"]);
                
                if (!string.IsNullOrEmpty(ContactPhone) && ContactPhone.Length >= 11)
                {
                    ContactPhone = ContactPhone.Substring(0, 3) + "****" + ContactPhone.Substring(7);
                }
            }

            // 加载公司信息
            try
            {
                DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopName, shopCompany, shopAddress, shopTel, shopImg, shopQQ FROM [dbo].[shops] WHERE userId = @userId AND dataFlag = 1",
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
                    string shopCompanyName = GetStringValue(shopRow["shopCompany"]);
                    if (!string.IsNullOrEmpty(shopCompanyName))
                    {
                        CompanyName = shopCompanyName;
                    }
                    // 如果CompanyName仍然为空（既没有 shops 也没有 userinfo），显示提示
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
                    
                    // shopTel 用于存储优势型号（多个型号用|分隔）
                    try
                    {
                        string modelsStr = GetStringValue(shopRow["shopTel"]);
                        string[] models = modelsStr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        Model1 = models.Length > 0 ? models[0] : "";
                        Model2 = models.Length > 1 ? models[1] : "";
                        Model3 = models.Length > 2 ? models[2] : "";
                        Model4 = models.Length > 3 ? models[3] : "";
                        Model5 = models.Length > 4 ? models[4] : "";
                    }
                    catch
                    {
                        // shopTel 字段可能不存在或值为空
                    }
                    
                    // 加载证照信息
                    BusinessLicense = GetStringValue(shopRow["shopImg"]);
                    IDCard = GetStringValue(shopRow["shopQQ"]);
                }
                else
                {
                    // 只有在userinfo也没有公司名称时才显示提示
                    if (string.IsNullOrEmpty(CompanyName))
                    {
                        CompanyName = "请完善公司信息";
                    }
                    CompanyDescription = "暂无公司简介";
                }
            }
            catch (Exception shopEx)
            {
                // shops表加载失败时，如果userinfo已有公司名称则保留
                if (string.IsNullOrEmpty(CompanyName))
                {
                    CompanyName = "请完善公司信息";
                }
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
                DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopId, shopImg, shopQQ FROM [dbo].[shops] WHERE userId = @userId AND dataFlag = 1",
                    DbHelper.CreateParameter("@userId", userId));

                if (shopDt != null && shopDt.Rows.Count > 0)
                {
                    int shopId = Convert.ToInt32(shopDt.Rows[0]["shopId"]);
                    
                    string existingLicense = GetStringValue(shopDt.Rows[0]["shopImg"]);
                    string existingIdCard = GetStringValue(shopDt.Rows[0]["shopQQ"]);
                    
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