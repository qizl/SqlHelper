using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.SqlAttribute
{
    /// <summary>
    /// 外键属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        public string Name { get; private set; }
        public ForeignKeyAttribute(string foreignKey)
        { this.Name = foreignKey; }
    }
}
