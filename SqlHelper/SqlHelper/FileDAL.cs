using Com.EnjoyCodes.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.EnjoyCodes.SqlHelper
{
    public class FileDAL
    {
        public int CreateTable<T>()
        { return SqlHelper<T>.CreateTable(SqlHelper.GetConnectionString_RW(this.GetType())); }

        public object Add<T>(T t)
        { return SqlHelper<T>.Create(SqlHelper.GetConnectionString_RW(this.GetType()), t); }

        /// <summary>
        /// 根据ID查询单条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get<T>(object id)
        {
            Tuple<string, string, string> t = null;
            try { t = SqlHelper<T>.GetTableAttributes(); }
            catch { }

            StringBuilder sqlStr = new StringBuilder();
            sqlStr.AppendFormat("SELECT * FROM {0} WHERE {1}{2}=@{2}", t.Item1, t.Item3, t.Item2);
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter()
            {
                ParameterName = "@" + t.Item2,
                SqlDbType = SqlHelper<T>.SqlDbTypes[typeof(T).GetProperty(t.Item2).PropertyType],
                Value = id
            };

            return SqlHelper<T>.Read(SqlHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr.ToString(), sqlParameters);
        }

        public FileTerm Get(Guid id)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.AppendFormat("SELECT * FROM FILETERMS WHERE FT_ID='{0}';SELECT * FROM FILETERMDETAILS WHERE FTD_FILETERMID='{0}';", id);
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr.ToString());

            StringBuilder sb = new StringBuilder();
            foreach (DataTable dt in ds.Tables)
                foreach (DataRow dr in dt.Rows)
                    for (int i = 0; i < dt.Columns.Count; i++)
                        sb.Append(dr[i].ToString());

            return null;
        }

        /// <summary>
        /// 查询数据集
        ///     分页方法
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="sqlOrderBy"></param>
        /// <returns></returns>
        public Pager<FileTerm> GetPaging(int pageNumber, int pageSize, string sqlWhere, string sqlOrderBy)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append("SELECT COUNT(1) FROM FILETERMS ");
            sqlStr.Append(string.IsNullOrEmpty(sqlWhere.Trim()) ? "" : ("WHERE " + sqlWhere));
            sqlStr.AppendFormat("SELECT * FROM (SELECT TOP {0} ROW_NUMBER() OVER (ORDER BY {1}) ROWINDEX, * FROM FILETERMS) F WHERE F.ROWINDEX BETWEEN {2} AND {3}", pageNumber * pageSize, string.IsNullOrEmpty(sqlOrderBy.Trim()) ? "ID" : sqlOrderBy, (pageNumber - 1) * pageSize + 1, pageNumber * pageSize);

            Pager<FileTerm> result = SqlHelper<FileTerm>.ReadPaging(SqlHelper.GetConnectionString_RW(base.GetType()), CommandType.Text, sqlStr.ToString());
            result.PageNumber = pageNumber;
            result.PageSize = pageSize;

            return result;
        }

        /// <summary>
        /// 查询数据集
        ///     指定索引
        /// </summary>
        /// <param name="startIndex">起始索引，从0开始</param>
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
            sqlStr.AppendFormat("WHERE T1.ROW BETWEEN {0} AND {1}", startIndex + 1, endIndex);

            return SqlHelper<FileTerm>.ReadList(SqlHelper.GetConnectionString_RW(base.GetType()), CommandType.Text, sqlStr.ToString());
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
        { return SqlHelper.ExecuteDataSet(SqlHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr); }

        public List<T> ReadList<T>()
        { return SqlHelper<T>.ReadList(SqlHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, string.Format("SELECT * FROM {0}", SqlHelper<T>.GetTableAttributes().Item1)); }

        public int Update<T>(T t)
        { return SqlHelper<T>.Update(SqlHelper.GetConnectionString_RW(this.GetType()), t); }

        public int Delete<T>(object id)
        {
            Tuple<string, string, string> t = null;
            try { t = SqlHelper<T>.GetTableAttributes(); }
            catch { }

            StringBuilder sqlStr = new StringBuilder();
            sqlStr.AppendFormat("DELETE FROM {0} WHERE {1}{2}=@{2}", t.Item1, t.Item3, t.Item2);
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter()
            {
                ParameterName = "@" + t.Item2,
                SqlDbType = SqlHelper<T>.SqlDbTypes[typeof(T).GetProperty(t.Item2).PropertyType],
                Value = id
            };

            return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr.ToString(), sqlParameters);
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

            object obj = SqlHelper.ExecuteScalar(SqlHelper.GetConnectionString_RW(this.GetType()), CommandType.Text, sqlStr.ToString());

            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        public bool IsExists(string sqlTableName, string sqlWhere = "")
        {
            string sqlStr = @"SELECT COUNT(*) FROM " + sqlTableName + (string.IsNullOrEmpty(sqlWhere) ? "" : (" " + sqlWhere));
            bool b = false;
            try { b = SqlHelper.IsExists(SqlHelper.GetConnectionString_RW(this.GetType()), sqlStr); }
            catch { }
            return b;
        }
    }
}
