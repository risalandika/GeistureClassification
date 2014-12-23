using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    class DataSet
    {
        public string ClassName { set; get; }
        private List<float> _attribute;
        public float this[int index]
        {
            set
            {
                _attribute[index] = value;
            }
            get
            {
                return _attribute[index];
            }
        }
        public int AttributeCount
        {
            get
            {
                return _attribute.Count;
            }
        }
        public DataSet()
        {
            _attribute = new List<float>();
        }
        public DataSet(int attributeCount)
        {
            _attribute = new List<float>();
            for (int i = 0; i < attributeCount; i++)
                _attribute.Add(0);
        }
    }
}
