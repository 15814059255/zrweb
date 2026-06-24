using System;
using System.Web.UI;
using System.Configuration;
using System.Data;

public partial class search : Page
{
    protected string PageTitle = "搜索结果 - 电子元器件 B2B 平台";
    protected string PageKeywords = "阻容网,搜索,电子元器件,贴片电容,贴片电阻";
    protected string PageDescription = "阻容网搜索功能，快速查找电子元器件供应和需求信息。";
    
    protected string SearchKeyword = "";
    protected int ResultCount = 0;
    protected string PaginationHtml = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        SearchKeyword = Request.QueryString["q"] ?? "";
        
        if (!IsPostBack)
        {
            BindSearchResults();
        }
    }

    private void BindSearchResults()
    {
        // 创建模拟数据表
        DataTable dt = new DataTable();
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("DetailUrl", typeof(string));
        dt.Columns.Add("TagClass", typeof(string));
        dt.Columns.Add("TypeLabel", typeof(string));
        dt.Columns.Add("CompanyName", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("QuantityDisplay", typeof(string));
        dt.Columns.Add("Validity", typeof(string));
        dt.Columns.Add("PriceClass", typeof(string));
        dt.Columns.Add("PriceDisplay", typeof(string));
        dt.Columns.Add("ActionClass", typeof(string));
        dt.Columns.Add("ActionText", typeof(string));
        dt.Columns.Add("ShopId", typeof(int));

        // 添加模拟搜索结果
        AddResultRow(dt, "supply", "GRM188R71H104KA93D", "深圳市华南电子有限公司", "村田 Murata · 0603 · 100nF · X7R · 50V", "现货 850/Kpcs", "72 小时", "¥12.8 /Kpcs", "立即询价", 1);
        AddResultRow(dt, "supply", "RC0603FR-0710KL", "深圳国巨现货供应链有限公司", "Yageo · 0603 · 10KΩ · ±1% · 1/10W", "现货 1200/Pcs", "72 小时", "¥6.5 /Pcs", "立即询价", 2);
        AddResultRow(dt, "supply", "GRM21BR71H105KA12L", "华强北被动件仓储中心", "村田 Murata · 0805 · 1uF · X7R · 50V", "现货 120/卷", "72 小时", "¥18.0 /卷", "立即询价", 3);
        AddResultRow(dt, "demand", "CL10B104KB8NNNC", "东莞某 EMS 工厂", "三星 · 0603 · 100nF · X7R · 50V", "需求 500/Kpcs", "72 小时", "期望 ¥11 /Kpcs", "我要报价", 4);
        AddResultRow(dt, "demand", "0603 4.7K ±1%", "苏州精密制造有限公司", "不限品牌 · 电阻 · 交期 3 天", "需求 1000/Kpcs", "72 小时", "面议", "我要报价", 5);
        AddResultRow(dt, "demand", "GRM188R71H104KA93D", "宁波工业控制设备有限公司", "村田 Murata · 0603 · 100nF · X7R · 50V", "需求 850/Kpcs", "72 小时", "期望 ¥12 /Kpcs", "我要报价", 6);

        ResultCount = dt.Rows.Count;
        rptSearchResults.DataSource = dt;
        rptSearchResults.DataBind();

        // 生成分页HTML
        PaginationHtml = GeneratePagination(1, 10, ResultCount);
    }

    private void AddResultRow(DataTable dt, string type, string model, string company, string brandParams, string quantity, string validity, string price, string action, int shopId)
    {
        DataRow row = dt.NewRow();
        row["Model"] = model;
        row["DetailUrl"] = type == "supply" ? "/supply-detail.aspx?id=" + shopId : "/demand-detail.aspx?id=" + shopId;
        row["TagClass"] = type == "supply" ? "blue" : "green";
        row["TypeLabel"] = type == "supply" ? "供应" : "需求";
        row["CompanyName"] = company;
        row["BrandParams"] = brandParams;
        row["QuantityDisplay"] = quantity;
        row["Validity"] = validity;
        row["PriceClass"] = type == "demand" && price.Contains("期望") ? "is-expected" : (price == "面议" ? "is-negotiable" : "");
        row["PriceDisplay"] = price;
        row["ActionClass"] = type == "demand" ? "is-demand-action" : "";
        row["ActionText"] = action;
        row["ShopId"] = shopId;
        dt.Rows.Add(row);
    }

    private string GeneratePagination(int currentPage, int pageSize, int totalCount)
    {
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (totalPages <= 1) return "";
        
        string html = "";
        if (currentPage > 1)
            html += "<button class=\"btn\" type=\"button\" onclick=\"location.href='/search.aspx?q=" + SearchKeyword + "&page=" + (currentPage - 1) + "'\">上一页</button>";
        
        html += "<span>第 " + currentPage + " / " + totalPages + " 页</span>";
        html += "<span class=\"page-size\">每页 " + pageSize + " 条</span>";
        
        if (currentPage < totalPages)
            html += "<button class=\"btn\" type=\"button\" onclick=\"location.href='/search.aspx?q=" + SearchKeyword + "&page=" + (currentPage + 1) + "'\">下一页</button>";
        
        return html;
    }
}