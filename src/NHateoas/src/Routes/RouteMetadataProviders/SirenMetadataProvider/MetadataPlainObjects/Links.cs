using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Routes.RouteMetadataProviders.SirenMetadataProvider.MetadataPlainObjects
{
    public class Links : List<SirenLink>
    {
    }

    public class SirenLink
    {
        public List<string> rel { get; set; }
        public string href { get; set; }
    }
}
