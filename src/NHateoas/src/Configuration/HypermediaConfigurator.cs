using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace NHateoas.Configuration
{
    public class HypermediaConfigurator<TModel, TController>
    {
        private readonly Dictionary<MethodInfo, ActionConfiguration>  _rules = new Dictionary<MethodInfo, ActionConfiguration>();
        private ActionConfiguration _currentActionConfiguration = null;

        public HypermediaConfigurator<TModel, TController> Map(Expression<Func<TModel, TController, IEnumerable<TModel>>> expression)
        {
            AddNewRule(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> Map(Expression<Func<TModel, TController, TModel>> expression)
        {
            AddNewRule(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> Map(Expression<Action<TModel, TController>> expression)
        {
            AddNewRule(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Func<TModel, TController, IEnumerable<TModel>>> expression)
        {
            SetCurrentAction(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Func<TModel, TController, TModel>> expression)
        {
            SetCurrentAction(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> For(Expression<Action<TModel, TController>> expression)
        {
            SetCurrentAction(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> MapReference<TOtherController>(Expression<Func<TModel, TOtherController, Object>> expression)
        {
            AddNewRule(expression.Body);
            return this;
        }
        public HypermediaConfigurator<TModel, TController> MapReference<TOtherController>(Expression<Action<TModel, TOtherController>> expression)
        {
            AddNewRule(expression.Body);
            return this;
        }

        public HypermediaConfigurator<TModel, TController> UseSirenSpecification()
        {
            ActionConfiguration.UseSirenSpecification();
            return this;
        }

        private void SetCurrentAction(Expression methodExpression)
        {
            var currentAction = ((MethodCallExpression)methodExpression).Method;
            if (!_rules.ContainsKey(currentAction))
            {
                ActionConfiguration = new ActionConfiguration(typeof(TController), currentAction);
                _rules.Add(currentAction, ActionConfiguration);
            }
        }

        private void AddNewRule(Expression expression)
        {
            var methodExpression = (MethodCallExpression)expression;
            var rule = new MappingRule(methodExpression);
            ActionConfiguration.AddMappingRule(rule);
        }

        private ActionConfiguration ActionConfiguration
        {
            get { return _currentActionConfiguration; }
            set
            {
                if (_currentActionConfiguration != null)
                    _currentActionConfiguration.Configure();
                _currentActionConfiguration = value;
            }
        }

        public void Configure()
        {
            if (_currentActionConfiguration != null)
                _currentActionConfiguration.Configure();

            var controllerConfiguration = HypermediaControllerConfiguration.Instance;

            if (controllerConfiguration.IsConfigured(typeof(TController)))
                return;

            controllerConfiguration.Setup(typeof(TController), _rules);
        }
    }
}
