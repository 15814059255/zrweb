using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using System.Net.Sockets;
using System.Text;
using System.Globalization;

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

public static class RedisHelper
{
    private static CachedConfiguration _config;
    private static string _host;
    private static int _port;
    private static string _password;

    static RedisHelper()
    {
        LoadConfig();
        ParseConnectionString();
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

    private static void ParseConnectionString()
    {
        if (_config != null && !string.IsNullOrEmpty(_config.WriteServerConStr))
        {
            string conStr = _config.WriteServerConStr;
            string[] parts = conStr.Split(',');
            
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (trimmed.Contains(":"))
                {
                    _host = trimmed.Split(':')[0];
                    string portStr = trimmed.Split(':')[1];
                    int.TryParse(portStr, out _port);
                }
                else if (trimmed.StartsWith("password=", StringComparison.OrdinalIgnoreCase))
                {
                    _password = trimmed.Substring("password=".Length);
                }
            }
        }
    }

    public static CachedConfiguration Config
    {
        get { return _config; }
    }

    public static void CacheUserInfo(int userId, string userName, string linkMan, string mobilePhone, int roseId, string userGuid, int shopId = 0, string shopName = "", string shopCompany = "")
    {
        if (_config == null) return;

        try
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                context.Session["UserID"] = userId;
                context.Session["UserName"] = userName;
                context.Session["LinkMan"] = linkMan;
                context.Session["MobilePhone"] = mobilePhone;
                context.Session["RoseID"] = roseId;
                context.Session["UserGuid"] = userGuid;
                context.Session["ShopId"] = shopId;
                context.Session["ShopName"] = shopName;
                context.Session["ShopCompany"] = shopCompany;
                
                WriteToRedis(userId, userName, linkMan, mobilePhone, roseId, userGuid, shopId, shopName, shopCompany);
            }
        }
        catch { }
    }

    private static void WriteToRedis(int userId, string userName, string linkMan, string mobilePhone, int roseId, string userGuid, int shopId = 0, string shopName = "", string shopCompany = "")
    {
        try
        {
            string userKey = "user:" + userId;
            
            var userData = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "UserName", userName },
                { "LinkMan", linkMan },
                { "MobilePhone", mobilePhone },
                { "RoseID", roseId.ToString() },
                { "UserGuid", userGuid },
                { "ShopId", shopId.ToString() },
                { "ShopName", shopName },
                { "ShopCompany", shopCompany },
                { "LoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            using (var socket = Connect())
            {
                if (socket == null) return;

                if (!string.IsNullOrEmpty(_password))
                {
                    SendCommand(socket, "AUTH", _password);
                    ReadResponse(socket);
                }

                SendCommand(socket, "DEL", userKey);
                
                foreach (var kvp in userData)
                {
                    SendCommand(socket, "HSET", userKey, kvp.Key, kvp.Value);
                    ReadResponse(socket);
                }

                SendCommand(socket, "EXPIRE", userKey, "604800");
                ReadResponse(socket);

                socket.Close();
            }

            System.Diagnostics.Trace.WriteLine(String.Format("Redis缓存用户信息成功: UserId={0}, UserName={1}", userId, userName));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("Redis缓存用户信息失败: {0}", ex.Message));
        }
    }

    public static Dictionary<string, string> GetUserInfo(int userId)
    {
        try
        {
            string userKey = "user:" + userId;
            var result = new Dictionary<string, string>();

            using (var socket = Connect())
            {
                if (socket == null) return null;

                if (!string.IsNullOrEmpty(_password))
                {
                    SendCommand(socket, "AUTH", _password);
                    ReadResponse(socket);
                }

                SendCommand(socket, "HGETALL", userKey);
                var response = ReadResponse(socket);

                socket.Close();

                if (response != null && response.Count % 2 == 0)
                {
                    for (int i = 0; i < response.Count; i += 2)
                    {
                        string key = response[i];
                        string value = response[i + 1];
                        result[key] = value;
                    }
                }
            }

            return result.Count > 0 ? result : null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("Redis获取用户信息失败: {0}", ex.Message));
            return null;
        }
    }

    public static void ClearUserCache(int userId)
    {
        try
        {
            string userKey = "user:" + userId;

            using (var socket = Connect())
            {
                if (socket == null) return;

                if (!string.IsNullOrEmpty(_password))
                {
                    SendCommand(socket, "AUTH", _password);
                    ReadResponse(socket);
                }

                SendCommand(socket, "DEL", userKey);
                ReadResponse(socket);

                socket.Close();
            }
        }
        catch { }
    }

    private static TcpClient Connect()
    {
        try
        {
            if (string.IsNullOrEmpty(_host) || _port <= 0)
                return null;

            var socket = new TcpClient();
            socket.Connect(_host, _port);
            return socket;
        }
        catch
        {
            return null;
        }
    }

    private static void SendCommand(TcpClient socket, string command, params string[] args)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("*").Append(args.Length + 1).Append("\r\n");
            
            byte[] cmdBytes = Encoding.UTF8.GetBytes(command);
            sb.Append("$").Append(cmdBytes.Length).Append("\r\n");
            sb.Append(command).Append("\r\n");

            foreach (string arg in args)
            {
                byte[] argBytes = Encoding.UTF8.GetBytes(arg);
                sb.Append("$").Append(argBytes.Length).Append("\r\n");
                sb.Append(arg).Append("\r\n");
            }

            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
            socket.GetStream().Write(data, 0, data.Length);
        }
        catch { }
    }

    private static List<string> ReadResponse(TcpClient socket)
    {
        try
        {
            var response = new List<string>();
            var stream = socket.GetStream();
            var buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            
            if (bytesRead == 0) return response;

            string responseStr = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string[] lines = responseStr.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            int index = 0;
            while (index < lines.Length)
            {
                string line = lines[index];
                if (line.StartsWith("*"))
                {
                    int count = int.Parse(line.Substring(1));
                    index++;
                    for (int i = 0; i < count; i++)
                    {
                        if (index < lines.Length && lines[index].StartsWith("$"))
                        {
                            int len = int.Parse(lines[index].Substring(1));
                            index++;
                            if (index < lines.Length)
                            {
                                response.Add(lines[index]);
                                index++;
                            }
                        }
                    }
                }
                else if (line.StartsWith("$"))
                {
                    int len = int.Parse(line.Substring(1));
                    index++;
                    if (index < lines.Length)
                    {
                        response.Add(lines[index]);
                        index++;
                    }
                }
                else if (line.StartsWith("+"))
                {
                    response.Add(line.Substring(1));
                    index++;
                }
                else
                {
                    index++;
                }
            }

            return response;
        }
        catch
        {
            return new List<string>();
        }
    }
}