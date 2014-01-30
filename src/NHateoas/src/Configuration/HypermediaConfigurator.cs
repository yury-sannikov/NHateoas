using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace NHateoas.Configuration
{
    public class HypermediaConfigurator<TModel, TController>
    {
        private readonly Dictionary<MethodInfo, ActionConfiguration>  _rules = new Dictionary<MethodInfo, ActionConfiguration>();
        private MethodInfo _currentAction = null;

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

        public HypermediaConfigurator<TModel, TController> MapReference<TOtherModel>(Expression<Func<TModel, Object>> expressionThis,
            Expression<Func<TOtherModel, Object>> expressionThat, object configuration)
        {
            return this;
        }

        private void SetCurrentAction(Expression methodExpression)
        {
            _currentAction = ((MethodCallExpression)methodExpression).Method;
            if (!_rules.ContainsKey(_currentAction))
            {
                _rules.Add(_currentAction, new ActionConfiguration());
            }
        }

        private void AddNewRule(Expression expression)
        {
            var methodExpression = (MethodCallExpression)expression;
            var rule = new MappingRule(methodExpression);
            Rules.AddMappingRule(rule);
        }

        private ActionConfiguration Rules
        {
            get { return _rules[_currentAction]; }
        }

        public void Configure()
        {
            var controllerConfiguration = HypermediaControllerConfiguration.Instance;

            if (controllerConfiguration.IsConfigured(typeof(TController)))
                return;

            controllerConfiguration.Setup(typeof(TController), _rules);
        }
    }
}
