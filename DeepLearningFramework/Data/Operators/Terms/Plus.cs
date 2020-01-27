using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Threading.Tasks;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Plus : Term
    {
        public Plus(params Term[] v) // make args
        {
            if (v.Length < 2)
                throw new Exception("length < 2!");
            Type = TermType.Plus;
            Terms = v;
            for (int i = 0; i < Terms.Length - 1; i++)
                if (!this.Terms[i].Shape.EqualShape(this.Terms[i + 1].Shape)) //will be shape, not d1 or d2
                {
                    throw new Exception("Terms to be sum should have the same dimensions!");
                }
            this.Shape = v[0].Shape.Clone();
        }

        public override void CalculateDerivate(Tensor<float> s)
        {
            for (int i = 0; i < Terms.Length; i++)
                Terms[i].Derivate(s);
        }

        public unsafe override Tensor<float> CalculateResult()
        {
            Tensor<float> res = new Tensor<float>(this.Shape.Clone());
            Vectorization.ElementWiseAddAVX((float*)Terms[0].GetResult().Array, (float*)Terms[1].GetResult().Array, (float*)res.Array, this.Shape.TotalSize);

            for (int i = 2; i < Terms.Length; i++) //Optimize here. 
                Vectorization.ElementWiseAddAVX((float*)res.Array, (float*)Terms[i].GetResult().Array, (float*)res.Array, this.Shape.TotalSize);

            return res;
        }

    }
}