using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Dynamic.Interfaces;

namespace NHateoas.Dynamic.Visitors
{
    internal class PropertyVisitor : ITypeBuilderVisitor
    {
        private readonly Type _propertyType;
        
        private readonly string _propertyName;
        private List<Func<CustomAttributeBuilder>> _customAttributes;

        public PropertyVisitor(Type propertyType, string propertyName)
        {
            _propertyType = propertyType;
            _propertyName = propertyName;
        }

        public static string PropertyFieldName(string propertyName)
        {
            return string.Format("_{0}", propertyName.ToLower());
        }

        public void AddCustomAttributeFactory(Func<CustomAttributeBuilder> factory)
        {
            if (_customAttributes == null)
                _customAttributes = new List<Func<CustomAttributeBuilder>>() {factory};
            else
                _customAttributes.Add(factory);
        }

        public void Visit(ITypeBuilderProvider provider)
        {

            var typeBuilder = provider.GetTypeBuilder();

            FieldBuilder fieldBuilder = typeBuilder.DefineField(PropertyFieldName(_propertyName),
                                                                 _propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(_propertyName, PropertyAttributes.HasDefault,
                                                                         _propertyType, null);

            if (_customAttributes != null)
                _customAttributes.ForEach(factory => propertyBuilder.SetCustomAttribute(factory()));

            AddGetter(typeBuilder, fieldBuilder, propertyBuilder);

            AddSetter(typeBuilder, fieldBuilder, propertyBuilder);
            
        }

        private void AddSetter(System.Reflection.Emit.TypeBuilder typeBuilder, FieldBuilder fieldBuilder, PropertyBuilder propertyBuilder)
        {
            MethodBuilder propertySetter = typeBuilder.DefineMethod("set_" + _propertyName,
                                 MethodAttributes.Public |
                                MethodAttributes.SpecialName |
                                MethodAttributes.HideBySig,
                                null, new Type[] { _propertyType });
            var propertySetterIl = propertySetter.GetILGenerator();
            propertySetterIl.Emit(OpCodes.Ldarg_0);
            propertySetterIl.Emit(OpCodes.Ldarg_1);
            propertySetterIl.Emit(OpCodes.Stfld, fieldBuilder);
            propertySetterIl.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(propertySetter);
        }

        private void AddGetter(System.Reflection.Emit.TypeBuilder typeBuilder, FieldBuilder fieldBuilder, PropertyBuilder propertyBuilder)
        {
            MethodBuilder propertyGetter = typeBuilder.DefineMethod("get_" + _propertyName,
                                 MethodAttributes.Public |
                                MethodAttributes.SpecialName |
                                MethodAttributes.HideBySig,
                                _propertyType, Type.EmptyTypes);
            var ilGenerator = propertyGetter.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(propertyGetter);
        }
    }
}
