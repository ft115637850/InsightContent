using System;
using System.Data;

namespace DataBaseAccessService
{
    public interface IDBAccessService
    {
        string EncryptKey { get; }
        DataTable GetData(string sqlTxt, Tuple<string, object>[] parms);
        object GetSingleValue(string sqlTxt, Tuple<string, object>[] parms);
        int ExecuteNonQuery(string sqlTxt, Tuple<string, object>[] parms);
        void BulkInsert(DataTable data, string preSql = "", Tuple<string, object>[] preSqlParms = null);
    }
}
