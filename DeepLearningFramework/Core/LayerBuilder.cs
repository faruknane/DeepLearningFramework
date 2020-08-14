using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Layers;
using DeepLearningFramework.Operators;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terms = DeepLearningFramework.Operators.Terms;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace DeepLearningFramework.Core
{
    public static class LayerBuilder
    {
        public static Layer SquaredError(Layer x1, Layer x2)
        {
            return new Power(new Subtract(x1, x2), 2);
        }

        public static Func<Layer, Layer> GetActivationFunction(string name)
        {
            name = name.ToLower(CultureInfo.GetCultureInfoByIetfLanguageTag("en"));
            if (name == "sigmoid")
                return (Layer x) => new Sigmoid(x);
            else if (name == "softmax")
                return (Layer x) => new SoftMax(x);
            else if (name == "relu")
                return (Layer x) => new ReLU(x);
            return (Layer x) => x;
        }

        public static Layer Dense(int size, Layer input, string act)
        {
            if (input.InnerDimensions.Length != 2)
                throw new Exception("Inner shape of Input Layer must be 2");

            //Index         0        1
            //Inner shape of the input = (batchsize, mysize)
            //X*W + B
            //X = m*n   (m is batchsize, n is layer size)
            //W =  (n, mysize)
            // X*W  = (m, mysize)  + B
            //B = (1, mysize)
            Variable W = new Variable(input.OuterDimensions, new Shape((input.InnerDimensions[1], size)));
            Variable B = new Variable(input.OuterDimensions, new Shape((1, size)));

            Layer res = new Add(new MatrixMultiply(input, W), new Expand(B, new Dimension[] { input.InnerDimensions[0], 1 }));
            res = GetActivationFunction(act)(res);
            return res;
        }

        public static Combiner Dense(int size, string act = "")
        {
            int prevsize = -999;
            Terms.Variable w = null;
            Terms.Variable b = new Terms.Variable(new Shape((1, size)));

            Layer DenseLayerFunction(Layer x)
            {
                if (x.InnerDimensions.Length != 2)
                    throw new Exception("Inner shape of Input Layer must be 2");

                if (w == null)
                {
                    prevsize = x.InnerDimensions[1];
                    w = new Terms.Variable(new Shape((prevsize, size)));
                }

                if (x.InnerDimensions[1].Value != prevsize)
                    throw new Exception("Prev Size doesn't match with the input layer");

                Variable W = new Variable(x.OuterDimensions, w);
                Variable B = new Variable(x.OuterDimensions, b);
                Layer res = new Add(new MatrixMultiply(x, W), new Expand(B, new Dimension[] { x.InnerDimensions[0], 1 }));
                res = GetActivationFunction(act)(res);
                return res;
            }

            Combiner dense = new Combiner( (Layer x) => DenseLayerFunction(x));
            return dense;
        }
        
        public class Combiner
        {
            public Func<Layer, Layer> f;

            public Combiner(Func<Layer,Layer> f)
            {
                this.f = f;
            }

            public Layer this[Layer x]
            {
                get
                {
                    return f(x);
                }
            }

            public Combiner this[Combiner f]
            {
                get
                {
                    return new Combiner((Layer x) => this[f[x]]);
                }
            }
        }
    }

    
}
