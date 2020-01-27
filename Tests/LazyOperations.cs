using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass]
    public unsafe class LazyOperations
    {
        [TestMethod]
        public void TestLazyOperations()
        {
            for (int aa = 0; aa < 1000; aa++)
            {

                Variable v1 = new Variable(new Shape(3, 2));
                    v1.SetValue(new float[3, 2] {
                    { 1, 2 },
                    { 3, 4 },
                    { 5, 6 }
                });

                Variable v2 = new Variable(new Shape(3, 2));
                    v2.SetValue(new float[3, 2] {
                    { 0, 1 },
                    { 2, 3 },
                    { 4, 5 }
                });


                Plus s = new Plus(v1, v2);


                Tensor<float> c = s.GetResult();
                float[] res = new float[6] { 1, 3, 5, 7, 9, 11 };

                for (int i = 0; i < c.Shape.TotalSize; i++)
                    if (((float*)c.Array)[i] != res[i])
                        throw new Exception("farklı");

                s.DeleteResults();
            }
        }

        //[TestMethod]
        //public void TestLazyOperations2()
        //{
        //    Variable v1 = new Variable(new Shape(3, 2));
        //    v1.SetValue(new float[3, 2] {
        //        { 1, 2 },
        //        { 3, 4 },
        //        { 5, 6 }
        //    });


        //    Variable v2 = new Variable(new Shape(2, 2));
        //    v2.SetValue(new float[2, 2] {
        //        { 2, 3 },
        //        { 4, 5 }
        //    });

        //    MatrixMultiply s = new MatrixMultiply(v1, v2);

        //    Matrix c = s.GetResult();
        //    Matrix res = new float[3, 2] {
        //        { 10, 13 },
        //        { 22, 29 },
        //        { 34, 45 }};
        //    Assert.AreEqual(c, res);
        //}

        //[TestMethod]
        //public void TestLazyOperations3()
        //{
        //    Variable v1 = new Variable(new Shape(3, 2));
        //    v1.SetValue(new float[3, 2] {
        //        { 1, 2 },
        //        { 3, 4 },
        //        { 5, 6 }
        //    });


        //    Variable v2 = new Variable(new Shape(2, 2));
        //    v2.SetValue(new float[2, 2] {
        //        { 2, 3 },
        //        { 4, 5 }
        //    });

        //    MatrixMultiply s = new MatrixMultiply(v1, v2);

        //    s.CalculateHowManyTimesUsed();
        //    s.CalculateDerivate(MMDerivative.I(s.D1, s.D2));
        //    s.DeleteResults();

        //    s = new MatrixMultiply(s, v2);

        //    s.CalculateHowManyTimesUsed();
        //    s.CalculateDerivate(MMDerivative.I(s.D1, s.D2));
        //    s.DeleteResults();
        //}


    }
}
