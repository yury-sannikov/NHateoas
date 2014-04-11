using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NHateoas.Configuration
{
    [SecuritySafeCritical]
    internal class HypermediaConfigurationLogic<TModel, TController>
    {
        private readonly Dictionary<MethodInfo, IActionConfiguration> _rules =
            new Dictionary<MethodInfo, IActionConfiguration>();

        private ActionConfiguration _currentActionConfiguration = null;
        private HttpConfiguration _httpConfiguration;
        
        public HypermediaConfigurationLogic(HttpConfiguration httpConfiguration)
        {
            _httpConfiguration = httpConfiguration;
        }

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
            var rule = new MappingRule(methodExpression, _httpConfiguration.Services.GetApiExplorer());
            ActionConfiguration.AddMappingRule(rule);
        }

        public void AddNewEmbeddedEntityMapping(Expression embeddedEntityExpression, Expression actionSelector, string[] rel)
        {
            var handlingSelector = actionSelector as MethodCallExpression;
            var rule = new EntityRule(embeddedEntityExpression, handlingSelector, EntityRule.EmbeddingRule.Embedded, rel);
            ActionConfiguration.AddEntityRule(rule);
        }

        public void AddNewLinkedEntityMapping(Expression embeddedEntityExpression, Expression actionSelector, string[] rel)
        {
            var handlingSelector = actionSelector as MethodCallExpression;
            var rule = new EntityRule(embeddedEntityExpression, handlingSelector, EntityRule.EmbeddingRule.Linked, rel);
            ActionConfiguration.AddEntityRule(rule);
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


        public Dictionary<MethodInfo, IActionConfiguration> Rules
        {
            get { return _rules; }
        }
    }
}
