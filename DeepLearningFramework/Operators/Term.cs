using DeepLearningFramework.Data;
using PerformanceWork.OptimizedNumerics;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Operators
{
    public class Term
    {
        public virtual Dimension D1 { get; internal set; } //Create a Dimension class to changable sizes.
        public virtual Dimension D2 { get; internal set; }
        public Matrix Result { get; internal set; }
        //add
        //contains trainable variable ? 
        //is variable ? 
        internal virtual Matrix CalculateResult()
        {
            throw new System.Exception("Undefined method!");
        }

        public virtual Matrix GetResult()
        {
            if(Result == null)
            {
                return Result = CalculateResult();
            }
            return Result;
        }
        public virtual void Derivate(MMDerivative s)
        {
            throw new System.Exception("Undefined method!");
        }
        public virtual void Minimize()
        {
            this.DeleteResults();
            this.Derivate(MMDerivative.I(D1, D2));
        }

        public virtual void Maximize()
        {
            this.DeleteResults();
            MMDerivative m = MMDerivative.I(D1, D2);
            m.Negative = true;
            this.Derivate(m);
        }

        public virtual void DeleteResults()
        {
            if (Result != null)
                Result.Dispose();
            Result = null;
        }
    }
}
