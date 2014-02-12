using System;
using System.Reflection;
using Moq;
using NHateoas.Dynamic;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.StrategyBuilderFactories;
using NUnit.Framework;

namespace NHateoas.Tests.Dynamic
{
    [TestFixture]
    public class TypeBuilderTest
    {
        [Test]
        public void Simple()
        {
            var typeBuilderStrategy = new Mock<ITypeBuilderStrategy>();
            typeBuilderStrategy.Setup(_ => _.ClassKey(It.IsAny<Type>())).Returns("key");
            var typeBuilder = new TypeBuilder(GetType(), typeBuilderStrategy.Object);
            var type = typeBuilder.BuildType();
            Assume.That(type.Name, Is.EqualTo(GetType().Name));
            Assume.That(type.FullName, Is.StringContaining("key." + GetType().Name));
        }

        [TearDown]
        public void Teardown()
        {
            TypeBuilder.Teardown();
            StrategyCache.Teardown();
        }

        [Test]
        public void WithVisitor()
        {

            var visitor = new Mock<ITypeBuilderVisitor>();

            visitor.Setup(_ => _.Visit(It.IsAny<ITypeBuilderProvider>())).Callback((ITypeBuilderProvider provider) =>
            {
                var tb = provider.GetTypeBuilder();
                tb.DefineField("definedField", typeof (int), FieldAttributes.Public);
            });

            var typeBuilderStrategy = new Mock<ITypeBuilderStrategy>();
            typeBuilderStrategy.Setup(_ => _.ClassKey(It.IsAny<Type>())).Returns("key2");
            typeBuilderStrategy.Setup(_ => _.Configure(It.IsAny<ITypeBuilderContainer>())).Callback(
                (ITypeBuilderContainer container) => container.AddVisitor(visitor.Object));

            var typeBuilder = new TypeBuilder(GetType(), typeBuilderStrategy.Object);
            var type = typeBuilder.BuildType();
            Assume.That(type.Name, Is.EqualTo(GetType().Name));
            Assume.That(type.FullName, Is.StringContaining("key2." + GetType().Name));

            var fields = type.GetFields();
            Assume.That(fields, Is.Not.Empty);
            Assume.That(fields[0].Name, Is.EqualTo("definedField"));
            Assume.That(fields[0].FieldType, Is.EqualTo(typeof(int)));
            Assume.That(fields[0].DeclaringType, Is.EqualTo(type));
        }
    }
}
