<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Text" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>修复数据库编码</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: #27ae60; font-weight: bold; }
        .error { color: #e74c3c; font-weight: bold; }
        .warning { color: #f39c12; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
        button { padding: 10px 20px; background: #e74c3c; color: white; border: none; cursor: pointer; font-size: 16px; }
        button:hover { background: #c0392b; }
        .repair-btn { background: #3498db; }
        .repair-btn:hover { background: #2980b9; }
    </style>
</head>
<body>
    <h1>修复数据库编码</h1>
    
    <h2>警告</h2>
    <p class="warning">此工具将直接修改数据库中的数据，请在执行前备份数据库！</p>
    
    <h2>检测乱码数据</h2>
    <%
        string[] tables = { "enquiryquoteprice", "userinfo", "shops" };
        string[][] fields = {
            new string[] { "fromCompany", "toCompany", "fromContact", "toContact" },
            new string[] { "CompanyName", "IDCardName" },
            new string[] { "shopCompany" }
        };
        
        int totalProblemCount = 0;
        
        for (int i = 0; i < tables.Length; i++) {
            string table = tables[i];
            string[] tableFields = fields[i];
            
            Response.Write("<h3>" + table + " 表</h3>");
            
            foreach (string field in tableFields) {
                string sql = string.Format("SELECT COUNT(*) FROM {0} WHERE {1} IS NOT NULL AND {1} != ''", table, field);
                object countObj = DbHelper.ExecuteScalar(sql);
                int total = countObj != null && countObj != DBNull.Value ? Convert.ToInt32(countObj) : 0;
                
                int problemCount = CountMojibakeRecords(table, field);
                totalProblemCount += problemCount;
                
                Response.Write("<p>" + field + ": 共 " + total + " 条记录，其中乱码 " + 
                    "<span class='" + (problemCount > 0 ? "error" : "success") + "'>" + problemCount + "</span> 条</p>");
            }
        }
    %>
    
    <h2>乱码数据预览</h2>
    <%
        if (totalProblemCount > 0) {
            foreach (DataRow row in GetMojibakeRecords().Rows) {
                string tableName = row["tableName"].ToString();
                string fieldName = row["fieldName"].ToString();
                string originalValue = row["originalValue"].ToString();
                string fixedValue = DbHelper.FixEncoding(originalValue);
                
                Response.Write("<p><strong>" + tableName + "." + fieldName + "</strong>: " + 
                    Server.HtmlEncode(originalValue) + " → " + 
                    "<span class='success'>" + Server.HtmlEncode(fixedValue) + "</span></p>");
            }
        } else {
            Response.Write("<p class='success'>没有发现乱码数据</p>");
        }
    %>
    
    <h2>执行修复</h2>
    <p>点击下方按钮将修复所有检测到的乱码数据：</p>
    <button onclick="doRepair()" class="repair-btn">执行编码修复</button>
    
    <div id="result" style="margin-top: 20px;"></div>
    
    <script>
        function doRepair() {
            var resultDiv = document.getElementById('result');
            resultDiv.innerHTML = '<p>正在修复...</p>';
            
            fetch('/repair-db-encoding.ashx?action=repair', {
                method: 'POST'
            })
            .then(function(response) {
                return response.text();
            })
            .then(function(data) {
                resultDiv.innerHTML = data;
                setTimeout(function() {
                    window.location.reload();
                }, 3000);
            })
            .catch(function(error) {
                resultDiv.innerHTML = '<p class="error">修复失败: ' + error + '</p>';
            });
        }
    </script>
    
    <%
        int CountMojibakeRecords(string table, string field) {
            string sql = string.Format("SELECT {0} FROM {1} WHERE {0} IS NOT NULL AND {0} != ''", field, table);
            DataTable dt = DbHelper.ExecuteQuery(sql);
            int count = 0;
            if (dt != null && dt.Rows.Count > 0) {
                foreach (DataRow row in dt.Rows) {
                    string val = row[field].ToString();
                    string fixedVal = DbHelper.FixEncoding(val);
                    if (val != fixedVal) {
                        count++;
                    }
                }
            }
            return count;
        }
        
        DataTable GetMojibakeRecords() {
            DataTable result = new DataTable();
            result.Columns.Add("tableName");
            result.Columns.Add("fieldName");
            result.Columns.Add("originalValue");
            
            string[] tables = { "enquiryquoteprice", "userinfo", "shops" };
            string[][] fields = {
                new string[] { "fromCompany", "toCompany", "fromContact", "toContact" },
                new string[] { "CompanyName", "IDCardName" },
                new string[] { "shopCompany" }
            };
            
            for (int i = 0; i < tables.Length; i++) {
                string table = tables[i];
                string[] tableFields = fields[i];
                
                foreach (string field in tableFields) {
                    string sql = string.Format("SELECT {0} FROM {1} WHERE {0} IS NOT NULL AND {0} != ''", field, table);
                    DataTable dt = DbHelper.ExecuteQuery(sql);
                    if (dt != null && dt.Rows.Count > 0) {
                        foreach (DataRow row in dt.Rows) {
                            string val = row[field].ToString();
                            string fixedVal = DbHelper.FixEncoding(val);
                            if (val != fixedVal) {
                                DataRow newRow = result.NewRow();
                                newRow["tableName"] = table;
                                newRow["fieldName"] = field;
                                newRow["originalValue"] = val;
                                result.Rows.Add(newRow);
                            }
                        }
                    }
                }
            }
            
            return result;
        }
    %>
</body>
</html>
