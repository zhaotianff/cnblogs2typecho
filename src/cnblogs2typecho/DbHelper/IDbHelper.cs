using System.Data;
using System.Data.Common;

namespace cnblogs2typecho.DbHelper
{
    public interface IDbHelper
    {
        string ConnectionString { get; set; }
        bool Open();
        bool Close();
        DataTable Query(string sql, DbParameter[] parameters = null);
        object QueryFirst(string sql, DbParameter[] parameters = null);
        DataTable ExecuteView(string sql, string viewName, DbParameter[] parameters = null);
        int ExecuteSql(string sql, DbParameter[] parameters = null);
    }
}
