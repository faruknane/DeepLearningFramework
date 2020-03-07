using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;
using Index = PerformanceWork.OptimizedNumerics.Index;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public abstract partial class Layer
    {
        public string Name { get; set; }
        public virtual Dimension[] OuterShape { get; internal set; }
        public virtual Dimension[] InnerShape { get; internal set; }
        internal Shape OuterS { get; set; }
        internal Shape InnerS { get; set; }


        public List<Term> Terms = new List<Term>();
        public List<Layer> InputLayers = new List<Layer>();

        private Terms.Variable EmptyVariable;
        private bool InRecursion = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual unsafe Term GetTerm(Index time)
        {
            if (time.N != OuterS.N)
                throw new Exception("");

            int index = 0;
            int mult = 1;

            for (int i = time.N - 1; i >= 0; i--)
            {
                index += time[i] * mult;
                mult *= OuterS[i];
                if (time[i] < 0 || time[i] >= OuterS[i])
                {

                    if (EmptyVariable == null)
                    {
                        EmptyVariable = new Terms.Variable(InnerS.Clone()) { Trainable = false };
                        EmptyVariable.Weights.SetValue(0);
                    }
                    else if (!EmptyVariable.Shape.EqualShape(InnerS))
                    {
                        EmptyVariable.Clean();
                        EmptyVariable.Dispose();
                        EmptyVariable = new Terms.Variable(InnerS.Clone()) { Trainable = false };
                        EmptyVariable.Weights.SetValue(0);
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
        
        public virtual void InnerShapeCalculation()
        {
            //check for inner and outer shape.
            for (int i = 0; i < InputLayers.Count - 1; i++)
            {
                var item = InputLayers[i];
                var item2 = InputLayers[i + 1];

                if (item.InnerShape.Length != item2.InnerShape.Length)
                    throw new Exception("Inner Shape incompatilbiity!");

                for (int j = 0; j < item.InnerShape.Length; j++)
                {
                    int val = item.InnerShape[i].Value;

                    if (val <= 0 || val != item2.InnerShape[i].Value)
                        throw new Exception("Inner Shape incompatilbiity!");
                }
            }

            if (InputLayers.Count > 0)
            {
                if (InnerS == null)
                {
                    InnerS = Shape.NewShapeN(this.InnerShape.Length);
                }

                unsafe
                {
                    for (int j = 0; j < this.InnerShape.Length; j++)
                    {
                        this.InnerShape[j] = InputLayers[0].InnerShape[j];
                        InnerS.Dimensions[j] = InputLayers[0].InnerShape[j].Value;
                    }
                }

                InnerS.CalculateMultiplied();
            }
        }

        public virtual void OuterShapeCalculation()
        {
            for (int i = 0; i < InputLayers.Count - 1; i++)
            {
                var item = InputLayers[i];
                var item2 = InputLayers[i + 1];

                if (item.OuterShape.Length != item2.OuterShape.Length)
                    throw new Exception("Outer Shape incompatilbiity!");

                for (int j = 0; j < item.OuterShape.Length; j++)
                {
                    int val = item.OuterShape[i].Value;

                    if (val <= 0 || val != item2.OuterShape[i].Value)
                        throw new Exception("Outer Shape incompatilbiity!");
                }
            }

            if (InputLayers.Count > 0)
            {
                if (OuterS == null)
                {
                    OuterS = Shape.NewShapeN(this.OuterShape.Length);
                }
                unsafe
                {
                    for (int j = 0; j < this.OuterShape.Length; j++)
                    {
                        this.OuterShape[j] = InputLayers[0].OuterShape[j];
                        OuterS.Dimensions[j] = InputLayers[0].OuterShape[j].Value;
                    }
                }
                OuterS.CalculateMultiplied();
            }
        }


        public virtual void PreCheckOperation()
        {
            InnerShapeCalculation();
            OuterShapeCalculation();
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
                if (Terms[i] != null && Terms[i].Type != TermType.Variable)
                {
                    Terms[i].DeleteResults();
                    Terms[i].Dispose();
                }
            Terms.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual unsafe void Minimize()
        {
            DeleteTerms();

            PreCheck();

            Index a = Index.NewIndex(OuterS);
            a.SetZero();

            for (int i = 0; i < OuterS.TotalSize; i++, a.Add(1))
                GetTerm(a);

            if(Terms.Count > 1)
            {
                Terms.Plus min = new Terms.Plus(Terms.ToArray());
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

    //public partial class Layer
    //{
    //    public static Layer SquaredError(Layer x1, Layer x2)
    //    {
    //        Layer l = new Power(new Minus(x1, x2), 2);
    //        l = new ShrinkSizeToOneByAdding(l); //D1 = 1 and D2 = 1
    //        l = new SumSequenceToOneByAdding(l); // SequenceLength = 1
    //        return l;
    //    }

    //    public static Func<Layer, Layer> GetActivationFunction(string name)
    //    {
    //        name = name.ToLower(CultureInfo.GetCultureInfoByIetfLanguageTag("en"));
    //        if (name == "sigmoid")
    //            return (Layer x) => new Sigmoid(x);
    //        else if (name == "softmax")
    //            return (Layer x) => new SoftMax(x);
    //        return (Layer x) => x;
    //    }

    //    public static Layer Dense(int size, Layer prev, string act)
    //    {
    //        Variable W = new Variable(size, prev.D1, prev.SequenceLength);
    //        Variable B = new Variable(size, 1, prev.SequenceLength);
    //        Layer res = new Plus(new MatrixMultiply(W, prev), new ExpandWithSame(B, 1, prev.BatchSize));
    //        res = GetActivationFunction(act)(res);
    //        return res;
    //    }

    //}

}
