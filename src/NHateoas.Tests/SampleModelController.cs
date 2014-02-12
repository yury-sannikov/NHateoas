using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHateoas.Attributes;

namespace NHateoas.Tests
{
    public class ModelSample
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public class ControllerSample
    {
        public ModelSample ControllerMethod(int id, string name, string query, int skip)
        {
            return null;
        }

        public static int SomeMethod()
        {
            return 2;
        }

        [Hypermedia(Names = new[] { "rel-name" })]
        public int FakeMethodWithAttribute()
        {
            return 0;
        }
        
        public int FakeMethod()
        {
            return 0;
        }
    }
}
