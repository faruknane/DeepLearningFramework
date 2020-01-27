//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using DeepLearningFramework.Data.Operators.Terms;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Layers
//{
//    public class ShrinkSizeToOneByAdding : Layer
//    {
//        public Layer L { get; private set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }
//        public ShrinkSizeToOneByAdding(Layer x)
//        {
//            this.L = x;
//            D1 = 1;
//            BatchSize = 1;
//            this.SequenceLength = L.SequenceLength;
//        }

//        public override Term CreateTerm(int time)
//        {
//            Term t = L.GetTerm(time);
//            return new Terms.ShrinkByAdding(t, t.D1, t.D2);
//        }

//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//            L.DeleteTerms();
//        }

//    }
//    public class SumSequenceToOneByAdding : Layer
//    {
//        public Layer L { get; private set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }
//        public SumSequenceToOneByAdding(Layer x)
//        {
//            this.L = x;
//            D1 = x.D1;
//            BatchSize = x.BatchSize;
//            this.SequenceLength = 1;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(int time)
//        {
//            Term t = null;
//            for (int i = 0; i < L.SequenceLength; i++)
//            {
//                if (t == null)
//                    t = L.GetTerm(i);
//                else
//                    t = new Terms.Plus(t, L.GetTerm(i));
//            }
//            return t;
//        }
//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//            L.DeleteTerms();
//        }
//    }

//    public class SumSequenceByAdding : Layer
//    {
//        public Layer L { get; private set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }
//        public SumSequenceByAdding(Layer x)
//        {
//            this.L = x;
//            D1 = x.D1;
//            BatchSize = x.BatchSize;
//            this.SequenceLength = this.L.SequenceLength;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(int time)
//        {
//            Term t = L.GetTerm(time);
//            if (time > 0)
//                t = new Terms.Plus(L.GetTerm(time - 1), t);
//            return t;
//        }
//        public override void DeleteTerms()
//        {
//            base.DeleteTerms();
//            L.DeleteTerms();
//        }
//    }
//}
