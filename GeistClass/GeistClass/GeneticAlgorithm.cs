using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    class GeneticAlgorithm
    {
        const int individualCount = 5;
        private Random random = new Random();
        private ClassificationClass classificationClass;
        private BackPropagation backPropagation;
        private NeuralNetwork network;
        private ListDataSet listDataSet;
        private List<Chromosom> chromosoms;

        public void Initialize(ListDataSet lds, ClassificationClass cc)
        {
            listDataSet = lds;
            classificationClass = cc;
        }

        public Chromosom Run()
        {
            //NeuralNetwork nn = new NeuralNetwork();
            //nn.InitialiseNetwork(listDataSet[0].AttributeCount, listDataSet[0].AttributeCount / 2, classificationClass.TargetCount);
            //nn.Seed = 0;
            //nn.InitialiseWeight();

            //BackPropagation bp = new BackPropagation();
            //bp.Initialise(nn, listDataSet, classificationClass);
            //bp.Run();

            //FeedForward ff = new FeedForward();
            //ff.Initialise(nn, listDataSet);
            //ff.Run();

            ChromosomInit();

            int iteration = 10;
            for (int i = 0; i < iteration; i++)
            {
                //Console.WriteLine("##################    "+ i);
                DoBackPropagation();
                DoSelection();
                DoCrossOver();

                int chanceMutation = GetRandom();
                if (chanceMutation < GetRandom())
                {
                    DoMutation();
                }
            }

            int index = 0;
            for (int i = 1; i < chromosoms.Count; i++)
            {
                if (chromosoms[i].FitnessValue > chromosoms[index].FitnessValue)
                {
                    index = i;
                }
            }

            Chromosom fittestChromosom = chromosoms[index];
            return fittestChromosom;
        }

        private void ChromosomInit()
        {
            chromosoms = new List<Chromosom>(individualCount);

            for (int i = 0; i < individualCount; i++)
            {
                chromosoms.Add(new Chromosom(GetRandomBinary()));
            }
        }

        private void DoBackPropagation()
        {
            for (int i = 0; i < chromosoms.Count;i++ )
            {
                ListDataSet lds = new ListDataSet(listDataSet);

                for (int j = 0; j < lds.Count; j++)
                {
                    int popCount = 0;
                    for (int k = 0; k < chromosoms[i].Length; k++)
                    {
                        if (chromosoms[i][k] == 0)
                        {
                            lds[j].RemoveBit(k - popCount);
                            popCount++;
                        }
                    }
                }

                NeuralNetwork nn = new NeuralNetwork();
                nn.InitialiseNetwork(lds[0].AttributeCount, lds[0].AttributeCount / 2, classificationClass.TargetCount);
                nn.InitialiseWeight();

                BackPropagation bp = new BackPropagation();
                bp.Initialise(nn, lds, classificationClass);
                bp.Run();

                FeedForward ff = new FeedForward();
                ff.Initialise(nn, lds);

                int totalCorrect = 0;
                for (int j = 0; j < lds.Count; j++)
                {
                    ff.Run(j);
                    //nn.Print();

                    //foreach (int ac in ff.GetActualClass())
                    //    Console.Write(ac + " ");
                    //Console.Write(" --> ");
                    //foreach (int ac in classificationClass.GetTarget(lds[j].ClassName))
                    //    Console.Write(ac + " ");
                    //Console.WriteLine();

                    bool correct = true;
                    int[] targetClass = classificationClass.GetTarget(lds[j].ClassName);
                    for (int k = 0; k < ff.GetActualClass().Length; k++)
                    {
                        if (targetClass[k] != ff.GetActualClass()[k])
                            correct = false;
                    }

                    if (correct)
                        totalCorrect++;
                }
                //Console.WriteLine("total: " + totalCorrect + "/" + lds.Count);
                chromosoms[i].FitnessValue = totalCorrect / (float)lds.Count;
                //chromosoms[i].Print();
            }
        }

        private void DoSelection()
        {
            chromosoms.Sort((val1, val2) => val1.FitnessValue.CompareTo(val2.FitnessValue));
        }

        private void DoCrossOver()
        {
            
            //Console.WriteLine();
            for (int i = 0; i < chromosoms.Count; i+=2)
            {
                int nextIndex = i+1;
                if (nextIndex >= chromosoms.Count)
                    nextIndex = i - 1;
                int[] bit1 = chromosoms[i].Bit;
                int[] bit2 = chromosoms[nextIndex].Bit;

                for (int j = bit1.Length/2; j < bit1.Length; j++)
                {
                    int t = bit1[j];
                    bit1[j] = bit2[j];
                    bit2[j] = t;
                }

                chromosoms[i].Bit = bit1;
                chromosoms[nextIndex].Bit = bit2;
            }
           
        }

        private void DoMutation()
        {
            for (int i=0;i<chromosoms.Count;i++)
            {
                int chanceChromoMut = GetRandom();

                if (chanceChromoMut <= GetRandom())
                {
                    int bitIndex = GetRandom(chromosoms[i].Length);

                    if (chromosoms[i][bitIndex] == 0)
                        chromosoms[i][bitIndex] = 1;
                    else
                        chromosoms[i][bitIndex] = 0;
                }
            }
        }

        private int[] GetRandomBinary()
        {
            int[] chromoTemp = new int[listDataSet[0].AttributeCount];

            for (int i = 0; i < listDataSet[0].AttributeCount; i++)
            {
                int ran = random.Next(0, 2);
                chromoTemp[i] = ran;
            }
            
            
            return chromoTemp;
        }

        private int GetRandom(int max)
        {
            return random.Next(0, max);
        }
        private int GetRandom()
        {
            return random.Next(0, 100);
        }
    }
}
