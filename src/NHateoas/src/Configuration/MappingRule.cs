using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal class MappingRule
    {
        private readonly MethodCallExpression _methodExpression;

        private readonly List<MappingRuleUrl> _urls = new List<MappingRuleUrl>();

        private readonly Dictionary<string, Delegate> _parametersDelegates = null;
 
        public MappingRule(MethodCallExpression methodExpression)
        {
            _methodExpression = methodExpression;
            _parametersDelegates = ParametersDelegateBuilder.Build(methodExpression);
        }

        public bool HasUrls()
        {
            return _urls.Count > 0;
        }

        public void AddUrl(MappingRuleUrl url)
        {
            _urls.Add(url);
        }

        public MethodCallExpression MethodExpression
        {
            get { return _methodExpression; }
        }

        public IEnumerable<MappingRuleUrl> Urls
        {
            get { return _urls; }
        }

        public Dictionary<string, Delegate> ParameterDelegates
        {
            get { return _parametersDelegates; }
        }
    }
}
