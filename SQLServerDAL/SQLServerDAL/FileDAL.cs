using Com.EnjoyCodes.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.SQLServerDAL
{
    public class FileDAL
    {
        public int Add(FileTerm fileTerm)
        {
            string sqlStr = @"INSERT INTO FILETERMS(ID,TITLE,DESCRIBE,GENREID,CREATETIME,ISNOTICE) VALUES(@ID,@TITLE,@DESCRIBE,@GENREID,@CREATETIME,@ISNOTICE)";
            SqlParameter[] parameters = {
                new SqlParameter("@ID",SqlDbType.UniqueIdentifier),
                new SqlParameter("@TITLE",SqlDbType.NVarChar),
                new SqlParameter("@DESCRIBE",SqlDbType.NVarChar),
                new SqlParameter("@GENREID",SqlDbType.UniqueIdentifier),
                new SqlParameter("@CREATETIME",SqlDbType.DateTime),
                new SqlParameter("@ISNOTICE",SqlDbType.Bit),
            };
            parameters[0].Value = fileTerm.ID;
            parameters[1].Value = fileTerm.Title;
            parameters[2].Value = fileTerm.Describe;
            parameters[3].Value = fileTerm.GenreID;
            parameters[4].Value = fileTerm.CreateTime;
            parameters[5].Value = fileTerm.IsNotice;

            return SQLHelper.ExecuteNonQuery(SQLHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr, parameters);
        }

        /// <summary>
        /// 根据ID查询单条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileTerm Get(Guid id)
        {
            var result = new FileTerm();
            string sqlStr = @"SELECT TOP 1 * FROM FILETERMS WHERE ID='" + id + "'";
            return SQLHelper<FileTerm>.Read(SQLHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr);
        }

        /// <summary>
        /// 查询数据集
        ///     分页方法
        /// </summary>
        /// <param name="startIndex">起始索引，从1开始</param>
        /// <param name="endIndex"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="sqlOrderBy"></param>
        /// <returns></returns>
        public List<FileTerm> Get(int startIndex, int endIndex, string sqlWhere, string sqlOrderBy)
        {
            List<FileTerm> result = new List<FileTerm>();
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append("SELECT * FROM (");
            sqlStr.Append("SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(sqlOrderBy.Trim()))
                sqlStr.Append("ORDER BY T." + sqlOrderBy);
            else
                sqlStr.Append("ORDER BY T.ID DESC");
            sqlStr.Append(")AS ROW, T.* FROM FILETERMS T ");
            if (!string.IsNullOrEmpty(sqlWhere.Trim()))
                sqlStr.Append("WHERE " + sqlWhere);
            sqlStr.Append(") T1 ");
            sqlStr.AppendFormat("WHERE T1.ROW BETWEEN {0} AND {1}", startIndex, endIndex);

            return SQLHelper<FileTerm>.ReadList(SQLHelper.GetConnectionString_RW(base.GetType()), CommandType.Text, sqlStr.ToString());
        }

        /// <summary>
        /// 查询数据集
        ///     获取前n行数据
        /// </summary>
        /// <param name="topCount"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="sqlOrderBy"></param>
        /// <returns></returns>
        public List<FileTerm> Get(int topCount, string sqlWhere, string sqlOrderBy)
        { return this.Get(1, topCount, sqlWhere, sqlOrderBy); }

        /// <summary>
        /// 查询数据集
        ///     获取前n行数据
        /// </summary>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public List<FileTerm> Get(int topCount)
        { return this.Get(topCount, string.Empty, "ID"); }

        /// <summary>
        /// 自定义sql查询数据集
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public DataSet Get(string sqlStr)
        { return SQLHelper.ExecuteDataSet(SQLHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr); }

        public int Update(FileTerm fileTerm)
        {
            string sqlStr = "UPDATE FILETERMS SET TITLE=@TITLE,DESCRIBE=@DESCRIBE,GENREID=@GENREID,CREATETIME=@CREATETIME,ISNOTICE=@ISNOTICE WHERE ID=@ID";
            SqlParameter[] parameters = {
                new SqlParameter("@ID",SqlDbType.UniqueIdentifier),
                new SqlParameter("@TITLE",SqlDbType.NVarChar),
                new SqlParameter("@DESCRIBE",SqlDbType.NVarChar),
                new SqlParameter("@GENREID",SqlDbType.UniqueIdentifier),
                new SqlParameter("@CREATETIME",SqlDbType.DateTime),
                new SqlParameter("@ISNOTICE",SqlDbType.Bit),
            };
            parameters[0].Value = fileTerm.ID;
            parameters[1].Value = fileTerm.Title;
            parameters[2].Value = fileTerm.Describe;
            parameters[3].Value = fileTerm.GenreID;
            parameters[4].Value = fileTerm.CreateTime;
            parameters[5].Value = fileTerm.IsNotice;

            return SQLHelper.ExecuteNonQuery(SQLHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr, parameters);
        }

        public int Delete(Guid id)
        {
            string sqlStr = @"DELETE FROM FILETERMS WHERE ID='" + id + "'";
            return SQLHelper.ExecuteNonQuery(SQLHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr);
        }

        /// <summary>
        /// 查询表行数
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <returns></returns>
        public int Count(string sqlWhere = "")
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append("SELECT COUNT(1) FROM FILETERMS ");
            if (!string.IsNullOrEmpty(sqlWhere.Trim()))
                sqlStr.Append("WHERE " + sqlWhere);

            DataSet result = SQLHelper.ExecuteDataSet(SQLHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr.ToString());

            return Convert.ToInt32(result.Tables[0].Rows[0][0]);
        }
    }
}
