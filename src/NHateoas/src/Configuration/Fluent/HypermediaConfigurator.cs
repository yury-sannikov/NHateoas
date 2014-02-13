using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using NHateoas.Attributes;
using NHateoas.Configuration.Fluent;

namespace NHateoas.Configuration
{
    [SecuritySafeCritical]
    public class HypermediaConfigurator<TModel, TController>
    {
        private readonly HypermediaConfigurationLogic<TModel, TController> _logic;

        private readonly SirenConfigurator<TModel, TController> _sirenConfigurator;

        public HypermediaConfigurator(HttpConfiguration configuration)
        {
            _logic = new HypermediaConfigurationLogic<TModel, TController>(configuration);
            _sirenConfigurator = new SirenConfigurator<TModel, TController>(this, _logic);    
        }

        public HypermediaConfigurator<TModel, TController> Map(Expression<Func<TModel, TController, IEnumerable<TModel>>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> Map(Expression<Func<TModel, TController, TModel>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> Map(Expression<Action<TModel, TController>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Func<TModel, TController, IEnumerable<TModel>>> expression)
        {
            _logic.SetCurrentAction(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Func<TModel, TController, TModel>> expression)
        {
            _logic.SetCurrentAction(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Action<TModel, TController>> expression)
        {
            _logic.SetCurrentAction(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> MapReference<TOtherController>(Expression<Func<TModel, TOtherController, Object>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }
        public HypermediaConfigurator<TModel, TController> MapReference<TOtherController>(Expression<Action<TModel, TOtherController>> expression)
        {
            _logic.AddNewRule(expression.Body);
            return this;
        }

        public SirenConfigurator<TModel, TController> UseSirenSpecification()
        {
            _logic.ActionConfiguration.UseSirenSpecification();
            return _sirenConfigurator;
        }

        public HypermediaConfigurator<TModel, TController> Named(string rel)
        {
            if (_logic.ActionConfigurationMappingRule == null)
                throw new Exception("Named should be used after Map");

            _logic.ActionConfigurationMappingRule.Names.Add(rel);
            return this;
        }

        public void Configure()
        {
            if (_logic.ActionConfiguration != null)
                _logic.ActionConfiguration.Configure();

            var controllerConfiguration = HypermediaControllerConfiguration.Instance;

            if (controllerConfiguration.IsConfigured(typeof(TController)))
                return;

            controllerConfiguration.Setup(typeof(TController), _logic.Rules);
        }
    }
}
