using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Layers;
using DeepLearningFramework.Operators;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Terms = DeepLearningFramework.Operators.Terms;
using DeepLearningFramework.Modules;

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
            //X*W  = (m, mysize)  + B
            //B = (1, mysize)
            Variable W = new Variable(input.OuterDimensions, new Shape(input.InnerDimensions[1], size));
            Variable B = new Variable(input.OuterDimensions, new Shape(1, size));

            Layer res = new Add(new MatrixMultiply(input, W), new Expand(B, new Dimension[] { input.InnerDimensions[0], 1 }));
            res = GetActivationFunction(act)(res);
            return res;
        }

        public static dynamic Dense(int size, string act = "")
        {
            return null;// new DenseModule(size, act);
        }

       
    }

    
}
