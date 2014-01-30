using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal interface IHypermediaControllerConfiguration
    {
        bool IsConfigured(Type controllerType);
        void Setup(Type controllerType, Dictionary<MethodInfo, ActionConfiguration> rules);
        ActionConfiguration GetcontrollerActionConfiguration(Type controllerType, MethodInfo actionMethodInfo);
    }
}
