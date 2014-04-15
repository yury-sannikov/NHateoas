using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal class HypermediaControllerConfiguration : IHypermediaControllerConfiguration
    {
        private static readonly Lazy<HypermediaControllerConfiguration> _controllerConfigurationInstance = new Lazy<HypermediaControllerConfiguration>(LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly ConcurrentDictionary<Type, Dictionary<MethodInfo, IActionConfiguration>> _controllerRules = new ConcurrentDictionary<Type, Dictionary<MethodInfo, IActionConfiguration>>();

        public static IHypermediaControllerConfiguration Instance 
        {
            get { return _controllerConfigurationInstance.Value; }
        }

        public bool IsConfigured(Type controllerType)
        {
            return _controllerRules.ContainsKey(controllerType);
        }

        public void Setup(Type controllerType, Dictionary<MethodInfo, IActionConfiguration> rules)
        {
            _controllerRules.TryAdd(controllerType, rules);
        }

        public IActionConfiguration GetcontrollerActionConfiguration(Type controllerType, MethodInfo actionMethodInfo, HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> acceptHeaders)
        {
            if (!_controllerRules.ContainsKey(controllerType))
                return null;
            var controller = _controllerRules[controllerType];

            if (!controller.ContainsKey(actionMethodInfo))
                return null;

            var result = controller[actionMethodInfo];

            if ((acceptHeaders != null) && !acceptHeaders.Contains(new MediaTypeWithQualityHeaderValue(result.MetadataProvider.ContentType)))
                return null;

            return result;
        }

    }
}
