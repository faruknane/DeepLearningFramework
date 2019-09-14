using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeepLearningFramework.Data
{
    public class Dimension
    {
        public virtual int Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; [MethodImpl(MethodImplOptions.AggressiveInlining)]  set; } = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SoftEquals(Dimension other)
        {
            if (other.Value == -1 || this.Value == -1) return true;
            return other.Value == this.Value;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HardEquals(Dimension other)
        {
            if (other.Value == -1 || this.Value == -1)
                throw new Exception("Terms should have an exact value!");
            return other.Value == this.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Dimension(int x)
        {
            Dimension d = new Dimension();
            d.Value = x;
            return d;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(Dimension x)
        {
            return x.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dimension operator +(Dimension a, Dimension b)
        {
            return new DimensionPlus(a, b);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dimension operator *(Dimension a, Dimension b)
        {
            return new DimensionMultiply(a, b);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dimension operator /(Dimension a, Dimension b)
        {
            return new DimensionDivide(a, b);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dimension operator -(Dimension a, Dimension b)
        {
            return new DimensionMinus(a, b);
        }

    }
    public class DimensionPlus : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return a.Value + b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DimensionPlus(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }
    public class DimensionMinus : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return a.Value - b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DimensionMinus(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }
    public class DimensionMultiply : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return a.Value * b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DimensionMultiply(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }
    public class DimensionDivide : Dimension
    {
        Dimension a, b;
        public override int Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return a.Value / b.Value; } set { } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DimensionDivide(Dimension a, Dimension b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
