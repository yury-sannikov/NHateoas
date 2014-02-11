using System;
using NHateoas.Configuration;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace NHateoas.Tests.Configuration
{
    [TestFixture]
    public class ParametersDelegateBuilderTest
    {
        private Fixture _fixture;

        [TestFixtureSetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _fixture.Customize(new RandomNumericSequenceCustomization());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(Exception), ExpectedMessage = "Controller arguments must be model member expressions")]
        public void BuildWrongExpressionConstant()
        {
            Expression<Func<ModelFixture, ControllerFixture, ModelFixture>> expression = (model, controllerFixture)
                => controllerFixture.ControllerMethod(10, model.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());
            ParametersDelegateBuilder.Build(expression.Body as MethodCallExpression);
        }
        [Test]
        [ExpectedException(ExpectedException = typeof(Exception), ExpectedMessage = "Controller arguments must be QueryParameter class method call expression")]
        public void BuildWrongExpressionMethod()
        {
            Expression<Func<ModelFixture, ControllerFixture, ModelFixture>> expression = (model, controllerFixture)
                => controllerFixture.ControllerMethod(ControllerFixture.SomeMethod(), model.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());
            ParametersDelegateBuilder.Build(expression.Body as MethodCallExpression);
        }

        [Test]
        public void BuildExtended()
        {
            Expression<Func<ModelFixture, ControllerFixture, ModelFixture>> expression = (model, controllerFixture)
                => controllerFixture.ControllerMethod(model.Id, model.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());

            var result = ParametersDelegateBuilder.Build(expression.Body as MethodCallExpression);
            Assume.That(result.Count, Is.EqualTo(4));
            Assume.That(result.Keys, Is.EquivalentTo(new []{"id", "name", "query", "skip"}));

            var modelFixture = _fixture.CreateAnonymous<ModelFixture>();
            Assume.That(result["id"].DynamicInvoke(modelFixture), Is.EqualTo(modelFixture.Id));
            Assume.That(result["name"].DynamicInvoke(modelFixture), Is.EqualTo(modelFixture.Name));
            Assume.That(result["query"].DynamicInvoke(modelFixture), Is.EqualTo(":query"));
            Assume.That(result["skip"].DynamicInvoke(modelFixture), Is.EqualTo(":skip"));
        }
    }

    class ModelFixture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    class ControllerFixture
    {
        public ModelFixture ControllerMethod(int id, string name, string query, int skip)
        {
            return null;
        }

        public static int SomeMethod()
        {
            return 2;
        }
    }
}
