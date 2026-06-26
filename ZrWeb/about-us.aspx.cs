using System;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.Data;

public partial class about_us : Page
{
    protected string PageTitle = "关于我们 - 阻容网";
    protected string PageKeywords = "阻容网,关于我们,电子元器件,B2B平台,供需撮合";
    protected string PageDescription = "阻容网是专业的电子元器件B2B交易平台，专注阻容元件供需信息撮合，帮助供应商和采购商快速对接。";
    
    protected string SiteDomain = ConfigurationManager.AppSettings["SiteDomain"] ?? "ZR.net.cn";
    protected string CompanyAddress = "深圳市福田区华强北电子商务产业带";
    protected string ServiceEmail = "service@ZR.net.cn";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["action"] == "submitFeedback")
        {
            HandleSubmitFeedback();
        }
    }

    private void HandleSubmitFeedback()
    {
        try
        {
            string name = Request["name"] != null ? Request["name"].Trim() : "";
            string contact = Request["contact"] != null ? Request["contact"].Trim() : "";
            string content = Request["content"] != null ? Request["content"].Trim() : "";
            
            if (string.IsNullOrEmpty(name))
            {
                Response.Write("{\"success\":false,\"message\":\"请填写您的称呼\"}");
                Response.End();
                return;
            }
            if (string.IsNullOrEmpty(contact))
            {
                Response.Write("{\"success\":false,\"message\":\"请填写联系方式\"}");
                Response.End();
                return;
            }
            
            int userId = 0;
            try
            {
                userId = Convert.ToInt32(Session["UserID"] != null ? Session["UserID"] : "0");
            }
            catch { }
            
            string userIP = Request.UserHostAddress;
            if (userIP == "::1") userIP = "127.0.0.1";
            
            string insertSQL = @"INSERT INTO feedbacks (name, contact, content, userId, userIP, status, createTime)
                                VALUES (@name, @contact, @content, @userId, @userIP, 0, GETDATE())";
            
            DbHelper.ExecuteNonQuery(insertSQL,
                DbHelper.CreateParameter("@name", name),
                DbHelper.CreateParameter("@contact", contact),
                DbHelper.CreateParameter("@content", content),
                DbHelper.CreateParameter("@userId", userId),
                DbHelper.CreateParameter("@userIP", userIP));
            
            Response.Write("{\"success\":true,\"message\":\"留言提交成功\"}");
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"" + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
        Response.End();
    }
}