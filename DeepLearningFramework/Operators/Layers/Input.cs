//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//
//using DeepLearningFramework.Operators.Layers;
//using DeepLearningFramework.Operators.Terms;
//using PerformanceWork.OptimizedNumerics;
//using DeepLearningFramework.Core;
//using Index = PerformanceWork.OptimizedNumerics.Index;

//namespace DeepLearningFramework.Operators.Layers
//{
//    public class Input : Layer
//    {
//        public Input(int size)
//        {
//            //inner shape, outer shape
//            D1 = size;
//            BatchSize = new Dimension();
//            this.SequenceLength = new Dimension();
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//        public override Term CreateTerm(Index time)
//        {

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
