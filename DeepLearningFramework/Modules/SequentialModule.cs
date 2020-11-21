using DeepLearningFramework.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearningFramework.Modules
{
    public class Sequential : Module
    {
        public Module[] Modules;

        public Sequential(params Module[] modules)
        {
            this.Modules = modules;
        }

        public override dynamic Forward(Term[] x)
        {
            dynamic a = x;
            for (int i = 0; i < Modules.Length; i++)
            {
                a = Modules[i].Forward(a);
            }
            return a;
        }

        public override dynamic Forward(dynamic x)
        {
            dynamic a = x;
            for (int i = 0; i < Modules.Length; i++)
            {
                a = Modules[i].Forward(a);
            }
            return a;
        }
    }
}
