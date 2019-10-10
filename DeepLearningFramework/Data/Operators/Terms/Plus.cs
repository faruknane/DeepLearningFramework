using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Threading.Tasks;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Terms
{
    public class Plus : Term
    {

        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }

        public Plus(Term v1, Term v2) // make args
        {
            Type = TermType.Plus;
            Terms = new Term[2] { v1, v2 };
            if (!this.Terms[0].D1.SoftEquals(this.Terms[1].D1) || !this.Terms[0].D2.SoftEquals(this.Terms[1].D2))
                throw new Exception("Terms to be sum should have the same dimensions!");
            D1 = this.Terms[0].D1;
            D2 = this.Terms[0].D2;
        }

        public override void CalculateDerivate(MMDerivative s)
        {
            if (!this.Terms[0].D1.HardEquals(this.Terms[1].D1) || !this.Terms[0].D2.HardEquals(this.Terms[1].D2))
                throw new Exception("Terms to be sum should have the same dimensions!");
            for(int i = 0; i < Terms.Length; i++)
                Terms[i].Derivate(s);
        }

        internal override Matrix CalculateResult()
        {
            if (!this.Terms[0].D1.HardEquals(this.Terms[1].D1) || !this.Terms[0].D2.HardEquals(this.Terms[1].D2))
                throw new Exception("Terms to be sum should have the same dimensions!");

            return Terms[0].GetResult() + Terms[1].GetResult();
        }
      
    }
}