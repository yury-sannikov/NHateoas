using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal static class ParametersDelegateBuilder
    {
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

                ParameterExpression paramExpression = null;
                if (argExpression is MemberExpression)
                    paramExpression = (ParameterExpression) ((MemberExpression) argExpression).Expression;
                else if (argExpression is ParameterExpression)
                    paramExpression = (ParameterExpression)argExpression;
                
                if (paramExpression == null)
                    throw new Exception("Controller arguments must be model member expressions");


                var expression = Expression.Lambda(argExpression, paramExpression);

                result[methodParameter.Name] = expression.Compile();
            }

            return result;
        }
    }
}
