//using PerformanceWork.OptimizedNumerics;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using DeepLearningFramework.Core;

//namespace DeepLearningFramework.Data.Operators.Terms
//{
//    public class PlaceHolder : Term
//    {
//        public PlaceHolder(int Size)
//        {
//            Type = TermType.PlaceHolder;
//            D1 = Size;
//            D2 = -1;
//            Terms = new Term[1];
//        }

//        public void SetVariable(Variable v)
//        {
//            if (Terms[0] != null)
//            {
//                Variable v11 = (Variable)Terms[0];
//                if (!v11.Weights.Returned)
//                    v11.Weights.Dispose();
//                v11 = null;
//                Terms[0] = null;
//            }
//            Terms[0] = v;
//            if (D1 != Terms[0].D1)
//                throw new Exception("Placeholder dimension!");
//            D2 = v.D2;
//            if(D2 <= 0)
//                throw new Exception("Batch Dimension (D2) should have an exact value!");
//        }

//        public override void CalculateDerivate(MMDerivative s)
//        {
//            Terms[0].Derivate(s);
//        }

//        internal override Matrix CalculateResult()
//        {
//            return Terms[0].GetResult();
//        }

//    }
//}
