using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NHateoas
{
    internal static class ActionCallContext
    {
        private static string Key = "_cc_971978BE-B77A-4FD7-A26A-94688C2249FE";

        public static void Set(object data)
        {
            CallContext.LogicalSetData(Key, data);
        }

        public static T Get<T>()
        {
            var obj = CallContext.LogicalGetData(Key);

            if (obj != null && !(obj is T))
            {
                throw new ArgumentException(string.Format("Unable to represent object {0} as {1}", obj.GetType(), typeof(T)));
            }

            return (T)obj;
        }


    }
}
