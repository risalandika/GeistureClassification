using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{

    class Node
    {
        private float _input;
        public float Input
        {
            set
            {
                _input = value;
            }
            get
            {
                return _input;
            }
        }
        private List<float> weight;
        public int NextNodeCount
        {
            get
            {
                return weight.Count;
            }
        }

        public void SetWeight(int nextNodeIndex, float val)
        {
            weight[nextNodeIndex] = val;
        }
        public float GetWeight(int nextNodeIndex)
        {
            return weight[nextNodeIndex];
        }
        public float GetOutput(int nextNodeIndex)
        {
            return weight[nextNodeIndex] * _input;
        }

        public Node(int count)
        {
            weight = new List<float>();
            for (int i = 0; i < count; i++)
                weight.Add(0);
        }
    }
}
