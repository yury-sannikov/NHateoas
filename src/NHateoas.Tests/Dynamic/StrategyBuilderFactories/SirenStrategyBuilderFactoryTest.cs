using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Moq;
using Newtonsoft.Json;
using NHateoas.Configuration;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.StrategyBuilderFactories;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System.Linq.Expressions;
using NHateoas.Dynamic;


namespace NHateoas.Tests.Dynamic.StrategyBuilderFactories
{
    [TestFixture]
    public class SirenStrategyBuilderFactoryTest
    {
        private ActionConfiguration _actionConfiguration;
        private IStrategyBuilderFactory _defaultStrategyFactory;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _defaultStrategyFactory = new SirenStrategyBuilderFactory();
            var controllerType = typeof(ControllerSample);
            var controllerMethodInfo = controllerType.GetMethod("ControllerMethod");
            _actionConfiguration = new ActionConfiguration(controllerType, controllerMethodInfo);

            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _fixture.Customize(new RandomNumericSequenceCustomization());

        }

        [TearDown]
        public void Teardown()
        {
            TypeBuilder.Teardown();
            StrategyCache.Teardown();
        }

        [Test]
        public void Simple()
        {
            _actionConfiguration.UseSirenSpecification();
            _actionConfiguration.Configure();
            var originalType = typeof(IsolatedModelSample);
            var strategy = _defaultStrategyFactory.Build(_actionConfiguration, originalType);
            var classKey = strategy.ClassKey(originalType);
            Assume.That(classKey, Is.StringContaining("_NHateoas.Tests.Dynamic.StrategyBuilderFactories.IsolatedModelSample_PP"));
        }
        
        [Test]
        public void Complex()
        {
            Expression<Func<ControllerSample, ModelSample, ModelSample>> lambda = (test, model) 
                => test.ControllerMethod(model.Id, model.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());
            var methodCallExpression = (MethodCallExpression)lambda.Body;


            var httpControllerDescriptor = _fixture.CreateAnonymous<HttpControllerDescriptor>();

            Expression<Func<ControllerSample, int>> lambda2 = (test) => test.FakeMethodWithAttribute();

            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                    ActionDescriptor = new ReflectedHttpActionDescriptor(httpControllerDescriptor, methodCallExpression.Method),
                    HttpMethod = HttpMethod.Get,
                    RelativePath = "/api"
                },
                new ApiDescription()
                {
                    ActionDescriptor = new ReflectedHttpActionDescriptor(httpControllerDescriptor, ((MethodCallExpression)lambda2.Body).Method),
                    HttpMethod = HttpMethod.Post,
                    RelativePath = "/api/test"
                    
                }
            });
            
            _actionConfiguration.AddMappingRule(new MappingRule(methodCallExpression, apiExplorerMoq.Object));

            
            var rule = new MappingRule((MethodCallExpression)lambda2.Body, apiExplorerMoq.Object)
            {
                Type = MappingRule.RuleType.ActionRule
            };
            rule.Names.Add("action-name");
            _actionConfiguration.AddMappingRule(rule);

            _actionConfiguration.UseSirenSpecification();
            _actionConfiguration.Configure();
            var originalType = typeof(ModelSample);
            var strategy = _defaultStrategyFactory.Build(_actionConfiguration, originalType);
            Assume.That(strategy.ClassKey(originalType), Is.StringContaining("_NHateoas.Tests.ModelSample_PP"));
            
            var typeBuilder = new TypeBuilder(originalType, strategy);
            var type = typeBuilder.BuildType();
            Assume.That(type.Name, Is.EqualTo(originalType.Name));
            Assume.That(type.FullName, Is.StringContaining("_NHateoas.Tests.ModelSample_PP"));

            var props = type.GetProperties();
            Assume.That(props, Is.Not.Empty);

            var propNames = new List<string>();
            props.ToList().ForEach(p =>
            {   propNames.Add(p.Name);
                var pt = (p.PropertyType.BaseType != null && p.PropertyType.BaseType.IsGenericType) ? 
                    p.PropertyType.BaseType.GetGenericArguments()[0] : p.PropertyType;
                pt.GetProperties().ToList().ForEach(sp => propNames.Add(sp.Name));
            });

            Assume.That(propNames, Is.EquivalentTo(new[] { "properties", "Id", "Name", "Price", "EMailAddress", "links", "RelList", "Href", "actions", "ActionName", "Class", "Title", "Method", "Href", "ContentType", "ActionFields" }));
            var propTypes = props.ToList().ConvertAll(p => p.PropertyType.Name);
            Assume.That(propTypes, Is.EquivalentTo(new[] { "ModelSample", "Links", "Actions"}));


            var instance = Activator.CreateInstance(type);
            var original = new ModelSample()
            {
                Id = 1,
                Name = "test",
                Price = 3.0,
                EMailAddress = "aa.bb@ccc"
            };
            strategy.ActivateInstance(instance, original, _actionConfiguration.MetadataProvider);

            var result = JsonConvert.SerializeObject(instance);
            Assume.That(result, Is.EqualTo("{\"properties\":{\"Id\":1,\"Name\":\"test\",\"Price\":3.0,\"EMailAddress\":\"aa.bb@ccc\"},\"links\":[{\"rel\":[\"get_modelsample_by_id_name_query_skip\"],\"href\":\"/api\"}],\"actions\":[{\"name\":\"rel-name\",\"method\":\"POST\",\"href\":\"/api/test\",\"type\":\"application/x-www-form-urlencoded\"}]}"));
        }
    }
}
