using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Net.Http;
using System.Web.Http.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHateoas.Configuration;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Response;
using NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace NHateoas.Tests.Routes
{
    [TestFixture]
    public class LinksGeneratorTest
    {
        private Fixture _fixture;
        private IActionConfiguration _actionConfiguration;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _fixture.Customize(new RandomNumericSequenceCustomization());

            Expression<Func<ControllerSample, ModelSample, IEnumerable<ModelSample>>> lambda = (c, m)
               => c.ControllerQueryMethod(m.Id, m.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());

            Expression<Func<ControllerSample, ModelSample, ModelSample>> lambda2 = (c, m)
               => c.ControllerMethodPut(m.Id, m);

            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                }
            });
           
            var actionConfiguration = new Mock<IActionConfiguration>();
            actionConfiguration.Setup(_ => _.MappingRules).Returns(() => new List<MappingRule>
            {
                new MappingRule( (MethodCallExpression)lambda.Body, apiExplorerMoq.Object)
                {
                    ApiDescriptions = new List<ApiDescription>()
                    {
                        new ApiDescription()
                        {
                            RelativePath = "/api/{id}?query={query}",
                            HttpMethod = HttpMethod.Get
                        }
                    },
                    Type = MappingRule.RuleType.Default

                },
                new MappingRule( (MethodCallExpression)lambda2.Body, apiExplorerMoq.Object)
                {
                    ApiDescriptions = new List<ApiDescription>()
                    {
                        new ApiDescription()
                        {
                            RelativePath = "/api/prod/{id}",
                            HttpMethod = HttpMethod.Put
                        }
                    },
                    Type = MappingRule.RuleType.Default

                }            
            });

            _actionConfiguration = actionConfiguration.Object;

        }  
        [Test]
        public void ComplexTest()
        {
            var routeRelations = new Dictionary<string, List<string>>();
            var payload = new ModelSample() {Id = 1, Name = "test", Price = 5.0, EMailAddress = "abc@def.com"};

            routeRelations.Add("GET/api/{id}?query={query}", new List<string>()
            {
                "get-query"
            });
            routeRelations.Add("PUT/api/prod/{id}", new List<string>()
            {
                "put-prod"
            });
            var links = LinksGenerator.Generate(_actionConfiguration, routeRelations, payload);

            var serialized = JsonConvert.SerializeObject(links);
            Assume.That(serialized, Is.EqualTo("[{\"rel\":[\"get-query\",\"__query\"],\"href\":\"http://localhost/api/1?query=:query\"}]"));
        }
    }
}
