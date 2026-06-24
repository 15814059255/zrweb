using System;
using System.Data;

public partial class register : System.Web.UI.Page
{
    public string PageTitle = "注册会员 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,电子元器件,B2B平台,注册";
    public string PageDescription = "注册阻容网会员，发布供需信息，参与询价报价，拓展电子元器件业务。";

    protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["action"] == "checkMobile")
            {
                CheckMobileExists();
                return;
            }

            if (!IsPostBack)
            {
                // 检查是否已登录，已登录则跳转到首页
                if (Session["UserID"] != null)
                {
                    Response.Redirect("/index.aspx");
                }
            }
        }

        private void CheckMobileExists()
        {
            Response.ContentType = "application/json";
            Response.Clear();
            string mobile = Request["mobile"] != null ? Request["mobile"].Trim() : "";
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^1[3-9]\d{9}$"))
            {
                Response.Write("{\"exists\":false,\"valid\":false,\"message\":\"手机号格式不正确\"}");
                Response.End();
                return;
            }

            string checkSql = "SELECT COUNT(*) FROM userinfo WHERE MobilePhone=@MobilePhone OR UserName=@UserName";
            object result = DbHelper.ExecuteScalar(checkSql,
                DbHelper.CreateParameter("@MobilePhone", mobile),
                DbHelper.CreateParameter("@UserName", mobile));
            int count = Convert.ToInt32(result);

            if (count > 0)
            {
                Response.Write("{\"exists\":true,\"valid\":true,\"message\":\"该手机号已注册\"}");
            }
            else
            {
                Response.Write("{\"exists\":false,\"valid\":true,\"message\":\"手机号可用\"}");
            }
            Response.End();
        }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        string role = hfRole.Value;
        string mobilePhone = txtMobilePhone.Text.Trim();
        string password = txtPassword.Text.Trim();
        string linkMan = txtLinkMan.Text.Trim();
        string shopCompany = txtShopCompany.Text.Trim();
        string qq = txtQQ.Text.Trim();
        string email = txtEmail.Text.Trim();
        string smsCode = txtSmsCode.Text.Trim();

        // 验证角色
        if (string.IsNullOrEmpty(role) || (role != "buyer" && role != "seller"))
        {
            ShowError("请选择身份");
            return;
        }

        // 验证必填项
        if (string.IsNullOrEmpty(mobilePhone))
        {
            ShowError("请输入手机号");
            return;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(mobilePhone, @"^1[3-9]\d{9}$"))
        {
            ShowError("请输入正确的手机号");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("请输入密码");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("密码长度至少6个字符");
            return;
        }

        if (string.IsNullOrEmpty(linkMan))
        {
            ShowError("请输入联系人姓名");
            return;
        }

        if (!chkPrivacy.Checked)
        {
            ShowError("请阅读并同意用户协议与隐私政策");
            return;
        }

        // 验证短信验证码（暂跳过，对应隐藏的验证码输入框）
        // if (smsCode != "123456")
        // {
        //     ShowError("短信验证码错误（演示版：123456）");
        //     return;
        // }

        // 验证邮箱格式（如果填写了）
        if (!string.IsNullOrEmpty(email) && !System.Text.RegularExpressions.Regex.IsMatch(email, @"^[\w\.-]+@[\w\.-]+\.\w+$"))
        {
            ShowError("邮箱格式不正确");
            return;
        }

        // 检查手机号是否已存在（同时检查UserName和MobilePhone）
        string checkSql = "SELECT COUNT(*) FROM userinfo WHERE MobilePhone=@MobilePhone OR UserName=@UserName";
        object result = DbHelper.ExecuteScalar(checkSql,
            DbHelper.CreateParameter("@MobilePhone", mobilePhone),
            DbHelper.CreateParameter("@UserName", mobilePhone));
        int count = Convert.ToInt32(result);

        if (count > 0)
        {
            ShowError("该手机号已注册，请直接登录或找回密码");
            return;
        }

        // 生成用户GUID
        string userGuid = Guid.NewGuid().ToString();

        // 密码MD5加密
        string encryptedPassword = GetMD5Hash(password);

        // 角色映射：buyer -> 采购商(2)，seller -> 供应商(3)
        int roseId = role == "buyer" ? 2 : 3;

        // 获取用户注册IP
        string registerIP = GetUserIP();

        try
        {
            // 插入用户数据，使用手机号作为用户名，并返回新插入的用户ID
            string insertSql = @"INSERT INTO userinfo
                (UserName, Password, UserGuid, SysStatus, LinkMan, MobilePhone, QQ, Email,
                 RoseID, IsCheck, CreateTime, Source)
                OUTPUT INSERTED.UserID
                VALUES (@UserName, @Password, @UserGuid, 0, @LinkMan, @MobilePhone,
                 @QQ, @Email, @RoseID, 1, GETDATE(), @Source)";

            object userIdObj = DbHelper.ExecuteScalar(insertSql,
                DbHelper.CreateParameter("@UserName", mobilePhone),
                DbHelper.CreateParameter("@Password", encryptedPassword),
                DbHelper.CreateParameter("@UserGuid", userGuid),
                DbHelper.CreateParameter("@LinkMan", linkMan),
                DbHelper.CreateParameter("@MobilePhone", mobilePhone),
                DbHelper.CreateParameter("@QQ", string.IsNullOrEmpty(qq) ? (object)DBNull.Value : qq),
                DbHelper.CreateParameter("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email),
                DbHelper.CreateParameter("@RoseID", roseId),
                DbHelper.CreateParameter("@Source", registerIP));

            int userId = userIdObj != DBNull.Value && userIdObj != null ? Convert.ToInt32(userIdObj) : 0;

            if (userId > 0)
            {
                // 自动创建店铺记录
                CreateShopForUser(userId, mobilePhone, linkMan, shopCompany);

                // 注册成功 - 隐藏表单，显示成功页
                ShowSuccessState();
            }
            else
            {
                ShowError("注册失败，请稍后重试");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("注册异常: " + ex.Message);
            ShowError("注册失败：" + ex.Message);
        }
    }

    private void CreateShopForUser(int userId, string telephone, string shopkeeper, string shopCompany)
    {
        try
        {
            string displayName = !string.IsNullOrEmpty(shopCompany) ? shopCompany :
                                 (!string.IsNullOrEmpty(shopkeeper) ? shopkeeper + "的店铺" : telephone + "的店铺");
            string insertShopSql = @"INSERT INTO shops (userId, shopName, shopCompany, telephone, shopkeeper, shopStatus, dataFlag, createTime)
                                    VALUES (@userId, @shopName, @shopCompany, @telephone, @shopkeeper, 0, 1, GETDATE())";
            DbHelper.ExecuteNonQuery(insertShopSql,
                DbHelper.CreateParameter("@userId", userId),
                DbHelper.CreateParameter("@shopName", displayName),
                DbHelper.CreateParameter("@shopCompany", shopCompany ?? ""),
                DbHelper.CreateParameter("@telephone", telephone),
                DbHelper.CreateParameter("@shopkeeper", shopkeeper));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("CreateShopForUser 错误: " + ex.Message);
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

        private string GetUserIP()
        {
            string ip = "";
            try
            {
                if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                {
                    ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0].Trim();
                }
                else if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_CLIENT_IP"]))
                {
                    ip = Request.ServerVariables["HTTP_CLIENT_IP"];
                }
                else
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch
            {
                ip = "未知";
            }
            return ip;
        }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visible = true;
        lblError.CssClass = "error-msg visible";
        lblSuccess.Visible = false;
        lblSuccess.CssClass = "success-msg";
    }

    private void ShowSuccess(string message)
    {
        lblSuccess.Text = message;
        lblSuccess.Visible = true;
        lblSuccess.CssClass = "success-msg visible";
        lblError.Visible = false;
        lblError.CssClass = "error-msg";
    }

    private void ShowSuccessState()
    {
        // 通过注册脚本触发成功页
        ClientScript.RegisterStartupScript(this.GetType(), "registerSuccess",
            "setTimeout(function(){ if(window.showRegisterSuccess) showRegisterSuccess(); }, 100);", true);
    }
}
