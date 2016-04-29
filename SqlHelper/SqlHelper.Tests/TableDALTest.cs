using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.EnjoyCodes.SqlHelper;
using Com.EnjoyCodes.Model;
using System.Text;
using System.Data;
using System.Diagnostics;
using Com.EnjoyCodes.SqlAttribute;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Com.EnjoyCodes.SqlHelper.Tests
{
    [TestClass]
    public class TableDALTest
    {
        private TableDAL _fileDAL = new TableDAL();

        [Table(Name = "CTests", Prefix = "ct_")]
        private class CTest
        {
            [Key]
            public int ID { get; set; }
            public Guid SID { get; set; }
            public string Name { get; set; }
            public E TE { get; set; }
            public DateTime CreateTime { get; set; }
        }
        private enum E
        {
            A,
            B
        }

        [TestMethod]
        public void CreateTable()
        {
            this._fileDAL.CreateTable<FileTerm>();
            this._fileDAL.CreateTable<FileTermDetail>();
            //this._fileDAL.CreateTable<TIdentity>();
            //this._fileDAL.CreateTable<CTest>();
        }

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

            var fileTerm1 = this._fileDAL.Get<FileTerm>(fileTerm.ID);
            fileTerm1.CreateTime = DateTime.Now;

            this._fileDAL.Update(fileTerm1);

            int r = this._fileDAL.Delete<FileTerm>(fileTerm.ID);

            // 2.查询数据集 - DataSet
            DataSet dataSet = this._fileDAL.Get("SELECT TOP 100 * FROM FILETERMS ORDER BY ft_CREATETIME DESC");
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dataSet.Tables[0].Rows)
                sb.Append(row["ft_ID"] + "," + row["ft_Title"] + "," + row["ft_CreateTime"] + Environment.NewLine);
            Debug.WriteLine(sb.ToString());

            // 3.查询数据集
            //var fileTerms = this._fileDAL.Get(10);
            var fileTerms = this._fileDAL.Get(0, 9, string.Empty, "ft_ID");

            // 4.查询表行数
            int count = this._fileDAL.Count();

            // 5.查询是否存在数据
            bool b = this._fileDAL.IsExists("t");
        }

        [TestMethod]
        public void AddByAttribute()
        {
            Guid id = Guid.NewGuid();
            var result = this._fileDAL.Add<FileTerm>(new FileTerm() { ID = id, Title = "LJAFLJAL", CreateTime = DateTime.Now });
            this._fileDAL.Add(new FileTermDetail() { FileTermID = id, Name = "D1", CreateTime = DateTime.Now });
            this._fileDAL.Add(new FileTermDetail() { FileTermID = id, Name = "D2", CreateTime = DateTime.Now });
            this._fileDAL.Add(new FileTermDetail() { FileTermID = id, Name = "D3", CreateTime = DateTime.Now });
        }

        [TestMethod]
        public void AddT()
        {
            for (int i = 0; i < 100; i++)
                this._fileDAL.Add<CTest>(new CTest() { SID = Guid.NewGuid(), Name = "Test" + i, TE = E.B, CreateTime = DateTime.Now });
        }

        [TestMethod]
        public void AddNullString()
        { var result = this._fileDAL.Add<CTest>(new CTest() { ID = 123, SID = Guid.NewGuid(), Name = null, CreateTime = DateTime.Now }); }

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
        public void Get()
        { var result = this._fileDAL.Get(Guid.Parse("C54AE2CD-3789-4D31-81E1-F4AEB79AE734")); }

        [TestMethod]
        public void GetPaging()
        { var fileTerms = this._fileDAL.GetPaging(2, 2); }

        [TestMethod]
        public void ReadListT()
        { var result = this._fileDAL.ReadList<CTest>(); }

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

        [TestMethod]
        public void UpdateByAttribute()
        { var result = this._fileDAL.Update<FileTerm>(new FileTerm() { ID = new Guid("B7393707-CDB0-44D4-85F0-DF21783D5CD4"), Title = "LJAFLJAL001", CreateTime = DateTime.Now }); }

        [TestMethod]
        public void UpdateT()
        { var result = this._fileDAL.Update<CTest>(new CTest() { ID = 1, Name = "Test01", CreateTime = DateTime.Now }); }
    }
}
