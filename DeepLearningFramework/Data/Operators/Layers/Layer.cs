using DeepLearningFramework.Data;
using DeepLearningFramework.Data.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Core;

namespace DeepLearningFramework.Data.Operators.Layers
{
    public abstract partial class Layer
    {
        public string Activation { get; internal set; } = "";
        public string Name { get; set; }
        public virtual Dimension D1 { get; internal set; }
        public virtual Dimension D2 { get; internal set; }

        public virtual Dimension SequenceLength { get; internal set; }

        public List<Term> Terms = new List<Term>();
        
        private Terms.Variable EmptyVariable;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual Term GetTerm(int time)
        {
            if (time < 0 || time > SequenceLength.Value)
            {
                if (EmptyVariable == null)
                {
                    EmptyVariable = new Terms.Variable(D1, D2) { Trainable = false };
                    EmptyVariable.Weights.SetZero();
                }

                return EmptyVariable;
            }

            while (Terms.Count <= time && Terms.Count < SequenceLength)
                Terms.Add(null);

            if (Terms[time] == null)
                return Terms[time] = CreateTerm(time); // Activation -> new sigmoid(createterm(time));

            return Terms[time];
        }

        public abstract Term CreateTerm(int time);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual void DeleteTerms()
        {
            for (int i = 0; i < Terms.Count; i++)
            {
                Terms[i].DeleteResults();
            }

            Terms.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public virtual void Minimize()
        {
            //first deleteresults of all terms of the layer
            //then delete terms of the layer
            //then we are ready to minimize
            DeleteTerms();
            for (int i = 0; i < SequenceLength; i++)
                GetTerm(i).Minimize();
        }

        #region Operators * + - / 

        public static Layer operator +(Layer x, Layer y)
        {
            return new Plus(x, y);
        }

        public static Layer operator *(Layer x, Layer y)
        {
            return new MatrixMultiply(x, y);
        }

        //public static Layer operator *(Layer x, float y)
        //{
        //    return new MultiplyByNumber(x, y); //add it's layer version !
        //}

        public static Layer operator -(Layer x, Layer y)
        {
            return new Minus(x, y);
        }

        #endregion
    }

    public partial class Layer
    {
        public static Layer SquaredError(Layer x1, Layer x2)
        {
            Layer l = new Power(new Minus(x1, x2), 2);
            l = new ShrinkSizeToOneByAdding(l); //D1 = 1 and D2 = 1
            l = new SumSequenceToOneByAdding(l); // SequenceLength = 1
            return l;
        }

        public static Func<Layer,Layer> GetActivationFunction(string name)
        {
            name = name.ToLower(CultureInfo.GetCultureInfoByIetfLanguageTag("en"));
            if (name == "sigmoid")
                return (Layer x) => new Sigmoid(x);
            else if(name == "softmax")
                return (Layer x) => new SoftMax(x);
            return (Layer x) => x;
        }

        public static Layer Dense(int size, Layer prev, string act)
        {
            Variable W = new Variable(size, prev.D1, prev.SequenceLength);
            Variable B = new Variable(size, 1, prev.SequenceLength);
            Layer res = new Plus(new MatrixMultiply(W, prev), new ExpandWithSame(B, 1, prev.D2));
            res = GetActivationFunction(act)(res);
            return res;
        }

    }

}
