using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.SqlAttribute
{
    /// <summary>
    /// 表属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 数据库表名
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 字段前缀
        /// </summary>
        public virtual string Prefix { get; set; }
    }
}
