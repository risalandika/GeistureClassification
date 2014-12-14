using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Leap;
using System.Collections;

namespace GeistClass
{
    class BackpropagationWithGA
    {
        const int max = 99999999;
        List<List<float>> dataSetCollection = new List<List<float>>();
        List<float> dataSet = new List<float>();
        string className = "";
        string[] splits;
        List<string> kelas = new List<string>();
        Random ran = new Random(0);
        float[] chromosom = new float[max];
        public BackpropagationWithGA()
        {
            
        }

        public void ReadFromFiles()
        {
            DirectoryInfo d = new DirectoryInfo("Tests\\");
            FileInfo[] files = d.GetFiles("*.txt");
            Console.WriteLine("Found " + files.Length + " files..");
            foreach (FileInfo f in files)
            {

                splits = f.Name.Split('.');
                if (!className.Equals(splits[0]))
                {
                    kelas.Add(className);
                }

                className = splits[0];
                
                using (StreamReader reader = f.OpenText())
                {
                    string tempString ="";
                    while ((tempString = reader.ReadLine()) != null)
                    {
                        if (!tempString.Equals(""))
                        {
                            float tempFloat = float.Parse(tempString);
                            //tempFloat /= 360f;
                            dataSet.Add(tempFloat);
                        }
                    }
                }
                dataSetCollection.Add(dataSet);
                dataSet = new List<float>();
            }
        }

        public void DataSetCollectionDoBP()
        {
            GeneticAlgorithm();
            for (int i = 0; i < dataSetCollection.Count; i++)
            {
                if (chromosom[i] == 1)
                {
                    Console.WriteLine("seleksi kromosom ke " + i);
                    BackPropagation(dataSetCollection[i]);
                }
            }
        }

        public void GeneticAlgorithm()
        {
            int length = dataSetCollection.Count;
            chromosom = new float[length];
            for (int i = 0; i < length; i++)
            {
                chromosom[i] = RandomBinary();
            }
           
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine(chromosom[i]);
            }

            //CrossOver(chromosom);
            //Console.WriteLine("after");
            //for (int i = 0; i < length; i++)
            //{
            //    Console.WriteLine(chromosom[i]);
            //}
            //Console.WriteLine("end");
        }

        public void CrossOver(float[] chrom)
        {
            int length = chrom.Length;
            int mid = length/2;
            for (int i = 0; i < mid; i++)
            {
                float temp = chrom[i];
                chrom[i] = chrom[i + mid];
                chrom[i + mid] = temp;
            }
        }

        public void BackPropagation(List<float> ds)
        {
            float[] error = new float[kelas.Count];
            float learningRate = 0.03f;
            int attributCount = ds.Count;
            float[] errorHidden = new float[attributCount];
            float[] outputHidden = new float[attributCount];
            float[] output = new float[kelas.Count];
            float[] dotProducts = new float[attributCount];
            float[,] weightsInputToHidden = new float[attributCount,attributCount];
            float[,] weightsHiddenToOutput = new float[attributCount,kelas.Count];

            int jumlahHidden = (int)Math.Ceiling((decimal)attributCount / 2);
            jumlahHidden = 2;
            int jumlahOutput = (int)Math.Ceiling((decimal)kelas.Count / 2);
            
            //Inisialisasi bobot awal dari input ke hidden
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {
                    float temp = RandomWeight();
                    weightsInputToHidden[i, j] = temp;
                }
            }

            Console.WriteLine("*** bobot awal input ke hidden node***");
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {
                    Console.WriteLine(weightsInputToHidden[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("***END***");

            //Inisialisasi bobot awal dari hidden ke output
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)    
                {
                    float temp = RandomWeight();
                    weightsHiddenToOutput[i,j] = temp;
                }
            }

            Console.WriteLine("*** bobot awal hidden ke output***");
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    Console.WriteLine(weightsHiddenToOutput[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("***END***");

            //Fungsi aktivasi di hidden layer adalah sigmoid, sedangkan fungsi aktivasi di output layer adalah linear
            
            //dot product
            dotProducts = new float[attributCount];
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {

                    dotProducts[j] += (ds[i] * weightsInputToHidden[i, j]);
                }
            }
            for (int i = 0; i < jumlahHidden; i++)
            {
                Console.WriteLine("Dot product " +i +": " + dotProducts[i]);
            }

            //sigmoid
            for (int i = 0; i < attributCount; i++)
            {
                outputHidden[i] = 1.0f / (1.0f + ((float)Math.Exp(dotProducts[i]*-1)));
            }

            for (int i = 0; i < jumlahHidden; i++)
            {
                Console.WriteLine("Output Hidden " + i+ ": " + outputHidden[i]);
            }

            output = new float[jumlahOutput]; 
            //linear
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    
                    output[j]+=(outputHidden[i] * weightsHiddenToOutput[i,j]);
                }
            }

            for (int i = 0; i < jumlahOutput; i++)
            {
                Console.WriteLine("Output " + i+ ": " + output[i]);
            }

            ////////////////////feed forward stop////////////////////////////////

            //hitung error di output

            for (int i = 0; i < jumlahOutput; i++)
            {
                float temp = 0;
                float target = i;
                temp = (target - output[i]) * (1 - output[i]) * output[i];
                error[i] = temp;
            }

            for (int i = 0; i < jumlahOutput; i++)
            {
                Console.WriteLine("Error output " + i + ": " + error[i]);
            }

            //update bobot di hidden layer
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    weightsHiddenToOutput[i, j] += (learningRate * (error[j] * outputHidden[j]));
                }
            }

            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    Console.WriteLine("update weight " + i + "," + j+" "+ weightsHiddenToOutput[i, j]);
                }
            }
            
            //hitung error di hidden
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    errorHidden[i] = error[j] * weightsHiddenToOutput[i, j] * (1 - outputHidden[i]) * outputHidden[i];
                }
            }

            for (int i = 0; i < jumlahHidden; i++)
            {
                    Console.WriteLine("Error hidden  " + i +": " + errorHidden[i]);
            }

            //update bobot di input
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {
                    //Console.WriteLine("update weight input " + i + "," + j + ": " + weightsInputToHidden[i, j] + "+(" +learningRate +"*" +errorHidden[j]+ "*" + ds[i]+")" );
                    weightsInputToHidden[i, j] = weightsInputToHidden[i,j]+ (learningRate * errorHidden[j] * ds[i]);
                }
            }

            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {
                    Console.WriteLine("update weight input " + i + "," + j+ ": " + weightsInputToHidden[i, j]);
                }
            }
            Console.WriteLine("End iteration");
        }

        public float RandomWeight()
        {
            float ranWeight = 0f;
            ranWeight = (float)ran.Next(-3, 3);
            ranWeight /= 10.0f;
            
            return ranWeight;
        }

        public float RandomBinary()
        {
            float ranBinary = 0f;
            ranBinary = (float)ran.Next(0,2);
            return ranBinary;
        }
    }
}
