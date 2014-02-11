using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHateoas.Attributes;
using NUnit.Framework;

namespace NHateoas.Tests.Attributes
{
    [TestFixture]
    public class HypermediaAttributeTest
    {
        [Test]
        public void TestEmptyConstructor()
        {
            var attribute = new HypermediaAttribute();
            Assume.That(attribute.Names, Is.EquivalentTo(new string[0]));
        }

        [Test]
        public void TestConstructor()
        {
            var attribute = new HypermediaAttribute() {Names = new []{"test"}};
            Assume.That(attribute.Names, Is.EquivalentTo(new[] { "test" }));
        }
    }
}
