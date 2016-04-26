using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.EnjoyCodes.SqlHelper;
using Com.EnjoyCodes.Model;
using System.Text;
using System.Data;
using System.Diagnostics;

namespace Com.EnjoyCodes.SqlHelper.Tests
{
    [TestClass]
    public class FileDALTest
    {
        private FileDAL _fileDAL = new FileDAL();

        [TestMethod]
        public void CRUD()
        {
            // 1.增删改
            var fileTerm = new FileTerm()
            {
                ID = Guid.NewGuid(),
                Title = "ado.net test",
                Describe = "add from ado.net",
                GenreID = Guid.Empty,
                IsNotice = true,
                CreateTime = DateTime.Now
            };
            this._fileDAL.Add(fileTerm);

            var fileTerm1 = this._fileDAL.Get(fileTerm.ID);
            fileTerm1.CreateTime = DateTime.Now;

            this._fileDAL.Update(fileTerm1);

            int r = this._fileDAL.Delete(fileTerm.ID);

            // 2.查询数据集 - DataSet
            DataSet dataSet = this._fileDAL.Get("SELECT TOP 100 * FROM FILETERMS ORDER BY CREATETIME DESC");
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dataSet.Tables[0].Rows)
                sb.Append(row["ID"] + "," + row["Title"] + "," + row["CreateTime"] + Environment.NewLine);
            Debug.WriteLine(sb.ToString());

            // 3.查询数据集
            //var fileTerms = this._fileDAL.Get(10);
            var fileTerms = this._fileDAL.Get(1, 9, string.Empty, "ID");

            // 4.查询表行数
            int count = this._fileDAL.Count();

            // 5.查询是否存在数据
            bool b = this._fileDAL.IsExists("t");
        }

        [TestMethod]
        public void AddDatas()
        {
            Random rad = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var fileTerm = new FileTerm()
                {
                    ID = Guid.NewGuid(),
                    Title = "ado.net test" + i,
                    Describe = "add from ado.net" + rad.NextDouble(),
                    GenreID = Guid.Empty,
                    IsNotice = true,
                    CreateTime = DateTime.Now,
                    Amounts = 100,
                    Type = Types.T2
                };
                this._fileDAL.Add(fileTerm);
            }
        }

        [TestMethod]
        public void AddIdentityData()
        { this._fileDAL.Add(new TIdentity() { Name = "test", CreateTime = DateTime.Now, Type = Types.T2 }); }

        [TestMethod]
        public void GetPaging()
        { var fileTerms = this._fileDAL.GetPaging(2, 100, string.Empty, "CREATETIME DESC"); }

        [TestMethod]
        public void Update()
        {
            this._fileDAL.Update(new FileTerm()
            {
                ID = Guid.Parse("A1808924-C25D-4C54-A17A-0D2C54262797"),
                CreateTime = DateTime.Now,
                Type = Types.T3
            });
        }
    }
}
