using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Response.ResponseTransformers;

namespace NHateoas.Response
{
    internal class ResponseTransformerFactory : IResponseTransformerFactory
    {
        private readonly List<IResponseTransformer>  _transformers = new List<IResponseTransformer>();
        
        public ResponseTransformerFactory()
        {
            _transformers.Add(new EnumerableTransformer());    
            _transformers.Add(new ModelTransformer());    
        }

        public IResponseTransformer Get(object data)
        {
            return _transformers.FirstOrDefault(t => t.CanTransform(data));
        }
    }
}
