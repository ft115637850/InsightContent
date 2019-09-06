using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace DataBaseAccessService.Test
{
    [TestClass]
    public class DataBaseAccessServiceTests
    {
        private IDBAccessService dbSvc = new DBAccessService("localhost", "wwuser", "Wonderware777", "cloud_viz");
        [TestMethod]
        public void TestGetData()
        {
            DbParameter[] parms = new MySqlParameter[]
            {
                new MySqlParameter("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
            };
            var result = dbSvc.GetData("select * from user where id=@id", parms);
            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual("小明", Convert.ToString(result.Rows[0]["name"]));
        }

        [TestMethod]
        public void TestGetDataFailure()
        {
            DbParameter[] parms = new MySqlParameter[]
            {
                new MySqlParameter("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
            };
            Assert.ThrowsException<MySqlException>(() => dbSvc.GetData("select * from user where id=@id id=@id", parms));
        }

        [TestMethod]
        public void TestGetSingleValue()
        {
            DbParameter[] parms = new MySqlParameter[]
            {
                new MySqlParameter("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
            };
            var result = dbSvc.GetSingleValue("select name from user where id=@id", parms);
            Assert.AreEqual("小明", Convert.ToString(result));
        }

        [TestMethod]
        public void TestExecuteNonQuery()
        {
            DbParameter[] parms = new MySqlParameter[]
            {
                new MySqlParameter("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
                new MySqlParameter("@note", "unit test")
            };
            var upd = dbSvc.ExecuteNonQuery("update user set note=@note where id=@id", parms);
            Assert.AreEqual(1, upd);

            var result = dbSvc.GetSingleValue("select note from user where id='50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666'", null);
            Assert.AreEqual("unit test", Convert.ToString(result));

            parms = new MySqlParameter[]
            {
                new MySqlParameter("@id", "50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666"),
                new MySqlParameter("@note", "unit test2")
            };
            upd = dbSvc.ExecuteNonQuery("update user set note=@note where id=@id", parms);
            Assert.AreEqual(1, upd);

            result = dbSvc.GetSingleValue("select note from user where id='50fa5b7d-d06d-11e9-a5f8-8cec4bc2b666'", null);
            Assert.AreEqual("unit test2", Convert.ToString(result));
        }
    }
}
