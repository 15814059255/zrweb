<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "text/html";
        Response.Write("<html><body><h1>商品表结构</h1>");
        
        try
        {
            string sql = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'goods' ORDER BY ORDINAL_POSITION";
            DataTable dt = DbHelper.ExecuteQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                Response.Write("<table border='1'><tr><th>字段名</th><th>类型</th><th>是否可空</th></tr>");
                foreach (DataRow row in dt.Rows)
                {
                    Response.Write("<tr>");
                    Response.Write("<td>" + row["COLUMN_NAME"] + "</td>");
                    Response.Write("<td>" + row["DATA_TYPE"] + "</td>");
                    Response.Write("<td>" + row["IS_NULLABLE"] + "</td>");
                    Response.Write("</tr>");
                }
                Response.Write("</table>");
            }
            
            // 查看现有数据示例
            Response.Write("<h2>现有数据示例</h2>");
            sql = "SELECT TOP 5 goodsId, goodsSn, Manufacturers, Packaging, goodsDesc, pubType FROM goods WHERE dataFlag = 1 ORDER BY createTime DESC";
            dt = DbHelper.ExecuteQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                Response.Write("<table border='1'><tr><th>goodsId</th><th>goodsSn</th><th>Manufacturers</th><th>Packaging</th><th>goodsDesc</th></tr>");
                foreach (DataRow row in dt.Rows)
                {
                    Response.Write("<tr>");
                    Response.Write("<td>" + row["goodsId"] + "</td>");
                    Response.Write("<td>" + row["goodsSn"] + "</td>");
                    Response.Write("<td>" + row["Manufacturers"] + "</td>");
                    Response.Write("<td>" + (row["Packaging"] == DBNull.Value ? "NULL" : row["Packaging"]) + "</td>");
                    Response.Write("<td>" + (row["goodsDesc"] == DBNull.Value ? "NULL" : row["goodsDesc"]) + "</td>");
                    Response.Write("</tr>");
                }
                Response.Write("</table>");
            }
        }
        catch (Exception ex)
        {
            Response.Write("<p style='color:red'>查询失败: " + ex.Message + "</p>");
        }
        
        Response.Write("</body></html>");
    }
</script>