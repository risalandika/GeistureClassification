using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    class ListDataSet
    {
        private List<DataSet> dataSetList;
        public List<DataSet> DataSetList
        {
            get
            {
                return dataSetList;
            }
        }
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

        public ListDataSet()
        {
            dataSetList = new List<DataSet>();
        }

        public ListDataSet(ListDataSet lds)
        {
            dataSetList = new List<DataSet>();

            for (int i = 0; i < lds.Count; i++)
            {
                dataSetList.Add(new DataSet(lds[i]));
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
