using System;
using System.Web.UI;
using System.Data;

public partial class supply_detail : Page
{
    protected string PageTitle = "";
    protected string PageKeywords = "";
    protected string PageDescription = "";
    
    protected string Model = "";
    protected string PriceDisplay = "";
    protected string Brand = "";
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
    protected string Remarks = "";
    protected string Quantity = "";
    protected string Validity = "";
    protected string CompanyName = "";
    protected string CompanyAddress = "";
    protected string AuthStatus = "";
    protected int GoodsId = 0;
    protected int ShopId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadSupplyDetail();
        }
    }

    private void LoadSupplyDetail()
    {
        int goodsId = 0;
        if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out goodsId) && goodsId > 0)
        {
            BindSupplyDetail(goodsId);
        }
        else
        {
            LoadDefaultData();
        }
    }

    private void BindSupplyDetail(int goodsId)
    {
        try
        {
            string sql = @"SELECT g.*, s.shopName, s.shopCompany, s.shopAddress, s.userId 
                          FROM goods g 
                          LEFT JOIN shops s ON g.shopId = s.shopId 
                          WHERE g.goodsId = @goodsId AND g.dataFlag = 1 AND g.pubType = 1 AND g.isSale = 1";

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
                string remarks = GetStringValue(row["remarks"]);
                int goodsStock = GetIntValue(row["goodsStock"], 0);
                string goodsUnit = GetStringValue(row["goodsUnit"]);
                decimal shopPrice = GetDecimalValue(row["shopPrice"], 0);
                int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
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

                Model = !string.IsNullOrEmpty(goodsSn) ? goodsSn : (!string.IsNullOrEmpty(name) ? name : "电子元器件");

                if (shopPrice > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    string taxText = isIncludingTax == 1 ? "(含税)" : "(未税)";
                    PriceDisplay = "¥" + shopPrice.ToString("0.00") + " /" + unit + " " + taxText;
                }
                else
                {
                    PriceDisplay = "面议";
                }

                Brand = !string.IsNullOrEmpty(brand) ? brand : (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "");
                Package = !string.IsNullOrEmpty(packaging) ? packaging : "";
                Remarks = !string.IsNullOrEmpty(remarks) ? remarks : "";

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
                
                ParametersSummary = paramList.Count > 0 ? string.Join(" · ", paramList) : (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "");
                
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
                    Quantity = "现货 " + goodsStock + "/" + unit;
                }
                else
                {
                    Quantity = "库存待定";
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

                CompanyName = !string.IsNullOrEmpty(shopCompany) ? shopCompany : (!string.IsNullOrEmpty(shopName) ? shopName : "匿名供应商");
                CompanyAddress = !string.IsNullOrEmpty(shopAddress) ? shopAddress : "未填写";
                
                if (userId > 0)
                {
                    string authSql = "SELECT IsCertified, CompanyName, Province, City, District, Street, Address FROM [dbo].[userinfo] WHERE UserID = @userId";
                    DataTable authDt = DbHelper.ExecuteQuery(authSql, DbHelper.CreateParameter("@userId", userId));
                    if (authDt != null && authDt.Rows.Count > 0)
                    {
                        int isCertified = GetIntValue(authDt.Rows[0]["IsCertified"], 0);
                        AuthStatus = isCertified == 1 ? "已认证" : "待审核";
                        
                        string userCompanyName = GetStringValue(authDt.Rows[0]["CompanyName"]);
                        if (!string.IsNullOrEmpty(userCompanyName))
                        {
                            CompanyName = userCompanyName;
                        }
                        
                        string province = GetStringValue(authDt.Rows[0]["Province"]);
                        string city = GetStringValue(authDt.Rows[0]["City"]);
                        string district = GetStringValue(authDt.Rows[0]["District"]);
                        string street = GetStringValue(authDt.Rows[0]["Street"]);
                        string address = GetStringValue(authDt.Rows[0]["Address"]);
                        
                        if (!string.IsNullOrEmpty(province) || !string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(district) || !string.IsNullOrEmpty(street) || !string.IsNullOrEmpty(address))
                        {
                            CompanyAddress = province + " " + city + " " + district + " " + street + " " + address;
                        }
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
            }
            else
            {
                LoadDefaultData();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadSupplyDetail 错误: " + ex.Message);
            LoadDefaultData();
        }

        SetSEO();
    }

    private void LoadDefaultData()
    {
        Model = "GRM188R71H104KA93D";
        PriceDisplay = "¥12.8 /Kpcs";
        Brand = "村田 Murata";
        Package = "0603";
        Capacitance = "100nF";
        Tolerance = "±10%";
        Voltage = "50V";
        Dielectric = "X7R";
        ParametersSummary = "村田 Murata · 0603 · 100nF · ±10% · 50V · X7R";
        Remarks = "原装正品，现货供应，支持样品";
        Quantity = "现货 850/Kpcs";
        Validity = "72 小时";
        CompanyName = "深圳市华南电子有限公司";
        CompanyAddress = "深圳市福田区华强北";
        AuthStatus = "已认证";
        
        SetSEO();
    }

    private void SetSEO()
    {
        PageTitle = Model + " 供应详情 - 阻容网";
        PageKeywords = Model + "," + Brand + ",贴片电容,0603,供应,阻容网";
        PageDescription = Model + " (" + Brand + " " + Package + " " + Parameters + ") 供应信息，" + Quantity + "，有效期" + Validity + "，供应商：" + CompanyName;
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