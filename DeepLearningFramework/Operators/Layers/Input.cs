using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using DeepLearningFramework.Operators.Layers;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using DeepLearningFramework.Core;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class Input : Layer
    {
        Tensor InputData;
        public int Size;
        public bool Trainable;

        public Input(int size, int innerdimension = 2, int outerdimension = 1, bool trainable = false)
        {
            Trainable = trainable;
            Size = size;
            InnerDimensions = new Dimension[innerdimension];
            OuterDimensions = new Dimension[outerdimension];

            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override void InnerDimensionCalculation()
        {
            for (int i = 0; i < InnerDimensions.Length - 1; i++)
                InnerDimensions[i] = new Dimension();
            InnerDimensions[InnerDimensions.Length - 1] = Size;
        }

        public override void OuterDimensionCalculation()
        {
            for (int i = 0; i < OuterDimensions.Length; i++)
                OuterDimensions[i] = new Dimension();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            int outerindex = this.OuterShape.Index(time);
            int begin = outerindex * this.InnerShape.TotalSize;
            int end = begin + this.InnerShape.TotalSize;

            //dont create the clone of innershape because these Weight Tensors of Variable Terms wont be diposed because arrayreturned is set true.
            return new Terms.Variable(Tensor.Cut(InputData, begin, end, this.InnerShape)) { Trainable = Trainable };
        }

        /// <summary>
        /// <br>the new data will be assigned.</br>
        /// </summary>
        /// 
        /// 
        /// <code>
        /// var x = new Input(size);
        /// Tensor&lt;float&gt; data = new Tensor&lt;float&gt;((.....));
        /// x.SetInput(data);
        /// </code>
        /// <param name="inp">The new input data</param>

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void SetInput(Tensor inp)
        {
            if (inp.Shape.N != InnerDimensions.Length + OuterDimensions.Length)
                throw new Exception("dimension incompatibility");

            this.DeleteTermsOperation();

            if (InputData != null && !InputData.ArrayReturned) 
                InputData.Dispose();

            InputData = inp;

            int i2 = 0;
            for (int i = 0; i < OuterDimensions.Length; i++)
                OuterDimensions[i].Value = InputData.Shape[i2++];

            for (int i = 0; i < InnerDimensions.Length; i++)
                InnerDimensions[i].Value = InputData.Shape[i2++]; //make this function in shape class?

            if (InnerDimensions[InnerDimensions.Length - 1].Value != Size)
                throw new Exception("Size!");

            PreCheck();
        }

        public override void DeleteTermsOperation()
        {
            foreach (var item in Terms)
            {
                if(item != null)
                {
                    Terms.Variable x = item as Terms.Variable;
                    x.Clean();
                }

            }
            Terms.Clear();
        }


    }
}
