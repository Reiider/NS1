using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS1
{
    [Serializable]
    class NeuroNet
    {
        private Layer[] layers;
        private double learningRate;
        public delegate double Activation(double x);
        Activation fActivate;

        public NeuroNet(double learningRate, int[] sizes, Activation funcActivate)
        {
            fActivate = funcActivate;
            this.learningRate = learningRate;
            layers = new Layer[sizes.Length];

            Random rand = new Random();
            for (int i = 0; i < sizes.Length; i++)
            {
                int nextSize = 0;
                if (i < sizes.Length - 1) nextSize = sizes[i + 1];
                layers[i] = new Layer(sizes[i], nextSize);
                for (int j = 0; j < sizes[i]; j++)
                {
                    layers[i].biases[j] = 0;
                    for (int k = 0; k < nextSize; k++)
                    {
                        layers[i].weights[j, k] = rand.NextDouble() * rand.Next(-1, 1);
                    }
                }
            }
        }

        public double[] feedForward(double[] inputs)
        {
            inputs.CopyTo(layers[0].neurons, 0);
            for (int i = 1; i < layers.Length; i++)
            {
                for (int j = 0; j < layers[i].size; j++)
                {
                    layers[i].neurons[j] = 0;
                    for (int k = 0; k < layers[i - 1].size; k++)
                    {
                        layers[i].neurons[j] += layers[i - 1].neurons[k] * layers[i - 1].weights[k, j];
                    }
                    layers[i].neurons[j] = fActivate(layers[i].neurons[j]);
                }
            }
            return layers[layers.Length - 1].neurons;
        }


        public void backpropagation(double[] target)
        {
            Layer ll = layers[layers.Length - 1];
            for (int i = 0; i < ll.size; i++)
            {
                ll.biases[i] = (target[i] - ll.neurons[i]) * ll.neurons[i] * (1 - ll.neurons[i]);
            }

            for (int i = layers.Length - 2; i >= 0; i--) //i - слой сети, начиная с предпоследнего
            {
                Layer lay = layers[i];
                Layer rightLay = layers[i + 1];
                for (int j = 0; j < lay.size; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < rightLay.size; k++)
                    {
                        sum += rightLay.biases[k] * lay.weights[j, k]; //??
                    }
                    lay.biases[j] = lay.neurons[j] * (1 - lay.neurons[j]) * sum;
                }
            }

            for (int i = 0; i < layers.Length - 1; i++) //слой
            {
                Layer l = layers[i];
                Layer rightl = layers[i + 1];
                for (int j = 0; j < l.size; j++) //узел в слое
                {
                    for (int k = 0; k < l.nextSize; k++) //вес для узла
                    {
                        l.weights[j, k] = l.weights[j, k] + learningRate * l.neurons[j] * rightl.biases[k];
                    }
                }
            }
        }
    }
}
