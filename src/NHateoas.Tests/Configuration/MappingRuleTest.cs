using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHateoas.Attributes;
using NHateoas.Configuration;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace NHateoas.Tests.Configuration
{
    [TestFixture]
    public class MappingRuleTest
    {
        private Fixture _fixture;

        [TestFixtureSetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        [Test]
        public void DefaultCtor()
        {
            Expression<Func<ControllerSample, int>> lambda = (test) => test.FakeMethod();
            var methodCallExpression = (MethodCallExpression) lambda.Body;
            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                }
            });

            var mappingRule = new MappingRule(methodCallExpression, apiExplorerMoq.Object);

            Assume.That(mappingRule.MethodExpression, Is.EqualTo(methodCallExpression));
            Assume.That(mappingRule.ApiDescriptions, Is.Empty);
            Assume.That(mappingRule.ParameterDelegates, Is.Empty);
            Assume.That(mappingRule.Names, Is.Empty);
            Assume.That(mappingRule.Type, Is.EqualTo(MappingRule.RuleType.Default));
        }

        [Test]
        public void MapApiDescription()
        {
            Expression<Func<ControllerSample, int>> lambda = (test) => test.FakeMethod();
            var methodCallExpression = (MethodCallExpression) lambda.Body;

            var actionDescriptor =
                new ReflectedHttpActionDescriptor(_fixture.CreateAnonymous<HttpControllerDescriptor>(),
                    methodCallExpression.Method);

            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                    ActionDescriptor = actionDescriptor
                }
            });

            var mappingRule = new MappingRule(methodCallExpression, apiExplorerMoq.Object);

            Assume.That(mappingRule.MethodExpression, Is.EqualTo(methodCallExpression));
            Assume.That(mappingRule.ApiDescriptions, Is.Not.Empty);
            Assume.That(mappingRule.ParameterDelegates, Is.Empty);
            Assume.That(mappingRule.Names, Is.Empty);
            Assume.That(mappingRule.Type, Is.EqualTo(MappingRule.RuleType.Default));
        }


        [Test]
        public void GetRelsFromAttributes()
        {
            Expression<Func<ControllerSample, int>> lambda = (test) => test.FakeMethodWithAttribute();
            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                }
            });

            var mappingRule = new MappingRule(methodCallExpression, apiExplorerMoq.Object);

            Assume.That(mappingRule.MethodExpression, Is.EqualTo(methodCallExpression));
            Assume.That(mappingRule.ApiDescriptions, Is.Empty);
            Assume.That(mappingRule.ParameterDelegates, Is.Empty);
            Assume.That(mappingRule.Names, Is.Not.Empty);
            Assume.That(mappingRule.Names, Is.EquivalentTo(new [] {"rel-name"})); 
            Assume.That(mappingRule.Type, Is.EqualTo(MappingRule.RuleType.Default));
        }
    }
}
