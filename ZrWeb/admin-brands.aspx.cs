using System;
using System.Data;
using System.Text;

public partial class admin_brands : System.Web.UI.Page
{
    public string AdminName = "管理员";
    public int BrandCount = 0;
    public string BrandsHtml = "";
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CheckLogin();
            LoadBrands();
        }
    }
    
    private void CheckLogin()
    {
        if (Session["AdminID"] == null)
        {
            Response.Redirect("admin-login.aspx");
        }
        else
        {
            object adminNameObj = Session["AdminName"];
            if (adminNameObj != null)
            {
                AdminName = adminNameObj.ToString();
            }
        }
    }
    
    private void LoadBrands()
    {
        try
        {
            // 先确保表存在并初始化数据
            EnsureBrandsTableExists();
            
            // 查询数据
            DataTable dt = DbHelper.ExecuteQuery("SELECT BrandId, BrandName, BrandLogo, BrandDesc, SortOrder, Status FROM brands WHERE Status = 1 ORDER BY SortOrder, BrandId");
            
            if (dt == null || dt.Rows.Count == 0)
            {
                // 初始化数据
                InitBrandsData();
                dt = DbHelper.ExecuteQuery("SELECT BrandId, BrandName, BrandLogo, BrandDesc, SortOrder, Status FROM brands WHERE Status = 1 ORDER BY SortOrder, BrandId");
            }
            
            if (dt != null)
            {
                BrandCount = dt.Rows.Count;
                
                if (dt.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    int index = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = row["BrandName"] != null ? row["BrandName"].ToString() : "";
                        string desc = row["BrandDesc"] != null ? row["BrandDesc"].ToString() : "";
                        string safeName = name.Replace("'", "\\'").Replace("\"", "\\\"");
                        string safeDesc = desc.Replace("'", "\\'").Replace("\"", "\\\"");
                        
                        sb.Append("<tr>");
                        sb.Append("<td>" + index++ + "</td>");
                        sb.Append("<td><strong>" + name + "</strong></td>");
                        sb.Append("<td>" + (string.IsNullOrEmpty(desc) ? "-" : desc) + "</td>");
                        sb.Append("<td>" + (row["SortOrder"] ?? 0) + "</td>");
                        sb.Append("<td><span class=\"tag green\">启用</span></td>");
                        sb.Append("<td>");
                        sb.Append("<button class=\"btn mini\" onclick=\"editBrand(" + row["BrandId"] + ",\'" + safeName + "\',\'" + safeDesc + "\'," + (row["SortOrder"] ?? 0) + ")\">编辑</button> ");
                        sb.Append("<button class=\"btn mini danger\" onclick=\"deleteBrand(" + row["BrandId"] + ")\">删除</button>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                    BrandsHtml = sb.ToString();
                }
                else
                {
                    BrandsHtml = "<tr><td colspan=\"6\" style=\"text-align:center;padding:40px;\">暂无品牌数据</td></tr>";
                }
            }
            else
            {
                BrandsHtml = "<tr><td colspan=\"6\" style=\"text-align:center;padding:40px;\">加载失败</td></tr>";
            }
        }
        catch (Exception ex)
        {
            BrandsHtml = "<tr><td colspan=\"6\" style=\"text-align:center;color:red;padding:40px;\">加载异常: " + ex.Message + "</td></tr>";
        }
    }
    
    private void EnsureBrandsTableExists()
    {
        string checkSql = "SELECT COUNT(*) FROM sys.tables WHERE name = 'brands'";
        object result = DbHelper.ExecuteScalar(checkSql);
        
        if (result == null || Convert.ToInt32(result) == 0)
        {
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
        }
        else
        {
            // 表已存在，检查并添加缺失字段
            EnsureColumnExists("brands", "BrandLogo", "NVARCHAR(500) NULL");
            EnsureColumnExists("brands", "BrandDesc", "NVARCHAR(500) NULL");
            EnsureColumnExists("brands", "SortOrder", "INT DEFAULT 0");
            EnsureColumnExists("brands", "Status", "INT DEFAULT 1");
            EnsureColumnExists("brands", "CreateTime", "DATETIME DEFAULT GETDATE()");
        }
    }
    
    private void EnsureColumnExists(string tableName, string columnName, string columnType)
    {
        try
        {
            string checkColSql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('" + tableName + "') AND name = '" + columnName + "'";
            object colResult = DbHelper.ExecuteScalar(checkColSql);
            if (colResult == null || Convert.ToInt32(colResult) == 0)
            {
                string addColSql = "ALTER TABLE " + tableName + " ADD " + columnName + " " + columnType;
                DbHelper.ExecuteNonQuery(addColSql);
            }
        }
        catch
        {
            // 字段可能已存在或其他错误，忽略
        }
    }
    
    private void InitBrandsData()
    {
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
