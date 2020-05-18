
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;
using DeepLearningFramework.Core;
using PerformanceWork;
using PerformanceWork.DeepLearning.Kernels.Cpu;
using System.Runtime.CompilerServices;

namespace DeepLearningFramework.Operators.Terms
{
    public class Power : Term
    {
        public int PowerOf { get; private set; }

        public Power(Term v1, int pow)
        {
            Type = TermType.Power;
            Terms = new Term[1] { v1 };

            this.PowerOf = pow;

            if (PowerOf <= 1)
                throw new Exception("Power cannot be less than two!");

            Shape = v1.Shape.Clone();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override unsafe void CalculateDerivate(Tensor s)
        {
            if (Terms[0].ContainsTrainable)
            {
                Tensor res = Terms[0].GetResult();

                if (PowerOf == 2)//todo move this power check to the power kernel
                {
                    Tensor combined = CpuKernels.Power2Float_GetGradient_0(s, res);
                    Terms[0].Derivate(combined);
                    combined.Dispose();
                }
                else
                {
                    throw new Exception("Unsupported Power factor!");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public unsafe override Tensor CalculateResult()
        {
            Tensor res = Terms[0].GetResult();
            if (PowerOf == 2)
            {
                return CpuKernels.Power2Float(res);
            }
            else
            {
                throw new Exception("Unsupported Power factor!");
            }
        }

    }
}
