using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal class HypermediaConfigurationLogic<TModel, TController>
    {
        private readonly Dictionary<MethodInfo, ActionConfiguration> _rules =
            new Dictionary<MethodInfo, ActionConfiguration>();

        private ActionConfiguration _currentActionConfiguration = null;

        public void SetCurrentAction(Expression methodExpression)
        {
            var currentAction = ((MethodCallExpression) methodExpression).Method;
            if (!_rules.ContainsKey(currentAction))
            {
                SetActionConfiguration(new ActionConfiguration(typeof(TController), currentAction));
                _rules.Add(currentAction, ActionConfiguration);
            }
        }

        public void AddNewRule(Expression expression)
        {
            var methodExpression = (MethodCallExpression) expression;
            var rule = new MappingRule(methodExpression);
            ActionConfiguration.AddMappingRule(rule);
        }

        public ActionConfiguration ActionConfiguration
        {
            get { return _currentActionConfiguration; }
        }

        public MappingRule ActionConfigurationMappingRule 
        {
            get
            {
                return _currentActionConfiguration == null ? null : (_currentActionConfiguration.MappingRules.Any() ? _currentActionConfiguration.MappingRules.Last() : null);
            }
        }

        public void SetActionConfiguration(ActionConfiguration actionConfiguration)
        {
            if (_currentActionConfiguration != null)
                _currentActionConfiguration.Configure();
            _currentActionConfiguration = actionConfiguration;
        }


        public Dictionary<MethodInfo, ActionConfiguration> Rules
        {
            get { return _rules; }
        }
    }
}
