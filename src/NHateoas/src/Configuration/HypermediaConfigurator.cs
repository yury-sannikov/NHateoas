using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHateoas.Configuration
{
    public class HypermediaConfigurator<TModel, TController>
    {
        public HypermediaConfigurator<TModel, TController> Map(Expression<Func<TModel, TController, IEnumerable<TModel>>> expression)
        {
            return this;
        }

        public HypermediaConfigurator<TModel, TController> Map(Expression<Func<TModel, TController, TModel>> expression)
        {
            return this;
        }

        public void Configure()
        {

        }
    }
}
