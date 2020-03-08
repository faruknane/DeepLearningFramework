using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DeepLearningFramework.Operators.Terms;
using DeepLearningFramework.Core;
using PerformanceWork.OptimizedNumerics;

namespace DeepLearningFramework.Operators.Layers
{
    public class ExpandWithSame : Layer
    {
        public Dimension[] Multiplier { get; internal set; }
        public Shape MultiplierS { get; internal set; }

        public ExpandWithSame(Layer x, Dimension[] m)
        {
            this.InputLayers.Add(x);
            this.OuterShape = new Dimension[x.OuterShape.Length];
            this.InnerShape = new Dimension[x.InnerShape.Length];
            Multiplier = m;
        }

        public override void InnerShapeCalculation()
        {
            //MultiplierS + innershape calculation.
            unsafe
            {
                if (this.MultiplierS == null)
                    this.MultiplierS = Shape.NewShapeN(InnerShape.Length);

                for (int i = 0; i < InnerShape.Length; i++)
                {
                    this.MultiplierS.Dimensions[i] = Multiplier[i].Value;
                    this.InnerShape[i] = new DimensionMultiply(InputLayers[0].InnerShape[i], Multiplier[i]);
                }
                this.MultiplierS.CalculateMultiplied();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override Term CreateTerm(Index time)
        {
            return new Terms.ExpandWithSame(InputLayers[0].GetTerm(time), MultiplierS);
        }

    }
}
