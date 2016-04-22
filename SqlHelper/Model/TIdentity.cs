using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.EnjoyCodes.Model
{
    public enum Types
    {
        T1 = 1,
        T2,
        T3
    }

    public class TIdentity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Types Type { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
