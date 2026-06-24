using System;
using System.Data;

public partial class admin_user_detail : System.Web.UI.Page
{
    public string AdminName = "";
    public int UserID = 0;
    public string UserName = "";
    public string LinkMan = "";
    public string MobilePhone = "";
    public string QQ = "";
    public string Email = "";
    public string CompanyName = "";
    public string Province = "";
    public string City = "";
    public string District = "";
    public string Street = "";
    public string Address = "";
    public int RoseID = 0;
    public string RoleName = "普通用户";
    public string RoleTagClass = "gray";
    public string Source = "";
    public string IDCardName = "";
    public string IDCardNumber = "";
    public int IsCheck = 0;
    public int SysStatus = 0;
    public int IsCertified = 0;
    public string BusinessLicense = "";
    public string IDCard = "";
    public string CreateTime = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckLogin();
        
        string action = Request.QueryString["action"];
        string userIdStr = Request.QueryString["id"];
        
        if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out UserID))
        {
            if (action == "certify")
            {
                int isCertified = Convert.ToInt32(Request.QueryString["isCertified"]);
                DbHelper.ExecuteNonQuery("UPDATE [dbo].[userinfo] SET IsCertified = @isCertified WHERE UserID = @userId",
                    DbHelper.CreateParameter("@isCertified", isCertified),
                    DbHelper.CreateParameter("@userId", UserID));
                Response.Redirect("/admin-user-detail.aspx?id=" + UserID);
                return;
            }
            else if (action == "changeRole")
            {
                int role = Convert.ToInt32(Request.QueryString["role"]);
                if (role == 1 || role == 2 || role == 3)
                {
                    DbHelper.ExecuteNonQuery("UPDATE [dbo].[userinfo] SET RoseID = @roseId WHERE UserID = @userId",
                        DbHelper.CreateParameter("@roseId", role),
                        DbHelper.CreateParameter("@userId", UserID));
                }
                Response.Redirect("/admin-user-detail.aspx?id=" + UserID);
                return;
            }
            else if (action == "saveIDCard")
            {
                string idCardName = Request.QueryString["name"] ?? "";
                string idCardNumber = Request.QueryString["number"] ?? "";
                DbHelper.ExecuteNonQuery("UPDATE [dbo].[userinfo] SET IDCardName = @name, IDCardNumber = @number WHERE UserID = @userId",
                    DbHelper.CreateParameter("@name", idCardName),
                    DbHelper.CreateParameter("@number", idCardNumber),
                    DbHelper.CreateParameter("@userId", UserID));
                Response.Redirect("/admin-user-detail.aspx?id=" + UserID);
                return;
            }
            else if (action == "saveCompanyName")
            {
                string companyName = Request.QueryString["name"] ?? "";
                // 保存到 shops 表
                DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopId FROM [dbo].[shops] WHERE userId = @userId AND dataFlag = 1",
                    DbHelper.CreateParameter("@userId", UserID));
                if (shopDt != null && shopDt.Rows.Count > 0)
                {
                    int shopId = Convert.ToInt32(shopDt.Rows[0]["shopId"]);
                    DbHelper.ExecuteNonQuery("UPDATE [dbo].[shops] SET shopCompany = @companyName WHERE shopId = @shopId",
                        DbHelper.CreateParameter("@companyName", companyName),
                        DbHelper.CreateParameter("@shopId", shopId));
                }
                else
                {
                    // 如果没有 shops 记录，插入一条
                    DbHelper.ExecuteNonQuery("INSERT INTO [dbo].[shops] (userId, shopCompany, telephone, shopStatus, dataFlag) VALUES (@userId, @companyName, @telephone, @shopStatus, @dataFlag)",
                        DbHelper.CreateParameter("@userId", UserID),
                        DbHelper.CreateParameter("@companyName", companyName),
                        DbHelper.CreateParameter("@telephone", ""),
                        DbHelper.CreateParameter("@shopStatus", 0),
                        DbHelper.CreateParameter("@dataFlag", 1));
                }
                // 同时更新 userinfo 表
                DbHelper.ExecuteNonQuery("UPDATE [dbo].[userinfo] SET CompanyName = @companyName WHERE UserID = @userId",
                    DbHelper.CreateParameter("@companyName", companyName),
                    DbHelper.CreateParameter("@userId", UserID));
                Response.Redirect("/admin-user-detail.aspx?id=" + UserID);
                return;
            }
            else if (action == "resetPassword")
            {
                string newPassword = Request.QueryString["pwd"] ?? "";
                if (newPassword.Length >= 6)
                {
                    // 密码MD5加密后存储
                    string encryptedPassword = GetMD5Hash(newPassword);
                    DbHelper.ExecuteNonQuery("UPDATE [dbo].[userinfo] SET Password = @password WHERE UserID = @userId",
                        DbHelper.CreateParameter("@password", encryptedPassword),
                        DbHelper.CreateParameter("@userId", UserID));
                }
                Response.Redirect("/admin-user-detail.aspx?id=" + UserID);
                return;
            }
            
            LoadUserDetail();
        }
        else
        {
            Response.Redirect("/admin-users.aspx");
        }
    }

    private void CheckLogin()
    {
        if (Session["AdminID"] == null)
        {
            Response.Redirect("/admin-login.aspx");
        }
        AdminName = Session["AdminName"] != null ? Session["AdminName"].ToString() : "";
    }

    private void LoadUserDetail()
    {
        try
        {
            string sql = @"SELECT UserID, UserName, LinkMan, MobilePhone, QQ, Email, 
                          CompanyName, Province, City, District, Street, Address,
                          RoseID, Source, IDCardName, IDCardNumber, IsCheck, SysStatus, IsCertified, CreateTime
                          FROM [dbo].[userinfo] WHERE UserID = @userId";
            
            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@userId", UserID));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                UserName = GetStringValue(row["UserName"]);
                LinkMan = GetStringValue(row["LinkMan"]);
                MobilePhone = GetStringValue(row["MobilePhone"]);
                QQ = GetStringValue(row["QQ"]);
                Email = GetStringValue(row["Email"]);
                CompanyName = GetStringValue(row["CompanyName"]);
                Province = GetStringValue(row["Province"]);
                City = GetStringValue(row["City"]);
                District = GetStringValue(row["District"]);
                Street = GetStringValue(row["Street"]);
                Address = GetStringValue(row["Address"]);
                RoseID = GetIntValue(row["RoseID"], 0);
                RoleName = GetRoleName(RoseID);
                RoleTagClass = GetRoleTagClass(RoseID);
                Source = GetStringValue(row["Source"]);
                IDCardName = GetStringValue(row["IDCardName"]);
                IDCardNumber = GetStringValue(row["IDCardNumber"]);
                IsCheck = GetIntValue(row["IsCheck"], 0);
                SysStatus = GetIntValue(row["SysStatus"], 0);
                IsCertified = GetIntValue(row["IsCertified"], 0);
                
                DateTime createTime = GetDateTimeValue(row["CreateTime"], DateTime.Now);
                CreateTime = createTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadUserDetail error: " + ex.Message);
        }
        
        // 优先从 shops 表读取公司名称（用户实际填写的信息）
        try
        {
            string shopSql = @"SELECT shopCompany FROM [dbo].[shops] WHERE userId = @userId AND dataFlag = 1";
            DataTable shopDt = DbHelper.ExecuteQuery(shopSql, DbHelper.CreateParameter("@userId", UserID));
            if (shopDt != null && shopDt.Rows.Count > 0)
            {
                string shopCompany = GetStringValue(shopDt.Rows[0]["shopCompany"]);
                if (!string.IsNullOrEmpty(shopCompany))
                {
                    CompanyName = shopCompany;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadShopCompany error: " + ex.Message);
        }
        
        // 如果 shops 表没有公司名称，使用 userinfo 表的 CompanyName
        if (string.IsNullOrEmpty(CompanyName))
        {
            CompanyName = "暂无公司信息";
        }
        
        try
        {
            string certSql = @"SELECT shopImg, shopQQ FROM [dbo].[shops] WHERE userId = @userId AND dataFlag = 1";
            DataTable certDt = DbHelper.ExecuteQuery(certSql, DbHelper.CreateParameter("@userId", UserID));
            if (certDt != null && certDt.Rows.Count > 0)
            {
                BusinessLicense = GetStringValue(certDt.Rows[0]["shopImg"]);
                IDCard = GetStringValue(certDt.Rows[0]["shopQQ"]);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadCertFiles error: " + ex.Message);
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

    private DateTime GetDateTimeValue(object value, DateTime defaultValue)
    {
        if (value == null || value == DBNull.Value)
            return defaultValue;
        DateTime result;
        return DateTime.TryParse(value.ToString(), out result) ? result : defaultValue;
    }

    private string GetRoleName(int roseId)
    {
        switch (roseId)
        {
            case 1: return "普通用户";
            case 2: return "采购商";
            case 3: return "供应商";
            default: return "普通用户";
        }
    }

    private string GetRoleTagClass(int roseId)
    {
        switch (roseId)
        {
            case 2: return "blue";
            case 3: return "purple";
            default: return "gray";
        }
    }

    /// <summary>
    /// MD5加密
    /// </summary>
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
}