using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Dynamic.Interfaces
{
    internal interface ITypeBuilderProvider
    {
        System.Reflection.Emit.TypeBuilder GetTypeBuilder();
    }
}
