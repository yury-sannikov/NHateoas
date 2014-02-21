using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using Moq;
using NHateoas.Configuration;
using NHateoas.Routes;
using NHateoas.Routes.RouteMetadataProviders;
using NUnit.Framework;

namespace NHateoas.Tests.Routes
{
    [TestFixture]
    public class DefaultRouteNameBuilderTest
    {
        private IRouteNameBuilder _routeNameBuilder;

        [SetUp]
        public void Setup()
        {
            _routeNameBuilder = new DefaultRouteNameBuilder();
        }

        [Test]
        public void ReturnModel()
        {
            Expression<Func<ControllerSample, ModelSample, ModelSample>> lambda = (c, m) 
                => c.ControllerMethod(m.Id, m.Name, QueryParameter.Is<string>(), QueryParameter.Is<int>());
            
            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var mappingRule = new MappingRule(methodCallExpression, null);

            var result = _routeNameBuilder.Build(mappingRule, "get");
            Assume.That(result, Is.EquivalentTo(new[] { "get_modelsample_by_id_name_query_skip" }));
        }
        [Test]
        public void ReturnHttpresponse()
        {
            Expression<Func<ControllerSample, HttpResponseMessage>> lambda = (c)
                => c.ControllerHttpResponseMessageMethod();

            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var mappingRule = new MappingRule(methodCallExpression, null);

            var result = _routeNameBuilder.Build(mappingRule, "get");
            Assume.That(result, Is.EquivalentTo(new[] { "get" }));
        }

        [Test]
        public void ReturnHttpresponseAttributed()
        {
            Expression<Func<ControllerSample, HttpResponseMessage>> lambda = (c)
                => c.ControllerHttpResponseMessageMethodWithType();

            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var mappingRule = new MappingRule(methodCallExpression, null);

            var result = _routeNameBuilder.Build(mappingRule, "get");
            Assume.That(result, Is.EquivalentTo(new[] { "get_string" }));
        }
        [Test]
        public void ReturnHttpresponseParam()
        {
            Expression<Func<ControllerSample, ModelSample, HttpResponseMessage>> lambda = (c, m)
                => c.ControllerHttpResponseMessageMethodParam(m.Id);

            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var mappingRule = new MappingRule(methodCallExpression, null);

            var result = _routeNameBuilder.Build(mappingRule, "get");
            Assume.That(result, Is.EquivalentTo(new[] { "get_by_id" }));
        }

        [Test]
        public void ReturnHttpresponseAttributedParam()
        {
            Expression<Func<ControllerSample, ModelSample, HttpResponseMessage>> lambda = (c, m)
                => c.ControllerHttpResponseMessageMethodWithTypeParam(m.Price);

            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var mappingRule = new MappingRule(methodCallExpression, null);

            var result = _routeNameBuilder.Build(mappingRule, "get");
            Assume.That(result, Is.EquivalentTo(new[] { "get_string_by_price" }));
        }
        [Test]
        public void ReturnEnumerable()
        {
            Expression<Func<ControllerSample, ModelSample, IEnumerable<ModelSample>>> lambda = (c, m)
                => c.ControllerQueryMethod(m.Id, m.Name, m.Name, m.Id);

            var methodCallExpression = (MethodCallExpression)lambda.Body;

            var mappingRule = new MappingRule(methodCallExpression, null);

            var result = _routeNameBuilder.Build(mappingRule, "get");
            Assume.That(result, Is.EquivalentTo(new[] { "query_modelsample_by_id_name_query_skip" }));
        }
    }
}
