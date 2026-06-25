using System;

public partial class api_mark_inquiry_read : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json";
        Response.Charset = "utf-8";

        try
        {
            int userId = UserHelper.GetUserId();
            if (userId <= 0)
            {
                Response.Write("{\"success\":false,\"msg\":\"未登录\"}");
                Response.End();
                return;
            }

            int eqId = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["eqId"]))
            {
                int.TryParse(Request.QueryString["eqId"], out eqId);
            }

            if (eqId <= 0)
            {
                Response.Write("{\"success\":false,\"msg\":\"参数错误\"}");
                Response.End();
                return;
            }

            string checkSql = @"SELECT toUserId FROM enquiryquoteprice WHERE eqId = @eqId AND dataFlag = 1";
            object toUserIdObj = DbHelper.ExecuteScalar(checkSql, DbHelper.CreateParameter("@eqId", eqId));
            int toUserId = 0;
            if (toUserIdObj != null && toUserIdObj != DBNull.Value)
            {
                int.TryParse(toUserIdObj.ToString(), out toUserId);
            }

            if (toUserId != userId)
            {
                Response.Write("{\"success\":false,\"msg\":\"无权操作\"}");
                Response.End();
                return;
            }

            string updateSql = @"UPDATE enquiryquoteprice SET readStatus = 1 WHERE eqId = @eqId";
            int result = DbHelper.ExecuteNonQuery(updateSql, DbHelper.CreateParameter("@eqId", eqId));

            if (result > 0)
            {
                Response.Write("{\"success\":true,\"msg\":\"已标记\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"msg\":\"更新失败\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"msg\":\"" + ex.Message.Replace("\"", "'") + "\"}");
        }

        Response.End();
    }
}
