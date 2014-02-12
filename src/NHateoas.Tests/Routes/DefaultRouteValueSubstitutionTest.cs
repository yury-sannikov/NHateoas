using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Web.Http.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHateoas.Configuration;
using NHateoas.Routes.RouteValueSubstitution;
using NUnit.Framework;

namespace NHateoas.Tests.Routes
{
    [TestFixture]
    public class DefaultRouteValueSubstitutionTest
    {
        [Test]
        public void TestSubstitution()
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

            var payload = new ModelSample()
            {
                Id = 1,
                Name = "test &?{}<>",
                Price = 3.2
            };

            var result = sub.Substitute("/Product/{id}/Details?query={query}&skip={skip}&displayname={name}", mr, payload);

            Assume.That(result, Is.EqualTo("/Product/1/Details?query=:query&skip=:skip&displayname=test+%26%3f%7b%7d%3c%3e"));
        }
    }
}
