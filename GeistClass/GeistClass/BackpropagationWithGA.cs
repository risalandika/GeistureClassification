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
        const int max = 99;
        List<List<float>> dataSetCollection = new List<List<float>>();
        List<List<float>> dataTestCollection = new List<List<float>>();
        List<float> dataSet = new List<float>();
        List<float> dataTest = new List<float>();
        List<List<float>> dataSetCollectionGA = new List<List<float>>();
        List<List<List<float>>> dataSetCollectionsGA = new List<List<List<float>>>();
        List<float> dataSetGA = new List<float>();
        float treshold = 0.5f;
        string className = "";
        string[] splits;
        float jumlahBenar, jumlahDataTest;
        List<string> kelas = new List<string>();
        Random ran = new Random(0);
        float[] chromosom = new float[max];
        List<float[]> chromosoms = new List<float[]>();
        int[] selectedChromosom = new int[5];
        float fitnessFunction = 0;
        int iteration = 0;
        List<string> allKelas = new List<string>();
        float[] fitnessValue = new float[5]{0f,0f,0f,0f,0f};
        float[,] weightsInputToHidden = new float[max, max];
        float[,] weightsHiddenToOutput = new float[max, max];
        int jumlahFitur = 26;
        List<float[]> targetClasses = new List<float[]>();
        List<float[]> actualClasses = new List<float[]>();
        float[] actualClass = new float[max];

        public BackpropagationWithGA()
        {
            
        }

        public void ReadDataTest(string dir)
        {
            DirectoryInfo d = new DirectoryInfo(dir);
            FileInfo[] files = d.GetFiles("*.txt");
            Console.WriteLine("Found " + files.Length + " files..");
            dataTestCollection.Clear();
            foreach (FileInfo f in files)
            {
                //Console.WriteLine("Reading from " +f.Name+" ...");
                //splits = f.Name.Split('.');
                ////Console.WriteLine(splits[0] +"+"+ className);
                //if (!className.Equals(splits[0]))
                //{
                //    kelas.Add(splits[0]);
                //}
                //className = splits[0];
                //allKelas.Add(className);
                
                using (StreamReader reader = f.OpenText())
                {
                    string tempString ="";
                    while ((tempString = reader.ReadLine()) != null)
                    {
                        if (!tempString.Equals(""))
                        {
                            float tempFloat = float.Parse(tempString);
                            //tempFloat /= 360f;
                            dataTest.Add(tempFloat);
                        }
                    }
                }
                dataTestCollection.Add(dataTest);
                dataTest = new List<float>();
            }
        }

        public void ReadFromFiles(string dir)
        {
            DirectoryInfo d = new DirectoryInfo(dir);
            FileInfo[] files = d.GetFiles("*.txt");
            jumlahDataTest = files.Length;
            Console.WriteLine("Found " + files.Length + " files..");
            foreach (FileInfo f in files)
            {
                //Console.WriteLine("Reading from " +f.Name+" ...");
                splits = f.Name.Split('.');
                //Console.WriteLine(splits[0] +"+"+ className);
                if (!className.Equals(splits[0]))
                {
                    kelas.Add(splits[0]);
                }
                className = splits[0];
                allKelas.Add(className);
                
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

        public void NormalizeData(List<List<float>> llf)
        {
            //float[] minimum=new float[max];
            //float[] maximum = new float [max];
            //float minX = 999;
            //float maxX = -999;

            ////mencari minX dan maxX
            //List<float> temp = llf[0];
            //for (int i = 0; i < temp.Count; i++)
            //{
            //    for (int j = 0; j < llf.Count; j++)
            //    {
            //        List<float> temp2 = llf[j];
            //        if (temp2[i] < minX)
            //        {
            //            minX = temp2[i];
            //        }
            //        if (temp2[i] >= maxX)
            //        {
            //            maxX = temp2[i];
            //        }
            //    }
            //    minimum[i] = minX;
            //    maximum[i] = maxX;
            //}
            //for (int i = 0; i < temp.Count; i++)
            //{
            //    Console.WriteLine(minimum[i]);
            //}
            for (int i = 0; i < llf.Count; i++)
            {
                for (int j = 0; j < llf[i].Count; j++)
                {
                    llf[i][j] = llf[i][j] / 360.0f;
                    //llf[i][j] = (llf[i][j] - minimum[j]) / (maximum[j] - minimum[j]);  
                    //Console.Write(tempDS[j]+" ");
                }
                //Console.WriteLine();
            }
        }
        bool check = false;
        public void DataSetCollectionDoBP()
        {
            NormalizeData(dataSetCollection);
            //for (int i = 0; i < kelas.Count; i++)
            //{
            //    Console.WriteLine(kelas[i]);
            //}
            ResetWeight(dataSetCollection[0].Count, dataSetCollection[0].Count / 4, kelas.Count);
            for (int j = 0; j < 10000; j++)
                for (int i = 0; i < dataSetCollection.Count; i++)
                {
                    BackPropagation(dataSetCollection[i]);
                    
                }

            //GeneticAlgorithm();
        }

        public float CalculateFitnessFunction()
        {
            jumlahBenar = 0;
            for (int i = 0; i < actualClasses.Count; i++)
            {
                bool isSame = true;
                for (int j = 0; j < actualClasses[i].Count(); j++)
                {
                    //Console.Write(actualClasses[i][j] + "->" + targetClasses[i][j] + " ");
                    if (actualClasses[i][j] != targetClasses[i][j])
                    {
                        isSame = false;
                        break;
                    }
                }
                //Console.WriteLine();
                if (isSame) jumlahBenar++;
            }
            //Console.WriteLine(jumlahBenar + "/" + jumlahDataTest);
            fitnessFunction = jumlahBenar / jumlahDataTest;
            actualClasses.Clear();
            targetClasses.Clear();
            
            return fitnessFunction;
        }

        public void GeneticAlgorithm()
        {
            int jumlahIndividu = 5;
            int iterasi = 3;
            float peluangMutasi = ran.Next(0, 100)/100;
            float mutasiGenerasi = ran.Next(0, 100) / 100;

            //initialization
            for (int j = 0; j < jumlahIndividu; j++)
            {
                float[] chrom = Initialization();
                chromosoms.Add(chrom);
            }
            
            for (int i = 0; i < iterasi; i++)
            {
                //setiap iterasi
                for (int chromo = 0; chromo < chromosoms.Count; chromo++)
                {
                    for (int k = 0; k < dataSetCollection.Count; k++)
                    {
                        
                        for (int j = 0; j < chromosoms[chromo].Length; j++)
                        {
                            float[] temp = chromosoms[chromo];
                            if (temp[j] == 1)
                            {
                                float temp2 = dataSetCollection[k][j];
                                dataSetGA.Add(temp2);
                            }
                        }
                        dataSetCollectionGA.Add(dataSetGA);
                        dataSetGA = new List<float>();
                    }
                    dataSetCollectionsGA.Add(dataSetCollectionGA);
                    dataSetCollectionGA = new List<List<float>>();
                    //Console.WriteLine();
                }
                //Console.WriteLine(dataSetCollectionsGA[0].Count);

                for (int j = 0; j < jumlahIndividu; j++)
                {
                    for (int dsc = 0; dsc < dataSetCollectionsGA[j].Count; dsc++)
                    {
                        BackPropagation(dataSetCollectionsGA[j][dsc]);
                    }
                    fitnessValue[j] = CalculateFitnessFunction();
                    Console.WriteLine("FitnessValue[" + j + "] :" + fitnessValue[j]*100);
                    iteration = 0;
                    jumlahBenar = 0;
                }

                float[] fitnessValueOrderedAsc = fitnessValue.OrderBy(a => a).ToArray();

                //for (int j = 0; j < fitnessValueOrderedAsc.Count(); j++)
                //{
                //    Console.WriteLine(j+":" +fitnessValueOrderedAsc[j]);
                //}
                Console.WriteLine("***seleksi***");

                //seleksi
                for (int j = 0; j < jumlahIndividu; j++)
                {
                    selectedChromosom[j] = Array.IndexOf(fitnessValue, fitnessValueOrderedAsc[j]);
                }

                //for (int j = 0; j < fitnessValueOrderedAsc.Count(); j++)
                //{
                //    Console.WriteLine(j + ":" + selectedChromosom[j]);
                //}

                
                //crossover
                for (int j = 0; j < jumlahIndividu; j++)
                {
                    int temp = j + 1;
                    if (temp == jumlahIndividu)
                    {
                        temp = temp-1;
                    }
                    CrossOver(chromosoms[selectedChromosom[j]], chromosoms[selectedChromosom[temp]]);
                }
                
                //mutasi
                if (ran.Next(0,100)/100.0f < peluangMutasi)
                {
                     Console.WriteLine("mutasi");
                     for (int j = 0; j < jumlahIndividu; j++)
                     {
                        if (ran.Next(0, 100) / 100.0f < mutasiGenerasi)
                        {
                            Mutation(chromosoms[selectedChromosom[j]]);
                        }
                     }
                }
                  
                dataSetCollectionGA = new List<List<float>>();
            }
        }

        public void Mutation(float[] chrom)
        {
            for (int i = 0; i < chrom.Count(); i++)
            {
                if (chrom[chrom.Count() - 1] == 0)
                {
                    chrom[chrom.Count() - 1] = 1;
                }
                else
                    chrom[chrom.Count() - 1] = 0;
                
            }
        }

        public float[] Initialization()
        {
            int length = jumlahFitur ;
            chromosom = new float[length];
            for (int i = 0; i < chromosom.Length; i++)
            {
                chromosom[i] = RandomBinary();
            }

            for (int i = 0; i < chromosom.Length; i++)
            {
                Console.Write(chromosom[i]+ " ");
            }
            Console.WriteLine();
            return chromosom;
        }

        public void CrossOver(float[] chromosomI, float[] chromosomJ)
        {
            int length = chromosomI.Length;
            int mid = length/2;
            //Console.WriteLine("before");
            //for (int i = 0; i < length; i++)
            //{
            //    if (i == mid)
            //        Console.Write("[" + chromosomI[i] + "] ");
            //    else
            //        Console.Write(chromosomI[i] + " ");

            //}
            //Console.Write(" xxx ");
            //for (int i = 0; i < length; i++)
            //{
            //    if (i == mid)
            //        Console.Write("[" + chromosomJ[i] + "] ");
            //    else
            //        Console.Write(chromosomJ[i] + " ");
            //}
            //Console.WriteLine();

            for (int i = 0; i < mid; i++)
            {
                float temp = chromosomI[i];
                chromosomI[i + mid] = chromosomJ[i + mid];
                chromosomJ[i + mid] = temp;
            }
            //Console.WriteLine("after");
            //for (int i = 0; i < length; i++)
            //{
            //    if (i == mid)
            //        Console.Write("["+chromosomI[i] + "] ");
            //    else
            //        Console.Write(chromosomI[i] + " ");
                
            //}
            //Console.Write(" xxx ");
            //for (int i = 0; i < length; i++)
            //{
            //    if (i == mid)
            //        Console.Write("[" + chromosomJ[i] + "] ");
            //    else
            //        Console.Write(chromosomJ[i] + " ");
            //}
            //Console.WriteLine();
            
        }

        public void DataTestCollectionDoFF()
        {
            NormalizeData(dataTestCollection);

            for (int i = 0; i < dataTestCollection.Count; i++)
            {
                Console.Write("#"+i+": \n");
                FeedForward(dataTestCollection[i]);
            }
        }

        public void FeedForward(List<float> ds)
        {
            //for (int i = 0; i < ds.Count; i++)
            //{
            //    Console.Write(ds[i] + " ");
            //}
            //Console.WriteLine();

            int attributCount = ds.Count;
            int jumlahHidden = (int)Math.Ceiling((decimal)attributCount / 4);
            float[] errorHidden = new float[attributCount + 1];
            float[] outputHidden = new float[jumlahHidden + 1];
            float[] output = new float[kelas.Count];
            float[] dotProducts = new float[attributCount + 1];

            int jumlahOutput = kelas.Count;
            int factor = 1;
            while (Math.Pow(2, factor) < kelas.Count)
            {
                factor++;
            }
            jumlahOutput = factor;

            dotProducts = new float[attributCount + 1];
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {
                    //Console.Write(ds[i] + " ");
                    dotProducts[j] += (ds[i] * weightsInputToHidden[i, j]);
                }
                //Console.WriteLine();
            }
            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    Console.WriteLine("Dot product " + i + ": " + dotProducts[i]);
            //}

            //sigmoid
            for (int i = 0; i < jumlahHidden; i++)
            {
                outputHidden[i] = 1.0f / (1.0f + ((float)Math.Exp(dotProducts[i] * -1)));
            }

            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    Console.WriteLine("Output Hidden " + i + ": " + outputHidden[i]);
            //}

            output = new float[jumlahOutput];
            //linear
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    output[j] += (outputHidden[i] * weightsHiddenToOutput[i, j]);
                }
            }

            for (int j = 0; j < jumlahOutput; j++)
            {
                Console.WriteLine("output " + j + ": " + output[j]);
            }

            //for (int i = 0; i < attributCount; i++)
            //{
            //    for (int j = 0; j < jumlahHidden; j++)
            //    {
            //        Console.WriteLine("bobot input->hidden " + i + "," + j + ": " + weightsInputToHidden[i, j]);
            //    }
            //}

            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    for (int j = 0; j < jumlahOutput; j++)
            //    {
            //        Console.WriteLine("bobot hidden->output " + i + "," + j + ": " + weightsHiddenToOutput[i, j]);
            //    }
            //}   
            //actualClass = new float[jumlahOutput];
            //for (int i = 0; i < jumlahOutput; i++)
            //{
            //    if (Math.Abs(output[i]) < treshold) actualClass[i] = 0;
            //    else actualClass[i] = 1;
            //    Console.WriteLine("[" + actualClass[i] + "] Output " + i + ": " + output[i]);
            //}
        }

        void ResetWeight(int attributCount, int jumlahHidden, int jumlahOutput)
        {
            //Inisialisasi bobot awal dari input ke hidden
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {
                    float temp = RandomWeight();
                    weightsInputToHidden[i, j] = temp;
                }
            }

            //for (int i = 0; i < attributCount; i++)
            //{
            //    for (int j = 0; j < jumlahHidden; j++)
            //    {
            //        Console.WriteLine("bobot awal input->hidden " + i + "," + j + ": " + weightsInputToHidden[i, j]);
            //    }
            //}

            //Inisialisasi bobot awal dari hidden ke output
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    float temp = RandomWeight();
                    weightsHiddenToOutput[i, j] = temp;
                }
            }

            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    for (int j = 0; j < jumlahOutput; j++)
            //    {
            //        Console.WriteLine("bobot awal hidden->output " + i + "," + j + ": " + weightsHiddenToOutput[i, j]);
            //    }
            //}
        }

        public void BackPropagation(List<float> ds)
        {
            //Console.WriteLine("**jumlah : " + ds.Count);
            float[] error = new float[kelas.Count];
            float learningRate = 0.1f;
            int attributCount = ds.Count;
            int jumlahHidden = (int)Math.Ceiling((decimal)attributCount / 4);
            float[] errorHidden = new float[attributCount+1];
            float[] outputHidden = new float[jumlahHidden+1];
            float[] output = new float[kelas.Count];
            float[] dotProducts = new float[attributCount+1];

            
            int jumlahOutput = kelas.Count;
            int factor = 1;
            while (Math.Pow(2, factor) < kelas.Count)
            {
                factor++;
            }
            jumlahOutput = factor;
            //Fungsi aktivasi di hidden layer adalah sigmoid, sedangkan fungsi aktivasi di output layer adalah linear
            
            //dot product
            dotProducts = new float[attributCount+1];
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {
                    //Console.Write(ds[i] + " ");
                    dotProducts[j] += (ds[i] * weightsInputToHidden[i, j]);
                }
                //Console.WriteLine();
            }
            
            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    Console.WriteLine("Dot product " + i + ": " + dotProducts[i]);
            //}

            //sigmoid
            for (int i = 0; i < jumlahHidden; i++)
            {
                outputHidden[i] = 1.0f / (1.0f + ((float)Math.Exp(dotProducts[i]*-1)));
            }

            
            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    Console.WriteLine("Output Hidden " + i + ": " + outputHidden[i]);
            //}

            output = new float[jumlahOutput]; 
            //linear
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    output[j]+=(outputHidden[i] * weightsHiddenToOutput[i,j]);
                }
            }

            //for (int i = 0; i < jumlahOutput; i++)
            //{
            //    output[i] = 1.0f / (1.0f + ((float)Math.Exp(output[i] * -1)));
            //}

            //if (itr == 99)
            //for (int i = 0; i < jumlahOutput; i++)
            //{
            //    Console.WriteLine("Output " + i + ": " + output[i]);
            //}

            //if (itr == 99 && iteration == 49)
            //    return;

            ////////////////////feed forward stop////////////////////////////////

            //hitung error di output
            float[] targetBinary = new float[jumlahOutput];
            for (int i = 0; i < jumlahOutput; i++)
            {
                float temp = 0;
                float target = 0;
                
                if (iteration >= allKelas.Count) iteration = 0;
               // Console.WriteLine();
                target = kelas.IndexOf(allKelas[iteration]);
                int tempTarget = (int)target;
                int count=jumlahOutput;
                while (tempTarget != 0)
                {
                    targetBinary[count - 1] = tempTarget % 2;
                    tempTarget /= 2;
                    count--;
                }
                //for (int j = 0; j < targetBinary.Count(); j++)
                //{
                //    Console.Write(targetBinary[j] + " ");
                //}
                //Console.WriteLine();
                // Console.WriteLine("target -> " + target);
                //temp = (targetBinary[i] - output[i]) * (1 - output[i]) * output[i];
               
                temp = targetBinary[i] - output[i];
                //Console.Console.WriteLine("("+target + "-" + output[i] + ")(" + "1-" + output[i] + ")" + output[i]);
                error[i] = temp;
                //if (itr == 999)
                //{
                //    Console.WriteLine(allKelas[iteration] + " " + "#Error [" + i + "]-> " + targetBinary[i] + ":" + temp);
                //}
            }
            //for (int i = 0; i < jumlahOutput; i++)
            //{
            //    Console.WriteLine("Error output " + i + ": " + error[i]);
            //}

            //update bobot di hidden layer
            //if(!check)
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    weightsHiddenToOutput[i, j] += (learningRate * (error[j] * outputHidden[j]));
                }
            }

            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    for (int j = 0; j < jumlahOutput; j++)
            //    {
            //        Console.WriteLine("update weight " + i + "," + j + " " + weightsHiddenToOutput[i, j]);
            //    }
            //}
            
            //hitung error di hidden
            for (int i = 0; i < jumlahHidden; i++)
            {
                for (int j = 0; j < jumlahOutput; j++)
                {
                    errorHidden[i] = error[j] * weightsHiddenToOutput[i, j] * (1 - outputHidden[i]) * outputHidden[i];
                }
            }

            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    Console.WriteLine("Error hidden  " + i + ": " + errorHidden[i]);
            //}

            //update bobot di input
            //if(!check)
            for (int i = 0; i < attributCount; i++)
            {
                for (int j = 0; j < jumlahHidden; j++)
                {
                   weightsInputToHidden[i, j] = weightsInputToHidden[i,j]+ (learningRate * errorHidden[j] * ds[i]);
                }
            }

            //for (int i = 0; i < attributCount; i++)
            //{
            //    for (int j = 0; j < jumlahHidden; j++)
            //    {
            //        Console.WriteLine("update weight input " + i + "," + j + ": " + weightsInputToHidden[i, j]);
            //    }
            //}


            //for (int i = 0; i < attributCount; i++)
            //{
            //    for (int j = 0; j < jumlahHidden; j++)
            //    {
            //        Console.WriteLine("bobot input->hidden " + i + "," + j + ": " + weightsInputToHidden[i, j]);
            //    }
            //}

            //for (int i = 0; i < jumlahHidden; i++)
            //{
            //    for (int j = 0; j < jumlahOutput; j++)
            //    {
            //        Console.WriteLine("bobot hidden->output " + i + "," + j + ": " + weightsHiddenToOutput[i, j]);
            //    }
            //}

            //if (check)
            //{
            //    for (int i = 0; i < ds.Count; i++)
            //    {
            //        Console.Write(ds[i]+" ");
            //    }
            //    Console.WriteLine();
            //    for (int i = 0; i < jumlahHidden; i++)
            //    {
            //        for (int j = 0; j < jumlahOutput; j++)
            //        {
            //            Console.WriteLine("bobot hidden->output " + i + "," + j + ": " + weightsHiddenToOutput[i, j]);
            //        }
            //    }
            //    //if (itr == 999)
            //    //{
            //    //    Console.WriteLine(allKelas[iteration] + " " + "#Error [" + i + "]-> " + targetBinary[i] + ":" + temp);
            //    //}
            //    actualClass = new float[jumlahOutput];
            //    for (int i = 0; i < jumlahOutput; i++)
            //    {
            //        if (Math.Abs(output[i]) < treshold) actualClass[i] = 0;
            //        else actualClass[i] = 1;
            //        Console.WriteLine("["+ actualClass[i] + "]->["+targetBinary[i]+"] "+allKelas[iteration] + " Output " + i + ": " + output[i]);
            //    }

            //    actualClasses.Add(actualClass);
            //    targetClasses.Add(targetBinary);
            ////    Console.WriteLine("////////////////////////////////////////");
            //}


            
            //Console.WriteLine("///End iteration////" + iteration);
            iteration++;
        }

        public float RandomWeight()
        {
            float ranWeight = 0f;
            while (ranWeight == 0f)
            {
                ranWeight = (float)ran.Next(-3, 3);

                ranWeight /= 10.0f;
            }
            return ranWeight;
        }

        public int RandomPick()
        {
            int randomPick = 0;
                randomPick = ran.Next(0, 5);
            return randomPick;
        }

        public float RandomBinary()
        {
            float ranBinary = 0f;
            ranBinary = (float)ran.Next(0,2);
            return ranBinary;
        }
    }
}
