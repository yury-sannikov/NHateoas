using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;

namespace NHateoas.Dynamic.Visitors
{
    internal class AggregatedPropertyVisitor : IAggregatedProperty
    {
        private FieldInfo _aggregate;
        
        private readonly PropertyInfo _propertyInfo;

        public AggregatedPropertyVisitor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public void SetAggregatedProperty(FieldInfo aggregate)
        {
            _aggregate = aggregate;
        }
        
        public void Visit(ITypeBuilderProvider provider)
        {
            var typeBuilder = provider.GetTypeBuilder();

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(_propertyInfo.Name, PropertyAttributes.HasDefault,
                                                                         _propertyInfo.PropertyType, null);

            AddGetter(typeBuilder, propertyBuilder);
            AddSetter(typeBuilder, propertyBuilder);
        }

        private void AddGetter(System.Reflection.Emit.TypeBuilder typeBuilder, PropertyBuilder propertyBuilder)
        {
            MethodBuilder propertyGetter = typeBuilder.DefineMethod("get_" + _propertyInfo.Name,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                _propertyInfo.PropertyType, Type.EmptyTypes);
            var ilGenerator = propertyGetter.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, _aggregate);
            MethodInfo getter = _propertyInfo.GetAccessors().First(accessor => accessor.ReturnType != typeof (void));
            ilGenerator.Emit(OpCodes.Callvirt, getter);
            ilGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(propertyGetter);
        }

        private void AddSetter(System.Reflection.Emit.TypeBuilder typeBuilder, PropertyBuilder propertyBuilder)
        {
            MethodBuilder propertySetter = typeBuilder.DefineMethod("set_" + _propertyInfo.Name,
                                 MethodAttributes.Public |
                                MethodAttributes.SpecialName |
                                MethodAttributes.HideBySig,
                                null, new Type[] { _propertyInfo.PropertyType });
            var propertySetterIl = propertySetter.GetILGenerator();
            propertySetterIl.Emit(OpCodes.Ldarg_0);
            propertySetterIl.Emit(OpCodes.Ldfld, _aggregate);
            propertySetterIl.Emit(OpCodes.Ldarg_1);
            MethodInfo setter = _propertyInfo.GetAccessors().First(accessor => accessor.ReturnType == typeof(void));
            propertySetterIl.Emit(OpCodes.Callvirt, setter);
            propertySetterIl.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(propertySetter);
        }
    }
}
