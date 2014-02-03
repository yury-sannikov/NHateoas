using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal static class ParametersDelegateBuilder
    {
        private delegate String GetParameterName(object data);

        public static Dictionary<string, Delegate> Build(MethodCallExpression methodExpression)
        {
            var result = new Dictionary<string, Delegate>();

            var controllerArgumentExpressions = methodExpression.Arguments;
            
            var methodParameters = methodExpression.Method.GetParameters();
            
            if (controllerArgumentExpressions.Count != methodParameters.Count())
                throw new Exception("Unable to build parameter delegates");

            var controllerArgumentExpressionsEnum = controllerArgumentExpressions.GetEnumerator();
            
            foreach (var methodParameter in methodParameters)
            {
                controllerArgumentExpressionsEnum.MoveNext();

                var argExpression = controllerArgumentExpressionsEnum.Current;

                if (argExpression is MethodCallExpression)
                {
                    AddNamedParameter(result, methodParameter, argExpression as MethodCallExpression);
                    
                    continue;
                }

                AddTypeExtractor(result, methodParameter, argExpression);
            }

            return result;
        }

        private static void AddTypeExtractor(Dictionary<string, Delegate> result, ParameterInfo methodParameter, Expression argExpression)
        {
            ParameterExpression paramExpression = null;
            if (argExpression is MemberExpression)
                paramExpression = (ParameterExpression)((MemberExpression)argExpression).Expression;
            else if (argExpression is ParameterExpression)
                paramExpression = (ParameterExpression)argExpression;

            if (paramExpression == null)
                throw new Exception("Controller arguments must be model member expressions");

            var expression = Expression.Lambda(argExpression, paramExpression);

            result[methodParameter.Name] = expression.Compile();
        }

        private static void AddNamedParameter(Dictionary<string, Delegate> result, ParameterInfo methodParameter, MethodCallExpression methodCall)
        {
            if (methodCall.Method.DeclaringType != typeof(QueryParameter))
                throw new Exception("Controller arguments must be QueryParameter class method call expression");

            var methodName = methodParameter.Name;

            GetParameterName getParameterName = data => string.Format(":{0}", methodName);

            result[methodParameter.Name] = getParameterName;
        }
    }
}
