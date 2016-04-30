using Com.EnjoyCodes.SqlAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.Model
{
    [Table(Name = "Genres", Prefix = "g_")]
    public class Genre
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
    }
}
