using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    class ListDataSet
    {
        private List<DataSet> dataSetList = new List<DataSet>();
        public int Count
        {
            get
            {
                return dataSetList.Count;
            }
        }
        public DataSet this[int index]
        {
            set
            {
                dataSetList[index] = value;
            }
            get
            {
                return dataSetList[index];
            }
        }

        public void Add(DataSet ds)
        {
            dataSetList.Add(ds);
        }

        public void Normalized()
        {
            for (int i = 0; i < dataSetList.Count; i++)
            {
                for (int j = 0; j < dataSetList[i].AttributeCount; j++)
                {
                    dataSetList[i][j] /= 360f; 
                }
            }
        }
    }
}
