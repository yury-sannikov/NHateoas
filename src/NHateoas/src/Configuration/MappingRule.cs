using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace NHateoas.Configuration
{
    internal class MappingRule
    {
        private readonly MethodCallExpression _methodExpression;

        private readonly List<ApiDescription> _apiDescriptions = new List<ApiDescription>();

        private readonly Dictionary<string, Delegate> _parametersDelegates = null;
 
        public MappingRule(MethodCallExpression methodExpression)
        {
            _methodExpression = methodExpression;
            _parametersDelegates = ParametersDelegateBuilder.Build(methodExpression);

            MapApiDesctiption();
        }

        private void MapApiDesctiption()
        {
            var apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();

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
    }
}
