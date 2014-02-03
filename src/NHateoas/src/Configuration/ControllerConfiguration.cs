using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal class HypermediaControllerConfiguration : IHypermediaControllerConfiguration
    {
        private static readonly Lazy<HypermediaControllerConfiguration> _controllerConfigurationInstance = new Lazy<HypermediaControllerConfiguration>(LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly ConcurrentDictionary<Type, Dictionary<MethodInfo, ActionConfiguration>> _controllerRules = new ConcurrentDictionary<Type, Dictionary<MethodInfo, ActionConfiguration>>();

        public static IHypermediaControllerConfiguration Instance 
        {
            get { return _controllerConfigurationInstance.Value; }
        }

        public bool IsConfigured(Type controllerType)
        {
            return _controllerRules.ContainsKey(controllerType);
        }

        public void Setup(Type controllerType, Dictionary<MethodInfo, ActionConfiguration> rules)
        {
            _controllerRules.TryAdd(controllerType, rules);
        }

        public ActionConfiguration GetcontrollerActionConfiguration(Type controllerType, MethodInfo actionMethodInfo)
        {
            if (!_controllerRules.ContainsKey(controllerType))
                return null;
            var controller = _controllerRules[controllerType];

            if (!controller.ContainsKey(actionMethodInfo))
                return null;

            return controller[actionMethodInfo];
        }

    }
}
