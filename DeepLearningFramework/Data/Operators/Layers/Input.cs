//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using DeepLearningFramework.Data;
//using DeepLearningFramework.Data.Operators.Layers;
//using DeepLearningFramework.Data.Operators.Terms;
//using PerformanceWork.OptimizedNumerics;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Layers
//{
//    public class Input : Layer
//    {
//        public override Dimension D1 { get; internal set; }
//        public override Dimension BatchSize { get; internal set; }

//        public Input(int size)
//        {
//            D1 = size;
//            BatchSize = new Dimension();
//            this.SequenceLength = new Dimension();
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(int time)
//        {
//            return new PlaceHolder(D1);
//        }


//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public void SetInput(int time, Matrix inp)
//        {
//            PlaceHolder h = (PlaceHolder)GetTerm(time);
//            h.SetVariable(new Terms.Variable(inp) { Name = "Input", Trainable = false });
//            BatchSize.Value = h.D2;
//        }
//        public void SetSequenceLength(int l)
//        {
//            this.SequenceLength.Value = l;
//        }

//        public override void DeleteTerms()
//        {

//        }
//    }
//}
