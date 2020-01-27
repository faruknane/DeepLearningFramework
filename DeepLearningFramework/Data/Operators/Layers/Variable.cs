//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using DeepLearningFramework.Data.Operators.Terms;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Layers
//{
//    public unsafe class Variable : Layer
//    {
//        public Terms.Variable W { get; private set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }
//        public Variable(int d1, int d2, Dimension Length, bool setzero = false, bool randomize=true, string RandMethod="")
//        {
//            W = new Terms.Variable(d1, d2);
//            if (setzero)
//                W.Weights.SetZero();
//            else if(randomize)
//                Randomiz.Randomize(W.Weights.Array, d1 * d2);
//            D1 = W.D1;
//            BatchSize = W.D2;
//            this.SequenceLength = Length;
//        }


//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(int time)
//        {
//            return W;
//        }

//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//        }
//    }
//}
