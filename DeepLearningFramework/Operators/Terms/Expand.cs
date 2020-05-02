
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DeepLearningFramework.Core;
using Index = PerformanceWork.OptimizedNumerics.Index;
using System.ComponentModel.DataAnnotations;
using PerformanceWork;
using DataType = PerformanceWork.DataType;
using PerformanceWork.DeepLearning.Kernels.Cpu;

namespace DeepLearningFramework.Operators.Terms
{
    public class Expand : Term
    {
        public Shape Multiplier { get; set; }

        /// <summary>
        /// Shape object wont be returned to the pool back. 
        /// </summary>
        public Expand(Term v1, Shape multiplier)
        {
            Type = TermType.ExpandWithSame;
            Terms = new Term[1] { v1 };
            Multiplier = multiplier;
            Shape = Shape.Multiply(Terms[0].Shape, Multiplier);
        }

        public override unsafe void CalculateDerivate(Tensor s)
        {
            Tensor combined = CpuKernels.ExpandFloat_GetGradient_0(s, this.Shape, Terms[0].Shape, Multiplier);
            Terms[0].Derivate(combined);
            combined.Dispose();
        }

        public unsafe override Tensor CalculateResult()
        {
            Tensor v = Terms[0].GetResult();
            Tensor res = CpuKernels.ExpandFloat(v, this.Shape, Terms[0].Shape, Multiplier);
            return res;
        }

        public override void Dispose()
        {
            //Shape.Return(Multiplier);
            base.Dispose();
        }

    }
}
