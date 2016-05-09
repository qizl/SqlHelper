using System;

namespace ReflectionTest.Entities
{
    public class Widget
    {
        #region Private Members

        private int _id;
        private string _name;

        #endregion

        #region Public Properties

        public int ID
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        #endregion

        #region Constructor

        public Widget(Int32 id, string name)
        {
            _id = id;
            _name = name;
        }

        #endregion
    }
}
