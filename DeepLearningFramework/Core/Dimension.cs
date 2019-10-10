using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Core
{
    public class Dimension
    {
        public virtual int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]  set; } = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool SoftEquals(Dimension other)
        {
            if (other.Value == -1 || this.Value == -1) return true;
            return other.Value == this.Value;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool HardEquals(Dimension other)
        {
            if (other.Value == -1 || this.Value == -1)
                throw new Exception("Terms should have an exact value!");
            return other.Value == this.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator Dimension(int x)
        {
            Dimension d = new Dimension();
            d.Value = x;
            return d;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator int(Dimension x)
        {
            return x.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Dimension operator +(Dimension a, Dimension b)
        {
            return new DimensionPlus(a, b);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Dimension operator *(Dimension a, Dimension b)
        {
            return new DimensionMultiply(a, b);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Dimension operator /(Dimension a, Dimension b)
        {
            return new DimensionDivide(a, b);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Dimension operator -(Dimension a, Dimension b)
        {
            return new DimensionMinus(a, b);
        }

    }
    public class DimensionPlus : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get { return a.Value + b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DimensionPlus(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }
    public class DimensionMinus : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get { return a.Value - b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DimensionMinus(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }
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

    public class ReflectDimension : Dimension
    {
        Dimension r;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get { return r.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReflectDimension(Dimension r)
        {
            this.r = r;
        }
    }
}
