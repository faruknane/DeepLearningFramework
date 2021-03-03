using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Runtime.CompilerServices;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public class Input : Layer
    {
        Tensor InputData;
        public int[] Size;
        public bool Trainable;

        public Input(int size, int innerdimension = 2, int outerdimension = 1, bool trainable = false)
        {
            Trainable = trainable;
            Size = new int[1] { size };
            InnerDimensions = new Dimension[innerdimension];
            OuterDimensions = new Dimension[outerdimension];

            InnerDimensionCalculation();
            OuterDimensionCalculation();
        }

        public Input(int[] size, int innerdimension = 2, int outerdimension = 1, bool trainable = false)
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
            for (int i = 0; i < InnerDimensions.Length - Size.Length; i++)
                InnerDimensions[i] = new Dimension();
            for (int i = InnerDimensions.Length - Size.Length; i < InnerDimensions.Length; i++)
                InnerDimensions[i] = Size[i - (InnerDimensions.Length - Size.Length)];
        }

        public override void OuterDimensionCalculation()
        {
            for (int i = 0; i < OuterDimensions.Length; i++)
                OuterDimensions[i] = new Dimension();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            int outerindex = this.OuterShape.Index(time.Indices);
            int begin = outerindex * this.InnerShape.TotalSize;
            int end = begin + this.InnerShape.TotalSize;

            //dont create the clone of innershape because these Weight Tensors of Variable Terms wont be diposed because arrayreturned is set true.
            return new Terms.Variable(Tensor.Cut(InputData, begin, this.InnerShape.Clone())) { Trainable = Trainable };
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

            for (int i = InnerDimensions.Length - Size.Length; i < InnerDimensions.Length; i++)
                if(InnerDimensions[i].Value != Size[i - (InnerDimensions.Length - Size.Length)])
                    throw new Exception($"Size Expected {Size[i - (InnerDimensions.Length - Size.Length)]}, found {InnerDimensions[i].Value}!");

            PreCheck();
        }

        public override void DeleteTermsOperation()
        {
            foreach (var item in Terms)
            {
                if (item != null)
                {
                    Terms.Variable x = item as Terms.Variable;
                    x.Clean();
                }

            }
            Terms.Clear();
        }


    }
}
