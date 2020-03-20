
using PerformanceWork.OptimizedNumerics;
using System;
using System.Threading.Tasks;
using DeepLearningFramework.Core;
using System.Threading;

namespace DeepLearningFramework.Operators.Terms
{
    public class Plus : Term
    {
        public Plus(params Term[] v) // make args
        {
            if (v.Length < 2)
                throw new Exception("length < 2!");
            Type = TermType.Plus;
            Terms = v;
            for (int i = 0; i < Terms.Length - 1; i++)
                if (!this.Terms[i].Shape.EqualShape(this.Terms[i + 1].Shape)) //will be shape, not d1 or d2
                {
                    throw new Exception("Terms to be sum should have the same dimensions!");
                }
            this.Shape = v[0].Shape.Clone();
        }

        public override void CalculateDerivate(Tensor<float> s)
        {
            if(Terms.Length > 1)
            {
                Task[] l = new Task[Terms.Length - 1];
                for (int i = 0; i < Terms.Length - 1; i++)
                {
                    l[i] = new Task(new Action<object>((object o) => { ((Term)o).Derivate(s); }), Terms[i]);
                    l[i].Start();
                }

                Terms[Terms.Length - 1].Derivate(s);

                for (int i = 0; i < Terms.Length - 1; i++)
                    l[i].Wait();
            }
            else
            {
                for (int i = 0; i < Terms.Length; i++)
                 Terms[i].Derivate(s);
            }

        }

        public unsafe override Tensor<float> CalculateResult()
        {
            if (Terms.Length > 1)
            {
                Task[] l = new Task[Terms.Length - 1];
                for (int i = 0; i < Terms.Length - 1; i++)
                {
                    l[i] = new Task(new Action<object>((object o) => { ((Term)o).GetResult(); }), Terms[i]);
                    l[i].Start();
                }

                Terms[Terms.Length - 1].GetResult();

                for (int i = 0; i < Terms.Length - 1; i++)
                    l[i].Wait();

                Tensor<float> res = new Tensor<float>(this.Shape.Clone());
                Vectorization.ElementWiseAddAVX((float*)Terms[0].GetResult().Array, (float*)Terms[1].GetResult().Array, (float*)res.Array, this.Shape.TotalSize);

                for (int i = 2; i < Terms.Length; i++) //Optimize here. 
                    Vectorization.ElementWiseAddAVX((float*)res.Array, (float*)Terms[i].GetResult().Array, (float*)res.Array, this.Shape.TotalSize);
                return res;
            }
            else
            {
                Tensor<float> res = new Tensor<float>(this.Shape.Clone());
                Vectorization.ElementWiseAddAVX((float*)Terms[0].GetResult().Array, (float*)Terms[1].GetResult().Array, (float*)res.Array, this.Shape.TotalSize);

                for (int i = 2; i < Terms.Length; i++) //Optimize here. 
                    Vectorization.ElementWiseAddAVX((float*)res.Array, (float*)Terms[i].GetResult().Array, (float*)res.Array, this.Shape.TotalSize);
                return res;

            }

        }

    }
}