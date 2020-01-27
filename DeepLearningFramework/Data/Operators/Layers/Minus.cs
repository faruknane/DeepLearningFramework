//using DeepLearningFramework.Data.Operators.Terms;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Layers
//{
//    public class Minus : Layer
//    {
//        public Layer L1 { get; private set; }
//        public Layer L2 { get; private set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }
//        public Minus(Layer l1, Layer l2)
//        {
//            this.L1 = l1;
//            this.L2 = l2;
//            D1 = l1.D1;
//            BatchSize = l1.BatchSize;
//            this.SequenceLength = l1.SequenceLength;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(int time)
//        {
//            if (!this.L1.SequenceLength.HardEquals(this.L2.SequenceLength))
//                throw new Exception("Sequence Length should match!");
//            return new Terms.Minus(L1.GetTerm(time), L2.GetTerm(time));
//        }

//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//            L1.DeleteTerms();
//            L2.DeleteTerms();
//        }
//    }
//}
