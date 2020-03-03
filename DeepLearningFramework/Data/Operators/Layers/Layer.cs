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

        public List<Term> Terms = new List<Term>();

        private Terms.Variable EmptyVariable;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual unsafe Term GetTerm(Index time)
        {
            //TODO yes

            if (time.N != OuterShape.Length)
                throw new Exception("");

            int index = 0;
            int mult = 1;

            for (int i = time.N - 1; i >= 0; i--)
            {
                index += time[i] * mult;
                int val = OuterShape[i].Value;
                mult *= val;
                if (time[i] < 0 || time[i] >= val)
                {
                    Shape s = Shape.NewShapeN(InnerShape.Length);
                    for (int a = 0; a < InnerShape.Length; a++)
                        s.Dimensions[a] = InnerShape[a].Value;
                    s.CalculateMultiplied();

                    if (EmptyVariable == null)
                    {
                        EmptyVariable = new Terms.Variable(s) { Trainable = false };
                        EmptyVariable.Weights.SetValue(0);
                    }
                    else if (!EmptyVariable.Shape.EqualShape(s))
                    {
                        EmptyVariable.Clean();
                        EmptyVariable.Dispose();
                        EmptyVariable = new Terms.Variable(s) { Trainable = false };
                        EmptyVariable.Weights.SetValue(0);
                    }
                    else
                        Shape.Return(s);

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
        public virtual void DeleteTerms()
        {
            for (int i = 0; i < Terms.Count; i++)
            {
                Terms[i].DeleteResults();
            }
            //call other layers' deleteterms() method automatically?
            //if outershape is converted to shape class, store it and reuse it and delete it here.
            Terms.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual unsafe void Minimize()
        {
            //first deleteresults of all terms of the layer
            //then delete terms of the layer
            //then we are ready to minimize
            DeleteTerms();
            
            Shape s = Shape.NewShapeN(OuterShape.Length);
            for (int i = 0; i < OuterShape.Length; i++)
                s.Dimensions[i] = OuterShape[i].Value;
            s.CalculateMultiplied();


            Index a = Index.NewIndex(s);
            a.SetZero();

            for (int i = 0; i < s.TotalSize; i++, a.Add(1))
            {
                GetTerm(a).Minimize();
            }

            Shape.Return(s);
            Index.Return(a);
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
