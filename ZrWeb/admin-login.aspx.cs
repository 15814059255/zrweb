using System;
using System.Data;

public partial class admin_login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["AdminID"] != null)
            {
                Response.Redirect("/admin-console.aspx");
            }
        }
    }

    protected void btnAdminLogin_Click(object sender, EventArgs e)
    {
        string adminName = txtAdminName.Text.Trim();
        string password = txtAdminPassword.Text.Trim();

        if (string.IsNullOrEmpty(adminName))
        {
            ShowError("请输入管理员账号");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("请输入登录密码");
            return;
        }

        string encryptedPassword = GetMD5Hash(password);

        string sql = "SELECT AdminID, AdminName, RealName FROM admin_users WHERE AdminName=@AdminName AND Password=@Password AND Status=1";
        DataTable dt = DbHelper.ExecuteQuery(sql,
            DbHelper.CreateParameter("@AdminName", adminName),
            DbHelper.CreateParameter("@Password", encryptedPassword));

        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            Session["AdminID"] = Convert.ToInt32(row["AdminID"]);
            Session["AdminName"] = row["AdminName"].ToString();
            Session["AdminRealName"] = row["RealName"].ToString();
            
            Response.Redirect("/admin-console.aspx");
        }
        else
        {
            ShowError("账号或密码错误");
        }
    }

    private void ShowError(string message)
    {
        lblLoginError.Text = message;
        lblLoginError.Visible = true;
        lblLoginError.CssClass = "error-msg visible";
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
}