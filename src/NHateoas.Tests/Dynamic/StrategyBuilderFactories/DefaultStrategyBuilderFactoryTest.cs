using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Moq;
using NHateoas.Configuration;
using NHateoas.Dynamic;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.StrategyBuilderFactories;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace NHateoas.Tests.Dynamic.StrategyBuilderFactories
{
    [TestFixture]
    public class DefaultStrategyBuilderFactoryTest
    {
        private ActionConfiguration _actionConfiguration;
        private IStrategyBuilderFactory _defaultStrategyFactory;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _defaultStrategyFactory = new DefaultStrategyBuilderFactory();
            var controllerType = typeof(ControllerSample);
            var controllerMethodInfo = controllerType.GetMethod("ControllerMethod");
            _actionConfiguration = new ActionConfiguration(controllerType, controllerMethodInfo);

            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _fixture.Customize(new RandomNumericSequenceCustomization());

        }

        [Test]
        public void Simple()
        {
            _actionConfiguration.Configure();
            var originalType = typeof(IsolatedModelSample);
            var strategy = _defaultStrategyFactory.Build(_actionConfiguration, originalType);
            var classKey = strategy.ClassKey(originalType);
            Assume.That(classKey, Is.EqualTo("_NHateoas.Tests.Dynamic.StrategyBuilderFactories.IsolatedModelSample_SP_SR_0_0_"));
        }
        
        [Test]
        public void Complex()
        {
            Expression<Func<ControllerSample, ModelSample, ModelSample>> lambda = (test, model) 
                => test.ControllerMethod(model.Id, model.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());
            var methodCallExpression = (MethodCallExpression)lambda.Body;


            var httpControllerDescriptor = _fixture.CreateAnonymous<HttpControllerDescriptor>();

            var apiExplorerMoq = new Mock<IApiExplorer>();
            apiExplorerMoq.Setup(_ => _.ApiDescriptions).Returns(new Collection<ApiDescription>()
            {
                new ApiDescription()
                {
                    ActionDescriptor = new ReflectedHttpActionDescriptor(httpControllerDescriptor, methodCallExpression.Method),
                    HttpMethod = HttpMethod.Get,
                    RelativePath = "/api"
                }
            });
            
            var mappingRule = new MappingRule(methodCallExpression, apiExplorerMoq.Object);
            
            _actionConfiguration.AddMappingRule(mappingRule);

            _actionConfiguration.Configure();
            var originalType = typeof(ModelSample);
            var strategy = _defaultStrategyFactory.Build(_actionConfiguration, originalType);
            Assume.That(strategy.ClassKey(originalType), Is.EqualTo("_NHateoas.Tests.ModelSample_SP_SR_3567185397_1_"));
            
            var typeBuilder = new TypeBuilder(originalType, strategy);
            var type = typeBuilder.BuildType();
            Assume.That(type.Name, Is.EqualTo(originalType.Name));
            Assume.That(type.FullName, Is.StringContaining("_NHateoas.Tests.ModelSample_SP_SR_3567185397_1_." + originalType.Name));

            var props = type.GetProperties();
            Assume.That(props, Is.Not.Empty);
            
            var propNames = props.ToList().ConvertAll(p => p.Name);
            Assume.That(propNames, Is.EquivalentTo(new[] { "Id", "Name", "Price", "get_modelsample_by_id_name_query_skip" }));
            var propTypes = props.ToList().ConvertAll(p => p.PropertyType.Name);
            Assume.That(propTypes, Is.EquivalentTo(new[] { "Int32", "String", "Double", "String" }));


            var instance = Activator.CreateInstance(type);
            var original = _fixture.CreateAnonymous<ModelSample>();
            strategy.ActivateInstance(instance, original, _actionConfiguration.MetadataProvider);

            var propValues = props.ToList().ConvertAll(p => p.GetValue(instance).ToString());
            Assume.That(propValues, Is.EquivalentTo(new[] { original.Id.ToString(), original.Name, original.Price.ToString(), "/api" }));
        }
    }
    
    public class IsolatedModelSample
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

}
