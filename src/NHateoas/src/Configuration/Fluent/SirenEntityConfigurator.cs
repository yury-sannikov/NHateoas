using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration.Fluent
{
    public class SirenEntityConfigurator<TModel, TController>
    {

        private readonly SirenConfigurator<TModel, TController> _sirenConfigurator;
        private readonly Action<string[]> _entityBuilder;
        
        internal SirenEntityConfigurator(SirenConfigurator<TModel, TController> sirenConfigurator, 
            HypermediaConfigurationLogic<TModel, TController> logic,
            Expression entityExpression,
            Expression actionExpression,
            bool isLinked)
        {
            _sirenConfigurator = sirenConfigurator;
            _entityBuilder = rel =>
            {
                if (isLinked)
                    logic.AddNewLinkedEntityMapping(entityExpression, actionExpression, rel);
                else
                    logic.AddNewEmbeddedEntityMapping(entityExpression, actionExpression, rel);
            };
        }

        public SirenConfigurator<TModel, TController> WitRel(string rel)
        {
            _entityBuilder(new [] {rel});
            return _sirenConfigurator;
        }

        public SirenConfigurator<TModel, TController> WitRels(string[] rel)
        {
            _entityBuilder(rel);
            return _sirenConfigurator;
        }
    }
}
