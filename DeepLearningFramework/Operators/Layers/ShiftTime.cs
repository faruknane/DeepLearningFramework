//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Operators.Layers
//{

//    public class ShiftTime : Layer
//    {
//        public Layer L { get; private set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }
//        public int Shift { get; internal set; }

//        public ShiftTime(Layer l, int shift)
//        {
//            this.L = l;
//            D1 = l.D1;
//            BatchSize = l.BatchSize;
//            this.SequenceLength = L.SequenceLength;
//            this.Shift = shift;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Terms.Term CreateTerm(int time)
//        {
//            return L.GetTerm(time + Shift);
//        }

//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//            L.DeleteTerms();
//        }
//    }
//}
