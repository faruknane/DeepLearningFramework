using DeepLearningFramework.Operators.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearningFramework.Modules
{
    public class Module
    {
        public virtual dynamic Forward(Term[] x)
        {
            throw new Exception("Not implemented!");
        }

        public virtual dynamic Forward(dynamic x)
        {
            throw new Exception("Not implemented!");
        }
    }
}
