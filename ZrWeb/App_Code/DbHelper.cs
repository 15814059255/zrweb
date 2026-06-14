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
        return new SqlConnection(connectionString);
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
}