using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DeepLearningFramework.Data
{
    public class Dimension
    {
        public int Value = -1;

        public bool SoftEquals(Dimension other)
        {
            if (other.Value == -1 || this.Value == -1) return true;
            return other.Value == this.Value;

        }
        public bool HardEquals(Dimension other)
        {
            if (other.Value == -1 || this.Value == -1)
                throw new Exception("Terms should have an exact value!");
            return other.Value == this.Value;
        }

        public static implicit operator Dimension(int x)
        {
            Dimension d = new Dimension();
            d.Value = x;
            return d;
        }
        public static implicit operator int(Dimension x)
        {
            return x.Value;
        }


    }

  
}
