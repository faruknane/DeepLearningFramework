using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Operators
{
    public class Power : Term
    {
        Term v1;
        public override Dimension D1 { get; internal set; }
        public override Dimension D2 { get; internal set; }
        public int PowerOf { get; private set; }

        public Power(Term v1, int pow)
        {
            this.v1 = v1;
            this.PowerOf = pow;
            if (PowerOf <= 1)
                throw new Exception("Power cannot less than two!");
            D1 = this.v1.D1;
            D2 = this.v1.D2;
        }

        public override void Derivate(MMDerivative s)
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix res = v1.GetResult();

            Matrix pow = Matrix.CreateCopy(res);//Res ^ 1

            for (int n = 0; n < PowerOf - 2; n++)
                pow.ElementWiseMultiply(res);
            //pow = res^ (powerof - 1)

            //MMDerivative m = new MMDerivative(D1, D2, D1, D2);
            MMDerivative combined = new MMDerivative(s.D1, s.D2, D1, D2);

            for (int x1 = 0; x1 < s.D1; x1++)
                for (int x2 = 0; x2 < s.D2; x2++)
                    for (int i1 = 0; i1 < D1; i1++)
                        for (int i2 = 0; i2 < D2; i2++)
                            combined[x1, x2, i1, i2] = s[x1, x2, i1, i2] * PowerOf * pow[i1, i2];//m[i1, i2, i1, i2] = PowerOf * pow[i1, i2];
            combined.Negative = s.Negative;
            v1.Derivate(combined);
            pow.Dispose();
            combined.Dispose();
        }

        internal override Matrix CalculateResult()
        {
            if (!D1.HardEquals(D1) || !D2.HardEquals(D2))
                throw new Exception("Terms should have an exact value!");

            Matrix res = v1.GetResult();
            Matrix m = Matrix.CreateCopy(res);
            for (int n = 0; n < PowerOf - 1; n++)
                m.ElementWiseMultiply(res);
            return m;
        }

        public override void DeleteResults()
        {
            base.DeleteResults();
            v1.DeleteResults();
        }
    }
}
