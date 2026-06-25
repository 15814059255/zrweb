<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "text/html";
        Response.Write("<html><body><h1>添加商品参数字段</h1>");
        
        try
        {
            string[] fields = {
                "Brand VARCHAR(100)",
                "Capacitance VARCHAR(50)",
                "Resistance VARCHAR(50)",
                "Tolerance VARCHAR(30)",
                "Voltage VARCHAR(30)",
                "Dielectric VARCHAR(30)",
                "Power VARCHAR(30)",
                "TempCoefficient VARCHAR(30)"
            };
            
            foreach (string field in fields)
            {
                string[] parts = field.Split(new[] {' '}, 2);
                string fieldName = parts[0];
                
                string checkSql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'goods' AND COLUMN_NAME = @fieldName";
                object countObj = DbHelper.ExecuteScalar(checkSql, DbHelper.CreateParameter("@fieldName", fieldName));
                int count = countObj != null && countObj != DBNull.Value ? Convert.ToInt32(countObj) : 0;
                
                if (count == 0)
                {
                    try
                    {
                        string sql = "ALTER TABLE goods ADD " + field;
                        int result = DbHelper.ExecuteNonQuery(sql);
                        Response.Write("<p style='color:green'>✓ 成功添加字段: " + fieldName + "</p>");
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<p style='color:red'>✗ 添加字段失败 " + fieldName + ": " + ex.Message + "</p>");
                    }
                }
                else
                {
                    Response.Write("<p style='color:blue'>● 字段已存在: " + fieldName + "</p>");
                }
            }
            
        }
        catch (Exception ex)
        {
            Response.Write("<p style='color:red'>操作失败: " + ex.Message + "</p>");
        }
        
        Response.Write("</body></html>");
    }
</script>