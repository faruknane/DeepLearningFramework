using DeepLearningFramework.Operators.Layers;
using DeepLearningFramework.Operators.Terms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceWork;
using PerformanceWork.OptimizedNumerics;
using System;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace Tests
{
    [TestClass]
    public class TermTests
    {

        [TestMethod]
        public unsafe void MultiplyTerm()
        {
            Input x = new Input(4, 2, 1);
            Tensor data = new Tensor((10, 3, 4), DataType.Type.Float, DeviceIndicator.Host());

            for (int i = 0; i < data.Shape.TotalSize; i++)
                ((float*)data.Array)[i] = i / 12;
            x.SetInput(data);

            Index a = new Index(x.OuterShape);
            a.SetZero();


            for (int i = 0; i < x.OuterShape.TotalSize; i++, a.Increase(1))
            {
                Term t = x.GetTerm(a);
                Console.WriteLine("Term " + i + ": " + x.GetTerm(a).GetResult());
                Term mul = new Multiply(t, t);
                Console.WriteLine("Term " + i + ": " + mul.GetResult());
                mul.Dispose();
                //todo check the result.
            }
        }
    }
}
