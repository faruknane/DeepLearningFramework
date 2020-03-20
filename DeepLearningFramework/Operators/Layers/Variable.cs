using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Operators;
using DeepLearningFramework.Core;
using PerformanceWork.OptimizedNumerics;

namespace DeepLearningFramework.Operators.Layers
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

            this.InnerShape = s;
            this.InnerDimensions = new Dimension[s.N];
            for (int i = 0; i < s.N; i++)
                this.InnerDimensions[i] = s[i];

            this.OuterDimensions = Length;
        }

        public override void InnerDimensionCheck()
        {

        }
        public override void OuterDimensionCheck()
        {

        }
        public override void InnerShapeCalculation()
        {

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
