using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace DataBaseAccessService.Test
{
    [TestClass]
    public class DataBaseAccessServiceTests
    {
        private IDBAccessService dbSvc = new MySqlAccessService("localhost", "wwuser", "Wonderware777", "cloud_viz", "Wonderware777");
        [TestMethod]
        public void TestGetData()
        {
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666")
            };
            var result = dbSvc.GetData("select * from user where id=@id", parms);
            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual("小明", Convert.ToString(result.Rows[0]["name"]));
        }

        [TestMethod]
        public void TestGetDataNoData()
        {
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@id", "50fa5b7d-1111-1111-a5f8-8cec4bc2b666"),
            };
            var result = dbSvc.GetData("select * from user where id=@id", parms);
            Assert.AreEqual(0, result.Rows.Count);
        }

        [TestMethod]
        public void TestGetDataFailure()
        {
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
            };
            Assert.ThrowsException<MySqlException>(() => dbSvc.GetData("select * from user where id=@id id=@id", parms));
        }

        [TestMethod]
        public void TestGetSingleValue()
        {
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
            };
            var result = dbSvc.GetSingleValue("select name from user where id=@id", parms);
            Assert.AreEqual("小明", Convert.ToString(result));
        }

        [TestMethod]
        public void TestGetSingleValueNoData()
        {
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@id", "50fa5b7d-1111-1111-a5f8-8cec4bc2b666"),
            };
            var result = dbSvc.GetSingleValue("select name from user where id=@id", parms);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestExecuteNonQuery()
        {
            var parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
                new Tuple<string, object>("@note", "unit test")
            };
            var upd = dbSvc.ExecuteNonQuery("update user set note=@note where id=@id", parms);
            Assert.AreEqual(1, upd);

            var result = dbSvc.GetSingleValue("select note from user where id='50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666'", null);
            Assert.AreEqual("unit test", Convert.ToString(result));

            parms = new Tuple<string, object>[]
            {
                new Tuple<string, object>("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
                new Tuple<string, object>("@note", "unit test2")
            };
            upd = dbSvc.ExecuteNonQuery("update user set note=@note where id=@id", parms);
            Assert.AreEqual(1, upd);

            result = dbSvc.GetSingleValue("select note from user where id='50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666'", null);
            Assert.AreEqual("unit test2", Convert.ToString(result));
        }

        [TestMethod]
        public void TestBulkInsert()
        {
            var testDt = new DataTable("testtable");
            testDt.Columns.AddRange(new DataColumn[] {
                new DataColumn("col1"),
                new DataColumn("col2")
            });

            for (var idx = 0; idx < 5000; idx++)
            {
                var newRow = testDt.NewRow();
                newRow[0] = Guid.NewGuid();
                newRow[1] = Guid.NewGuid();
                testDt.Rows.Add(newRow);
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            dbSvc.BulkInsert(testDt, "delete from testtable");
            stopwatch.Stop(); 
            TimeSpan timespan = stopwatch.Elapsed;
            Console.WriteLine(timespan.TotalMilliseconds);
        }
    }
}
