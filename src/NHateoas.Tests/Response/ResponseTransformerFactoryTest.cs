using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NHateoas.Configuration;
using NHateoas.Dynamic;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Response;
using NHateoas.Response.ResponseTransformers;
using NUnit.Framework;

namespace NHateoas.Tests.Response
{
    [TestFixture]
    public class ResponseTransformerFactoryTest
    {
        private ResponseTransformerFactory _responseTransformerFactory;
        private IActionConfiguration _actionConfiguration;
        [SetUp]
        public void Setup()
        {
            _responseTransformerFactory = new ResponseTransformerFactory();
            var strategy = new Mock<ITypeBuilderStrategy>();
            strategy.Setup(_ => _.ClassKey(It.IsAny<Type>())).Returns((Type t) => t.Name);

            var strategyFactory = new Mock<IStrategyBuilderFactory>();
            strategyFactory.Setup(_ => _.Build(It.IsAny<IActionConfiguration>(), It.IsAny<Type>()))
                .Returns(() => strategy.Object);

            var actionConfiguration = new Mock<IActionConfiguration>();
            actionConfiguration.Setup(_ => _.StrategyBuilderFactory).Returns(() => strategyFactory.Object);
            actionConfiguration.Setup(_ => _.ResponseTransformerFactory).Returns(_responseTransformerFactory);

            _actionConfiguration = actionConfiguration.Object;
        }

        [Test]
        public void TestGetter()
        {
            Assume.That(_responseTransformerFactory.Get(new object()), Is.TypeOf<ModelTransformer>());
            Assume.That(_responseTransformerFactory.Get(1), Is.Null);
            Assume.That(_responseTransformerFactory.Get(typeof(IActionConfiguration)), Is.Null);
            Assume.That(_responseTransformerFactory.Get(new List<string>()), Is.TypeOf<EnumerableTransformer>());
            Assume.That(_responseTransformerFactory.Get(new List<List<string>>()), Is.TypeOf<EnumerableTransformer>());
            Assume.That(_responseTransformerFactory.Get(new HashSet<List<string>>()), Is.TypeOf<EnumerableTransformer>());
            Assume.That(_responseTransformerFactory.Get(new string[0]), Is.TypeOf<EnumerableTransformer>());
        }

        [Test]
        public void TestModelTransformer()
        {
            var payload = new object();
            var modelTransformer = _responseTransformerFactory.Get(payload);
            Assume.That(modelTransformer, Is.TypeOf<ModelTransformer>());

            var transformed = modelTransformer.Transform(_actionConfiguration, payload);
            Assume.That(transformed, Is.Not.Null);
            Assume.That(transformed.GetType().AssemblyQualifiedName, Is.StringContaining("Object.Object, DynHateoas_"));
        }
        
        [Test]
        public void TestModelTransformerEnumberableTransformerChaining()
        {
            var payload = new List<object>();
            var modelTransformer = _responseTransformerFactory.Get(payload);
            Assume.That(modelTransformer, Is.TypeOf<EnumerableTransformer>());

            var transformed = modelTransformer.Transform(_actionConfiguration, payload);
            Assume.That(transformed, Is.Null);
            
            payload = new List<object>() {new object()};
            transformed = modelTransformer.Transform(_actionConfiguration, payload);

            Assume.That(transformed, Is.AssignableTo<IList>());
            Assume.That(((IList)transformed).Count, Is.EqualTo(1));
            Assume.That(((IList)transformed)[0].GetType().AssemblyQualifiedName, Is.StringContaining("Object.Object, DynHateoas_"));
        }
    }
}
