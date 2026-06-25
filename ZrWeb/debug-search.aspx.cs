using System;
using System.Web.UI;
using System.Data;

public partial class debug_search : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string keyword = Request.QueryString["q"] ?? "";
            if (!string.IsNullOrEmpty(keyword))
            {
                BindDebugResults(keyword);
            }
        }
    }

    private void BindDebugResults(string keyword)
    {
        Response.Write("<h2>搜索关键词: " + Server.HtmlEncode(keyword) + "</h2>");
        
        string sql = @"SELECT goodsId, goodsSn, Name, Manufacturers, goodsDesc, 
            isSale, goodsStatus, dataFlag, pubType, shopId, createTime
            FROM goods 
            WHERE goodsSn LIKE @kw OR Name LIKE @kw OR Manufacturers LIKE @kw OR goodsDesc LIKE @kw
            ORDER BY createTime DESC";
        
        DataTable dt = DbHelper.ExecuteQuery(sql, 
            DbHelper.CreateParameter("@kw", "%" + keyword + "%"));
        
        if (dt == null || dt.Rows.Count == 0)
        {
            Response.Write("<p><b>没有找到匹配的商品！</b></p>");
            return;
        }
        
        Response.Write("<table>");
        Response.Write("<tr><th>goodsId</th><th>goodsSn</th><th>Name</th><th>Manufacturers</th>");
        Response.Write("<th>isSale</th><th>goodsStatus</th><th>dataFlag</th><th>pubType</th>");
        Response.Write("<th>状态</th></tr>");
        
        foreach (DataRow row in dt.Rows)
        {
            int isSale = GetInt(row["isSale"], 0);
            int goodsStatus = GetInt(row["goodsStatus"], 0);
            int dataFlag = GetInt(row["dataFlag"], 0);
            
            string statusClass = "highlight";
            string statusText = "未知";
            
            if (dataFlag == 1 && goodsStatus == 1 && isSale == 1)
            {
                statusClass = "visible";
                statusText = "✅ 可搜索";
            }
            else
            {
                statusClass = "hidden";
                statusText = "❌ 不可搜索";
                if (dataFlag != 1) statusText += "(dataFlag=" + dataFlag + ")";
                if (goodsStatus != 1) statusText += "(goodsStatus=" + goodsStatus + ")";
                if (isSale != 1) statusText += "(isSale=" + isSale + ")";
            }
            
            Response.Write("<tr class='" + statusClass + "'>");
            Response.Write("<td>" + GetInt(row["goodsId"], 0) + "</td>");
            Response.Write("<td>" + GetStr(row["goodsSn"]) + "</td>");
            Response.Write("<td>" + GetStr(row["Name"]) + "</td>");
            Response.Write("<td>" + GetStr(row["Manufacturers"]) + "</td>");
            Response.Write("<td>" + isSale + "</td>");
            Response.Write("<td>" + goodsStatus + "</td>");
            Response.Write("<td>" + dataFlag + "</td>");
            Response.Write("<td>" + pubTypeText(GetInt(row["pubType"], 0)) + "</td>");
            Response.Write("<td><b>" + statusText + "</b></td>");
            Response.Write("</tr>");
        }
        
        Response.Write("</table>");
        Response.Write("<p><i>提示：只有 isSale=1、goodsStatus=1、dataFlag=1 的商品才会出现在搜索结果中</i></p>");
    }
    
    private int GetInt(object val, int def)
    {
        if (val == null || val == DBNull.Value) return def;
        return Convert.ToInt32(val);
    }
    
    private string GetStr(object val)
    {
        if (val == null || val == DBNull.Value) return "";
        return Server.HtmlEncode(val.ToString());
    }
    
    private string pubTypeText(int type)
    {
        return type == 2 ? "需求" : "供应";
    }
}
