using System;
using System.Data;
using System.Web;
using System.Web.Security;

public partial class login : System.Web.UI.Page
{
    public string PageTitle = "登录 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,电子元器件,B2B平台,登录";
    public string PageDescription = "阻容网 - 电子元器件供需撮合平台，提供电容、电阻等阻容元件的供应与需求信息发布服务。";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 检查是否已登录
            if (Session["UserID"] != null)
            {
                Response.Redirect("/index.aspx");
            }
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string userName = txtUserName.Text.Trim();
        string password = txtPassword.Text.Trim();

        // 验证输入
        if (string.IsNullOrEmpty(userName))
        {
            ShowError("请输入手机号");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("请输入密码");
            return;
        }

        if (!chkPrivacy.Checked)
        {
            ShowError("请阅读并同意隐私政策");
            return;
        }

        // 密码MD5加密
        string encryptedPassword = GetMD5Hash(password);

        // 查询用户
        string sql = "SELECT UserID, UserName, LinkMan, MobilePhone, RoseID, IsCheck, SysStatus, UserGuid FROM userinfo WHERE UserName=@UserName AND Password=@Password";
        DataTable dt = DbHelper.ExecuteQuery(sql,
            DbHelper.CreateParameter("@UserName", userName),
            DbHelper.CreateParameter("@Password", encryptedPassword));

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            
            int sysStatus = GetIntValue(row["SysStatus"], 0);
            if (sysStatus == 1)
            {
                ShowError("账号已被禁用，请联系管理员");
                return;
            }
            
            // 安全获取 IsCheck（处理 DBNull）
            int isCheck = GetIntValue(row["IsCheck"], 0);

            if (isCheck == 0)
            {
                ShowError("账号未审核，请等待管理员审核");
                return;
            }

            int userId = GetIntValue(row["UserID"], 0);
            string dbUserName = GetStringValue(row["UserName"]);
            string linkMan = GetStringValue(row["LinkMan"]);
            string mobilePhone = GetStringValue(row["MobilePhone"]);
            int roseId = GetIntValue(row["RoseID"], 0);
            string userGuid = GetStringValue(row["UserGuid"]);

            // 查询店铺信息
            DataTable shopDt = DbHelper.ExecuteQuery("SELECT shopId, shopName, shopCompany, shopkeeper, telephone, shopStatus FROM shops WHERE userId = @userId AND dataFlag = 1", 
                DbHelper.CreateParameter("@userId", userId));
            int shopId = 0;
            string shopName = "";
            string shopCompany = "";
            string shopkeeper = "";
            string shopTelephone = "";
            int shopStatus = 0;
            
            if (shopDt != null && shopDt.Rows.Count > 0)
            {
                DataRow shopRow = shopDt.Rows[0];
                shopId = GetIntValue(shopRow["shopId"], 0);
                shopName = GetStringValue(shopRow["shopName"]);
                shopCompany = GetStringValue(shopRow["shopCompany"]);
                shopkeeper = GetStringValue(shopRow["shopkeeper"]);
                shopTelephone = GetStringValue(shopRow["telephone"]);
                shopStatus = GetIntValue(shopRow["shopStatus"], 0);
            }
            else
            {
                // 如果没有店铺记录，自动创建
                shopId = CreateShopForUser(userId, mobilePhone, linkMan);
            }

            // 登录成功，保存Session
            Session["UserID"] = userId;
            Session["UserName"] = dbUserName;
            Session["LinkMan"] = linkMan;
            Session["MobilePhone"] = mobilePhone;
            Session["RoseID"] = roseId;
            Session["UserGuid"] = userGuid;
            Session["ShopId"] = shopId;
            Session["ShopName"] = shopName;
            Session["ShopCompany"] = shopCompany;

            // 保存用户信息到Cookie（有效期7天）
            SaveUserCookie(userId, dbUserName, linkMan, mobilePhone, roseId, userGuid, shopId, shopName, shopCompany);

            // 缓存用户信息到Redis
            RedisHelper.CacheUserInfo(userId, dbUserName, linkMan, mobilePhone, roseId, userGuid);

            // 转到首页或指定页面
            string returnUrl = Request.QueryString["returnUrl"];
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = roseId == 2 ? "/buyer-workbench.aspx" : "/merchant-workbench.aspx";
            }
            Response.Redirect(returnUrl);
        }
        else
        {
            ShowError("用户名或密码错误");
        }
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visible = true;
        lblError.CssClass = "error-msg visible";
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

    /// <summary>
    /// 安全获取整数值（处理 DBNull）
    /// </summary>
    private int GetIntValue(object value, int defaultValue)
    {
        if (value == DBNull.Value || value == null)
        {
            return defaultValue;
        }
        return Convert.ToInt32(value);
    }

    /// <summary>
    /// 安全获取字符串值（处理 DBNull）
    /// </summary>
    private string GetStringValue(object value)
    {
        if (value == DBNull.Value || value == null)
        {
            return "";
        }
        return value.ToString();
    }

    /// <summary>
    /// 保存用户信息到Cookie
    /// </summary>
    private void SaveUserCookie(int userId, string userName, string linkMan, string mobilePhone, int roseId, string userGuid, int shopId, string shopName, string shopCompany)
    {
        HttpCookie userCookie = new HttpCookie("ZrWebUser");
        userCookie["UserID"] = userId.ToString();
        userCookie["UserName"] = userName;
        userCookie["LinkMan"] = linkMan;
        userCookie["MobilePhone"] = mobilePhone;
        userCookie["RoseID"] = roseId.ToString();
        userCookie["UserGuid"] = userGuid;
        userCookie["ShopId"] = shopId.ToString();
        userCookie["ShopName"] = shopName;
        userCookie["ShopCompany"] = shopCompany;
        userCookie.Expires = DateTime.Now.AddDays(7);
        userCookie.HttpOnly = true;
        // 移除 Secure 设置，允许在 HTTP 和 HTTPS 环境下都能工作
        // userCookie.Secure = Request.IsSecureConnection;
        userCookie.Path = "/";
        Response.Cookies.Add(userCookie);
    }

    /// <summary>
    /// 为用户创建店铺记录
    /// </summary>
    private int CreateShopForUser(int userId, string telephone, string shopkeeper)
    {
        try
        {
            string insertShopSql = @"INSERT INTO shops (userId, telephone, shopkeeper, shopStatus, dataFlag, createTime) 
                                    OUTPUT INSERTED.shopId VALUES (@userId, @telephone, @shopkeeper, 0, 1, GETDATE())";
            object shopIdObj = DbHelper.ExecuteScalar(insertShopSql,
                DbHelper.CreateParameter("@userId", userId),
                DbHelper.CreateParameter("@telephone", telephone),
                DbHelper.CreateParameter("@shopkeeper", shopkeeper));
            
            if (shopIdObj != null && shopIdObj != DBNull.Value)
            {
                return Convert.ToInt32(shopIdObj);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("CreateShopForUser 错误: " + ex.Message);
        }
        return 0;
    }
}