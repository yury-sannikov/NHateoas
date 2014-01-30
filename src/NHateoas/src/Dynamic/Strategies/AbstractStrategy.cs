using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;

namespace NHateoas.Dynamic.Strategies
{
    internal abstract class AbstractStrategy : ITypeBuilderStrategy
    {
        public abstract string ClassKey(Type originalType);

        public abstract void Configure(ITypeBuilderContainer container);

        public virtual void ActivateInstance(object proxyInstance, object originalInstance, Dictionary<string, object> routes)
        {
            
        }
       
    }
}
