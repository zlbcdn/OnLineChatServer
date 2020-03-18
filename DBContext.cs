using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace OnChatServer
{
    public class DBContext
    {
        #region 获得数据库连接
        public static OracleConnection GetZLOracleConnection()
        {
            string connectionStr = @"User Id=dimsprog; Password=dimsprog;  Data Source=oracle_main";

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connectionStr;
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
            return conn;
        }
        #endregion

        //获取Oracle的String类型数据
        public static string getOracleStringItem(OracleDataReader reader, string columnName)
        {
            if (!Convert.IsDBNull(reader[columnName]))
            {
                return reader.GetString(reader.GetOrdinal(columnName));
            }
            else
            {
                return "";
            }
        }
    }
}
