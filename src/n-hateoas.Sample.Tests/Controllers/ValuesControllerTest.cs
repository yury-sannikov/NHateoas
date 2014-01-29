using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHateoas.Sample;
using NHateoas.Sample.Controllers;
using NHateoas.Sample.Models;

namespace NHateoas.Sample.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            IEnumerable<Product> result = controller.Get();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.ElementAt(0).Id);
        }

    }
}
