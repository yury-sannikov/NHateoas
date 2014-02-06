using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using NHateoas.Configuration;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider
{
    internal static class ActionFieldsGenerator
    {
        public static MetadataPlainObjects.Fields Generate(MappingRule mappingRule, ApiDescription apiDescription, object originalObject)
        {
            var fromBodyParameterType = (from param in mappingRule.MethodExpression.Method.GetParameters()
                let fromBodyAttr = param.GetCustomAttribute<FromBodyAttribute>()
                where fromBodyAttr != null
                select param.ParameterType).FirstOrDefault();

            if (fromBodyParameterType == null)
                return GenerateFromGet(mappingRule, apiDescription, originalObject);

            var result = new MetadataPlainObjects.Fields();
            result.AddRange(from property in fromBodyParameterType.GetProperties()
                let fieldName = GetFieldName(property)
                let propVal = property.GetValue(originalObject)
                select new MetadataPlainObjects.Field
                {
                    FieldName = fieldName,
                    FieldType = GetFieldType(property),
                    FieldValue = propVal == null ? null : propVal.ToString()
                });


            return result;
        }

        private static MetadataPlainObjects.Fields GenerateFromGet(MappingRule mappingRule, ApiDescription apiDescription,
            object originalObject)
        {
            if (apiDescription.HttpMethod != HttpMethod.Get)
                return null;

            var result = new MetadataPlainObjects.Fields();

            int argumentCounter = 0;
            result.AddRange(
                from parameter in mappingRule.MethodExpression.Method.GetParameters()
                let isMemberExpression = mappingRule.MethodExpression.Arguments[argumentCounter++] is MemberExpression
                where mappingRule.ParameterDelegates.ContainsKey(parameter.Name)
                let paramDelegate = mappingRule.ParameterDelegates.ContainsKey(parameter.Name) ? mappingRule.ParameterDelegates[parameter.Name] : null
                select new MetadataPlainObjects.Field
                {
                    FieldName = parameter.Name,
                    FieldValue = (isMemberExpression && paramDelegate != null) ? paramDelegate.DynamicInvoke(originalObject).ToString() : null
                });

            return result;
        }
        
        private static string GetFieldType(PropertyInfo property)
        {
            var dataTypeAttrib = property.GetCustomAttribute<DataTypeAttribute>();
            
            if (dataTypeAttrib == null)
                return null;

            switch (dataTypeAttrib.DataType)
            {
                case DataType.Date:
                    return "date";
                case DataType.DateTime:
                    return "datetime";
                case DataType.EmailAddress:
                    return "email";
                case DataType.ImageUrl:
                    return "image";
                case DataType.Password:
                    return "password";
                case DataType.Time:
                    return "time";
                case DataType.Upload:
                    return "file";

                default:
                    return typeof(HttpPostedFileBase).IsAssignableFrom(property.PropertyType) ? "file" : "text";
            }
        }

        private static string GetFieldName(PropertyInfo property)
        {
            var name = property.Name;

            var datamemberAttrib = property.GetCustomAttribute<DataMemberAttribute>();

            if (datamemberAttrib != null && !string.IsNullOrEmpty(datamemberAttrib.Name))
                name = datamemberAttrib.Name;
            return name;
        }
    }
}
