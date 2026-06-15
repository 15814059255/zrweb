using System;
using System.Web;
using System.Collections.Generic;

public static class CachedHelper
{
    private static readonly string KeyPrefix = "zrweb:";

    public static bool StringSet(string key, string value)
    {
        return StringSet(key, value, null);
    }

    public static bool StringSet(string key, string value, TimeSpan? expiry)
    {
        try
        {
            string cacheKey = KeyPrefix + key;
            
            if (expiry.HasValue)
            {
                HttpContext.Current.Cache.Insert(
                    cacheKey,
                    value,
                    null,
                    DateTime.Now.Add(expiry.Value),
                    System.Web.Caching.Cache.NoSlidingExpiration
                );
            }
            else
            {
                HttpContext.Current.Cache.Insert(
                    cacheKey,
                    value,
                    null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    System.Web.Caching.Cache.NoSlidingExpiration
                );
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string StringGet(string key)
    {
        try
        {
            string cacheKey = KeyPrefix + key;
            return HttpContext.Current.Cache[cacheKey] as string;
        }
        catch
        {
            return null;
        }
    }

    public static bool StringSetUserInfo(int userId, Dictionary<string, string> userData, int expireMinutes = 120)
    {
        try
        {
            string key = "user:" + userId;
            string value = SerializeUserData(userData);
            return StringSet(key, value, TimeSpan.FromMinutes(expireMinutes));
        }
        catch
        {
            return false;
        }
    }

    public static Dictionary<string, string> StringGetUserInfo(int userId)
    {
        try
        {
            string key = "user:" + userId;
            string value = StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return DeserializeUserData(value);
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public static bool Remove(string key)
    {
        try
        {
            string cacheKey = KeyPrefix + key;
            HttpContext.Current.Cache.Remove(cacheKey);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string SerializeUserData(Dictionary<string, string> data)
    {
        List<string> parts = new List<string>();
        foreach (var kvp in data)
        {
            parts.Add(String.Format("{0}={1}", EscapeKey(kvp.Key), EscapeValue(kvp.Value)));
        }
        return string.Join("&", parts);
    }

    private static Dictionary<string, string> DeserializeUserData(string value)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string[] parts = value.Split('&');
        foreach (string part in parts)
        {
            int idx = part.IndexOf('=');
            if (idx > 0)
            {
                string key = UnescapeKey(part.Substring(0, idx));
                string val = UnescapeValue(part.Substring(idx + 1));
                data[key] = val;
            }
        }
        return data;
    }

    private static string EscapeKey(string key)
    {
        return key.Replace("=", "%3D").Replace("&", "%26");
    }

    private static string UnescapeKey(string key)
    {
        return key.Replace("%26", "&").Replace("%3D", "=");
    }

    private static string EscapeValue(string value)
    {
        return value.Replace("=", "%3D").Replace("&", "%26");
    }

    private static string UnescapeValue(string value)
    {
        return value.Replace("%26", "&").Replace("%3D", "=");
    }
}