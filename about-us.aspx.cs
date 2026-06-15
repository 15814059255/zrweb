using System;
using System.Web.UI;
using System.Configuration;

public partial class about_us : Page
{
    protected string PageTitle = "关于我们 - 阻容网";
    protected string PageKeywords = "阻容网,关于我们,电子元器件,B2B平台,供需撮合";
    protected string PageDescription = "阻容网是专业的电子元器件B2B交易平台，专注阻容元件供需信息撮合，帮助供应商和采购商快速对接。";
    
    protected string SiteDomain = ConfigurationManager.AppSettings["SiteDomain"] ?? "ZR.net.cn";
    protected string CompanyAddress = "深圳市福田区华强北电子商务产业带";
    protected string ServiceEmail = "service@ZR.net.cn";

    protected void Page_Load(object sender, EventArgs e)
    {
    }
}