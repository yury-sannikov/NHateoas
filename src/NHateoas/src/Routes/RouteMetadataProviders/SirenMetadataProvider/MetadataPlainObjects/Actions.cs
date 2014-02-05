using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider.MetadataPlainObjects
{
    public class Fields : List<Field>
    {
    }

    [DataContract(Name = "field")]
    public class Field
    {
        [DataMember(Name = "name")]
        public string FieldName { get; set; }

        [DataMember(Name = "type")]
        public string FieldType { get; set; }

        [DataMember(Name = "value", EmitDefaultValue = false)]
        public string FieldValue { get; set; }
    }

    public class Actions : List<Action>
    {
    }

    [DataContract(Name = "action")]
    public class Action
    {
        [DataMember(Name = "name")]
        public string ActionName { get; set; }

        [DataMember(Name = "title", EmitDefaultValue = false)]
        public string Title { get; set; }

        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false)]
        public string ContentType { get; set; }

        [DataMember(Name = "fields", EmitDefaultValue = false)]
        public Fields ActionFields { get; set; }
    }
}
