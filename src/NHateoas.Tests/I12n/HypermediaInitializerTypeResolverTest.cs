using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Moq;
using NHateoas.Dynamic;
using NHateoas.Dynamic.Interfaces;
using NHateoas.Dynamic.Strategies;
using NHateoas.I12n;
using NUnit.Framework;
using TypeBuilder = NHateoas.Dynamic.TypeBuilder;

namespace NHateoas.Tests.I12n
{
    [TestFixture]
    public class HypermediaInitializerTypeResolverTest
    {
        [Test]
        public void ResolveEmptyCollection()
        {
            var assembliesResolver = new Mock<IAssembliesResolver>();

            assembliesResolver.Setup(_ => _.GetAssemblies()).Returns(new List<Assembly>());

            var resolver = new HypermediaInitializerTypeResolver();
            
            var collecton = resolver.GetControllerTypes(assembliesResolver.Object);

            Assume.That(collecton, Is.Empty);
        }

        [Test]
        public void ResolveThisClass()
        {
            var assembliesResolver = new Mock<IAssembliesResolver>();

            var typeBuilderStrategy = new Mock<ITypeBuilderStrategy>();

            typeBuilderStrategy.Setup(_ => _.ClassKey(It.IsAny<Type>())).Returns("KK");
            typeBuilderStrategy.Setup(_ => _.Configure(It.IsAny<ITypeBuilderContainer>()))
                .Callback((ITypeBuilderContainer c) => c.AddVisitor(new HypermediaApiControllerConfiguratorVisitor()));

            var typeBuilder = new TypeBuilder(typeof(ModelSample), typeBuilderStrategy.Object);

            typeBuilder.WithInterface(typeof (IHypermediaApiControllerConfigurator));

            var fakeType = typeBuilder.BuildType();

            assembliesResolver.Setup(_ => _.GetAssemblies()).Returns(new List<Assembly>() { new FakeAssembly(fakeType) });

            var resolver = new HypermediaInitializerTypeResolver();

            var collecton = resolver.GetControllerTypes(assembliesResolver.Object);

            Assume.That(collecton, Is.Not.Empty);
            Assume.That(collecton, Has.Member(fakeType));
        }
    }

    class FakeAssembly : Assembly
    {
        private readonly Type _myType;

        public FakeAssembly(Type myType)
        {
            _myType = myType;
        }
        public override bool IsDynamic
        {
            get { return false; }
        }

        public override Type[] GetTypes()
        {
            return new Type[] {_myType};
        }
    }

    class HypermediaApiControllerConfiguratorVisitor : ITypeBuilderVisitor
    {
        public void Visit(ITypeBuilderProvider provider)
        {
            var tb = provider.GetTypeBuilder();
            
            tb.AddInterfaceImplementation(typeof(IHypermediaApiControllerConfigurator));

            var mb = tb.DefineMethod("ConfigureHypermedia", MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] {typeof(HttpConfiguration)});

            var il = mb.GetILGenerator();
            
            il.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(mb, typeof(IHypermediaApiControllerConfigurator).GetMethod("ConfigureHypermedia"));
        }
    }
}
