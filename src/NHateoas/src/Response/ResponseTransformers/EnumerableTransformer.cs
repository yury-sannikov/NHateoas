using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Configuration;

namespace NHateoas.Response.ResponseTransformers
{
    internal class EnumerableTransformer : IResponseTransformer
    {
        public object Transform(ActionConfiguration actionConfiguration, object payload)
        {
            var enumerable = payload as IEnumerable;

            if (enumerable == null)
                return null;
            
            IResponseTransformer innerTransformer = null;

            IList result = null;
            
            foreach (var item in enumerable)
            {
                if (innerTransformer == null)
                    innerTransformer = actionConfiguration.ResponseTransformerFactory.Get(item);

                var transformed = innerTransformer.Transform(actionConfiguration, item);

                if (result == null)
                {
                    var resultType = typeof (List<>).MakeGenericType(new [] {transformed.GetType()});

                    result = (IList)Activator.CreateInstance(resultType);
                }
                
                result.Add(transformed);
            }

            return result;

        }

        public bool CanTransform(object data)
        {
            if (data is IEnumerable)
                return true;

            var type = data.GetType();
            
            if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                return true;

            return false;
        }
    }
}
