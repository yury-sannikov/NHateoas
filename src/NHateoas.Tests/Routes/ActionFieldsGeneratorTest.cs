using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Web.Http.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHateoas.Configuration;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.StrategyBuilderFactories;
using NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider;
using NHateoas.Routes.RouteValueSubstitution;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace NHateoas.Tests.Routes
{
    [TestFixture]
    public class ActionFieldsGeneratorTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _fixture.Customize(new RandomNumericSequenceCustomization());

        }  
        [Test]
        public void TestNotFromBodyNotGet()
        {

            var sub = new DefaultRouteValueSubstitution();
            Expression<Func<ControllerSample, ModelSample, ModelSample>> lambda = (c, m)
                => c.ControllerMethod(m.Id, m.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());

            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                }
            });

            var mr = new MappingRule(methodCallExpression, apiExplorerMoq.Object);

            var payload = _fixture.CreateAnonymous<ModelSample>();
            var fields = ActionFieldsGenerator.Generate(mr, apiExplorerMoq.Object.ApiDescriptions[0], payload);
            Assume.That(fields, Is.Null);

        }
        [Test]
        public void TestNotFromBodyGet()
        {
            var sub = new DefaultRouteValueSubstitution();
            Expression<Func<ControllerSample, ModelSample, ModelSample>> lambda = (c, m)
                => c.ControllerMethod(m.Id, m.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());

            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                    HttpMethod = HttpMethod.Get
                }
            });

            var mr = new MappingRule(methodCallExpression, apiExplorerMoq.Object);

            var payload = _fixture.CreateAnonymous<ModelSample>();
            var fields = ActionFieldsGenerator.Generate(mr, apiExplorerMoq.Object.ApiDescriptions[0], payload);
            Assume.That(fields, Is.Not.Null);
            var names = fields.ConvertAll(f => f.FieldName);
            Assume.That(names, Is.EquivalentTo(new []{"id", "name", "query", "skip"}));
            var types = fields.ConvertAll(f => f.FieldType);
            Assume.That(types, Is.All.Null);
            var values = fields.ConvertAll(f => f.FieldValue);
            Assume.That(values, Is.EqualTo(new object[]{payload.Id.ToString(), payload.Name, null, null}));
        }
        [Test]
        public void TestFromBody()
        {
            var sub = new DefaultRouteValueSubstitution();
            Expression<Func<ControllerSample, ModelSample, ModelSample>> lambda = (c, m)
                => c.ControllerMethodPut(m.Id, m);

            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                    HttpMethod = HttpMethod.Get
                }
            });

            var mr = new MappingRule(methodCallExpression, apiExplorerMoq.Object);

            var payload = _fixture.CreateAnonymous<ModelSample>();
            var fields = ActionFieldsGenerator.Generate(mr, apiExplorerMoq.Object.ApiDescriptions[0], payload);
            Assume.That(fields, Is.Not.Null);
            var names = fields.ConvertAll(f => f.FieldName);
            Assume.That(names, Is.EquivalentTo(new[] { "Id", "Name", "Price", "email_address" }));
            var types = fields.ConvertAll(f => f.FieldType);
            Assume.That(types, Is.EqualTo(new object[]{null, null, null, "email"}));
            var values = fields.ConvertAll(f => f.FieldValue);
            Assume.That(values, Is.EqualTo(new object[] { payload.Id.ToString(), payload.Name, payload.Price.ToString(), payload.EMailAddress }));
        }
    }
}
