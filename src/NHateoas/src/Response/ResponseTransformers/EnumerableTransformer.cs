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
        public object Transform(IActionConfiguration actionConfiguration, object payload)
        {
            var enumerable = payload as IEnumerable;

            if (enumerable == null)
                return null;
            
            IResponseTransformer innerTransformer = null;

            IList resultList = null;
            
            foreach (var item in enumerable)
            {
                if (innerTransformer == null)
                {
                    innerTransformer = actionConfiguration.ResponseTransformerFactory.Get(item);
                    if (innerTransformer == null)
                        throw new Exception(string.Format("Unable to get response transformer for response type {0}", item.GetType()));
                }

                var transformed = innerTransformer.Transform(actionConfiguration, item);

                if (transformed == null)
                    continue;

                if (resultList == null)
                {
                    var resultType = typeof (List<>).MakeGenericType(new [] {transformed.GetType()});

                    resultList = (IList)Activator.CreateInstance(resultType);
                }
                
                resultList.Add(transformed);
            }

            return resultList;

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
