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
        void Setup(Type controllerType, Dictionary<MethodInfo, IActionConfiguration> rules);
        IActionConfiguration GetcontrollerActionConfiguration(Type controllerType, MethodInfo actionMethodInfo);
    }
}
