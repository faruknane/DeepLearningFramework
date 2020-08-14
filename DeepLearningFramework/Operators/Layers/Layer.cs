
using DeepLearningFramework.Core;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
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
        private bool InRecursion = false; //todo does nothing, you can delete

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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void PreCheck()
        {
            if (InRecursion) return;
            InRecursion = true;

            foreach (var item in InputLayers)
                item.PreCheck();

            PreCheckOperation();

            InRecursion = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual void PreCheckOperation()
        {
            BeforePreCheck();
            InnerDimensionCheck();
            OuterDimensionCheck();
            InnerShapeCalculation();
            OuterShapeCalculation();
            AfterPreCheck();
        }

        public virtual void BeforePreCheck() { }
        public virtual void AfterPreCheck() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual void InnerShapeCalculation()
        {
            //assign inner shape.
            if (InnerShape == null)
                InnerShape = new Shape(this.InnerDimensions.Length);

            unsafe
            {
                for (int j = 0; j < this.InnerDimensions.Length; j++)
                    InnerShape.Dimensions[j] = InnerDimensions[j].Value;
            }
            InnerShape.CalculateMultiplied();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual void OuterShapeCalculation()
        {
            //assign outer shape.
            if (OuterShape == null)
                OuterShape = new Shape(this.OuterDimensions.Length);

            unsafe
            {
                for (int j = 0; j < this.OuterDimensions.Length; j++)
                    OuterShape.Dimensions[j] = this.OuterDimensions[j].Value;
            }
            OuterShape.CalculateMultiplied();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

            Index a = new Index(OuterShape);
            a.SetZero();

            for (int i = 0; i < OuterShape.TotalSize; i++, a.Increase(1))
                GetTerm(a);

            if (Terms.Count > 1)
            {
                Terms.Add min = new Terms.Add(Terms.ToArray());
                min.Minimize();
                min.Dispose();
            }
            else if (Terms.Count == 1)
            {
                Terms[0].Minimize();
            }


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

   

}



