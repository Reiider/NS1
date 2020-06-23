using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS1
{
    [Serializable]
    class Layer
    {
        public int size;
        public int nextSize;
        public double[] neurons; //значения нейронов
        public double[] biases;
        public double[,] weights; //значения весов

        public Layer(int size, int nextSize) {
            this.size = size;
            this.nextSize = nextSize;
            neurons = new double[size];
            biases = new double[size];
            weights = new double[size, nextSize];
        }
    }
}
