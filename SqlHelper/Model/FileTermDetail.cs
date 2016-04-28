using Com.EnjoyCodes.SqlAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.Model
{
    [Table(Name = "FileTermDetails", Prefix = "ftd_")]
    public class FileTermDetail
    {
        [Key]
        public int ID { get; set; }
        public int FileTermID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
