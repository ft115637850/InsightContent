using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace DataBaseAccessService
{
    public class DBAccessService : IDBAccessService
    {
        private string connectStr;

        public DBAccessService(string server, string dbUsr, string dbpwd, string dbName)
        {
            this.connectStr = new MySqlConnectionStringBuilder()
            {
                Server = server,
                UserID = dbUsr,
                Password = dbpwd,
                Database = dbName
            }.ToString();
        }

        public DataTable GetData(string sqlTxt, DbParameter[] parms)
        {
            using (var cnn = this.GetConnection())
            {
                try
                {
                    var cmd = this.PrepareCommand(cnn, sqlTxt, parms);
                    cnn.Open();
                    var rdr = cmd.ExecuteReader();
                    var data = new DataTable();
                    data.Load(rdr, LoadOption.OverwriteChanges);
                    return data;
                }
                catch (DbException ex)
                {
                    Debug.Print("Error SQL:" + sqlTxt);
                    throw ex;
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        public object GetSingleValue(string sqlTxt, DbParameter[] parms)
        {
            using (var cnn = this.GetConnection())
            {
                try
                {
                    var cmd = this.PrepareCommand(cnn, sqlTxt, parms);
                    cnn.Open();
                    var data = cmd.ExecuteScalar();
                    if (data == null || Convert.IsDBNull(data))
                    {
                        return null;
                    }
                    else
                    {
                        return data;
                    }
                }
                catch (DbException ex)
                {
                    Debug.Print("Error SQL:" + sqlTxt);
                    throw ex;
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        public int ExecuteNonQuery(string sqlTxt, DbParameter[] parms)
        {
            using (var cnn = this.GetConnection())
            {
                try
                {
                    var cmd = this.PrepareCommand(cnn, sqlTxt, parms);
                    cnn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    Debug.Print("Error SQL:" + sqlTxt);
                    throw ex;
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        protected IDbConnection GetConnection()
        {
            return new MySqlConnection(this.connectStr);
        }

        private IDbCommand PrepareCommand(IDbConnection cnn, string sqlTxt, DbParameter[] parms)
        {
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlTxt;
            if (parms != null)
            {
                foreach (var parm in parms)
                {
                    cmd.Parameters.Add(parm);
                }
            }

            return cmd;
        }
    }
}
