using System;
using System.Data;
using System.Web;

public partial class received_inquiries : System.Web.UI.Page
{
    public string PageTitle = "收到询价 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,收到询价,询价管理,供应商";
    public string PageDescription = "查看采购商发来的询价信息，及时回复报价，促成交易。";
    
    public int CurrentPage = 1;
    public int TotalPages = 2;

    protected bool IsLoggedIn = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        BindInquiries();
    }

    private void BindInquiries()
    {
        try
        {
            int userId = UserHelper.GetUserId();
            IsLoggedIn = userId > 0;
            
            if (!IsLoggedIn)
            {
                rptInquiries.DataSource = null;
                rptInquiries.DataBind();
                return;
            }
            
            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }
            
            if (shopId == 0)
            {
                shopId = GetDefaultShopId(userId);
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("EqId", typeof(int));
            dt.Columns.Add("GoodsId", typeof(int));
            dt.Columns.Add("Status");
            dt.Columns.Add("StatusClass");
            dt.Columns.Add("Model");
            dt.Columns.Add("BrandParams");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Unit");
            dt.Columns.Add("ExpectedPrice");
            dt.Columns.Add("BuyerName");
            dt.Columns.Add("BuyerContact");
            dt.Columns.Add("InquiryTime");
            dt.Columns.Add("Remarks");
            dt.Columns.Add("Validity");
            dt.Columns.Add("FromShopId", typeof(int));
            dt.Columns.Add("IsGoodsActive", typeof(bool));

            string sql = @"SELECT TOP 50 
                e.eqId, e.goodsId, e.goodsSn, e.fromQuantity, e.toQuantity, e.toPrice, e.fromPrice,
                e.isIncludingTax, e.fromRemarks, e.createTime, e.readStatus, e.validity,
                e.fromCompany, e.fromContact, e.fromTel, e.brandName, e.fromShopId, e.fromLot,
                g.Manufacturers, g.Packaging, g.goodsDesc, g.goodsUnit, g.shopPrice, g.Lot,
                g.isSale, g.goodsStatus, g.dataFlag as GoodsDataFlag
                FROM enquiryquoteprice e
                LEFT JOIN goods g ON e.goodsId = g.goodsId
                WHERE e.toShopId = @shopId AND e.eqType = 1 AND e.dataFlag = 1 AND (e.toDataFlag IS NULL OR e.toDataFlag = 1)
                ORDER BY e.createTime DESC";

            DataTable sourceDt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@shopId", shopId));

            if (sourceDt != null && sourceDt.Rows.Count > 0)
            {
                string updateSql = "UPDATE enquiryquoteprice SET readStatus = 1 WHERE toShopId = @shopId AND eqType = 1 AND dataFlag = 1 AND readStatus = 0";
                DbHelper.ExecuteNonQuery(updateSql, DbHelper.CreateParameter("@shopId", shopId));
                foreach (DataRow row in sourceDt.Rows)
                {
                    DataRow newRow = dt.NewRow();
                    
                    newRow["EqId"] = GetIntValue(row["eqId"], 0);
                    newRow["GoodsId"] = GetIntValue(row["goodsId"], 0);
                    newRow["Model"] = GetStringValue(row["goodsSn"]);
                    string buyerName = CleanStringValue(row["fromCompany"]);
                    if (string.IsNullOrEmpty(buyerName))
                    {
                        buyerName = CleanStringValue(row["fromContact"]);
                    }
                    if (string.IsNullOrEmpty(buyerName))
                    {
                        buyerName = "匿名采购商";
                    }
                    newRow["BuyerName"] = buyerName;
                    newRow["BuyerContact"] = GetStringValue(row["fromContact"]);
                    newRow["InquiryTime"] = Convert.ToDateTime(row["createTime"]).ToString("yyyy-MM-dd HH:mm");
                    newRow["Remarks"] = GetStringValue(row["fromRemarks"]);
                    newRow["FromShopId"] = GetIntValue(row["fromShopId"], 0);

                    bool isActive = true;
                    int goodsId = GetIntValue(row["goodsId"], 0);
                    
                    if (goodsId > 0)
                    {
                        int isSale = GetIntValue(row["isSale"], 0);
                        int goodsStatus = GetIntValue(row["goodsStatus"], 0);
                        int goodsDataFlag = GetIntValue(row["GoodsDataFlag"], 0);
                        isActive = (isSale == 1 && goodsStatus == 1 && goodsDataFlag == 1);
                    }
                    else
                    {
                        string goodsSn = GetStringValue(row["goodsSn"]);
                        if (!string.IsNullOrEmpty(goodsSn))
                        {
                            string checkSql = "SELECT TOP 1 isSale, goodsStatus, dataFlag FROM goods WHERE goodsSn = @goodsSn AND dataFlag = 1 ORDER BY goodsId DESC";
                            DataTable checkDt = DbHelper.ExecuteQuery(checkSql, DbHelper.CreateParameter("@goodsSn", goodsSn));
                            if (checkDt != null && checkDt.Rows.Count > 0)
                            {
                                int isSale = GetIntValue(checkDt.Rows[0]["isSale"], 0);
                                int goodsStatus = GetIntValue(checkDt.Rows[0]["goodsStatus"], 0);
                                int goodsDataFlag = GetIntValue(checkDt.Rows[0]["dataFlag"], 0);
                                isActive = (isSale == 1 && goodsStatus == 1 && goodsDataFlag == 1);
                            }
                        }
                    }
                    newRow["IsGoodsActive"] = isActive;

                    string manufacturers = GetStringValue(row["Manufacturers"]);
                    string packaging = GetStringValue(row["Packaging"]);
                    string goodsDesc = GetStringValue(row["goodsDesc"]);
                    string brandName = GetStringValue(row["brandName"]);
                    
                    if (goodsId == 0)
                    {
                        string goodsSn = GetStringValue(row["goodsSn"]);
                        if (!string.IsNullOrEmpty(goodsSn))
                        {
                            string checkSql = "SELECT TOP 1 Manufacturers, Packaging, goodsDesc, Brand, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Power, TempCoefficient FROM goods WHERE goodsSn = @goodsSn AND dataFlag = 1 ORDER BY goodsId DESC";
                            DataTable checkDt = DbHelper.ExecuteQuery(checkSql, DbHelper.CreateParameter("@goodsSn", goodsSn));
                            if (checkDt != null && checkDt.Rows.Count > 0)
                            {
                                manufacturers = GetStringValue(checkDt.Rows[0]["Manufacturers"]);
                                packaging = GetStringValue(checkDt.Rows[0]["Packaging"]);
                                goodsDesc = GetStringValue(checkDt.Rows[0]["goodsDesc"]);
                                
                                string brand = GetStringValue(checkDt.Rows[0]["Brand"]);
                                string cap = GetStringValue(checkDt.Rows[0]["Capacitance"]);
                                string res = GetStringValue(checkDt.Rows[0]["Resistance"]);
                                string tol = GetStringValue(checkDt.Rows[0]["Tolerance"]);
                                string vol = GetStringValue(checkDt.Rows[0]["Voltage"]);
                                string die = GetStringValue(checkDt.Rows[0]["Dielectric"]);
                                string pow = GetStringValue(checkDt.Rows[0]["Power"]);
                                string tcr = GetStringValue(checkDt.Rows[0]["TempCoefficient"]);
                                
                                if (!string.IsNullOrEmpty(manufacturers) && manufacturers.Contains("·"))
                                {
                                }
                                else
                                {
                                    System.Collections.Generic.List<string> paramsList = new System.Collections.Generic.List<string>();
                                    if (!string.IsNullOrEmpty(brand)) paramsList.Add(brand);
                                    if (!string.IsNullOrEmpty(cap)) paramsList.Add("容值: " + cap);
                                    if (!string.IsNullOrEmpty(res)) paramsList.Add("阻值: " + res);
                                    if (!string.IsNullOrEmpty(tol)) paramsList.Add("精度: " + tol);
                                    if (!string.IsNullOrEmpty(vol)) paramsList.Add("耐压: " + vol);
                                    if (!string.IsNullOrEmpty(die)) paramsList.Add("介质: " + die);
                                    if (!string.IsNullOrEmpty(pow)) paramsList.Add("功率: " + pow);
                                    if (!string.IsNullOrEmpty(tcr)) paramsList.Add("温漂: " + tcr);
                                    if (!string.IsNullOrEmpty(packaging)) paramsList.Add("封装: " + packaging);
                                    
                                    if (paramsList.Count > 0)
                                    {
                                        manufacturers = string.Join(" · ", paramsList);
                                    }
                                }
                            }
                        }
                    }
                    
                    System.Collections.Generic.List<string> paramList = new System.Collections.Generic.List<string>();
                    
                    if (!string.IsNullOrEmpty(manufacturers)) paramList.Add(manufacturers);
                    if (!string.IsNullOrEmpty(packaging) && !manufacturers.Contains(packaging)) paramList.Add(packaging);
                    
                    if (!string.IsNullOrEmpty(goodsDesc))
                    {
                        string[] descParts = goodsDesc.Split(new[] { '·', '|', '/', '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in descParts)
                        {
                            string trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !paramList.Contains(trimmed) && !manufacturers.Contains(trimmed))
                            {
                                paramList.Add(trimmed);
                            }
                        }
                    }
                    
                    if (paramList.Count > 0)
                    {
                        newRow["BrandParams"] = string.Join(" · ", paramList);
                    }
                    else
                    {
                        string brand = GetStringValue(row["brandName"]);
                        newRow["BrandParams"] = !string.IsNullOrEmpty(brand) ? brand : "品牌不限";
                    }

                    // 获取数量
                    int fromQty = GetIntValue(row["fromQuantity"], 0);
                    int toQty = GetIntValue(row["toQuantity"], 0);
                    int qty = toQty > 0 ? toQty : fromQty;
                    newRow["Quantity"] = qty > 0 ? qty.ToString() : "0";
                    newRow["Unit"] = "Kpcs";

                    // 获取期望单价
                    decimal price = GetDecimalValue(row["toPrice"], 0);
                    if (price == 0)
                    {
                        price = GetDecimalValue(row["fromPrice"], 0);
                    }
                    if (price > 0)
                    {
                        string priceStr = price.ToString("0.0000");
                        priceStr = priceStr.TrimEnd('0').TrimEnd('.');
                        if (!priceStr.Contains(".")) priceStr += ".00";
                        newRow["ExpectedPrice"] = priceStr;
                    }
                    else
                    {
                        newRow["ExpectedPrice"] = "面议";
                    }

                    // 获取有效期
                    string validity = GetStringValue(row["validity"]);
                    if (string.IsNullOrEmpty(validity))
                    {
                        DateTime createTime = Convert.ToDateTime(row["createTime"]);
                        TimeSpan diff = DateTime.Now - createTime;
                        if (diff.TotalHours < 24)
                        {
                            validity = "24小时";
                        }
                        else if (diff.TotalDays < 2)
                        {
                            validity = "48小时";
                        }
                        else if (diff.TotalDays < 3)
                        {
                            validity = "72小时";
                        }
                        else
                        {
                            validity = createTime.AddDays(3).ToString("yyyy-MM-dd");
                        }
                    }
                    newRow["Validity"] = validity;

                    // 状态
                    int readStatus = GetIntValue(row["readStatus"], 0);
                    if (readStatus == 0)
                    {
                        newRow["Status"] = "新询价";
                        newRow["StatusClass"] = "green";
                    }
                    else
                    {
                        newRow["Status"] = "已读";
                        newRow["StatusClass"] = "blue";
                    }

                    dt.Rows.Add(newRow);
                }

                TotalPages = (int)Math.Ceiling((double)dt.Rows.Count / 50);
                if (TotalPages < 1) TotalPages = 1;
            }

            rptInquiries.DataSource = dt;
            rptInquiries.DataBind();
            
            // 显示空数据提示
            if (dt.Rows.Count == 0)
            {
                divEmpty.Visible = true;
                if (!IsLoggedIn)
                {
                    litEmptyMsg.Text = "<p>请先登录查看收到的询价</p><a href=\"login.aspx?returnUrl=received-inquiries.aspx\" class=\"btn primary\" style=\"margin-top:10px;\">去登录</a>";
                }
                else
                {
                    litEmptyMsg.Text = "<p>您还没有收到询价</p><p style=\"color:#999;font-size:12px;margin-top:5px;\">发布供应信息后，采购商向您询价时会在这里显示</p>";
                }
            }
            else
            {
                divEmpty.Visible = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("BindInquiries 错误: " + ex.Message);
            // 发生错误时显示空表格
            rptInquiries.DataSource = null;
            rptInquiries.DataBind();
            divEmpty.Visible = true;
            litEmptyMsg.Text = "<p>加载询价列表失败，请稍后重试</p>";
        }
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

    private string GetStringValue(object value)
    {
        if (value == DBNull.Value || value == null)
            return "";
        return value.ToString();
    }
    
    private int GetDefaultShopId(int userId)
    {
        try
        {
            string sql = "SELECT TOP 1 shopId FROM shops WHERE userId = @userId AND dataFlag = 1";
            object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@userId", userId));
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
        }
        catch
        {
        }
        return 0;
    }
    
    private string CleanStringValue(object value)
    {
        if (value == DBNull.Value || value == null)
            return "";
        string str = value.ToString();
        if (string.IsNullOrEmpty(str))
            return "";
        char[] cleanChars = new char[str.Length];
        int idx = 0;
        foreach (char c in str)
        {
            if (c >= 0x20 && c <= 0x7E || 
                c >= 0x4E00 && c <= 0x9FFF || 
                c >= 0x3000 && c <= 0x303F ||
                c >= 0xFF00 && c <= 0xFFEF)
            {
                cleanChars[idx++] = c;
            }
        }
        return new string(cleanChars, 0, idx).Trim();
    }
}