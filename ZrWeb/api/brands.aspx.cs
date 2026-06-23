using System;
using System.Data;

public partial class api_brands : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json";
        Response.Charset = "utf-8";
        
        string action = Request["action"] ?? "list";
        
        try
        {
            // 确保表存在
            EnsureTableExists();
            
            switch (action)
            {
                case "list":
                    GetBrands();
                    break;
                case "add":
                    AddBrand();
                    break;
                case "update":
                    UpdateBrand();
                    break;
                case "delete":
                    DeleteBrand();
                    break;
                default:
                    GetBrands();
                    break;
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"异常: " + EscapeJsonString(ex.Message) + "\"}");
        }
    }
    
    private string EscapeJsonString(string str)
    {
        if (string.IsNullOrEmpty(str)) return "";
        return str
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
    
    private void EnsureTableExists()
    {
        string checkSql = "SELECT COUNT(*) FROM sys.tables WHERE name = 'brands'";
        object result = DbHelper.ExecuteScalar(checkSql);
        
        if (result == null || Convert.ToInt32(result) == 0)
        {
            // 创建品牌表
            string createSql = @"CREATE TABLE brands (
                BrandId INT IDENTITY(1,1) PRIMARY KEY,
                BrandName NVARCHAR(200) NOT NULL,
                BrandLogo NVARCHAR(500) NULL,
                BrandDesc NVARCHAR(500) NULL,
                SortOrder INT DEFAULT 0,
                Status INT DEFAULT 1,
                CreateTime DATETIME DEFAULT GETDATE()
            )";
            DbHelper.ExecuteNonQuery(createSql);
            
            // 插入默认品牌（从电子元器件选型系统提取的品牌）
            string[] defaultBrands = {
                "FOJAN(富捷)", "UNI-ROYAL(厚声)", "SAMSUNG(三星)", "HRK(鸿瑞凯电子)", "YAGEO(国巨)",
                "MURATA(村田)", "FENGHUA(风华高科)", "TORCH(火炬电子)", "HONGYUAN(鸿远电子)", "HONGDA(宏达电子)",
                "SANHUAN(三环集团)", "SUNLORD(顺络电子)", "AIHUA(艾华集团)", "FARATRONIC(法拉电子)", "JIANGHAI(江海股份)",
                "YUYANG(宇阳科技)", "CCTC(潮州三环)", "TECSTAR(台星)", "WALTER(华德电子)", "EVER(亿能电子)",
                "TA-I-CN(大毅科技大陆)", "ROYALOHM-CN(厚生大陆)", "VIKING-CN(光颉科技大陆)", "TTC-CN(台湾电阻大陆)",
                "PSA-CN(信昌电陶大陆)", "TDK-CN(TDK中国)", "MURATA-CN(村田中国)", "SAMSUNG-CN(三星中国)",
                "AVX-CN(AVX中国)", "VISHAY-CN(威世中国)", "KEMET-CN(基美中国)", "BOURNS-CN(伯恩斯中国)",
                "HRE(芯声)", "PANASONIC-CN(松下中国)", "KYOCERA-CN(京瓷中国)", "AISHI(艾华)", "SUNTAN(日电导)",
                "KINJI(金积)", "UNIOHM(厚声)", "RALEC(旺诠)", "TAITEK(泰铭)", "SINOCARTA(华瓷)",
                "CHINOCERA(中瓷)", "SUSUMU-CN(进工业中国)", "KOA-CN(KOA中国)", "WALSIN-TW(华新科)",
                "YAGEO-TW(国巨台湾)", "TA-I-TW(大毅台湾)", "VIKING-TW(光颉台湾)", "ROYALOHM-TW(厚生台湾)",
                "TTC-TW(台湾电阻台湾)", "HUIJU(汇聚)", "PSH(信昌电陶台湾)", "HOLYSTONE(禾伸堂)",
                "TDK-TW(TDK台湾)", "MURATA-TW(村田台湾)", "AVX-TW(AVX台湾)", "KEMET-TW(基美台湾)",
                "VISHAY-TW(威世台湾)", "BOURNS-TW(伯恩斯台湾)", "LITTELFUSE-TW(力特台湾)", "SAMSUNG-TW(三星台湾)",
                "PANASONIC-TW(松下台湾)", "MURATA-JP(村田日本)", "TDK-JP(TDK日本)", "TAIYO(太阳诱电)",
                "KYOCERA(京瓷)", "PANASONIC(松下)", "SANYO(三洋)", "RUBYCON(红宝石)", "NICHICON(尼吉康)",
                "CHEMI-CON(贵弥功)", "ELNA(依娜)", "NIPPON(日本化工)", "NCC(日本贵弥功)", "DARFON(达方)",
                "GYZ(昀冢科技)", "SAMWHA(三和)", "OKAYA(冈谷)", "SAMSUNG-KR(三星电机韩国)", "SEMCO(三星电机)",
                "KEMET-KR(基美韩国)", "AVX(AVX)", "Knowles(楼氏)", "JOHANSON(约翰逊)", "MEGSINE(兆讯)",
                "VISHAY(威世)", "KEMET(基美)", "BOURNS(伯恩斯)"
            };
            
            for (int i = 0; i < defaultBrands.Length; i++)
            {
                string insertSql = "INSERT INTO brands (BrandName, SortOrder, Status) VALUES (@name, @order, 1)";
                DbHelper.ExecuteNonQuery(insertSql,
                    DbHelper.CreateParameter("@name", defaultBrands[i]),
                    DbHelper.CreateParameter("@order", i + 1));
            }
        }
    }
    
    private void GetBrands()
    {
        // 从数据库表读取品牌数据
        DataTable dt = DbHelper.ExecuteQuery("SELECT BrandId, BrandName, BrandLogo, BrandDesc, SortOrder, Status FROM brands WHERE Status = 1 ORDER BY SortOrder, BrandId");
        
        string json = "{\"success\":true,\"brands\":[";
        if (dt != null && dt.Rows.Count > 0)
        {
            bool first = true;
            foreach (DataRow row in dt.Rows)
            {
                if (!first) json += ",";
                json += "{\"BrandId\":" + row["BrandId"];
                json += ",\"BrandName\":\"" + EscapeJsonString(row["BrandName"] == null ? "" : row["BrandName"].ToString()) + "\"";
                json += ",\"BrandLogo\":\"" + EscapeJsonString(row["BrandLogo"] == null ? "" : row["BrandLogo"].ToString()) + "\"";
                json += ",\"BrandDesc\":\"" + EscapeJsonString(row["BrandDesc"] == null ? "" : row["BrandDesc"].ToString()) + "\"";
                json += ",\"SortOrder\":" + (row["SortOrder"] ?? 0);
                json += ",\"Status\":" + (row["Status"] ?? 1) + "}";
                first = false;
            }
        }
        json += "]}";
        
        Response.Write(json);
    }
    
    private void AddBrand()
    {
        string brandName = Request["brandName"] ?? "";
        string brandLogo = Request["brandLogo"] ?? "";
        string brandDesc = Request["brandDesc"] ?? "";
        int sortOrder = 0;
        int.TryParse(Request["sortOrder"] ?? "0", out sortOrder);
        
        if (string.IsNullOrEmpty(brandName))
        {
            Response.Write("{\"success\":false,\"message\":\"品牌名称不能为空\"}");
            return;
        }
        
        string sql = "INSERT INTO brands (BrandName, BrandLogo, BrandDesc, SortOrder) VALUES (@name, @logo, @desc, @order)";
        int rows = DbHelper.ExecuteNonQuery(sql,
            DbHelper.CreateParameter("@name", brandName),
            DbHelper.CreateParameter("@logo", brandLogo),
            DbHelper.CreateParameter("@desc", brandDesc),
            DbHelper.CreateParameter("@order", sortOrder));
        
        Response.Write(rows > 0 ? "{\"success\":true,\"message\":\"添加成功\"}" : "{\"success\":false,\"message\":\"添加失败\"}");
    }
    
    private void UpdateBrand()
    {
        int brandId = 0;
        int.TryParse(Request["brandId"] ?? "0", out brandId);
        string brandName = Request["brandName"] ?? "";
        string brandLogo = Request["brandLogo"] ?? "";
        string brandDesc = Request["brandDesc"] ?? "";
        int sortOrder = 0;
        int.TryParse(Request["sortOrder"] ?? "0", out sortOrder);
        
        if (brandId == 0)
        {
            Response.Write("{\"success\":false,\"message\":\"品牌ID无效\"}");
            return;
        }
        
        if (string.IsNullOrEmpty(brandName))
        {
            Response.Write("{\"success\":false,\"message\":\"品牌名称不能为空\"}");
            return;
        }
        
        string sql = "UPDATE brands SET BrandName=@name, BrandLogo=@logo, BrandDesc=@desc, SortOrder=@order WHERE BrandId=@id";
        int rows = DbHelper.ExecuteNonQuery(sql,
            DbHelper.CreateParameter("@name", brandName),
            DbHelper.CreateParameter("@logo", brandLogo),
            DbHelper.CreateParameter("@desc", brandDesc),
            DbHelper.CreateParameter("@order", sortOrder),
            DbHelper.CreateParameter("@id", brandId));
        
        Response.Write(rows > 0 ? "{\"success\":true,\"message\":\"更新成功\"}" : "{\"success\":false,\"message\":\"更新失败\"}");
    }
    
    private void DeleteBrand()
    {
        int brandId = 0;
        int.TryParse(Request["brandId"] ?? "0", out brandId);
        
        if (brandId == 0)
        {
            Response.Write("{\"success\":false,\"message\":\"品牌ID无效\"}");
            return;
        }
        
        // 软删除
        string sql = "UPDATE brands SET Status = 0 WHERE BrandId = @id";
        int rows = DbHelper.ExecuteNonQuery(sql, DbHelper.CreateParameter("@id", brandId));
        
        Response.Write(rows > 0 ? "{\"success\":true,\"message\":\"删除成功\"}" : "{\"success\":false,\"message\":\"删除失败\"}");
    }
}
