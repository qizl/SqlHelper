using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.Model
{
    public class FileTerm
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Describe { get; set; }
        public Guid GenreID { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsNotice { get; set; }
    }
}
