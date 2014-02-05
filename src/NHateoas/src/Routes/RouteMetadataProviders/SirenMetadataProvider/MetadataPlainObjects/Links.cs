using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider.MetadataPlainObjects
{
    public class Links : List<SirenLink>
    {
    }

    [DataContract]
    public class SirenLink
    {
        [DataMember(Name = "rel")]
        public List<string> RelList { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
    }
}
