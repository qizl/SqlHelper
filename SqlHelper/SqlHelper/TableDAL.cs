using Com.EnjoyCodes.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.EnjoyCodes.SqlHelper
{
    public class TableDAL
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
            Tuple<string, string, string> t = SqlHelper<T>.GetTableAttributes(typeof(T));

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

        /// <summary>
        /// 联合查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileTerm Get(Guid id)
        { return SqlHelper<FileTerm>.Read(SqlHelper.GetConnectionString_RW(this.GetType()), string.Format("ft_ID='{0}'", id)); }

        /// <summary>
        /// 查询数据集
        ///     分页方法
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="sqlOrderBy"></param>
        /// <returns></returns>
        public Pager<T> GetPaging<T>(int pageNumber, int pageSize)
        { return SqlHelper<T>.ReadPaging(SqlHelper.GetConnectionString_RW(this.GetType()), pageNumber, pageSize); }

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
        { return SqlHelper<T>.ReadList(SqlHelper.GetConnectionString_RW(this.GetType())); }

        public int Update<T>(T t)
        { return SqlHelper<T>.Update(SqlHelper.GetConnectionString_RW(this.GetType()), t); }

        public int Delete<T>(object id)
        {
            Tuple<string, string, string> t = SqlHelper<T>.GetTableAttributes(typeof(T));

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
