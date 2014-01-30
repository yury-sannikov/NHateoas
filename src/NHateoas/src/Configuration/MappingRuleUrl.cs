using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas.Configuration
{
    internal class MappingRuleUrl
    {
        public string RouteName { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string Documentation { get; set; }
    }
}
