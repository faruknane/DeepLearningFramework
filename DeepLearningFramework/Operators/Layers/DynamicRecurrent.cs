//using DeepLearningFramework.Core;
//using DeepLearningFramework.Operators.Terms;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;

//namespace DeepLearningFramework.Operators.Layers
//{
//    public class DynamicRecurrent : Layer
//    {
//        public Layer[] L { get; internal set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }

//        Func<Layer, Layer[], int, Term> F;

//        public DynamicRecurrent(int size, Dimension sequenceLength, Dimension batchSize, Layer[] l, Func<Layer, Layer[], int, Term> func)
//        {
//            this.L = l;
//            this.SequenceLength = sequenceLength;
//            this.BatchSize = batchSize;
//            this.D1 = size;
//            this.F = func;
//        }


//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(int time)
//        {
//            Term res = F(this, L, time);
//            if (res.D1 != D1 || res.D2 != BatchSize)
//                throw new Exception($"DynamicRecurrent has returned {res.D1} x {res.D2}. Expected: {D1} x {BatchSize}");
//            return res;
//        }

//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//            for (int i = 0; i < L.Length; i++)
//                L[i].DeleteTerms();
//        }
//    }
//}
