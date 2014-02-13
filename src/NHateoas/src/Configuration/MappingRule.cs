using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using NHateoas.Attributes;

namespace NHateoas.Configuration
{
    [SecuritySafeCritical]
    internal class MappingRule
    {
        private readonly MethodCallExpression _methodExpression;

        private readonly List<ApiDescription> _apiDescriptions = new List<ApiDescription>();

        private readonly Dictionary<string, Delegate> _parametersDelegates = null;

        private readonly List<string> _rels = new List<string>();

        public enum RuleType
        {
            Default,
            LinkRule,
            ActionRule
        }

        private RuleType _ruleType = RuleType.Default;

        public MappingRule(MethodCallExpression methodExpression, IApiExplorer apiExplorer = null)
        {
            _methodExpression = methodExpression;
            _parametersDelegates = ParametersDelegateBuilder.Build(methodExpression);

            MapApiDesctiption(apiExplorer);

            AddRelsFromAttribute(methodExpression);
        }

        private void AddRelsFromAttribute(MethodCallExpression methodExpression)
        {
            var attribute = methodExpression.Method.GetCustomAttributes<HypermediaAttribute>().FirstOrDefault();

            if (attribute != null && attribute.Names.Any())
                _rels.AddRange(attribute.Names);
        }

        private void MapApiDesctiption(IApiExplorer apiExplorer)
        {
            if (apiExplorer == null)
                apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();

            foreach (var description in apiExplorer.ApiDescriptions)
            {
                var actionDescriptor = description.ActionDescriptor as ReflectedHttpActionDescriptor;

                if (actionDescriptor == null)
                    continue;

                if (_methodExpression.Method != actionDescriptor.MethodInfo)
                    continue;

                _apiDescriptions.Add(description);
            }
        }

        public MethodCallExpression MethodExpression
        {
            get { return _methodExpression; }
        }

        public IEnumerable<ApiDescription> ApiDescriptions
        {
            get { return _apiDescriptions; }
        }

        public Dictionary<string, Delegate> ParameterDelegates
        {
            get { return _parametersDelegates; }
        }

        public List<string> Names
        {
            get { return _rels; }
        }

        public RuleType Type
        {
            get { return _ruleType; }
            set { _ruleType = value; }
        }

        public ContentType ContentType { get; set; }
    }
}
