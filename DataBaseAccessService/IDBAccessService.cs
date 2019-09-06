using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DataBaseAccessService
{
    public interface IDBAccessService
    {
        DataTable GetData(string sqlTxt, DbParameter[] parms);
        object GetSingleValue(string sqlTxt, DbParameter[] parms);
        int ExecuteNonQuery(string sqlTxt, DbParameter[] parms);
    }
}
