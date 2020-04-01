using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Core
{
    /// <summary>
    /// Dynamic Dimension class holds a formula or a value for a dimension.
    /// </summary>
    public class Dimension
    {
        /// <summary>
        /// Dimension value initialized to -1.
        /// </summary>
        public virtual int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]  set; } = -1;
        
        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Dimension()
        {
        }

       
        /// <param name="val">Initial Value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Dimension(int val)
        {
            this.Value = val;
        }

        /// <summary>
        /// Integer to Dimension conversion.
        /// </summary>
        /// <param name="x">Integer Value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator Dimension(int x)
        {
            Dimension d = new Dimension();
            d.Value = x;
            return d;
        }

        /// <summary>
        /// Dimension to Integer conversion.
        /// </summary>
        /// <param name="x">Dimension object</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator int(Dimension x)
        {
            return x.Value;
        }

        /// <summary>
        /// Add Operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns cref="DimensionAdd">A DimensionPlus object that holds value of a + b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Dimension operator +(Dimension a, Dimension b)
        {
            return new DimensionAdd(a, b);
        }

        /// <summary>
        /// Multiply Operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns cref="DimensionMultiply">A DimensionMultiply object that holds value of a * b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Dimension operator *(Dimension a, Dimension b)
        {
            return new DimensionMultiply(a, b);
        }

        /// <summary>
        /// Divide Operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns cref="DimensionDivide">A DimensionDivide object that holds value of a / b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Dimension operator /(Dimension a, Dimension b)
        {
            return new DimensionDivide(a, b);
        }

        /// <summary>
        /// Subtract Operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns cref="DimensionSubtract">A DimensionSubtract object that holds value of a - b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Dimension operator -(Dimension a, Dimension b)
        {
            return new DimensionSubtract(a, b);
        }

    }

    /// <summary>
    /// DimensionAdd class helps holding the addition process of two dimensions dynamically.
    /// </summary>
    public class DimensionAdd : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get { return a.Value + b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DimensionAdd(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }

    /// <summary>
    /// DimensionSubtract class helps holding the subtraction process of two dimensions dynamically.
    /// </summary>
    public class DimensionSubtract : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get { return a.Value - b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DimensionSubtract(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }

    /// <summary>
    /// DimensionMultiply class helps holding the multiplication process of two dimensions dynamically.
    /// </summary>
    public class DimensionMultiply : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get { return a.Value * b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DimensionMultiply(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }

    /// <summary>
    /// DimensionDivide class helps holding the division process of two dimensions dynamically.
    /// </summary>
    public class DimensionDivide : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get { return a.Value / b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DimensionDivide(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }

}
