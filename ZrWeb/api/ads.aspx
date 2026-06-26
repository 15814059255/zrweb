<%@ Page Language="C#" AutoEventWireup="true" %>
<%
    Response.ContentType = "application/json";
    Response.Charset = "utf-8";
    
    try
    {
        System.Data.DataTable dt = DbHelper.ExecuteQuery("SELECT keyWord as AdSlot, adName as Title, adPositionId as Position, adURL as LinkUrl, adStartDate as StartDate, adEndDate as EndDate, dataFlag as Status FROM ads WHERE dataFlag = 1 ORDER BY adPositionId ASC");
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("[");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append("{");
            sb.Append("\"AdSlot\":\"").Append(dt.Rows[i]["AdSlot"].ToString().Replace("\"", "\\\"")).Append("\",");
            sb.Append("\"Title\":\"").Append(dt.Rows[i]["Title"].ToString().Replace("\"", "\\\"")).Append("\",");
            sb.Append("\"Position\":\"").Append(dt.Rows[i]["Position"].ToString().Replace("\"", "\\\"")).Append("\",");
            sb.Append("\"LinkUrl\":\"").Append(dt.Rows[i]["LinkUrl"].ToString().Replace("\"", "\\\"")).Append("\",");
            sb.Append("\"StartDate\":\"").Append(dt.Rows[i]["StartDate"].ToString()).Append("\",");
            sb.Append("\"EndDate\":\"").Append(dt.Rows[i]["EndDate"].ToString()).Append("\",");
            sb.Append("\"Status\":").Append(dt.Rows[i]["Status"].ToString());
            sb.Append("}");
        }
        sb.Append("]");
        
        Response.Write(sb.ToString());
    }
    catch (Exception ex)
    {
        Response.Write("[{\"error\":\"" + ex.Message.Replace("\"", "\\\"") + "\"}]");
    }
%>