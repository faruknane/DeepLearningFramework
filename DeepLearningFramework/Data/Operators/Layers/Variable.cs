using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Data.Operators;
using DeepLearningFramework.Core;
using PerformanceWork.OptimizedNumerics;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public unsafe class Variable : Layer
    {
        public Terms.Variable W { get; private set; }
        public Variable(Shape s, Dimension[] Length, bool setzero = false, bool randomize = true, string RandMethod = "")
        {
            W = new Terms.Variable(s);
            
            if (setzero)
                W.Weights.SetValue(0);
            else if (randomize)
                Randomiz.Randomize((float*)W.Weights.Array, s.TotalSize);

            this.InnerS = s;
            this.InnerShape = new Dimension[s.N];
            for (int i = 0; i < s.N; i++)
                this.InnerShape[i] = s[i];

            this.OuterShape = Length;
        }

        public override void PreCheckOperation()
        {
            if (OuterS == null)
            {
                OuterS = Shape.NewShapeN(this.OuterShape.Length);
            }
            unsafe
            {
                for (int j = 0; j < this.OuterShape.Length; j++)
                    OuterS.Dimensions[j] = OuterShape[j].Value;
            }
            OuterS.CalculateMultiplied();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Terms.Term CreateTerm(Index time)
        {
            return W;
        }

        public override void DeleteTermsOperation()
        {
            W.DeleteResults();
            Terms.Clear();
        }
    }
}
