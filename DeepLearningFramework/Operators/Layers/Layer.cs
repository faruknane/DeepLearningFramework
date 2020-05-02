﻿
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Operators.Layers
{
    public abstract partial class Layer
    {
        public string Name { get; set; }
        public virtual Dimension[] OuterDimensions { get; internal set; }
        public virtual Dimension[] InnerDimensions { get; internal set; }
        public Shape OuterShape { get; set; }
        public Shape InnerShape { get; set; }


        public List<Term> Terms = new List<Term>();
        public List<Layer> InputLayers = new List<Layer>();
        internal Terms.Variable EmptyVariable;
        private bool InRecursion = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual unsafe Term GetTerm(Index time)
        {
            if (time.N != OuterShape.N)
                throw new Exception("");

            int index = 0;
            int mult = 1;

            for (int i = time.N - 1; i >= 0; i--)
            {
                index += time[i] * mult;
                mult *= OuterShape[i];
                if (time[i] < 0 || time[i] >= OuterShape[i])
                {

                    if (EmptyVariable == null)
                    {
                        EmptyVariable = new Terms.Variable(InnerShape.Clone()) { Trainable = false };
                        EmptyVariable.Weights.SetFloat(0);
                    }
                    else if (!EmptyVariable.Shape.EqualShape(InnerShape))
                    {
                        EmptyVariable.Clean();
                        EmptyVariable = new Terms.Variable(InnerShape.Clone()) { Trainable = false };
                        EmptyVariable.Weights.SetFloat(0);
                    }

                    return EmptyVariable;
                }
            }

            while (Terms.Count <= index)
                Terms.Add(null);

            if (Terms[index] == null)
                return Terms[index] = CreateTerm(time); 

            return Terms[index];
        }

        public abstract Term CreateTerm(Index time);
        public void PreCheck()
        {
            if (InRecursion) return;
            InRecursion = true;

            foreach (var item in InputLayers)
                item.PreCheck();

            PreCheckOperation();

            InRecursion = false;
        }

        public virtual void PreCheckOperation()
        {
            InnerDimensionCheck();
            OuterDimensionCheck();
            InnerShapeCalculation();
            OuterShapeCalculation();
        }

        public virtual void OuterDimensionCheck()
        {
            //default check for outer dimensions
            for (int i = 0; i < InputLayers.Count - 1; i++)
            {
                var item = InputLayers[i];
                var item2 = InputLayers[i + 1];

                if (item.OuterDimensions.Length != item2.OuterDimensions.Length)
                    throw new Exception("Outer Shape incompatilbiity!");

                for (int j = 0; j < item.OuterDimensions.Length; j++)
                {
                    int val = item.OuterDimensions[j].Value;

                    if (val <= 0 || val != item2.OuterDimensions[j].Value)
                        throw new Exception("Outer Shape incompatilbiity!");
                }
            }
        }

        public virtual void InnerDimensionCheck()
        {
            //default check for inner dimensions
            for (int i = 0; i < InputLayers.Count - 1; i++)
            {
                var item = InputLayers[i];
                var item2 = InputLayers[i + 1];

                if (item.InnerDimensions.Length != item2.InnerDimensions.Length)
                    throw new Exception("Inner Shape incompatilbiity!");

                for (int j = 0; j < item.InnerDimensions.Length; j++)
                {
                    int val = item.InnerDimensions[j].Value;

                    if (val <= 0 || val != item2.InnerDimensions[j].Value)
                        throw new Exception("Inner Shape incompatilbiity!");
                }
            }
        }

        public virtual void InnerShapeCalculation()
        {
            //assign inner shape.
            if (InnerShape == null)
                InnerShape = Shape.NewShapeN(this.InnerDimensions.Length);

            unsafe
            {
                for (int j = 0; j < this.InnerDimensions.Length; j++)
                    InnerShape.Dimensions[j] = InnerDimensions[j].Value;
            }
            InnerShape.CalculateMultiplied();
        }

        public virtual void OuterShapeCalculation()
        {
            //assign outer shape.
            if (OuterShape == null)
                OuterShape = Shape.NewShapeN(this.OuterDimensions.Length);

            unsafe
            {
                for (int j = 0; j < this.OuterDimensions.Length; j++)
                    OuterShape.Dimensions[j] = this.OuterDimensions[j].Value;
            }
            OuterShape.CalculateMultiplied();
        }
        public virtual void InnerDimensionCalculation()
        {
            //assign inner dimensions
            if (InputLayers.Count > 0)
            {
                if (InnerDimensions != null)
                    throw new Exception("Outer Dimensions are already defined!");

                InnerDimensions = new Dimension[InputLayers[0].InnerDimensions.Length];

                for (int j = 0; j < this.InnerDimensions.Length; j++)
                    this.InnerDimensions[j] = InputLayers[0].InnerDimensions[j];
            }
        }

        public virtual void OuterDimensionCalculation()
        {
            //assign outer dimensions
            if (InputLayers.Count > 0)
            {
                if (OuterDimensions != null)
                    throw new Exception("Outer Dimensions are already defined!");

                OuterDimensions = new Dimension[InputLayers[0].OuterDimensions.Length];

                for (int j = 0; j < this.OuterDimensions.Length; j++)
                    this.OuterDimensions[j] = InputLayers[0].OuterDimensions[j];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void DeleteTerms()
        {
            if (InRecursion) return;
            InRecursion = true;

            DeleteTermsOperation();

            foreach (var item in InputLayers)
                item.DeleteTerms();

            InRecursion = false;
        }

        public virtual void DeleteTermsOperation()
        {
            for (int i = 0; i < Terms.Count; i++)
                if (Terms[i] != null)
                {
                    Terms[i].DeleteResults();
                    //if(Terms[i].Type != TermType.Variable)
                        Terms[i].Dispose(); //term.disposed doesnt work for Terms.Variable
                }
            Terms.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual unsafe void Minimize()
        {
            DeleteTerms();

            PreCheck();

            Index a = Index.NewIndex(OuterShape);
            a.SetZero();

            for (int i = 0; i < OuterShape.TotalSize; i++, a.Add(1))
                GetTerm(a);

            if(Terms.Count > 1)
            {
                Terms.Add min = new Terms.Add(Terms.ToArray());
                min.Minimize();
                min.Dispose();
            }
            else if(Terms.Count == 1)
            {
                Terms[0].Minimize();
            }
            

            Index.Return(a);
            DeleteTerms();
        }

        #region Operators * + - / 

        //public static Layer operator +(Layer x, Layer y)
        //{
        //    return new Plus(x, y);
        //}

        //public static Layer operator *(Layer x, Layer y)
        //{
        //    return new MatrixMultiply(x, y);
        //}

        ////public static Layer operator *(Layer x, float y)
        ////{
        ////    return new MultiplyByNumber(x, y); //add it's layer version !
        ////}

        //public static Layer operator -(Layer x, Layer y)
        //{
        //    return new Minus(x, y);
        //}

        #endregion
    }

    public partial class Layer
    {
        public static Layer SquaredError(Layer x1, Layer x2)
        {
            return new Power(new Subtract(x1, x2), 2);
        }

        public static Func<Layer, Layer> GetActivationFunction(string name)
        {
            name = name.ToLower(CultureInfo.GetCultureInfoByIetfLanguageTag("en"));
            if (name == "sigmoid")
                return (Layer x) => new Sigmoid(x);
            else if (name == "softmax")
                return (Layer x) => new SoftMax(x);
            else if (name == "relu")
                return (Layer x) => new ReLU(x);
            return (Layer x) => x;
        }

        public static Layer Dense(int size, Layer input, string act)
        {
            if (input.InnerDimensions.Length != 2)
                throw new Exception("Inner shape of Input Layer must be 2");

            //X*W + B
            //X = m*n   (m is batchsize, n is layer size)
            //W =  (n, mysize)
            // X*W  = (m, mysize)  + B
            //B = (1, mysize)
            Variable W = new Variable(input.OuterDimensions, Shape.NewShape(input.InnerDimensions[1], size));
            Variable B = new Variable(input.OuterDimensions, Shape.NewShape(1, size));

            Layer res = new Add(new MatrixMultiply(input, W), new Expand(B, new Dimension[] { input.InnerDimensions[0], 1  }));
            res = GetActivationFunction(act)(res);
            return res;
        }

    }

}

//Index         0        1
//Input = (batchsize, mysize)


