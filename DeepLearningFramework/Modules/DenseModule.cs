using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Layers;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terms = DeepLearningFramework.Operators.Terms;

namespace DeepLearningFramework.Modules
{
    public class DenseModule : Module
    {
        public int prevsize;
        public int size;
        public string activation;
        public Terms.Variable w, b;

        public unsafe DenseModule(int prevsize, int size, string act = "")
        {
            this.activation = act;
            this.size = size;
            this.prevsize = prevsize;
            w = new Terms.Variable(new Shape(prevsize, size));
            b = new Terms.Variable(new Shape(1, size));
            Randomiz.Randomize((float*)w.Weights.Array, w.Shape.TotalSize);
            Randomiz.Randomize((float*)b.Weights.Array, b.Shape.TotalSize);
        }

        public override dynamic Forward(Term[] x)
        {
            Term[] res = new Term[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                dynamic xn = x[i];
                xn = new Terms.Add(new Terms.MatrixMultiply(xn, w), new Terms.Expand(b, new Shape(xn.Shape[0], 1)));
                if (this.activation == "sigmoid")
                    xn = new Terms.Sigmoid(xn);
                else
                    throw new Exception();
                res[i] = xn;
            }
            //todo add activation function
            return res;
        }

        public override dynamic Forward(dynamic x)
        {
            throw new Exception("asda");
        }

    }
}
