using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Dynamic.Interfaces
{
    internal interface IStrategyBuilderFactory
    {
        ITypeBuilderStrategy Build(IActionConfiguration actionConfiguration, Type returnType);
    }
}
