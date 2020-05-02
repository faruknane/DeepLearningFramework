﻿using System.Collections.Generic;
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
        public Variable(Dimension[] OuterDimensions, Shape s , bool setzero = false, bool randomize = true, string RandMethod = "") //add initializers etc
        {
            W = new Terms.Variable(s.Clone());
            
            if (setzero)
                W.Weights.SetFloat(0);
            else if (randomize)
                Randomiz.Randomize((float*)W.Weights.Array, s.TotalSize);

            this.InnerShape = s;
            this.InnerDimensions = new Dimension[s.N];
            for (int i = 0; i < s.N; i++)
                this.InnerDimensions[i] = s[i];

            this.OuterDimensions = OuterDimensions;
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

        public void Dispose() //experimental
        {
            DeleteTerms();
            
            if(OuterShape != null)
                Shape.Return(OuterShape);
            
            if(InnerShape != null)
                Shape.Return(InnerShape);
            
            if (EmptyVariable != null && !EmptyVariable.IsDisposed)
                EmptyVariable.Clean();
            
            W.Clean();
        }
    }
}
