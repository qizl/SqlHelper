using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.SQLServerDAL
{
    public class SQLHelper
    {
        #region Constructures & Private Utility Methods
        private SQLHelper() { }

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
                    case "Com.EnjoyCodes.SQLServerDAL":
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

        #region ExcuteDataSet
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
    }

    public class SQLHelper<T>
    {
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

        private static void fill(T obj, IDataReader dr)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var item in properties)
                if (dr[item.Name] != null)
                    item.SetValue(obj, convertObject(dr[item.Name], item.PropertyType), null);
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
    }
}
