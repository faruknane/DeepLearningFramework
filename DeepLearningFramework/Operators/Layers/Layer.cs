using DeepLearningFramework.Data;
using DeepLearningFramework.Operators.Terms;
using PerformanceWork.OptimizedNumerics;
using System.Collections.Generic;
using System.Text;

namespace DeepLearningFramework.Operators.Layers
{
    public abstract class Layer
    {
        public int Size { get; internal set; }
        public string Activation { get; internal set; } = "";
        public string Name { get; set; }

        public List<Term> Terms = new List<Term>();
        public Term GetTerm(int time)
        {
            while (Terms.Count <= time)
                Terms.Add(null);

            if (Terms[time] == null)
                return Terms[time] = CreateTerm(time);
            return Terms[time];
        }

        public abstract Term CreateTerm(int time);
    }
}
