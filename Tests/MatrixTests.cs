using DeepLearningFramework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceWork.OptimizedNumerics;

namespace Tests
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        public void TestOperations()
        {
            Matrix a;
            Matrix b;
            Matrix c;
            Matrix res;

            a = 5;
            b = 6;
            c = a + b;
            res = 11;
            Assert.AreEqual(c, res);


            a = 5;
            b = 6;
            c = a - b;
            res = -1;
            Assert.AreEqual(c, res);
        }
       
        
    }
}
