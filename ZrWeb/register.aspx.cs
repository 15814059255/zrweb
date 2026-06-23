using System;
using System.Data;

public partial class register : System.Web.UI.Page
{
    public string PageTitle = "注册会员 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,电子元器件,B2B平台,注册";
    public string PageDescription = "注册阻容网会员，发布供需信息，参与询价报价，拓展电子元器件业务。";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 检查是否已登录，已登录则跳转到首页
            if (Session["UserID"] != null)
            {
                Response.Redirect("/index.aspx");
            }
        }
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        string mobilePhone = txtMobilePhone.Text.Trim();
        string password = txtPassword.Text.Trim();
        string linkMan = txtLinkMan.Text.Trim();

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
            ShowError("请阅读并同意隐私政策");
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

        try
        {
            // 插入用户数据，使用手机号作为用户名，并返回新插入的用户ID
            string insertSql = "INSERT INTO userinfo (UserName, Password, UserGuid, SysStatus, LinkMan, MobilePhone, RoseID, IsCheck, CreateTime) OUTPUT INSERTED.UserID VALUES (@UserName, @Password, @UserGuid, 0, @LinkMan, @MobilePhone, 1, 1, GETDATE())";
            object userIdObj = DbHelper.ExecuteScalar(insertSql,
                DbHelper.CreateParameter("@UserName", mobilePhone),
                DbHelper.CreateParameter("@Password", encryptedPassword),
                DbHelper.CreateParameter("@UserGuid", userGuid),
                DbHelper.CreateParameter("@LinkMan", linkMan),
                DbHelper.CreateParameter("@MobilePhone", mobilePhone));

            int userId = userIdObj != DBNull.Value && userIdObj != null ? Convert.ToInt32(userIdObj) : 0;

            if (userId > 0)
            {
                // 自动创建店铺记录
                CreateShopForUser(userId, mobilePhone, linkMan);

                // 注册成功后验证数据是否正确写入
                string verifySql = "SELECT UserID, Password, IsCheck FROM userinfo WHERE UserID=@UserID";
                DataTable verifyDt = DbHelper.ExecuteQuery(verifySql, DbHelper.CreateParameter("@UserID", userId));
                
                if (verifyDt != null && verifyDt.Rows.Count > 0)
                {
                    string storedPassword = verifyDt.Rows[0]["Password"].ToString();
                    if (storedPassword == encryptedPassword)
                    {
                        ShowSuccess("注册成功！使用手机号登录。");
                        txtMobilePhone.Text = "";
                        txtPassword.Text = "";
                        txtLinkMan.Text = "";
                    }
                    else
                    {
                        ShowError("注册验证失败，密码不匹配");
                    }
                }
                else
                {
                    ShowError("注册验证失败，无法读取用户数据");
                }
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

    private void CreateShopForUser(int userId, string telephone, string shopkeeper)
    {
        try
        {
            string insertShopSql = @"INSERT INTO shops (userId, telephone, shopkeeper, shopStatus, dataFlag, createTime) 
                                    VALUES (@userId, @telephone, @shopkeeper, 0, 1, GETDATE())";
            DbHelper.ExecuteNonQuery(insertShopSql,
                DbHelper.CreateParameter("@userId", userId),
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
}