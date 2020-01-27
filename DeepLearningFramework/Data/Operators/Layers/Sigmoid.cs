//using DeepLearningFramework.Core;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;

//namespace DeepLearningFramework.Data.Operators.Layers
//{
//    public class Sigmoid : Layer
//    {
//        public Layer L { get; private set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }
//        public Sigmoid(Layer x)
//        {
//            this.L = x;
//            D1 = x.D1;
//            BatchSize = x.BatchSize;
//            this.SequenceLength = L.SequenceLength;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Terms.Term CreateTerm(int time)
//        {
//            var aa = new Terms.Sigmoid(L.GetTerm(time));
//            return aa;
//        }
//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//            L.DeleteTerms();
//        }
//    }
//}
