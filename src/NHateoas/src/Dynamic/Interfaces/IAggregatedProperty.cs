using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Dynamic.Interfaces
{
    internal interface IAggregatedProperty : ITypeBuilderVisitor
    {
        void SetAggregatedProperty(FieldInfo aggregate);
    }
}
