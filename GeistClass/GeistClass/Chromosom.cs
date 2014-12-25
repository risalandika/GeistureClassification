using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    class Chromosom
    {
        private int[] bit;
        public int[] Bit
        {
            set
            {
                bit = value;
            }
            get
            {
                return bit;
            }
        }
        public int this[int index]
        {
            set
            {
                bit[index] = value;
            }
            get
            {
                return bit[index];
            }
        }
        public int Length
        {
            get
            {
                return bit.Length;
            }
        }
        private float fitnessValue = 0;
        public float FitnessValue
        {
            set
            {
                fitnessValue = value;
            }
            get
            {
                return fitnessValue;
            }
        }

        public Chromosom(int[] newBit)
        {
            bit = newBit;
        }

        public void Print()
        {
            for (int i=0;i<bit.Length;i++)
            {
                Console.Write(bit[i] + " ");
            }
            Console.WriteLine(" --> " + fitnessValue);
        }
    }
}
