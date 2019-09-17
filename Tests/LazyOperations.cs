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
    public class LazyOperations
    {
        [TestMethod]
        public void TestLazyOperations()
        {
            Variable v1 = new Variable(3, 2)
            {
                Weights = new float[3, 2] {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }}
            };

            Variable v2 = new Variable(3, 2)
            {
                Weights = new float[3, 2] {
                { 0, 1 },
                { 2, 3 },
                { 4, 5 }}
            };

            Plus s = new Plus(v1, v2);

            Matrix c = s.GetResult();
            Matrix res = new float[3, 2] {
                { 1, 3 },
                { 5, 7 },
                { 9, 11 }};
            Assert.AreEqual(c, res);
        }

        [TestMethod]
        public void TestLazyOperations2()
        {
            Variable v1 = new Variable(3, 2)
            {
                Weights = new float[3, 2] {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }}
            };

            Variable v2 = new Variable(2, 2)
            {
                Weights = new float[2, 2] {
                { 2, 3 },
                { 4, 5 }}
            };

            MatrixMultiply s = new MatrixMultiply(v1, v2);

            Matrix c = s.GetResult();
            Matrix res = new float[3, 2] {
                { 10, 13 },
                { 22, 29 },
                { 34, 45 }};
            Assert.AreEqual(c, res);
        }

        [TestMethod]
        public void TestLazyOperations3()
        {
            Variable v1 = new Variable(3, 2)
            {
                Weights = new float[3, 2] {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }}
            };

            Variable v2 = new Variable(2, 2)
            {
                Weights = new float[2, 2] {
                { 2, 3 },
                { 4, 5 }}
            };

            MatrixMultiply s = new MatrixMultiply(v1, v2);

            s.CalculateHowManyTimesUsed();
            s.CalculateDerivate(MMDerivative.I(s.D1, s.D2));
            s.DeleteResults();

            s = new MatrixMultiply(s, v2);

            s.CalculateHowManyTimesUsed();
            s.CalculateDerivate(MMDerivative.I(s.D1, s.D2));
            s.DeleteResults();
        }

       
    }
}
