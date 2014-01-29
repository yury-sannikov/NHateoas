using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;

namespace NHateoas.Dynamic.Visitors
{
    internal class AggregateTypeBuilder : ITypeBuilderVisitor, ITypeBuilderContainer
    {
        private readonly Type _aggregateType;
        private readonly string _fieldName;
        private readonly List<ITypeBuilderVisitor> _aggregatedProperties = new List<ITypeBuilderVisitor>();

        public AggregateTypeBuilder(Type aggregateType, string fieldName)
        {
            _aggregateType = aggregateType;
            _fieldName = fieldName;
        }

        public void Visit(ITypeBuilderProvider provider)
        {
            var typeBuilder = provider.GetTypeBuilder();

            var aggregateField = typeBuilder.DefineField(_fieldName, _aggregateType, FieldAttributes.Private);

            _aggregatedProperties.ForEach(prop =>
            {
                var agregatedProperty = (prop as IAggregatedProperty);
                
                if (agregatedProperty != null)
                    agregatedProperty.SetAggregatedProperty(aggregateField);

                prop.Visit(provider);
            });
        }

        public ITypeBuilderContainer AddVisitor(ITypeBuilderVisitor visitor)
        {
            _aggregatedProperties.Add(visitor);
            return this;
        }
    }
}
