using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace DataBaseAccessService
{
    public abstract class DBAccessService : IDBAccessService
    {
        protected string connectStr;
        public string EncryptKey { get; protected set; }

        public DataTable GetData(string sqlTxt, Tuple<string, object>[] parms)
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

        public object GetSingleValue(string sqlTxt, Tuple<string, object>[] parms)
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

        public int ExecuteNonQuery(string sqlTxt, Tuple<string, object>[] parms)
        {
            using (var cnn = this.GetConnection())
            {
                cnn.Open();
                using (var tran = cnn.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var cmd = this.PrepareCommand(cnn, sqlTxt, parms);
                        cmd.Transaction = tran;
                        var result = cmd.ExecuteNonQuery();
                        tran.Commit();
                        return result;
                    }
                    catch (DbException ex)
                    {
                        Debug.Print("Error SQL:" + sqlTxt);
                        tran.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }
        }

        public abstract void BulkInsert(DataTable data, string preSql, Tuple<string, object>[] preSqlParms);

        protected abstract IDbConnection GetConnection();        

        protected abstract IDbCommand PrepareCommand(IDbConnection cnn, string sqlTxt, Tuple<string, object>[] parms);
    }
}
