using System;
using System.Data;

public partial class test_db : System.Web.UI.Page
{
    protected string TestResult = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            TestResult += "<h3>1. 测试数据库连接...</h3>";
            
            DataTable dt = DbHelper.ExecuteQuery("SELECT TOP 1 goodsId, goodsSn FROM goods");
            
            if (dt != null && dt.Rows.Count > 0)
            {
                TestResult += "<p style='color:green'>数据库连接成功！</p>";
                TestResult += "<p>查询到数据：</p>";
                TestResult += "<table border='1'>";
                foreach (DataColumn col in dt.Columns)
                {
                    TestResult += "<th>" + col.ColumnName + "</th>";
                }
                foreach (DataRow row in dt.Rows)
                {
                    TestResult += "<tr>";
                    foreach (object cell in row.ItemArray)
                    {
                        TestResult += "<td>" + cell.ToString() + "</td>";
                    }
                    TestResult += "</tr>";
                }
                TestResult += "</table>";
                
                TestResult += "<h3>2. 测试插入数据...</h3>";
                
                string sql = @"INSERT INTO goods (goodsSn, [Name], goodsStock, goodsUnit, shopPrice, isIncludingTax, pubType, dataFlag, goodsStatus, isSale, createTime, updateTime)
                              VALUES ('TEST_MODEL_001', '测试商品', 100, 'Kpcs', 0.01, 0, 1, 1, 1, 1, GETDATE(), GETDATE())";
                int result = DbHelper.ExecuteNonQuery(sql);
                
                if (result > 0)
                {
                    TestResult += "<p style='color:green'>插入数据成功！影响行数：" + result + "</p>";
                }
                else
                {
                    TestResult += "<p style='color:red'>插入数据失败，影响行数：" + result + "</p>";
                }
            }
            else
            {
                TestResult += "<p style='color:orange'>数据库连接成功，但没有数据。</p>";
            }
        }
        catch (Exception ex)
        {
            TestResult += "<p style='color:red'>错误：" + ex.Message + "</p>";
            TestResult += "<p>堆栈：" + ex.StackTrace + "</p>";
        }
    }
}