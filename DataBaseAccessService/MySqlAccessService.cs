using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DataBaseAccessService
{
    public class MySqlAccessService : DBAccessService
    {
        public MySqlAccessService(string server, string dbUsr, string dbpwd, string dbName, string encryptKey)
        {
            this.connectStr = new MySqlConnectionStringBuilder()
            {
                Server = server,
                UserID = dbUsr,
                Password = dbpwd,
                Database = dbName
            }.ToString();
            this.EncryptKey = encryptKey;
        }

        public override void BulkInsert(DataTable[] data, string preSql, Tuple<string, object>[] preSqlParms)
        {
            using (var cnn = new MySqlConnection(this.connectStr))
            {
                cnn.Open();
                using (var tran = cnn.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var cmd = string.IsNullOrEmpty(preSql) ? cnn.CreateCommand() :
                            this.PrepareCommand(cnn, preSql, preSqlParms) as MySqlCommand;
                        cmd.Transaction = tran;
                        cmd.CommandType = CommandType.Text;
                        if (!string.IsNullOrEmpty(preSql))
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }

                        foreach(var dt in data)
                        {
                            if (dt.Rows.Count == 0)
                                continue;
                            cmd.CommandText = this.GenBulkInsertSQL(dt);
                            var parms = this.GenBulkInsertParameters(dt);
                            cmd.Parameters.AddRange(parms);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }

                        tran.Commit();
                    }
                    catch (DbException ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }

            // when exceeds MySql maximun package size, consider MySqlBulkLoader as below:
            /*
            string tempCsvFileSpec = @"C:\ProgramData\MySQL\MySQL Server 8.0\Uploads\dump.csv";
            using (StreamWriter writer = new StreamWriter(tempCsvFileSpec))
            {
                Rfc4180Writer.WriteDataTable(data, writer, false);
            }
            var msbl = new MySqlBulkLoader(new MySqlConnection(this.connectStr));
            msbl.TableName = data.TableName;
            msbl.FileName = tempCsvFileSpec;
            msbl.FieldTerminator = ",";
            msbl.FieldQuotationCharacter = '"';
            msbl.LineTerminator = "\r\n";
            msbl.Load();
            File.Delete(tempCsvFileSpec);*/
        }

        protected override IDbConnection GetConnection()
        {
            return new MySqlConnection(this.connectStr);
        }

        protected override IDbCommand PrepareCommand(IDbConnection cnn, string sqlTxt, Tuple<string, object>[] parms)
        {
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlTxt;
            if (parms != null)
            {
                foreach (var parm in parms)
                {
                    cmd.Parameters.Add(new MySqlParameter(parm.Item1, parm.Item2));
                }
            }

            return cmd;
        }

        private IDbDataParameter[] GenBulkInsertParameters(DataTable dt)
        {
            var parms = new IDbDataParameter[dt.Rows.Count * dt.Columns.Count];
            int parmIdx = 0;
            for (int rowIdx = 0; rowIdx < dt.Rows.Count; rowIdx++)
            {
                for (int colIdx = 0; colIdx < dt.Columns.Count; colIdx++)
                {
                    parms[parmIdx++] = new MySqlParameter($"@{dt.Columns[colIdx].ColumnName}{rowIdx}", dt.Rows[rowIdx][colIdx]);
                }
            }
            return parms;
        }

        private string GenBulkInsertSQL(DataTable dt)
        {
            var columns = this.GetColumnsStr(dt);
            var sql = new StringBuilder($"insert into {dt.TableName} ({columns}) values");
            for (int idx = 0; idx < dt.Rows.Count; idx++)
            {
                sql.Append($"({this.GetColumnsStr(dt, "@", idx.ToString())})");
                if (idx < dt.Rows.Count - 1)
                {
                    sql.Append(',');
                }
            }
            return sql.ToString();
        }

        private StringBuilder GetColumnsStr(DataTable dt, string prefix = "", string suffix = "")
        {
            var columns = new StringBuilder();
            for (int idx = 0; idx < dt.Columns.Count; idx++)
            {
                columns.Append(prefix + dt.Columns[idx].ColumnName + suffix);
                if (idx < dt.Columns.Count - 1)
                {
                    columns.Append(',');
                }
            }
            return columns;
        }
    }
}
