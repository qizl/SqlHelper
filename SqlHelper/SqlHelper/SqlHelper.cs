using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.SqlHelper
{
    public sealed class SqlHelper
    {
        #region Constructures & Private Utility Methods
        private SqlHelper() { }

        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">an array of SqlParameters tho be added to command</param>
        private static void attachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    p.Value = DBNull.Value;

                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command.
        /// </summary>
        /// <param name="command">the SqlCommand to be prepared</param>
        /// <param name="connection">a valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">a valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        private static void prepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
                connection.Open();

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if (transaction != null)
                command.Transaction = transaction;

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
                attachParameters(command, commandParameters);
        }
        #endregion

        #region GetConnectionString
        /// <summary>
        /// 获取读写操作的数据库连接字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetConnectionString_RW(Type type)
        {
            string connectionStr = string.Empty;
            if (type != null)
            {
                string ns = type.Namespace;
                if (!string.IsNullOrEmpty(ns))
                {
                    string key = string.Empty;
                    switch (ns)
                    {
                        case "Com.EnjoyCodes.SqlHelper":
                        default: key = "MSSQLConnectionString"; break;
                    }
                    connectionStr = GetConnectionString(key);
                }
            }
            return connectionStr;
        }

        public static string GetConnectionString(string key)
        { return ConfigurationManager.AppSettings[key]; }
        #endregion

        #region ExecuteNonQuery
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            int result = 0;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                try
                {
                    result = ExecuteNonQuery(cn, commandType, commandText, commandParameters);
                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }

            return result;
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            prepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

            //finally, execute the command.
            int retval = cmd.ExecuteNonQuery();

            // detach the SqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();

            return retval;
        }
        #endregion

        #region ExecuteDataSet
        public static DataSet ExecuteDataSet(string connectionString, CommandType commandType, string commandText)
        { return ExecuteDataSet(connectionString, commandType, commandText, null); }

        public static DataSet ExecuteDataSet(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            DataSet result = null;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                try
                {
                    result = ExecuteDataSet(cn, commandType, commandText, commandParameters);
                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }

            return result;
        }

        public static DataSet ExecuteDataSet(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            prepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            da.Fill(ds);

            // detach the SqlParameters from the command object, so they can be used again.			
            cmd.Parameters.Clear();

            //return the dataset
            return ds;
        }
        #endregion

        #region ExecuteScalar
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        { return ExecuteScalar(connectionString, commandType, commandText, null); }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            object result = null;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                try
                {
                    result = ExecuteScalar(cn, commandType, commandText, commandParameters);
                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }
            return result;
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            prepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

            object retval = cmd.ExecuteScalar();

            cmd.Parameters.Clear();
            return retval;
        }

        public static bool IsExists(string connectionString, string commandText)
        { return Convert.ToInt32(ExecuteScalar(connectionString, CommandType.Text, commandText, null)) != 0; }

        public static bool IsExists(string connectionString, string commandText, params SqlParameter[] commandParameters)
        { return Convert.ToInt32(ExecuteScalar(connectionString, CommandType.Text, commandText, commandParameters)) != 0; }
        #endregion
    }

    public class SqlHelper<T>
    {
        #region Members & Private Utility Methods
        /// <summary>
        /// C#类型与SQLServer类型对照字典
        /// </summary>
        private static Dictionary<Type, SqlDbType> sqlDbType = new Dictionary<Type, SqlDbType>() {
            {typeof(long),SqlDbType.BigInt},
            {typeof(int),SqlDbType.Int},
            {typeof(short),SqlDbType.SmallInt},
            {typeof(byte),SqlDbType.TinyInt},
            {typeof(decimal),SqlDbType.Decimal},
            {typeof(double),SqlDbType.Float},
            {typeof(float),SqlDbType.Real},
            {typeof(bool),SqlDbType.Bit},
            {typeof(string),SqlDbType.NVarChar},
            {typeof(char),SqlDbType.Char},
            {typeof(DateTime),SqlDbType.DateTime},
            {typeof(TimeSpan),SqlDbType.Timestamp},
            {typeof(Guid),SqlDbType.UniqueIdentifier},
            {typeof(Enum),SqlDbType.Int}
        };

        private static void fill(T obj, IDataReader dr)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var item in properties)
                try
                { if (dr[item.Name] != null) item.SetValue(obj, convertObject(dr[item.Name], item.PropertyType), null); }
                catch { }
        }

        /// <summary>
        /// 将一个对象转换为指定类型
        /// </summary>
        /// <param name="obj">待转换的对象</param>
        /// <param name="type">目标类型</param>
        /// <returns></returns>
        private static object convertObject(object obj, Type type)
        {
            if (type == null) return obj;
            if (obj == null) return type.IsValueType ? Activator.CreateInstance(type) : null;

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (type.IsAssignableFrom(obj.GetType()))
            {
                // 如果待转换对象的类型与目标类型兼容，则无需转换
                return obj;
            }
            else if ((underlyingType ?? type).IsEnum)
            {
                // 如果待转换的对象的基类型为枚举

                if (underlyingType != null && string.IsNullOrEmpty(obj.ToString()))
                {
                    // 如果目标类型为可空枚举，并且待转换对象为null 则直接返回null值
                    return null;
                }
                else
                    return Enum.Parse(underlyingType ?? type, obj.ToString());
            }
            else if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type))
            {
                // 如果目标类型的基类型实现了IConvertible，则直接转换
                try
                {
                    return Convert.ChangeType(obj, underlyingType ?? type, null);
                }
                catch
                {
                    return underlyingType == null ? Activator.CreateInstance(type) : null;
                }
            }
            else
            {
                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                if (converter.CanConvertFrom(obj.GetType()))
                    return converter.ConvertFrom(obj);

                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    object o = constructor.Invoke(null);
                    PropertyInfo[] propertys = type.GetProperties();
                    Type oldType = obj.GetType();
                    foreach (PropertyInfo property in propertys)
                    {
                        PropertyInfo p = oldType.GetProperty(property.Name);
                        if (property.CanWrite && p != null && p.CanRead)
                            property.SetValue(o, convertObject(p.GetValue(obj, null), property.PropertyType), null);
                    }
                    return o;
                }
            }
            return obj;
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object getDefaultValue(Type type) { return type.IsValueType ? Activator.CreateInstance(type) : null; }
        #endregion

        #region CRUD,Page
        /// <summary>
        /// 添加一条表数据
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="model"></param>
        /// <param name="modelTableName">表名称</param>
        /// <param name="modelPrimaryKey">表主键</param>
        /// <returns></returns>
        public static object Create(string connectionString, T model, string modelTableName, string modelPrimaryKey)
        {
            object primaryKeyValue = null;
            Type modelPrimaryKeyType = typeof(T).GetProperty(modelPrimaryKey).PropertyType;

            // 获取有值的属性
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<PropertyInfo> propertyInfoes = new List<PropertyInfo>();
            List<object> values = new List<object>();
            foreach (var item in properties)
                if (!item.GetValue(model).Equals(getDefaultValue(item.PropertyType)))
                {
                    propertyInfoes.Add(item);
                    if (item.PropertyType.BaseType == typeof(Enum)) // 枚举类型，保存int值
                        values.Add((int)item.GetValue(model));
                    else
                        values.Add(item.GetValue(model));
                }

            // INSERT SQL 字符串
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.AppendFormat("INSERT INTO {0}({1}) VALUES({2});", modelTableName, string.Join(",", propertyInfoes.Select(k => k.Name)), "@" + string.Join(",@", propertyInfoes.Select(k => k.Name)));
            if (modelPrimaryKeyType != typeof(Guid))
                sqlStr.Append("SET @ID_FYUJMNBVFGHJ=SCOPE_IDENTITY();");

            // 参数设置
            List<SqlParameter> parameters = new List<SqlParameter>();
            for (int i = 0; i < propertyInfoes.Count; i++)
                parameters.Add(new SqlParameter()
                {
                    ParameterName = "@" + propertyInfoes[i].Name,
                    SqlDbType = sqlDbType[values[i].GetType()],
                    Value = values[i]
                });

            // 输出主键
            if (modelPrimaryKeyType != typeof(Guid))
                parameters.Add(new SqlParameter() { ParameterName = "@ID_FYUJMNBVFGHJ", SqlDbType = sqlDbType[modelPrimaryKeyType], Direction = ParameterDirection.Output });

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqlStr.ToString();
                    cmd.Parameters.AddRange(parameters.ToArray());

                    if (cmd.ExecuteNonQuery() > 0)
                        primaryKeyValue = modelPrimaryKeyType != typeof(Guid) ? parameters[parameters.Count - 1].Value : typeof(T).GetProperty(modelPrimaryKey).GetValue(model);

                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }

            return primaryKeyValue;
        }

        public static T Read(string connectionString, CommandType commandType, string commandText)
        { return Read(connectionString, commandType, commandText, null); }

        public static T Read(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            T result;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (commandParameters != null)
                    cmd.Parameters.AddRange(commandParameters);
                cn.Open();
                try
                {
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        result = read(sdr);
                        sdr.Close();
                        sdr.Dispose();
                    }
                    cmd.Parameters.Clear();
                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }
            return result;
        }
        private static T read(IDataReader dr)
        {
            var result = Activator.CreateInstance<T>();
            if (dr.Read())
                fill(result, dr);
            return result;
        }

        public static List<T> ReadList(string connectionString, CommandType commandType, string commandText)
        { return ReadList(connectionString, commandType, commandText, null); }

        public static List<T> ReadList(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            List<T> result;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (commandParameters != null)
                    cmd.Parameters.AddRange(commandParameters);
                cn.Open();
                try
                {
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        result = readList(sdr);
                        sdr.Close();
                        sdr.Dispose();
                    }
                    cmd.Parameters.Clear();
                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }
            return result;
        }

        private static List<T> readList(IDataReader dr)
        {
            var result = new List<T>();
            while (dr.Read())
            {
                var obj = Activator.CreateInstance<T>();
                fill(obj, dr);
                result.Add(obj);
            }
            return result;
        }

        public static Pager<T> ReadPaging(string connectionString, CommandType commandType, string commandText)
        {
            Pager<T> result = new Pager<T>();
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                cn.Open();
                try
                {
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            result.RecordCount = sdr.GetInt32(0);
                            result.Datas = new List<T>();
                            if (sdr.NextResult())
                                while (sdr.Read())
                                {
                                    var obj = Activator.CreateInstance<T>();
                                    fill(obj, sdr);
                                    result.Datas.Add(obj);
                                }
                        }
                        sdr.Close();
                        sdr.Dispose();
                    }
                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }
            return result;
        }

        public static Pager<T> ReadPaging(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            Pager<T> result = new Pager<T>();
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                if (commandParameters != null)
                    cmd.Parameters.AddRange(commandParameters);
                try
                {
                    cn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            result.RecordCount = sdr.GetInt32(0);
                            if (sdr.NextResult())
                                while (sdr.Read())
                                {
                                    var obj = Activator.CreateInstance<T>();
                                    fill(obj, sdr);
                                    result.Datas.Add(obj);
                                }
                        }
                        sdr.Close();
                        sdr.Dispose();
                    }
                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// 更新一条表数据
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="model"></param>
        /// <param name="modelTableName">表名称</param>
        /// <param name="modelPrimaryKey">表主键</param>
        /// <returns></returns>
        public static int Update(string connectionString, T model, string modelTableName, string modelPrimaryKey)
        {
            int result = 0;
            Type modelPrimaryKeyType = typeof(T).GetProperty(modelPrimaryKey).PropertyType;

            // 获取有值的属性
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<PropertyInfo> propertyInfoes = new List<PropertyInfo>();
            List<object> values = new List<object>();
            foreach (var item in properties)
                if (!item.GetValue(model).Equals(getDefaultValue(item.PropertyType)))
                {
                    propertyInfoes.Add(item);
                    values.Add(item.GetValue(model));
                }

            // UPDATE SQL 字符串
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.AppendFormat("UPDATE {0} SET ", modelTableName);
            foreach (var item in propertyInfoes)
                if (item.Name.ToLower() != modelPrimaryKey.ToLower())
                    sqlStr.AppendFormat("{0}=@{0},", item.Name);
            sqlStr.Remove(sqlStr.Length - 1, 1);
            sqlStr.AppendFormat(" WHERE {0}=@{0}", modelPrimaryKey);

            // 参数设置
            List<SqlParameter> parameters = new List<SqlParameter>();
            for (int i = 0; i < propertyInfoes.Count; i++)
                parameters.Add(new SqlParameter()
                {
                    ParameterName = "@" + propertyInfoes[i].Name,
                    SqlDbType = sqlDbType[values[i].GetType()],
                    Value = values[i]
                });

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqlStr.ToString();
                    cmd.Parameters.AddRange(parameters.ToArray());

                    result = cmd.ExecuteNonQuery();

                    cn.Close();
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    cn.Close();
                    cn.Dispose();
                    throw ex;
                }
            }

            return result;
        }
        #endregion
    }
}
