using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    class BackPropagation
    {
        private float learningRate = 0.03f;
        private FeedForward feedForward;
        private NeuralNetwork lastStableNetwork;
        public NeuralNetwork Network
        {
            set
            {
                feedForward.Network = value;
            }
            get
            {
                return feedForward.Network;
            }
        }
        public ListDataSet DataSetList
        {
            get
            {
                return feedForward.DataSetList;
            }
        }
        private ClassificationClass classificationClass;

        public void Initialise(NeuralNetwork nn, ListDataSet dsl, ClassificationClass cc)
        {
            feedForward = new FeedForward();
            feedForward.Initialise(nn, dsl);
            classificationClass = cc;
        }

        public void Run(int maxIter)
        {
            float lastAvgError = float.MaxValue;
            for (int it = 0; it < maxIter; it++)
            {
                float avgError = 0;
                for (int k = 0; k < DataSetList.Count; k++)
                {
                    Network.InitaliseInput();
                    feedForward.Run(k);

                    float[] errorOutput = GetErrorOutput(k);
                    float totalError = 0;
                    for (int i = 0; i < errorOutput.Length; i++)
                    {
                        totalError += Math.Abs(errorOutput[i]);
                    }
                    avgError += totalError;
                    //foreach (float f in GetErrorOutput(k))
                    //    Console.Write(f + " ");
                    //Console.WriteLine();
                }
                avgError /= DataSetList.Count;
                //Console.WriteLine(it + ": " + (double)avgError + " " + (avgError - lastAvgError));

                if(it%5==0)
                {
                    if (avgError - lastAvgError < 0.01)
                    {
                        lastAvgError = avgError;
                        lastStableNetwork = new NeuralNetwork(Network);
                    }
                    else
                    {
                        Network = new NeuralNetwork(lastStableNetwork);
                        break;
                    }
                }
            }
            //Network.Print();
        }

        public void Run()
        {
            Run(100);
        }

        private float[] GetErrorOutput(int index)
        {
            float[] outputError = new float[classificationClass.TargetCount];

            for (int i = 0; i < classificationClass.TargetCount; i++)
            {
               
                outputError[i] = classificationClass.GetTarget(DataSetList[index].ClassName)[i] - Network.OutputLayer[i].Input;
               // Console.Write(classificationClass.GetTarget(DataSetList[index].ClassName)[i] + " ");
                //outputError[i] = Network.OutputLayer[i].Input *
                //    (1 - Network.OutputLayer[i].Input) *
                //    (classificationClass.GetTarget(DataSetList[index].ClassName)[i] - Network.OutputLayer[i].Input);
                //Console.WriteLine(Network.OutputLayer[i].GetOutput(0) + "*(" + "1-" + Network.OutputLayer[i].GetOutput(0) + ")*" + "(" + classificationClass.GetTarget(DataSetList[index].ClassName)[i] + "-" + Network.OutputLayer[i].GetOutput(0)+")");
            }
            UpdateWeightHidden(outputError, index);

            return outputError;
        }

        private void UpdateWeightHidden(float[] outputError, int index)
        {
            for (int i = 0; i < Network.HiddenLayer.Count; i++)
            {
                for (int j = 0; j < Network.OutputLayer.Count; j++)
                {
                    float newWeight = Network.HiddenLayer[i].GetWeight(j) + 
                        (this.learningRate * outputError[j] * Network.HiddenLayer[i].Input);
                    Network.HiddenLayer[i].SetWeight(j, newWeight);
                }
            }

            GetErrorHidden(outputError, index);
        }

        private void GetErrorHidden(float[] outputError, int index)
        {
            float[] hiddenError = new float[Network.HiddenLayer.Count];

            for (int i = 0; i < Network.HiddenLayer.Count; i++)
            {
                float linear = 0;
                for (int j = 0; j < Network.OutputLayer.Count; j++)
                {
                    linear += outputError[j]*Network.HiddenLayer[i].GetWeight(j);
                }

                hiddenError[i] = Network.HiddenLayer[i].Input * (1 - Network.HiddenLayer[i].Input) * linear;
            }

            UpdateWeightInput(hiddenError, index);
        }
        private void UpdateWeightInput(float[] hiddenError, int index)
        {
            for (int i = 0; i < Network.InputLayer.Count; i++)
            {
                for (int j = 0; j < Network.HiddenLayer.Count; j++)
                {
                    float newWeight = Network.InputLayer[i].GetWeight(j) + 
                        (learningRate * hiddenError[j] * Network.InputLayer[i].Input);
                    Network.InputLayer[i].SetWeight(j, newWeight);
                }
            }
        }

    }
}
