using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;

namespace cnblogs2typecho.DbHelper
{
    public class MariaDbHelper : IDbHelper,IDisposable
    {
        private MySql.Data.MySqlClient.MySqlConnection con = new MySqlConnection();

        private string connectionString;

        public string ConnectionString
        {
            get => connectionString;
            set
            {
                connectionString = value;
                con.ConnectionString = connectionString;
            }
        }

        public ConnectionState ConnectionState => con == null ? ConnectionState.Broken : con.State;

        public MariaDbHelper()
        {
            
        }

        public MariaDbHelper(string connectionStr)
        {
            this.connectionString = connectionStr;
        }

        public MariaDbHelper(string ip,string port,string username,string password)
        {
            ConnectionString = $"Server={ip}; Port={port}; Database=ganshornlfx; Uid={username}; Pwd={password}";
        }

        public MariaDbHelper(string ip, string port, string username, string password,string database)
        {
            ConnectionString = $"Server={ip}; Port={port}; Database={database}; Uid={username}; Pwd={password}";
        }

        public MariaDbHelper(string ip, string port, string username, string password, string database,int timeout)
        {
            ConnectionString = $"Server={ip}; Port={port}; Database={database}; Uid={username}; Pwd={password}; Connection Timeout={timeout}";
        }

        public bool Open()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new Exception("连接字体串为空");

            con = new MySqlConnection(ConnectionString);

            try
            {
                con.Open();

                if (con.State == ConnectionState.Open)
                    return true;
            }
            catch
            {
                throw;
            }

            return false;
        }

        public bool Close()
        {
            if (con != null && con.State == ConnectionState.Open)
            {
                try
                {
                    con.Close();
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            return true;
        }

        public DataTable Query(string sql, DbParameter[] parameters = null)
        {
            DataTable dt = null;

            if (con.State != ConnectionState.Open)
                con.Open();

            if (con.State != ConnectionState.Open)
                throw new Exception("数据库打开失败");

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                cmd.Connection = con;
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                mySqlDataAdapter.Fill(ds);

                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];

                Close();

                return dt;
            }
            catch
            {
                throw;
            }
        }

        public object QueryFirst(string sql, DbParameter[] parameters = null)
        {
            if (con.State != ConnectionState.Open)
                Open();

            if (con.State != ConnectionState.Open)
                throw new Exception("数据库打开失败");

            object result = null;

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand();
                mySqlCommand.CommandText = sql;
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = con;

                result = mySqlCommand.ExecuteScalar();
            }
            catch
            {
                throw;
            }

            Close();

            return result;
        }

        public DataTable ExecuteView(string sql,string viewName, DbParameter[] parameters = null)
        {
            if (con.State != ConnectionState.Open)
                Open();

            if (con.State != ConnectionState.Open)
                throw new Exception("数据库打开失败");

            DataTable dt = new DataTable();

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand();
                mySqlCommand.CommandText = sql;
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = con;
                mySqlCommand.Parameters.AddRange(parameters);
                mySqlCommand.ExecuteNonQuery();
                dt = Query($"select * from {viewName}");
               
            }
            catch
            {
                throw;
            }

            Close();

            return dt;
        }

        public void Dispose()
        {
            if (con == null)
                return;

            if (con.State == ConnectionState.Open)
                con.Close();

            con.Dispose();
        }

        public int ExecuteSql(string sql, DbParameter[] parameters = null)
        {
            var result = 0;
            if (con.State != ConnectionState.Open)
                Open();

            if (con.State != ConnectionState.Open)
                throw new Exception("数据库打开失败");
            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand();
                mySqlCommand.CommandText = sql;
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Connection = con;
                if (parameters != null)
                {
                    mySqlCommand.Parameters.AddRange(parameters);
                }
                result = mySqlCommand.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            Close();
            return result;
        }
    }
}
