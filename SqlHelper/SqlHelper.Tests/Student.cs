using Com.EnjoyCodes.SqlAttribute;
using System;

namespace Com.EnjoyCodes.SqlHelper.Tests
{
    [Table(Name = "Students", Prefix = "s_")]
    public class Student
    {
        public int IID { get; set; }
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public bool Is { get; set; }
        public int Age { get; set; }
        public DateTime Time { get; set; }
        public string S1 { get; set; }
        public string S2 { get; set; }
        public string S3 { get; set; }
        public string S4 { get; set; }
        public string S5 { get; set; }
        public string S6 { get; set; }
        public string S7 { get; set; }
        public string S8 { get; set; }
        public string S9 { get; set; }
        public string S10 { get; set; }
        public string S11 { get; set; }
        public string S12 { get; set; }
        public string S13 { get; set; }
        public string S14 { get; set; }
        public string S15 { get; set; }
        public string S16 { get; set; }
        public string S17 { get; set; }
        public string S18 { get; set; }
        public string S19 { get; set; }
        public string S20 { get; set; }
        public string S21 { get; set; }
        public string S22 { get; set; }
        public string S23 { get; set; }
        public string S24 { get; set; }

        public virtual C c { get; set; }
    }

    public class C { }

    public enum Sex
    {
        Girl = 0,
        Boy
    }

    [Table(Name = "StudentDetails")]
    public class StudentDetails
    {
        [Key]
        public int ID { get; set; }
        public Guid SID { get; set; }
        public DateTime Time { get; set; }
        public int Amounts { get; set; }
    }
}
