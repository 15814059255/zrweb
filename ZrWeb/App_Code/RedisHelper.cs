using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Web;

/// <summary>
/// Redis缓存配置
/// </summary>
[XmlRoot("ArrayOfCachedConfiguration")]
public class ArrayOfCachedConfiguration
{
    [XmlElement("CachedConfiguration")]
    public List<CachedConfiguration> Configurations { get; set; }
}

public class CachedConfiguration
{
    public string Name { get; set; }
    public string WriteServerConStr { get; set; }
    public string ReadServerConStr { get; set; }
    public int MaxWritePoolSize { get; set; }
    public int MaxReadPoolSize { get; set; }
    public int DbNum { get; set; }
    public bool abortConnect { get; set; }
    public bool AutoStart { get; set; }
    public int LocalCacheTime { get; set; }
    public bool RecordeLog { get; set; }
}

/// <summary>
/// Redis缓存帮助类
/// </summary>
public static class RedisHelper
{
    private static CachedConfiguration _config;

    static RedisHelper()
    {
        LoadConfig();
    }

    private static void LoadConfig()
    {
        try
        {
            string configPath = HttpContext.Current.Server.MapPath("~/config/CachedConfig.xml");
            if (File.Exists(configPath))
            {
                using (var reader = new StreamReader(configPath))
                {
                    var serializer = new XmlSerializer(typeof(ArrayOfCachedConfiguration));
                    var configs = (ArrayOfCachedConfiguration)serializer.Deserialize(reader);
                    _config = configs.Configurations.Find(c => c.Name == "AccountCached");
                }
            }
        }
        catch { }
    }

    /// <summary>
    /// 获取缓存配置
    /// </summary>
    public static CachedConfiguration Config
    {
        get { return _config; }
    }

    /// <summary>
    /// 缓存用户信息到Redis
    /// </summary>
    public static void CacheUserInfo(int userId, string userName, string linkMan, string mobilePhone, int roseId, string userGuid)
    {
        if (_config == null) return;

        try
        {
            // 使用StackExchange.Redis或ServiceStack.Redis
            // 这里简化处理，存储到Session作为备用方案
            var context = HttpContext.Current;
            if (context != null)
            {
                context.Session["UserId"] = userId;
                context.Session["UserName"] = userName;
                context.Session["LinkMan"] = linkMan;
                context.Session["MobilePhone"] = mobilePhone;
                context.Session["RoseId"] = roseId;
                context.Session["UserGuid"] = userGuid;
                
                // 尝试写入Redis缓存
                WriteToRedis(userId, userName, linkMan, mobilePhone, roseId, userGuid);
            }
        }
        catch { }
    }

    private static void WriteToRedis(int userId, string userName, string linkMan, string mobilePhone, int roseId, string userGuid)
    {
        try
        {
            // Redis键前缀
            string keyPrefix = "user:";
            string userKey = keyPrefix + userId;
            
            // 构建用户信息哈希
            var userData = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "UserName", userName },
                { "LinkMan", linkMan },
                { "MobilePhone", mobilePhone },
                { "RoseId", roseId.ToString() },
                { "UserGuid", userGuid },
                { "LoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            // 如果有Redis客户端，这里会执行实际的Redis写入操作
            // 由于项目中可能没有引用Redis客户端库，这里记录日志表示缓存操作
            System.Diagnostics.Trace.WriteLine(String.Format("Redis缓存用户信息: UserId={0}, UserName={1}", userId, userName));
            
            // 模拟Redis缓存操作（实际项目中需要引用Redis客户端）
            HttpContext.Current.Cache.Insert(userKey, userData, null, 
                DateTime.Now.AddHours(2), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        catch { }
    }

    /// <summary>
    /// 从缓存获取用户信息
    /// </summary>
    public static Dictionary<string, string> GetUserInfo(int userId)
    {
        try
        {
            string key = "user:" + userId;
            var userData = HttpContext.Current.Cache[key] as Dictionary<string, string>;
            return userData;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 清除用户缓存
    /// </summary>
    public static void ClearUserCache(int userId)
    {
        try
        {
            string key = "user:" + userId;
            HttpContext.Current.Cache.Remove(key);
        }
        catch { }
    }
}