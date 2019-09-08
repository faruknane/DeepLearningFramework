using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Data
{
    public class MMDerivative
    {
        public int D1 { get; set; }
        public int D2 { get; set; }
        public int D3 { get; set; }
        public int D4 { get; set; }
        public bool Negative { get; set; } = false;

        public float[] Derivatives;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MMDerivative(int d1, int d2, int d3, int d4)
        {
            D1 = d1;
            D2 = d2;
            D3 = d3;
            D4 = d4;
            Derivatives = Matrix.Pool.Rent(d1 * d2 * d3 * d4);
            Vectorization.ElementWiseSetValueAVX(Derivatives, 0, d1 * d2 * d3 * d4);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Matrix.Pool.Return(Derivatives);
        }
        public float this[int x1, int x2, int x3, int x4]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Derivatives[x1 * D2 * D3 * D4 + x2 * D3 * D4 + x3 * D4 + x4];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Derivatives[x1 * D2 * D3 * D4 + x2 * D3 * D4 + x3 * D4 + x4] = value;
            }
        }

        public static MMDerivative I(int d1, int d2)
        {
            MMDerivative res = new MMDerivative(d1, d2, d1, d2);
            for (int i = 0; i < d1; i++)
                for (int i2 = 0; i2 < d2; i2++)
                    res[i, i2, i, i2] = 1;
            return res;
        }

        public static MMDerivative I(Dimension d1, Dimension d2)
        {
            MMDerivative res = new MMDerivative(d1, d2, d1, d2);
            for (int i = 0; i < d1; i++)
                for (int i2 = 0; i2 < d2; i2++)
                    res[i, i2, i, i2] = 1;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MultiplyBy(float x)
        {
            Vectorization.ElementWiseMultiplyAVX(this.Derivatives, x, this.Derivatives, D1 * D2 * D3 * D4);
        }

        internal void Add(MMDerivative m)
        {
            Vectorization.ElementWiseAddAVX(this.Derivatives, m.Derivatives, this.Derivatives, D1 * D2 * D3 * D4);
        }

        public static MMDerivative Clone(MMDerivative m)
        {
            MMDerivative n = new MMDerivative(m.D1, m.D2, m.D3, m.D4);
            n.Negative = m.Negative;
            n.Derivatives = Matrix.Pool.Rent(m.D1 * m.D2 * m.D3 * m.D4);
            Vectorization.ElementWiseAssignAVX(n.Derivatives, m.Derivatives, m.D1 * m.D2 * m.D3 * m.D4);
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DivideBy(float x)
        {
            Vectorization.ElementWiseMultiplyAVX(this.Derivatives, 1/x, this.Derivatives, D1 * D2 * D3 * D4);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public MMDerivative CombineWith(MMDerivative m)
        //{
        //    if (D3 != m.D1 || D4 != m.D2)
        //        throw new Exception("The last two dimensions and The first two dimensions should match!");


        //    //1) Care for negative

        //    MMDerivative res = new MMDerivative(D1, D2, m.D3, m.D4);

        //    bool turn = m.Negative != this.Negative;
        //    if (turn)
        //    {
        //        for (int i1 = 0; i1 < D1; i1++)
        //            for (int i2 = 0; i2 < D2; i2++)
        //                for (int i3 = 0; i3 < m.D3; i3++)
        //                    for (int i4 = 0; i4 < m.D4; i4++)
        //                        for (int x1 = 0; x1 < D3; x1++)
        //                            for (int x2 = 0; x2 < D4; x2++)
        //                                res[i1, i2, i3, i4] -= this[i1, i2, x1, x2] * m[x1, x2, i3, i4];
        //    }
        //    else
        //    {
        //        for (int i1 = 0; i1 < D1; i1++)
        //            for (int i2 = 0; i2 < D2; i2++)
        //                for (int i3 = 0; i3 < m.D3; i3++)
        //                    for (int i4 = 0; i4 < m.D4; i4++)
        //                        for (int x1 = 0; x1 < D3; x1++)
        //                            for (int x2 = 0; x2 < D4; x2++)
        //                                res[i1, i2, i3, i4] += this[i1, i2, x1, x2] * m[x1, x2, i3, i4];
        //    }
        //    return res;
        //}

    }
}
