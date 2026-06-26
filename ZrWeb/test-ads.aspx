<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>测试广告数据</title>
</head>
<body>
    <%
        try
        {
            System.Data.DataTable dt = DbHelper.ExecuteQuery("SELECT adId as AdID, keyWord as AdSlot, adName as Title, adPositionId as Position, adURL as LinkUrl, adStartDate as StartDate, adEndDate as EndDate, dataFlag as Status FROM ads ORDER BY adId ASC");
            Response.Write("查询成功，行数: " + dt.Rows.Count + "<br/>");
            foreach (System.Data.DataColumn col in dt.Columns)
            {
                Response.Write("列名: " + col.ColumnName + " 类型: " + col.DataType.Name + "<br/>");
            }
            foreach (System.Data.DataRow row in dt.Rows)
            {
                Response.Write("<hr/>");
                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    Response.Write(col.ColumnName + ": " + row[col].ToString() + "<br/>");
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("错误: " + ex.Message);
        }
    %>
</body>
</html>