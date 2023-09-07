using System;
using MySql.Data.MySqlClient;
namespace CSharpServer.ServerTools
{
    public class ConnHelper
    {
        public const string CONNECTIONSTRING = "server=127.0.0.1;port=3306;uid=root;pwd=123456;database=game";

        public static MySqlConnection? Connect()
        {
            var conn = new MySqlConnection(CONNECTIONSTRING);
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                LogManager.LogError(string.Format("连接数据库错误：{0}", e));
                return null;
            }

        }

        public static void CloseConnection(MySqlConnection? connection)
        {
            if (connection != null)
            {
                connection.Close();
            }
            else
            {
                LogManager.LogError("关闭数据库发生错误: MySqlConnection不能为空");
            }
        }
    }
}