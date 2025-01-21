using LungWorkStation.DAL.DbHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnblogs2typecho.DAL
{
    public class TypechoDal
    {
        private MariaDbHelper mariaDbHelper;


        public bool OpenTypechoDatabase(string server, string port, string database, string uid, string pwd)
        {
            try
            {
                var connectString = $"Server={server}; Port={port}; Database={database}; Uid={uid}; Pwd={pwd}";

                mariaDbHelper = new MariaDbHelper(connectString);
                mariaDbHelper.Open();

                if (mariaDbHelper.ConnectionState == System.Data.ConnectionState.Open)
                    return true;


                return false;
            }
            catch
            {
                return false;
            }
        }


    }
}
