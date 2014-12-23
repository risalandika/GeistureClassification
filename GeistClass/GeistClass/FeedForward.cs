using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    class FeedForward
    {
        private NeuralNetwork neuralNetwork;
        public NeuralNetwork Network
        {
            get
            {
                return neuralNetwork;
            }
        }
        private ListDataSet dataSetList;
        public ListDataSet DataSetList
        {
            get
            {
                return dataSetList;
            }
        }

        public void Initialise(NeuralNetwork nn, ListDataSet dsl)
        {
            neuralNetwork = nn;
            dataSetList = dsl;
        }

        public void Run()
        {
            for (int i = 0; i < dataSetList.Count; i++)
            {
                neuralNetwork.InitaliseInput();
                DoInputLayer(i);
                Network.Print();
            }
            DoPrediction();
        }

        public void Run(int index)
        {
            neuralNetwork.InitaliseInput();
            DoInputLayer(index);
        }

        private void DoInputLayer(int index)
        {
            for (int i = 0; i < dataSetList[index].AttributeCount; i++)
            {
                neuralNetwork.InputLayer[i].Input = dataSetList[index][i];
            }

            DoHiddenLayer();
        }

        private void DoHiddenLayer()
        {
            for (int i = 0; i < neuralNetwork.InputLayer.Count; i++)
            {
                for (int j = 0; j < neuralNetwork.HiddenLayer.Count; j++)
                {
                    neuralNetwork.HiddenLayer[j].Input += neuralNetwork.InputLayer[i].GetOutput(j);
                }
            }

            DoOutputLayer();
        }

        private void DoOutputLayer()
	    {

            for (int i = 0; i < neuralNetwork.HiddenLayer.Count; i++)
            {
                for (int j = 0; j < neuralNetwork.OutputLayer.Count; j++)
                {
                    neuralNetwork.OutputLayer[j].Input += Sigmoid(neuralNetwork.HiddenLayer[i].Input) * neuralNetwork.HiddenLayer[i].GetWeight(j);
                }
            }
	    }

        public void DoPrediction()
        {
            for (int j = 0; j < neuralNetwork.OutputLayer.Count; j++)
            {
                if(Math.Abs(neuralNetwork.OutputLayer[j].Input) < neuralNetwork.treshold)
                {
                    Console.Write("0" + " ");
                }
                else Console.Write("1" + " ");
            }
            Console.WriteLine();
        }

        private float Sigmoid(float val)
        {
            //return val;
            return 1 / (1.0f + (float)Math.Exp(-val));
        }
    }
}
