using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHateoas.Configuration;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace NHateoas.Tests.Configuration
{
    [TestFixture]
    public class ControllerConfigurationTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        [Test]
        public void DefaultInstance()
        {
            var moqMethodInfo = new Mock<MethodInfo>();
            var moqType = new Mock<Type>();
            
            var instance = HypermediaControllerConfiguration.Instance;
            Assume.That(instance.IsConfigured(moqType.Object), Is.False);
            Assume.That(instance.GetcontrollerActionConfiguration(moqType.Object, moqMethodInfo.Object), Is.Null);
        }

        [Test]
        public void ControllerConfigurationWithEmptyRules()
        {
            var moqMethodInfo = new Mock<MethodInfo>();
            var instance = HypermediaControllerConfiguration.Instance;
            var moqType = GetType();
            instance.Setup(moqType, new Dictionary<MethodInfo, IActionConfiguration>());
            Assume.That(instance.IsConfigured(moqType), Is.True);
            Assume.That(instance.GetcontrollerActionConfiguration(moqType, moqMethodInfo.Object), Is.Null);
        }
        [Test]
        public void ControllerConfigurationWithSetOfRules()
        {
            var moqType = _fixture.CreateAnonymous<Type>();
            var moqMethodInfo = _fixture.CreateAnonymous<MethodInfo>();
            var moqActionConfig = _fixture.CreateAnonymous<IActionConfiguration>();
            var instance = HypermediaControllerConfiguration.Instance;
            instance.Setup(moqType, new Dictionary<MethodInfo, IActionConfiguration> { { moqMethodInfo, moqActionConfig } });
            Assume.That(instance.IsConfigured(moqType), Is.True);
            Assume.That(instance.GetcontrollerActionConfiguration(moqType, moqMethodInfo), Is.EqualTo(moqActionConfig));
        }
    }
}
