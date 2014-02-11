using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHateoas.Dynamic.StrategyBuilderFactories;
using NHateoas.Response;
using NHateoas.Routes;
using NHateoas.Routes.RouteMetadataProviders.SimpleMetadataProvider;
using NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider;
using NUnit.Framework;

namespace NHateoas.Tests.Configuration
{
    [TestFixture]
    public class ActionConfigurationTest
    {
        private NHateoas.Configuration.ActionConfiguration _actionConfiguration;
        
        [SetUp]
        public void SetupTests()
        {
            var moqMethodInfo = new Mock<MethodInfo>();
            var moqType = new Mock<Type>();
            _actionConfiguration = new NHateoas.Configuration.ActionConfiguration(moqType.Object, moqMethodInfo.Object);
        }

        [Test]
        public void DefaultSettings()
        {
            Assume.That(_actionConfiguration.MappingRules, Is.Not.Null);
            Assume.That(_actionConfiguration.MetadataProvider, Is.Null);
            Assume.That(_actionConfiguration.ResponseTransformerFactory, Is.Null);
            Assume.That(_actionConfiguration.StrategyBuilderFactory, Is.Null);
        }

        [Test]
        public void DefaultSettingsWithConfigured()
        {
            _actionConfiguration.Configure();
            Assume.That(_actionConfiguration.MappingRules, Is.Not.Null);
            Assume.That(_actionConfiguration.MetadataProvider, Is.InstanceOf<IMetadataProvider>());
            Assume.That(_actionConfiguration.MetadataProvider, Is.InstanceOf<SimpleMetadataProvider>());
            Assume.That(_actionConfiguration.ResponseTransformerFactory, Is.InstanceOf<ResponseTransformerFactory>());
            Assume.That(_actionConfiguration.StrategyBuilderFactory, Is.InstanceOf<DefaultStrategyBuilderFactory>());
        }

        [Test]
        public void DefaultSettingsWithConfiguredAsSiren()
        {
            _actionConfiguration.UseSirenSpecification();
            _actionConfiguration.Configure();
            Assume.That(_actionConfiguration.MappingRules, Is.Not.Null);
            Assume.That(_actionConfiguration.MetadataProvider, Is.InstanceOf<IMetadataProvider>());
            Assume.That(_actionConfiguration.MetadataProvider, Is.InstanceOf<SirenMetadataProvider>());
            Assume.That(_actionConfiguration.ResponseTransformerFactory, Is.InstanceOf<ResponseTransformerFactory>());
            Assume.That(_actionConfiguration.StrategyBuilderFactory, Is.InstanceOf<SirenStrategyBuilderFactory>());
        }
    }
}
