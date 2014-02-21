using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.StrategyBuilderFactories;
using NHateoas.Response;
using NHateoas.Routes;
using NHateoas.Routes.RouteMetadataProviders.SimpleMetadataProvider;
using NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider;

namespace NHateoas.Configuration
{
    internal class ActionConfiguration : IActionConfiguration
    {
        private readonly Type _controllerType;
        private readonly MethodInfo _actionMethodInfo;
        private readonly List<MappingRule> _mappingRules = new List<MappingRule>();
        private readonly List<EntityRule> _entityRules = new List<EntityRule>();
        private IMetadataProvider _metadataProvider = null;
        private IResponseTransformerFactory _responseTransformerFactory = null;
        private IStrategyBuilderFactory _strategyBuilderFactory;

        private Type _strategyBuilderFactoryType = typeof(DefaultStrategyBuilderFactory);
        private Type _metadataProviderType = typeof(SimpleMetadataProvider);

        public ActionConfiguration(Type controllerType, MethodInfo actionMethodInfo)
        {
            _controllerType = controllerType;
            _actionMethodInfo = actionMethodInfo;
        }

        public void UseSirenSpecification()
        {
            _strategyBuilderFactoryType = typeof(SirenStrategyBuilderFactory);
            _metadataProviderType = typeof(SirenMetadataProvider);
        }

        public void Configure()
        {
            _responseTransformerFactory = new ResponseTransformerFactory();

            _metadataProvider = (IMetadataProvider)Activator.CreateInstance(_metadataProviderType, this);
            _strategyBuilderFactory = (IStrategyBuilderFactory)Activator.CreateInstance(_strategyBuilderFactoryType);
        }

        public Type ControllerType
        {
            get { return _controllerType; }
        }

        public MethodInfo ActionMethodInfo
        {
            get { return _actionMethodInfo; }
        }

        public void AddMappingRule(MappingRule rule)
        {
            _mappingRules.Add(rule);
        }

        public void AddEntityRule(EntityRule entityRule)
        {
            _entityRules.Add(entityRule);
        }

        public IEnumerable<MappingRule> MappingRules
        {
            get { return _mappingRules; }
        }

        public IEnumerable<EntityRule> EntityRules
        {
            get { return _entityRules; }
        }

        public IMetadataProvider MetadataProvider
        {
            get { return _metadataProvider; }
            set { _metadataProvider = value; }
        }

        public IResponseTransformerFactory ResponseTransformerFactory
        {
            get { return _responseTransformerFactory; }
            set { _responseTransformerFactory = value; }
        }

        public IStrategyBuilderFactory StrategyBuilderFactory
        {
            get { return _strategyBuilderFactory; }
        }

    }
}
