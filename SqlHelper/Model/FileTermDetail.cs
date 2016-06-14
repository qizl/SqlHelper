using Com.EnjoyCodes.Model.DynamicMethod;
using Com.EnjoyCodes.SqlAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.Model
{
    [Table(Name = "FileTermDetails", Prefix = "ftd_")]
    public class FileTermDetail : IMemberAccessor
    {
        [Key]
        public int ID { get; set; }
        public Guid FileTermID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }

        public object GetValue(object instance, string memberName)
        {
            var f = instance as FileTermDetail;
            if (f != null)
                switch (memberName)
                {
                    case "ID": return f.ID;
                    case "FileTermID": return f.FileTermID;
                    case "Name": return f.Name;
                    case "CreateTime": return f.CreateTime;
                    default: return null;
                }
            else throw new InvalidProgramException();
        }

        public void SetValue(object instance, string memberName, object newValue)
        {
            var f = instance as FileTermDetail;
            if (f != null)
                switch (memberName)
                {
                    case "ID": f.ID = Convert.ToInt32(newValue); break;
                    case "FileTermID": f.FileTermID = Guid.Parse(newValue.ToString()); break;
                    case "Name": f.Name = newValue.ToString(); break;
                    case "CreateTime": f.CreateTime = Convert.ToDateTime(newValue); break;
                }
            else throw new InvalidProgramException();
        }
    }
}
