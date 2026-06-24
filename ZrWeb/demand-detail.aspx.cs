using System;
using System.Web.UI;
using System.Configuration;
using System.Data;

public partial class demand_detail : Page
{
    protected string PageTitle = "";
    protected string PageKeywords = "";
    protected string PageDescription = "";
    
    protected string Title = "";
    protected string Model = "";
    protected string PriceDisplay = "";
    protected string PriceClass = "";
    protected string BrandRequirement = "";
    protected string Package = "";
    protected string Capacitance = "";
    protected string Resistance = "";
    protected string Tolerance = "";
    protected string Voltage = "";
    protected string Dielectric = "";
    protected string Power = "";
    protected string TempCoefficient = "";
    protected string Parameters = "";
    protected string ParametersSummary = "";
    protected string Quantity = "";
    protected string Validity = "";
    protected string DeliveryRequirement = "";
    protected string CompanyName = "";
    protected string CompanyAddress = "";
    protected string AuthStatus = "";
    protected int GoodsId = 0;
    protected string GoodsSn = "";
    protected int ShopId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            string action = Request["action"];
            if (action == "submit_quote")
            {
                HandleSubmitQuote();
                return;
            }
        }

        if (!IsPostBack)
        {
            LoadDemandDetail();
        }
    }

    private void HandleSubmitQuote()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);
            
            string goodsSn = Request["goodsSn"] ?? "";
            int fromQuantity = 0;
            int.TryParse(Request["quantity"], out fromQuantity);
            decimal fromPrice = 0;
            decimal.TryParse(Request["price"], out fromPrice);
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);
            string fromRemarks = Request["remarks"] ?? "";
            string brandName = Request["brandName"] ?? "";

            int fromUserId = 0;
            if (Session["UserID"] != null)
            {
                int.TryParse(Session["UserID"].ToString(), out fromUserId);
            }

            string fromCompany = "";
            string fromContact = "";
            string fromTel = "";
            if (Session["LinkMan"] != null)
            {
                fromContact = Session["LinkMan"].ToString();
            }
            if (Session["MobilePhone"] != null)
            {
                fromTel = Session["MobilePhone"].ToString();
            }

            string toCompany = CompanyName;
            int toUserId = 0;
            
            int fromShopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out fromShopId);
            }
            int toShopId = 0;

            if (goodsId <= 0 || fromQuantity <= 0 || fromPrice <= 0 || fromUserId <= 0)
            {
                ResponseClearAndWrite(false, "参数错误");
                return;
            }

            EnquiryQuoteService service = new EnquiryQuoteService();
            bool success = service.SubmitQuote(
                goodsId, goodsSn, fromQuantity, fromPrice, isIncludingTax,
                fromCompany, fromContact, fromTel, fromRemarks,
                toCompany, toUserId, fromUserId, brandName,
                fromShopId, toShopId);

            if (success)
            {
                ResponseClearAndWrite(true, "报价提交成功");
            }
            else
            {
                ResponseClearAndWrite(false, "报价提交失败");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("HandleSubmitQuote 错误: " + ex.Message);
            ResponseClearAndWrite(false, "报价提交异常: " + ex.Message.Replace("\"", "'"));
        }
    }

    private void ResponseClearAndWrite(bool success, string message)
    {
        Response.Clear();
        Response.ContentType = "application/json";
        Response.Charset = "utf-8";
        Response.Write("{\"success\":" + (success ? "true" : "false") + ",\"message\":\"" + message + "\"}");
        Response.End();
    }

    private void LoadDemandDetail()
    {
        int goodsId = 0;
        if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out goodsId) && goodsId > 0)
        {
            BindDemandDetail(goodsId);
        }
        else
        {
            LoadDefaultData();
        }
    }

    private void BindDemandDetail(int goodsId)
    {
        try
        {
            string sql = @"SELECT g.*, s.shopName, s.shopCompany, s.shopAddress, s.userId 
                          FROM goods g 
                          LEFT JOIN shops s ON g.shopId = s.shopId 
                          WHERE g.goodsId = @goodsId AND g.dataFlag = 1 AND g.pubType = 2";

            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                
                GoodsId = goodsId;
                ShopId = GetIntValue(row["shopId"], 0);
                
                string goodsSn = GetStringValue(row["goodsSn"]);
                string name = GetStringValue(row["Name"]);
                string manufacturers = GetStringValue(row["Manufacturers"]);
                string packaging = GetStringValue(row["Packaging"]);
                string lot = GetStringValue(row["Lot"]);
                string goodsDesc = GetStringValue(row["goodsDesc"]);
                int goodsStock = GetIntValue(row["goodsStock"], 0);
                string goodsUnit = GetStringValue(row["goodsUnit"]);
                decimal shopPrice = GetDecimalValue(row["shopPrice"], 0);
                int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
                DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
                DateTime validityDate = GetDateTimeValue(row["validityDate"], DateTime.Now.AddDays(3));
                string shopName = GetStringValue(row["shopName"]);
                string shopCompany = GetStringValue(row["shopCompany"]);
                string shopAddress = GetStringValue(row["shopAddress"]);
                int userId = GetIntValue(row["userId"], 0);

                if (!string.IsNullOrEmpty(goodsSn))
                {
                    Title = "求购 " + goodsSn;
                    Model = goodsSn;
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    Title = "求购 " + name;
                    Model = name;
                }
                else
                {
                    Title = "求购电子元器件";
                    Model = "不限型号";
                }

                if (shopPrice > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    string taxText = isIncludingTax == 1 ? "(含税)" : "(未税)";
                    PriceDisplay = "期望 ¥" + shopPrice.ToString("0.00") + " /" + unit + " " + taxText;
                    PriceClass = "is-expected";
                }
                else
                {
                    PriceDisplay = "面议";
                    PriceClass = "is-negotiable";
                }

                BrandRequirement = !string.IsNullOrEmpty(manufacturers) ? manufacturers : "";
                Package = !string.IsNullOrEmpty(packaging) ? packaging : "";

                System.Collections.Generic.List<string> paramList = new System.Collections.Generic.List<string>();
                System.Collections.Generic.Dictionary<string, string> paramDict = new System.Collections.Generic.Dictionary<string, string>();
                
                if (!string.IsNullOrEmpty(manufacturers)) { paramList.Add(manufacturers); paramDict["品牌"] = manufacturers; }
                if (!string.IsNullOrEmpty(packaging)) { paramList.Add(packaging); paramDict["封装"] = packaging; }
                
                if (!string.IsNullOrEmpty(goodsDesc)) 
                {
                    string[] descParts = goodsDesc.Split(new[] { '·', '|', '/', '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string part in descParts)
                    {
                        string trimmed = part.Trim();
                        if (!string.IsNullOrEmpty(trimmed) && !paramList.Contains(trimmed))
                        {
                            paramList.Add(trimmed);
                            string attrName = IdentifyAttribute(trimmed);
                            if (!string.IsNullOrEmpty(attrName) && !paramDict.ContainsKey(attrName))
                            {
                                paramDict[attrName] = trimmed;
                            }
                        }
                    }
                }
                
                Capacitance = paramDict.ContainsKey("容值") ? paramDict["容值"] : "";
                Resistance = paramDict.ContainsKey("阻值") ? paramDict["阻值"] : "";
                Tolerance = paramDict.ContainsKey("精度") ? paramDict["精度"] : "";
                Voltage = paramDict.ContainsKey("耐压") ? paramDict["耐压"] : "";
                Dielectric = paramDict.ContainsKey("介质") ? paramDict["介质"] : "";
                Power = paramDict.ContainsKey("功率") ? paramDict["功率"] : "";
                TempCoefficient = paramDict.ContainsKey("温漂") ? paramDict["温漂"] : "";
                
                ParametersSummary = string.Join(" · ", paramList);
                
                System.Collections.Generic.List<string> paramPairs = new System.Collections.Generic.List<string>();
                string[] attrOrder = new string[] { "品牌", "封装", "容值", "阻值", "精度", "耐压", "介质", "功率", "温漂" };
                foreach (string attr in attrOrder)
                {
                    if (paramDict.ContainsKey(attr))
                    {
                        paramPairs.Add(attr + ": " + paramDict[attr]);
                    }
                }
                Parameters = paramPairs.Count > 0 ? string.Join(" · ", paramPairs) : (!string.IsNullOrEmpty(goodsDesc) ? goodsDesc : "");

                if (goodsStock > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    Quantity = "需求 " + goodsStock + "/" + unit;
                }
                else
                {
                    Quantity = "需求数量待定";
                }

                TimeSpan diff = validityDate - DateTime.Now;
                if (diff.TotalHours < 24)
                {
                    Validity = "24 小时";
                }
                else if (diff.TotalDays < 3)
                {
                    Validity = "3 天";
                }
                else if (diff.TotalDays < 7)
                {
                    Validity = "7 天";
                }
                else if (diff.TotalDays < 30)
                {
                    Validity = (int)diff.TotalDays + " 天";
                }
                else
                {
                    Validity = validityDate.ToString("yyyy-MM-dd");
                }

                DeliveryRequirement = "尽快";

                CompanyName = !string.IsNullOrEmpty(shopCompany) ? shopCompany : (!string.IsNullOrEmpty(shopName) ? shopName : "匿名采购商");
                CompanyAddress = !string.IsNullOrEmpty(shopAddress) ? shopAddress : "未填写";
                
                if (userId > 0)
                {
                    string authSql = "SELECT IsCertified FROM [dbo].[userinfo] WHERE UserID = @userId";
                    DataTable authDt = DbHelper.ExecuteQuery(authSql, DbHelper.CreateParameter("@userId", userId));
                    if (authDt != null && authDt.Rows.Count > 0)
                    {
                        int isCertified = GetIntValue(authDt.Rows[0]["IsCertified"], 0);
                        AuthStatus = isCertified == 1 ? "已认证" : "待审核";
                    }
                    else
                    {
                        AuthStatus = "未认证";
                    }
                }
                else
                {
                    AuthStatus = "未认证";
                }
                
                GoodsId = goodsId;
                GoodsSn = goodsSn;
            }
            else
            {
                LoadDefaultData();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadDemandDetail 错误: " + ex.Message);
            LoadDefaultData();
        }

        SetSEO();
    }

    private void LoadDefaultData()
    {
        Title = "求购 100K 0603 贴片电阻";
        Model = "不限型号";
        PriceDisplay = "¥6.0 /盘";
        PriceClass = "is-expected";
        BrandRequirement = "";
        Package = "0603";
        Resistance = "100KΩ";
        Tolerance = "±1%";
        ParametersSummary = "0603 · 100KΩ · ±1%";
        Quantity = "需求 184/盘";
        Validity = "24 小时";
        DeliveryRequirement = "3天内";
        CompanyName = "东莞智造电子科技有限公司";
        CompanyAddress = "东莞市松山湖";
        AuthStatus = "已认证";
        
        SetSEO();
    }

    private void SetSEO()
    {
        PageTitle = Title + " 需求详情 - 阻容网";
        PageKeywords = Title + ",采购需求,电子元器件,阻容网";
        PageDescription = Title + " 需求信息，" + Quantity + "，有效期" + Validity + "，采购商：" + CompanyName;
    }

    private int GetIntValue(object value, int defaultValue)
    {
        if (value == DBNull.Value || value == null)
            return defaultValue;
        return Convert.ToInt32(value);
    }

    private decimal GetDecimalValue(object value, decimal defaultValue)
    {
        if (value == DBNull.Value || value == null)
            return defaultValue;
        return Convert.ToDecimal(value);
    }

    private DateTime GetDateTimeValue(object value, DateTime defaultValue)
    {
        if (value == DBNull.Value || value == null)
            return defaultValue;
        return Convert.ToDateTime(value);
    }

    private string GetStringValue(object value)
    {
        if (value == DBNull.Value || value == null)
            return "";
        return value.ToString();
    }

    private string IdentifyAttribute(string value)
    {
        value = value.Trim();
        
        if (value.EndsWith("Ω") || value.EndsWith("kΩ") || value.EndsWith("MΩ"))
            return "阻值";
        
        if (value.EndsWith("F") && (value.Contains("n") || value.Contains("u") || value.Contains("p") || value.Contains("m")))
            return "容值";
        
        if (value.StartsWith("±") || (value.Contains("%") && !value.EndsWith("F")))
            return "精度";
        
        if (value.EndsWith("V") && !value.StartsWith("±"))
            return "耐压";
        
        if (value == "X5R" || value == "X7R" || value == "C0G" || value == "Y5V" || value == "NP0")
            return "介质";
        
        if (value.EndsWith("W"))
            return "功率";
        
        if (value.Contains("ppm"))
            return "温漂";
        
        return "";
    }
}