using System;
using System.Data;

public partial class quote_records : System.Web.UI.Page
{
    public string PageTitle = "我的报价 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,我的报价,报价记录,供应商";
    public string PageDescription = "查看已提交的报价记录，跟踪报价状态，了解采购商反馈。";
    
    public int CurrentPage = 1;
    public int TotalPages = 1;
    public bool HasData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindQuoteRecords();
        }
    }

    private void BindQuoteRecords()
    {
        DataTable dt = null;
        try
        {
            int userId = UserHelper.GetUserId();
            int shopId = 0;

            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }

            EnquiryQuoteService service = new EnquiryQuoteService();
            
            if (shopId > 0)
            {
                dt = service.GetQuotesByShop(shopId);
            }
            else if (userId > 0)
            {
                dt = service.GetSentQuotes(userId);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("BindQuoteRecords 错误: " + ex.Message);
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            HasData = false;
            dt = new DataTable();
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("StatusClass", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("BrandParams", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("BuyerName", typeof(string));
            dt.Columns.Add("QuoteTime", typeof(string));
            dt.Columns.Add("Validity", typeof(string));
        }
        else
        {
            HasData = true;
            TotalPages = (int)Math.Ceiling((double)dt.Rows.Count / 50);
        }

        rptQuoteRecords.DataSource = dt;
        rptQuoteRecords.DataBind();
    }
}