using NHateoas.Dynamic.Interfaces;
using NHateoas.Response;
using NHateoas.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal interface IActionConfiguration
    {
        Type ControllerType { get; }

        MethodInfo ActionMethodInfo { get; }

        IEnumerable<MappingRule> MappingRules { get; }

        IEnumerable<EntityRule> EntityRules { get; }

        string[] Class { get; }

        IMetadataProvider MetadataProvider { get; }

        IResponseTransformerFactory ResponseTransformerFactory { get; }

        IStrategyBuilderFactory StrategyBuilderFactory { get; }
    }
}
