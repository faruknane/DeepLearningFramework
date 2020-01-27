//using DeepLearningFramework.Data.Operators.Terms;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Layers
//{
//    public class StaticRecurrent : Layer
//    {
//        public Layer[] L { get; internal set; }
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }

//        public Func<Layer, Layer[], Layer> F;
//        public Layer past;
//        public Layer res;

//        public StaticRecurrent(int size, Dimension sequenceLength, Dimension batchSize, Layer[] l, Func<Layer, Layer[], Layer> func)
//        {
//            Initialize(size, sequenceLength, batchSize, l, func);
//        }

//        public StaticRecurrent(int size, Layer[] l, Func<Layer, Layer[], Layer> func)
//        {
//            Initialize(size, l[0].SequenceLength, l[0].BatchSize, l, func);
//        }

//        void Initialize(int size, Dimension sequenceLength, Dimension batchSize, Layer[] l, Func<Layer, Layer[], Layer> func)
//        {
//            this.L = l;
//            D1 = size;
//            this.SequenceLength = sequenceLength;
//            this.BatchSize = batchSize;
//            this.past = new ShiftTime(this, -1);
//            F = func;
//            res = F(past, L);
//            BatchSize = res.BatchSize;
//            if (!res.D1.HardEquals(D1))
//                throw new Exception("Dimensions should match!");
//        }


//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(int time)
//        {
//            if (!this.SequenceLength.HardEquals(res.SequenceLength) || !this.BatchSize.HardEquals(res.BatchSize))
//                throw new Exception("SequenceLength or BatchSize determined should match with the resulting layer!");
//            return res.GetTerm(time);
//        }

//        public override void DeleteTerms()
//        {
//            if(Terms.Count != 0)
//            {
//                base.DeleteTerms();
//                res.DeleteTerms();
//            }
//            //past, res has already deleted their Terms.
//        }
//    }
//}
