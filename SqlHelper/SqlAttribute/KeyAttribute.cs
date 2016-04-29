using System;

namespace Com.EnjoyCodes.SqlAttribute
{
    /// <summary>
    /// 主键属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    { }
}
