using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistClass
{
    class BackPropagation
    {
        private float learningRate = 0.1f;
        private FeedForward feedForward;
        public NeuralNetwork Network
        {
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

        public void Run()
        {
            //for (int i = 0; i < DataSetList.Count; i++)
            //{
                feedForward.Run(1);

                GetErrorOutput(1);
            //}
        }

        private void GetErrorOutput(int index)
        {
            float[] outputError = new float[classificationClass.TargetCount];
            
            for (int i = 0; i < classificationClass.TargetCount; i++)
            {
                outputError[i] = classificationClass.GetTarget(DataSetList[index].ClassName)[i] - Network.OutputLayer[i].GetOutput(0);
            }

            UpdateWeightHidden(outputError, index);
        }

        private void UpdateWeightHidden(float[] outputError, int index)
        {
            for (int i = 0; i < Network.HiddenLayer.Count; i++)
            {
                for (int j = 0; j < Network.OutputLayer.Count; j++)
                {
                    float newWeight = Network.HiddenLayer[i].GetWeight(j) + (this.learningRate * outputError[j] * Network.HiddenLayer[i].GetOutput(j));
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
                //outputError[j] * Network.HiddenLayer[i].GetWeight(j) * (1 - Network.HiddenLayer[i].GetOutput(j)) * Network.HiddenLayer[i].GetOutput(j); 
            }

            UpdateWeightInput(hiddenError, index);
        }
        private void UpdateWeightInput(float[] hiddenError, int index)
        {
            for (int i = 0; i < Network.InputLayer.Count; i++)
            {
                for (int j = 0; j < Network.HiddenLayer.Count; j++)
                {
                    float newWeight = Network.InputLayer[i].GetWeight(j) + (learningRate * hiddenError[j] * Network.InputLayer[i].Input);
                    Network.InputLayer[i].SetWeight(j, newWeight);
                }
            }
        }
    }
}
