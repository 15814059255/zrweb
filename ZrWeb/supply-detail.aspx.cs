using System;
using System.Web.UI;
using System.Configuration;

public partial class supply_detail : Page
{
    protected string PageTitle = "";
    protected string PageKeywords = "";
    protected string PageDescription = "";
    
    protected string Model = "";
    protected string PriceDisplay = "";
    protected string Brand = "";
    protected string Package = "";
    protected string Parameters = "";
    protected string Quantity = "";
    protected string Validity = "";
    protected string CompanyName = "";
    protected string CompanyAddress = "";
    protected string AuthStatus = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        Model = Request.QueryString["model"] ?? "GRM188R71H104KA93D";
        
        if (!IsPostBack)
        {
            LoadSupplyDetail();
        }
    }

    private void LoadSupplyDetail()
    {
        // 模拟数据 - 后续对接数据库
        Model = "GRM188R71H104KA93D";
        PriceDisplay = "¥12.8 /Kpcs";
        Brand = "村田 Murata";
        Package = "0603";
        Parameters = "100nF · X7R · 50V";
        Quantity = "现货 850/Kpcs";
        Validity = "72 小时";
        CompanyName = "深圳市华南电子有限公司";
        CompanyAddress = "深圳市福田区华强北";
        AuthStatus = "已认证";

        // SEO 设置
        PageTitle = Model + " 供应详情 - 阻容网";
        PageKeywords = Model + "," + Brand + ",贴片电容,0603,供应,阻容网";
        PageDescription = Model + " (" + Brand + " " + Package + " " + Parameters + ") 供应信息，" + Quantity + "，有效期" + Validity + "，供应商：" + CompanyName;
    }
}