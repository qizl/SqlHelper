using Com.EnjoyCodes.Model;
using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Com.EnjoyCodes.SqlHelper.Client
{
    public partial class FormMain : Form
    {
        private FileDAL _fileDAL = new FileDAL();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 1.增删改
            //var fileTerm = new FileTerm()
            //{
            //    ID = Guid.NewGuid(),
            //    Title = "ado.net test",
            //    Describe = "add from ado.net",
            //    GenreID = Guid.Empty,
            //    IsNotice = true,
            //    CreateTime = DateTime.Now
            //};

            //this._fileDAL.Add(fileTerm);

            //var fileTerm1 = this._fileDAL.Get(fileTerm.ID);
            //fileTerm1.Data.CreateTime = DateTime.Now;

            //this._fileDAL.Update(fileTerm1.Data);

            //int r = this._fileDAL.Delete(fileTerm.ID);

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
        }
    }
}
