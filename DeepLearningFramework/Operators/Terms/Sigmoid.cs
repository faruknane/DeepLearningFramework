
using PerformanceWork.OptimizedNumerics;
using DeepLearningFramework.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DeepLearningFramework.Operators.Terms
{
    public class Sigmoid : Term
    {
        public Sigmoid(Term v1)
        {
            Type = TermType.Sigmoid;
            Terms = new Term[1] { v1 };
            this.Shape = v1.Shape.Clone();
        }

        public override unsafe void CalculateDerivate(Tensor s)
        {
            Tensor sigmo = GetResult();

            Tensor combined = new Tensor(s.Shape.Clone(), Data.Type.Float, DeviceIndicator.Host());

            //m[i1, i2, i1, i2] = sigmo[i1, i2] * (1 - sigmo[i1, i2]);
            
            VectorizationFloat.ElementWise_A_MultipliedBy_1_Minus_A_MultipliedByB((float*)sigmo.Array, (float*)s.Array, (float*)combined.Array, sigmo.Shape.TotalSize);
               
            Terms[0].Derivate(combined);
            combined.Dispose();
        }


        public override unsafe Tensor CalculateResult()
        {
            Tensor sigmo = new Tensor(this.Shape.Clone(), Data.Type.Float, DeviceIndicator.Host());
            Tensor v = Terms[0].GetResult();
            VectorizationFloat.Sigmoid((float*)v.Array, (float*)sigmo.Array, sigmo.Shape.TotalSize);
            return sigmo;
        }
    }
}
