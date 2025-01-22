using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace cnblogs2typecho.Extension
{
    public static class DataTableExtension
    {
        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            List<T> list = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T obj = new T();
                foreach (DataColumn column in dataTable.Columns)
                {
                    PropertyInfo prop = typeof(T).GetProperty(column.ColumnName);
                    if (prop != null && row[column] != DBNull.Value)
                    {
                        prop.SetValue(obj, Convert.ChangeType(row[column], prop.PropertyType), null);
                    }
                }
                list.Add(obj);
            }

            return list;
        }
    }

}
