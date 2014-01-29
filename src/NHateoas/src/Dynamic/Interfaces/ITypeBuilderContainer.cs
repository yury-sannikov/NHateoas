using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Dynamic.Interfaces
{
    internal interface ITypeBuilderContainer
    {
        ITypeBuilderContainer AddVisitor(ITypeBuilderVisitor visitor);
    }
}
