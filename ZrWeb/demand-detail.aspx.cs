using System;
using System.Web;
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
    protected bool IsLoggedIn = false;
    protected string TradeActionText = "我要报价";

    protected void Page_Load(object sender, EventArgs e)
    {
        IsLoggedIn = Session["UserID"] != null && !string.IsNullOrEmpty(Session["UserID"].ToString());
        
        int currentRoseID = 0;
        if (Session["RoseID"] != null)
        {
            int.TryParse(Session["RoseID"].ToString(), out currentRoseID);
        }
        
        if (currentRoseID == 0)
        {
            HttpCookie userCookie = Request.Cookies["ZrWebUser"];
            if (userCookie != null && !string.IsNullOrEmpty(userCookie["RoseID"]))
            {
                int.TryParse(userCookie["RoseID"], out currentRoseID);
            }
        }
        
        TradeActionText = currentRoseID == 2 ? "立即询价" : "我要报价";
        
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

            int fromRoseID = 0;
            if (Session["RoseID"] != null)
            {
                int.TryParse(Session["RoseID"].ToString(), out fromRoseID);
            }
            if (fromRoseID == 0)
            {
                HttpCookie userCookie = Request.Cookies["ZrWebUser"];
                if (userCookie != null && !string.IsNullOrEmpty(userCookie["RoseID"]))
                {
                    int.TryParse(userCookie["RoseID"], out fromRoseID);
                }
            }
            if (fromRoseID == 2)
            {
                ResponseClearAndWrite(false, "采购商身份不允许提交报价，请使用询价功能");
                return;
            }

            string fromCompany = "";
            string fromContact = "";
            string fromTel = "";
            if (Session["ShopCompany"] != null)
            {
                fromCompany = Session["ShopCompany"].ToString();
            }
            if (string.IsNullOrEmpty(fromCompany) && Session["ShopName"] != null)
            {
                fromCompany = Session["ShopName"].ToString();
            }
            if (string.IsNullOrEmpty(fromCompany))
            {
                fromCompany = "匿名供应商";
            }
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

            toShopId = ShopId;
            
            // 不能给自己店铺的询价进行报价
            if (fromShopId == toShopId)
            {
                ResponseClearAndWrite(false, "不能对自己店铺的询价进行报价");
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
                          WHERE g.goodsId = @goodsId AND g.dataFlag = 1 AND g.pubType = 2 AND g.isSale = 1";

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

                string brand = GetStringValue(row["Brand"]);
                string capacitance = GetStringValue(row["Capacitance"]);
                string resistance = GetStringValue(row["Resistance"]);
                string tolerance = GetStringValue(row["Tolerance"]);
                string voltage = GetStringValue(row["Voltage"]);
                string dielectric = GetStringValue(row["Dielectric"]);
                string power = GetStringValue(row["Power"]);
                string tempCoefficient = GetStringValue(row["TempCoefficient"]);

                if (!string.IsNullOrEmpty(goodsSn))
                {
                    Title = goodsSn;
                    Model = goodsSn;
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    Title = name;
                    Model = name;
                }
                else
                {
                    Title = "电子元器件";
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

                BrandRequirement = !string.IsNullOrEmpty(brand) ? brand : (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "");
                Package = !string.IsNullOrEmpty(packaging) ? packaging : "";

                System.Collections.Generic.List<string> paramList = new System.Collections.Generic.List<string>();
                System.Collections.Generic.Dictionary<string, string> paramDict = new System.Collections.Generic.Dictionary<string, string>();
                
                if (!string.IsNullOrEmpty(brand)) { paramList.Add(brand); paramDict["品牌"] = brand; }
                else if (!string.IsNullOrEmpty(manufacturers)) { paramList.Add(manufacturers); paramDict["品牌"] = manufacturers; }
                if (!string.IsNullOrEmpty(packaging)) { paramList.Add(packaging); paramDict["封装"] = packaging; }
                
                if (!string.IsNullOrEmpty(capacitance)) { paramList.Add(capacitance); paramDict["容值"] = capacitance; }
                if (!string.IsNullOrEmpty(resistance)) { paramList.Add(resistance); paramDict["阻值"] = resistance; }
                if (!string.IsNullOrEmpty(tolerance)) { paramList.Add(tolerance); paramDict["精度"] = tolerance; }
                if (!string.IsNullOrEmpty(voltage)) { paramList.Add(voltage); paramDict["耐压"] = voltage; }
                if (!string.IsNullOrEmpty(dielectric)) { paramList.Add(dielectric); paramDict["介质"] = dielectric; }
                if (!string.IsNullOrEmpty(power)) { paramList.Add(power); paramDict["功率"] = power; }
                if (!string.IsNullOrEmpty(tempCoefficient)) { paramList.Add(tempCoefficient); paramDict["温漂"] = tempCoefficient; }
                
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
                
                Capacitance = !string.IsNullOrEmpty(capacitance) ? capacitance : (paramDict.ContainsKey("容值") ? paramDict["容值"] : "");
                Resistance = !string.IsNullOrEmpty(resistance) ? resistance : (paramDict.ContainsKey("阻值") ? paramDict["阻值"] : "");
                Tolerance = !string.IsNullOrEmpty(tolerance) ? tolerance : (paramDict.ContainsKey("精度") ? paramDict["精度"] : "");
                Voltage = !string.IsNullOrEmpty(voltage) ? voltage : (paramDict.ContainsKey("耐压") ? paramDict["耐压"] : "");
                Dielectric = !string.IsNullOrEmpty(dielectric) ? dielectric : (paramDict.ContainsKey("介质") ? paramDict["介质"] : "");
                Power = !string.IsNullOrEmpty(power) ? power : (paramDict.ContainsKey("功率") ? paramDict["功率"] : "");
                TempCoefficient = !string.IsNullOrEmpty(tempCoefficient) ? tempCoefficient : (paramDict.ContainsKey("温漂") ? paramDict["温漂"] : "");
                
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
                if (diff.TotalDays < 0)
                {
                    Validity = "已过期";
                }
                else if (diff.TotalDays < 1)
                {
                    Validity = "小于1天";
                }
                else
                {
                    int days = (int)diff.TotalDays;
                    Validity = days + "天";
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
        Title = "100K 0603 贴片电阻";
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
        PageTitle = Title + " 采购需求 - 阻容网";
        PageKeywords = Title + ",采购需求,电子元器件,阻容网,电阻电容";
        PageDescription = "【采购】" + Title + " " + ParametersSummary + "，" + Quantity + "，期望价格" + PriceDisplay + "，有效期" + Validity + "，采购商：" + CompanyName;
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