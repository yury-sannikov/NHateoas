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
 
        public MappingRule(MethodCallExpression methodExpression)
        {
            _methodExpression = methodExpression;
        }

        public bool HasUrls()
        {
            return _urls.Count > 0;
        }

        public void AddUrl(MappingRuleUrl url)
        {
            _urls.Add(url);
        }

        public MethodInfo Method
        {
            get { return _methodExpression.Method; }
        }

        public IEnumerable<MappingRuleUrl> Urls
        {
            get { return _urls; }
        }
    }
}
