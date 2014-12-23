using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace GeistClass
{
    class FileReader
    {
        public ListDataSet ReadFile(string path, ClassificationClass cc)
        {
            path = path.TrimEnd('\\').TrimEnd('/') + "\\";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fileInfo = dirInfo.GetFiles("*.txt");

            ListDataSet dataSetList = new ListDataSet();
            foreach(FileInfo fi in fileInfo)
            {
                string className = fi.Name.Split('.')[0];
                cc.Add(className);
                StreamReader reader = new StreamReader(path + fi.Name);
                List<float> attr = new List<float>();

                while (!reader.EndOfStream)
                {
                    attr.Add(float.Parse(reader.ReadLine()));
                }

                DataSet ds = new DataSet(attr.Count);

                for (int i = 0; i < attr.Count; i++)
                {
                    ds[i] = attr[i];
                }

                dataSetList.Add(ds);
            }

            return dataSetList;
        }
    }
}
