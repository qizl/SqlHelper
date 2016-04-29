using Com.EnjoyCodes.SqlAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.Model
{
    [Table(Name = "FileTerms", Prefix = "ft_")]
    public class FileTerm
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
    }
}
