using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Routes;

namespace NHateoas.Dynamic.Strategies
{
    internal abstract class AbstractStrategy : ITypeBuilderStrategy
    {
        public abstract string ClassKey(Type originalType);

        public abstract void Configure(ITypeBuilderContainer container);

        public virtual void ActivateInstance(object proxyInstance, object originalInstance, IRoutesBuilder routesBuilder)
        {
        }
       
    }
}
