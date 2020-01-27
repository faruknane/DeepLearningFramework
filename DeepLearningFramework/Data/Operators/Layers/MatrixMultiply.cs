//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using DeepLearningFramework.Data.Operators.Terms;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Layers
//{
//    public class MatrixMultiply : Layer
//    {
//        public Layer L1 { get; private set; }
//        public Layer L2 { get; private set; }

//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }
//        public MatrixMultiply(Layer x1, Layer x2)
//        {
//            this.L1 = x1;
//            this.L2 = x2;
//            D1 = x1.D1;
//            BatchSize = x2.BatchSize;
//            this.SequenceLength = L1.SequenceLength;
//        }
    
//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(int time)
//        {
//            if (!this.L1.SequenceLength.HardEquals(this.L2.SequenceLength))
//                throw new Exception("Sequence Length should match!");
//            Term aa = new Terms.MatrixMultiply(L1.GetTerm(time), L2.GetTerm(time));
//            return aa;
//        }

//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//            L1.DeleteTerms();
//            L2.DeleteTerms();
//        }
//    }
//}
