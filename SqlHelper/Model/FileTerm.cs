using Com.EnjoyCodes.Model.DynamicMethod;
using Com.EnjoyCodes.SqlAttribute;
using System;
using System.Collections.Generic;

namespace Com.EnjoyCodes.Model
{
    [Table(Name = "FileTerms", Prefix = "ft_")]
    public class FileTerm : IMemberAccessor
    {
        [Key]
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Describe { get; set; }
        public Guid GenreID { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsNotice { get; set; }
        public int Amounts { get; set; }
        public Types Type { get; set; }

        [ForeignKey("FileTermID")]
        public virtual List<FileTermDetail> Details { get; set; }
        [ForeignKey("GenreID")]
        public virtual Genre Genre { get; set; }

        public object GetValue(object instance, string memberName)
        {
            var f = instance as FileTerm;
            if (f != null)
                switch (memberName)
                {
                    case "ID": return f.ID;
                    case "Title": return f.Title;
                    case "Describe": return f.Describe;
                    case "GenreID": return f.GenreID;
                    case "CreateTime": return f.CreateTime;
                    case "IsNotice": return f.IsNotice;
                    case "Amounts": return f.Amounts;
                    case "Type": return f.Type;
                    default: return null;
                }
            else throw new InvalidProgramException();
        }

        public void SetValue(object instance, string memberName, object newValue)
        {
            var f = instance as FileTerm;
            if (f != null)
                switch (memberName)
                {
                    case "ID": f.ID = Guid.Parse(newValue.ToString()); break;
                    case "Title": f.Title = newValue.ToString(); break;
                    case "Describe": f.Describe = newValue.ToString(); break;
                    case "GenreID": f.GenreID = Guid.Parse(newValue.ToString()); break;
                    case "CreateTime": f.CreateTime = Convert.ToDateTime(newValue); break;
                    case "IsNotice": f.IsNotice = Convert.ToBoolean(newValue); break;
                    case "Amounts": f.Amounts = Convert.ToInt32(newValue); break;
                    case "Type": f.Type = (Types)Enum.Parse(typeof(Types), newValue.ToString()); break;
                }
            else throw new InvalidProgramException();
        }
    }
}
