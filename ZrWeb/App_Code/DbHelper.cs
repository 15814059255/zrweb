using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// 数据库帮助类
/// </summary>
public class DbHelper
{
    private static string connectionString = ConfigurationManager.ConnectionStrings["ZrWebConnectionString"].ConnectionString;

    /// <summary>
    /// 获取数据库连接
    /// </summary>
    public static SqlConnection GetConnection()
    {
        SqlConnection conn = new SqlConnection(connectionString);
        return conn;
    }

    /// <summary>
    /// 执行查询，返回DataTable
    /// </summary>
    public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = GetConnection())
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    /// <summary>
    /// 执行查询，返回DataSet
    /// </summary>
    public static DataSet ExecuteQueryDataSet(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = GetConnection())
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
    }

    /// <summary>
    /// 执行非查询语句，返回受影响的行数
    /// </summary>
    public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = GetConnection())
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 执行查询，返回第一行第一列
    /// </summary>
    public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = GetConnection())
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd.ExecuteScalar();
        }
    }

    /// <summary>
    /// 创建SqlParameter
    /// </summary>
    public static SqlParameter CreateParameter(string name, object value)
    {
        return new SqlParameter(name, value);
    }

    /// <summary>
    /// 修复字符串编码问题（GBK/UTF-8 转换）
    /// 用于修复"娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃" -> "深圳市浩之东科技有限公司"这类乱码
    /// </summary>
    public static string FixEncoding(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        
        try
        {
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(str);
            string gbkStr = System.Text.Encoding.GetEncoding("GBK").GetString(utf8Bytes);
            byte[] gbkBytes = System.Text.Encoding.GetEncoding("GBK").GetBytes(gbkStr);
            string utf8Str = System.Text.Encoding.UTF8.GetString(gbkBytes);
            
            if (utf8Str != str)
            {
                return utf8Str;
            }
        }
        catch
        {
        }
        
        return str;
    }

    /// <summary>
    /// 修复字符串并过滤无效字符
    /// </summary>
    public static string FixAndCleanString(string str)
    {
        string fixedStr = FixEncoding(str);
        if (string.IsNullOrEmpty(fixedStr)) return "";
        
        char[] cleanChars = new char[fixedStr.Length];
        int idx = 0;
        foreach (char c in fixedStr)
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